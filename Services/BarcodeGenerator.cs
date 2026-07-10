using System.Text;
using PttBarkodGenerator.Models;

namespace PttBarkodGenerator.Services;

/// <summary>
/// Service for generating PTT barcode numbers using the standard check digit algorithm.
/// </summary>
public class BarcodeGenerator
{
    private readonly BarcodeConfig _config;

    public BarcodeGenerator(BarcodeConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    /// <summary>
    /// Validates that the 10-digit data string is numeric and exactly 10 characters.
    /// </summary>
    public bool ValidateData(string data, out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            errorMessage = "Data cannot be empty.";
            return false;
        }

        if (data.Length != 10)
        {
            errorMessage = $"Data must be exactly 10 digits. Provided: {data.Length} digits.";
            return false;
        }

        if (!data.All(char.IsDigit))
        {
            errorMessage = "Data must contain only numeric digits (0-9).";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }

    /// <summary>
    /// Calculates the check digit for a 10-digit data string using the PTT algorithm.
    /// </summary>
    /// <remarks>
    /// Algorithm:
    /// 1. Multiply each digit by alternating weights (1, 3, 1, 3, ...)
    /// 2. Sum all products
    /// 3. Check digit = (10 - (sum % 10)) % 10
    ///    If sum is a multiple of 10, check digit is 0.
    /// </remarks>
    public int CalculateCheckDigit(string data)
    {
        if (!ValidateData(data, out _))
            throw new ArgumentException($"Data must be a 10-digit numeric string. Received: '{data}'", nameof(data));

        int sum = 0;
        int[] weights = { 1, 3 };

        for (int i = 0; i < data.Length; i++)
        {
            int digit = data[i] - '0';
            sum += digit * weights[i % 2];
        }

        return (10 - (sum % 10)) % 10;
    }

    /// <summary>
    /// Generates a single barcode string: Prefix + 10-digit data + check digit.
    /// </summary>
    public string GenerateSingle(string data)
    {
        int checkDigit = CalculateCheckDigit(data);
        return $"{_config.Prefix}{data}{checkDigit}";
    }

    /// <summary>
    /// Generates a batch of barcodes starting from the configured start data.
    /// </summary>
    public List<string> GenerateBatch()
    {
        return GenerateBatch(_config.StartData, _config.Quantity);
    }

    /// <summary>
    /// Generates a batch of sequential barcodes starting from the given start data.
    /// </summary>
    public List<string> GenerateBatch(string startData, int quantity)
    {
        if (!ValidateData(startData, out _))
            throw new ArgumentException($"StartData must be a 10-digit numeric string. Received: '{startData}'", nameof(startData));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0.", nameof(quantity));

        var barcodes = new List<string>(quantity);

        // Parse start data as a long integer for sequential generation
        if (!long.TryParse(startData, out long currentNumber))
            throw new InvalidOperationException($"Cannot parse start data as a number: '{startData}'");

        for (int i = 0; i < quantity; i++)
        {
            string data = currentNumber.ToString("D10"); // Pad to 10 digits with leading zeros
            string barcode = GenerateSingle(data);
            barcodes.Add(barcode);
            currentNumber++;
        }

        return barcodes;
    }
}
