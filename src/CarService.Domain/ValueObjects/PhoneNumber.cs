using System.Text.RegularExpressions;
using CarService.Domain.Exceptions;

namespace CarService.Domain.ValueObjects;

/// <summary>O'zbekiston telefon raqami, +998XXXXXXXXX formatida saqlanadi.</summary>
public sealed partial record PhoneNumber
{
    public string Value { get; }

    private PhoneNumber(string value) => Value = value;

    public static PhoneNumber Create(string raw)
    {
        var digits = NonDigits().Replace(raw ?? string.Empty, string.Empty);
        if (digits.Length == 9) digits = "998" + digits;
        if (digits.Length != 12 || !digits.StartsWith("998"))
            throw new DomainException($"Telefon raqami noto'g'ri: {raw}");
        return new PhoneNumber("+" + digits);
    }

    public override string ToString() => Value;

    [GeneratedRegex(@"\D")]
    private static partial Regex NonDigits();
}
