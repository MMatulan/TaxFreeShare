using System.Globalization;
using System.Text;
using TaxFreeShareAPI.Data;
using TaxFreeShareAPI.Models;

namespace TaxFreeShareAPI.Services;

public class ProductService
{
    private readonly TaxFreeDbContext _context;

    public ProductService(TaxFreeDbContext context)
    {
        _context = context;
    }

    public async Task ImportProductsFromCsv(string csvFilePath)
    {
        if (!File.Exists(csvFilePath))
        {
            throw new FileNotFoundException($"Filen {csvFilePath} ble ikke funnet.");
        }

        var products = new List<Product>();

        using (var sr = new StreamReader(csvFilePath, Encoding.UTF8))
        {
            int lineNo = 0;
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                lineNo++;

                // Hopp over første linje hvis det er en header
                if (lineNo == 1) continue;

                string[] values = line.Split(',');

                if (values.Length < 5) // Skal være 5 kolonner pga. Id-kolonnen
                {
                    Console.WriteLine($" Feil format på linje {lineNo}, hoppes over.");
                    continue;
                }

                try
                {
                    // Hopp over Id-kolonnen (values[0]) og hent riktige verdier
                    string name = values[1].Trim();
                    string category = values[2].Trim();
                    string brand = values[3].Trim();
                    string priceString = values[4].Replace(",", ".").Trim();

                    if (!decimal.TryParse(priceString, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
                    {
                        Console.WriteLine($" Feil på linje {lineNo}: Ugyldig prisverdi '{values[4]}'. Hoppes over.");
                        continue;
                    }

                    var product = new Product
                    {
                        Name = name,
                        Category = category,
                        Brand = brand,
                        Price = price
                    };

                    products.Add(product);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" Feil på linje {lineNo}: {ex.Message}");
                }
            }
        }

        // Lagre i database
        if (products.Count > 0)
        {
            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✅ {products.Count} produkter importert.");
        }
        else
        {
            Console.WriteLine("⚠ Ingen produkter ble importert. Sjekk CSV-filen for feil.");
        }
    }
}