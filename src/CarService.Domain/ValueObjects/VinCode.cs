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

    /// <summary>Ishlab chiqaruvchi kodi (WMI): VIN'ning birinchi 3 belgisi.</summary>
    public string Wmi => Value[..3];

    /// <summary>
    /// 10-belgidan model yilini aniqlaydi. Kod har 30 yilda takrorlanadi,
    /// shuning uchun <paramref name="currentYear"/> + 1 dan oshmaydigan eng yangi yil olinadi.
    /// </summary>
    public int? GetModelYear(int currentYear)
    {
        var index = YearCodes.IndexOf(Value[9]);
        if (index < 0) return null;
        var year = 2010 + index;
        while (year > currentYear + 1) year -= 30;
        return year;
    }

    public override string ToString() => Value;

    // A=2010 ... Y=2030, 1=2031 ... 9=2039 (I, O, Q, U, Z, 0 ishlatilmaydi)
    private const string YearCodes = "ABCDEFGHJKLMNPRSTVWXY123456789";

    [GeneratedRegex("^[A-HJ-NPR-Z0-9]{17}$")]
    private static partial Regex VinPattern();
}
