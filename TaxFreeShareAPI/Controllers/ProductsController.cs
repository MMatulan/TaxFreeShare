using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaxFreeShareAPI.Data;
using TaxFreeShareAPI.Models;
using TaxFreeShareAPI.Models.ProductDto;

namespace TaxFreeShareAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly TaxFreeDbContext _context;
    private readonly ILogger<ProductsController> _logger;
    
    public ProductsController(TaxFreeDbContext context, ILogger<ProductsController> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    // GET: api/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        _logger.LogInformation("Henter alle produkter.");

        var products = await _context.Products
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Category = p.Category,
                Price = p.Price,
                Brand = p.Brand
            }).ToListAsync();

        _logger.LogInformation($"Fant {products.Count} produkter.");
        return Ok(products);
    }
    
    
    // GET: api/products/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        _logger.LogInformation($"Henter produkt med ID {id}.");

        var product = await _context.Products
            .Where(p => p.Id == id)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Category = p.Category,
                Price = p.Price,
                Brand = p.Brand
            }).FirstOrDefaultAsync();

        if (product == null)
        {
            _logger.LogWarning($"Produkt med ID {id} ble ikke funnet.");
            return NotFound();
        }

        _logger.LogInformation($"Fant produkt: {product.Name} (ID: {id}).");
        return Ok(product);
    }
    
    // POST: api/products
    [HttpPost]
    public async Task<ActionResult<ProductDto>> PostProduct([FromBody] CreateProductDto productDto)
    {
        _logger.LogInformation($"Oppretter nytt produkt: {productDto.Name}.");

        var product = new Product
        {
            Name = productDto.Name,
            Category = productDto.Category,
            Price = productDto.Price,
            Brand = productDto.Brand,
            CreatedAt = DateTime.UtcNow 
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var responseDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Category = product.Category,
            Price = product.Price,
            Brand = product.Brand
        };

        _logger.LogInformation($"Produkt opprettet med ID {product.Id}.");
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, responseDto);
    }
    
    // PUT: api/products/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduct(int id, [FromBody] UpdateProductDto productDto)
    {
        _logger.LogInformation($"Oppdaterer produkt med ID {id}.");

        var existingProduct = await _context.Products.FindAsync(id);

        if (existingProduct == null)
        {
            _logger.LogWarning($"Produkt med ID {id} ble ikke funnet.");
            return NotFound();
        }

        // Oppdaterer feltene
        existingProduct.Name = productDto.Name;
        existingProduct.Category = productDto.Category;
        existingProduct.Price = productDto.Price;
        existingProduct.Brand = productDto.Brand;

        await _context.SaveChangesAsync();
        _logger.LogInformation($"Produkt med ID {id} ble oppdatert.");
        return NoContent();
    }
    
    
    // DELETE: api/products/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        _logger.LogInformation($"Forsøker å slette produkt med ID {id}.");

        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            _logger.LogWarning($"Produkt med ID {id} ble ikke funnet ved sletting.");
            return NotFound();
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Produkt med ID {id} ble slettet.");
        return NoContent();
    }
    
}