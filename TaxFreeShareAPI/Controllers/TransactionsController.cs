using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaxFreeShareAPI.Data;
using TaxFreeShareAPI.Models;
using TaxFreeShareAPI.Models.TransactionsDto;

namespace TaxFreeShareAPI.Controllers;


    [ApiController]
    [Route("api/transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly TaxFreeDbContext _context;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(TaxFreeDbContext context, ILogger<TransactionsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Opprette en transaksjon
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionDto transactionDto)
        {
            _logger.LogInformation("Forsøker å opprette en ny transaksjon.");

            var order = await _context.Orders.FindAsync(transactionDto.OrderId);
            if (order == null)
            {
                _logger.LogWarning($"Bestilling med ID {transactionDto.OrderId} ble ikke funnet.");
                return NotFound("Bestillingen finnes ikke.");
            }

            var traveler = await _context.Users.FindAsync(transactionDto.TravelerId);
            if (traveler == null)
            {
                _logger.LogWarning($"Reisende med ID {transactionDto.TravelerId} ble ikke funnet.");
                return NotFound("Reisende finnes ikke.");
            }

            var transaction = new Transaction
            {
                OrderId = transactionDto.OrderId,
                TravelerId = transactionDto.TravelerId,
                Amount = transactionDto.Amount,
                Status = "Unpaid",
                CreatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Transaksjon {transaction.Id} opprettet for bestilling {transactionDto.OrderId}.");

            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, new TransactionDto
            {
                Id = transaction.Id,
                OrderId = transaction.OrderId,
                TravelerId = transaction.TravelerId,
                Amount = transaction.Amount,
                Status = transaction.Status,
                CreatedAt = transaction.CreatedAt
            });
        }

        // Hente en transaksjon
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<TransactionDetailsDto>> GetTransaction(int id)
        {
            _logger.LogInformation($"Henter transaksjon med ID {id}.");

            var transaction = await _context.Transactions
                .Include(t => t.Order)
                .Include(t => t.Traveler)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null) return NotFound("Transaksjonen ble ikke funnet.");

            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                _logger.LogWarning("Kunne ikke hente e-post fra token.");
                return Unauthorized("Kunne ikke hente brukerens e-post.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
            {
                _logger.LogWarning($"Brukeren {userEmail} finnes ikke.");
                return Unauthorized("Brukeren finnes ikke.");
            }

            int userId = user.Id;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (transaction.Order.UserId != userId && transaction.TravelerId != userId && userRole != "Admin")
            {
                _logger.LogWarning($"Bruker {userEmail} ({userId}) forsøkte å hente transaksjon {id} uten tillatelse.");
                return Forbid();
            }

            var detailsDto = new TransactionDetailsDto
            {
                Id = transaction.Id,
                OrderId = transaction.OrderId,
                TravelerId = transaction.TravelerId,
                TravelerEmail = transaction.Traveler?.Email ?? "",
                Amount = transaction.Amount,
                Status = transaction.Status,
                CreatedAt = transaction.CreatedAt,
                OrderDate = transaction.Order.OrderDate,
                OrderTotalAmount = transaction.Order.TotalAmount
            };

            return Ok(detailsDto);
        }



        // Oppdatere transaksjonsstatus (bare admin)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTransactionStatus(int id, [FromBody] UpdateTransactionStatusDto statusDto)
        {
            _logger.LogInformation($"Admin oppdaterer transaksjon {id} til status {statusDto.Status}.");

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                _logger.LogWarning($"Transaksjon med ID {id} ble ikke funnet.");
                return NotFound("Transaksjonen finnes ikke.");
            }

            if (statusDto.Status != "Paid" && statusDto.Status != "Unpaid")
            {
                _logger.LogWarning($"Ugyldig status {statusDto.Status} sendt for transaksjon {id}.");
                return BadRequest("Ugyldig status. Velg 'Paid' eller 'Unpaid'.");
            }

            transaction.Status = statusDto.Status;
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Transaksjon {id} er oppdatert til {transaction.Status}.");

            return Ok($"Transaksjon {id} er oppdatert til {transaction.Status}.");
        }
    }    