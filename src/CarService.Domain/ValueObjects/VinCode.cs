using System.Text.RegularExpressions;
using CarService.Domain.Exceptions;

namespace CarService.Domain.ValueObjects;

/// <summary>17 belgili VIN kod (I, O, Q harflarisiz).</summary>
public sealed partial record VinCode
{
    public string Value { get; }

    private VinCode(string value) => Value = value;

    public static VinCode Create(string raw)
    {
        var value = (raw ?? string.Empty).Trim().ToUpperInvariant();
        if (!VinPattern().IsMatch(value))
            throw new DomainException($"VIN kod noto'g'ri: {raw}");
        return new VinCode(value);
    }

    public override string ToString() => Value;

    [GeneratedRegex("^[A-HJ-NPR-Z0-9]{17}$")]
    private static partial Regex VinPattern();
}
