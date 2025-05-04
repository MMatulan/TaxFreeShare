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

    private int? GetCurrentUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out var id) ? id : null;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
    {
        var userId = GetCurrentUserId();
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        if (userId == null || string.IsNullOrEmpty(userRole))
            return BadRequest("Ugyldig bruker.");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
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

        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized("Ugyldig bruker.");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) return BadRequest("Brukeren finnes ikke.");

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
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized("Ugyldig bruker-ID.");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
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
        _logger.LogInformation("Bruker-ID: {UserId}", userId);
        _logger.LogInformation("Antall ordrelinjer: {Count}", orderDto.OrderItems.Count);

        foreach (var item in orderDto.OrderItems)
        {
            _logger.LogInformation("Produkt-ID: {ProductId}, Antall: {Quantity}", item.ProductId, item.Quantity);
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product == null)
            {
                _logger.LogWarning("Fant ikke produkt med ID {Id}", item.ProductId);
                continue;
            }

            _logger.LogInformation("Produkt: {Name}, Pris: {Price}", product.Name, product.Price);

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
        var sellerId = GetCurrentUserId();
        _logger.LogInformation("AssignOrderToSeller() - SelgerId hentet fra token: {SellerId}", sellerId);

        if (sellerId == null)
        {
            _logger.LogWarning("SelgerId er null!");
            return Unauthorized("Ugyldig selger.");
        }

        var order = await _context.Orders.FindAsync(orderId);
        if (order == null)
        {
            _logger.LogWarning("Ordre ikke funnet med ID: {OrderId}", orderId);
            return NotFound("Bestilling ikke funnet.");
        }

        if (order.SellerId != null)
        {
            _logger.LogWarning("Ordre #{OrderId} er allerede tildelt.", orderId);
            return BadRequest("Bestillingen er allerede tildelt en selger.");
        }

        order.SellerId = sellerId.Value;
        order.Status = "Bekreftet";

        _logger.LogInformation("Ordre #{OrderId} tildelt selger #{SellerId} og satt til 'Bekreftet'", orderId, sellerId);
    
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
