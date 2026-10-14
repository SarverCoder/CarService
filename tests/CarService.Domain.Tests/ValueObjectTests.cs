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
