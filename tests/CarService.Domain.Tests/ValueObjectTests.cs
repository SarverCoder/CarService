using CarService.Domain.Exceptions;
using CarService.Domain.ValueObjects;

namespace CarService.Domain.Tests;

public class PhoneNumberTests
{
    [Theory]
    [InlineData("+998 90 123 45 67")]
    [InlineData("998901234567")]
    [InlineData("90-123-45-67")]
    public void Create_normalizes_to_international_format(string raw) =>
        Assert.Equal("+998901234567", PhoneNumber.Create(raw).Value);

    [Theory]
    [InlineData("")]
    [InlineData("12345")]
    [InlineData("+7 900 123 45 67")]
    public void Create_rejects_invalid_numbers(string raw) =>
        Assert.Throws<DomainException>(() => PhoneNumber.Create(raw));
}

public class VinCodeTests
{
    [Fact]
    public void Create_uppercases_valid_vin() =>
        Assert.Equal("XTA21099023184727", VinCode.Create(" xta21099023184727 ").Value);

    [Theory]
    [InlineData("XTA2109902318472")]   // 16 belgi
    [InlineData("XTA2109902318472O")]  // O harfi taqiqlangan
    public void Create_rejects_invalid_vin(string raw) =>
        Assert.Throws<DomainException>(() => VinCode.Create(raw));
}

public class VinModelYearTests
{
    [Theory]
    [InlineData("XWB3L32EDHA012345", 2017)]  // H = 2017
    [InlineData("KMHD841CBMU123456", 2021)]  // M = 2021
    [InlineData("XTA21099023184727", 2002)]  // 2 = 2032 → 30 yil oldin
    public void GetModelYear_decodes_tenth_character(string vin, int expected) =>
        Assert.Equal(expected, VinCode.Create(vin).GetModelYear(2026));

    [Fact]
    public void Wmi_is_first_three_characters() =>
        Assert.Equal("XWB", VinCode.Create("XWB3L32EDHA012345").Wmi);
}
