using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaxFreeShareAPI.Data;
using TaxFreeShareAPI.Models;
using TaxFreeShareAPI.Services.Interface;
using TaxFreeShareAPI.Models.DTOs.OrderDto;

namespace TaxFreeShareAPI.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly TaxFreeDbContext _context;
    private readonly ILogger<OrdersController> _logger;
    private readonly IEmailService _emailService;

    public OrdersController(TaxFreeDbContext context, ILogger<OrdersController> logger, IEmailService emailService)
    {
        _context = context;
        _logger = logger;
        _emailService = emailService;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
    {
        var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(userRole))
            return BadRequest("Ugyldig bruker.");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
        if (user == null) return BadRequest("Brukeren finnes ikke.");

        IQueryable<Order> query = _context.Orders.Include(o => o.OrderItems);

        if (userRole == "Kjøper")
        {
            query = query.Where(o => o.UserId == user.Id);
        }
        else if (userRole == "Selger")
        {
            query = query.Where(o => o.SellerId == user.Id || (o.Status == "Pending" && o.SellerId == null));
        }
        else return Forbid();

        var orders = await query.ToListAsync();

        return Ok(orders.Select(order => new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            SellerId = order.SellerId,
            OrderDate = order.OrderDate,
            Status = order.Status ?? "Pending",
            TotalAmount = order.TotalAmount,
            OrderItems = order.OrderItems.Select(oi => new UpdateOrderItemDto
            {
                ProductId = oi.ProductId,
                Quantity = oi.Quantity
            }).ToList()
        }));
    }


    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        var order = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return NotFound("Ordren ble ikke funnet.");

        var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (user == null || (order.UserId != user.Id && userRole != "admin"))
            return Forbid();

        return Ok(new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            OrderDate = order.OrderDate,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            OrderItems = order.OrderItems.Select(item => new UpdateOrderItemDto
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList()
        });
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto orderDto)
    {
        var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

        if (user == null) return BadRequest("Brukeren finnes ikke.");
        if (orderDto.OrderItems == null || !orderDto.OrderItems.Any())
            return BadRequest("Ordren må inneholde minst ett produkt.");

        var newOrder = new Order
        {
            UserId = user.Id,
            OrderDate = DateTime.UtcNow,
            Status = "Pending",
            TotalAmount = 0
        };

        _context.Orders.Add(newOrder);
        await _context.SaveChangesAsync();

        var orderItems = new List<OrderItem>();

        foreach (var item in orderDto.OrderItems)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product == null) continue;

            orderItems.Add(new OrderItem
            {
                OrderId = newOrder.Id,
                ProductId = product.Id,
                Quantity = item.Quantity,
                Price = product.Price
            });

            newOrder.TotalAmount += item.Quantity * product.Price;
        }

        if (!orderItems.Any()) return BadRequest("Ingen gyldige produkter.");

        _context.OrderItems.AddRange(orderItems);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetOrder), new { id = newOrder.Id }, new OrderDto
        {
            Id = newOrder.Id,
            UserId = newOrder.UserId,
            OrderDate = newOrder.OrderDate,
            Status = newOrder.Status,
            TotalAmount = newOrder.TotalAmount,
            OrderItems = orderItems.Select(oi => new UpdateOrderItemDto
            {
                ProductId = oi.ProductId,
                Quantity = oi.Quantity
            }).ToList()
        });
    }

    [HttpPut("status/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto statusDto)
    {
        var allowedStatuses = new[] { "Pending", "Fullført" };
        if (!allowedStatuses.Contains(statusDto.Status)) return BadRequest("Ugyldig status.");

        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();

        order.Status = statusDto.Status;
        await _context.SaveChangesAsync();

        return Ok($"Ordren #{id} er oppdatert til status '{order.Status}'.");
    }
    
    [HttpPost("assign/{orderId}")]
    [Authorize(Roles = "Selger")]
    public async Task<IActionResult> AssignOrderToSeller(int orderId)
    {
        var sellerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(sellerId) || !int.TryParse(sellerId, out var id))
        {
            return Unauthorized("Ugyldig selger.");
        }

        var order = await _context.Orders.FindAsync(orderId);
        if (order == null)
        {
            return NotFound("Bestilling ikke funnet.");
        }

        if (order.SellerId != null)
        {
            return BadRequest("Bestillingen er allerede tildelt en selger.");
        }

        order.SellerId = id;
        order.Status = "Bekreftet";
        await _context.SaveChangesAsync();
        
        return Ok("Bestilling tildelt selger og bekreftet.");
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return Ok("Ordren er slettet.");
    }
    
    
}
