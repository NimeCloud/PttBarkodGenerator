namespace PttBarkodGenerator.Models;

/// <summary>
/// Configuration model for PTT barcode generation.
/// </summary>
public class BarcodeConfig
{
    /// <summary>
    /// Static prefix for all generated barcodes (default: "RR").
    /// </summary>
    public string Prefix { get; set; } = "RR";

    /// <summary>
    /// Starting 10-digit data (without check digit).
    /// Example: "0608576613"
    /// </summary>
    public string StartData { get; set; } = string.Empty;

    /// <summary>
    /// Number of barcodes to generate.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Validates that the configuration is correct.
    /// </summary>
    public bool Validate(out string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(StartData))
        {
            errorMessage = "StartData cannot be empty.";
            return false;
        }

        if (StartData.Length != 10)
        {
            errorMessage = $"StartData must be exactly 10 digits. Provided: {StartData.Length} digits.";
            return false;
        }

        if (!StartData.All(char.IsDigit))
        {
            errorMessage = "StartData must contain only numeric digits (0-9).";
            return false;
        }

        if (Quantity <= 0)
        {
            errorMessage = "Quantity must be greater than 0.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }
}
