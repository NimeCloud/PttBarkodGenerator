using PttBarkodGenerator.Models;
using PttBarkodGenerator.Services;

// ===========================================
//  Configuration - Hardcoded values
//  Change these values as needed.
// ===========================================
var config = new BarcodeConfig
{
    Prefix = "RR",
    StartData = "0608576613", // 10-digit starting data (without check digit)
    Quantity = 10             // Number of barcodes to generate
};
// ===========================================

// Validate configuration
if (!config.Validate(out string? errorMessage))
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Configuration Error: {errorMessage}");
    Console.ResetColor();
    return;
}

// Initialize the generator
var generator = new BarcodeGenerator(config);

// Display header
Console.WriteLine(new string('=', 50));
Console.WriteLine("  PTT Barkod Üretici");
Console.WriteLine(new string('=', 50));
Console.WriteLine($"  Ön Ek:     {config.Prefix}");
Console.WriteLine($"  Başlangıç: {config.StartData}");
Console.WriteLine($"  Adet:      {config.Quantity}");
Console.WriteLine(new string('=', 50));
Console.WriteLine();

try
{
    // Generate barcodes
    var barcodes = generator.GenerateBatch();

    // Display each barcode
    foreach (var barcode in barcodes)
    {
        Console.WriteLine($"  {barcode}");
    }

    Console.WriteLine();
    Console.WriteLine(new string('=', 50));
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"  Tamamlandı - {barcodes.Count} barkod üretildi.");
    Console.ResetColor();
    Console.WriteLine(new string('=', 50));
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"  Hata: {ex.Message}");
    Console.ResetColor();
}
