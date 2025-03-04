using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebApp.Controllers;
using WebApp.Interfaces;
using WebApp.Models;

namespace WebApp.Test.Controllers
{
    public class CurrencyControllerTest
    {
        private readonly Mock<ICurrenyConversionService> _currenyConversionService;
        private readonly Mock<ILogger<CurrencyController>> _logger;
        private readonly CurrencyController _currencyController;

        public CurrencyControllerTest()
        {
            _logger = new Mock<ILogger<CurrencyController>>();
            _currenyConversionService = new Mock<ICurrenyConversionService>();
            _currencyController = new CurrencyController(_logger.Object, _currenyConversionService.Object);
        }

        [Theory]
        [InlineData("", 0)]
        public async Task GetDKKEquivalent_ShouldReturnBadRequestWhenDataIsNotProvided(string currency, long value)
        {
            // Arrange

            // Act
            var response = await _currencyController.GetDKKEquivalent(currency, value);

            // Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Theory]
        [InlineData("USD", 20, 0)]
        public async Task GetDKKEquivalent_ShouldReturnResponseWhenDataIsProvided(string currency, long value, double output)
        {
            // Arrange
            _currenyConversionService.Setup((c) => c.GetDKKEquivalentOf(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<CancellationToken>())).ReturnsAsync(output);

            // Act
            var response = await _currencyController.GetDKKEquivalent(currency, value);

            // Assert
            Assert.IsType<OkObjectResult>(response);
        }

        [Fact]
        public async Task GetAllCurrencyRates_ShouldReturnList()
        {
            var list = new List<Currency>();
            // Arrange
            _currenyConversionService.Setup(c => c.GetAllCurrenciesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(list);

            // Act
            var response = await _currencyController.GetAllCurrencyRates();

            //Assert
            Assert.IsType<OkObjectResult>(response);
        }
    }
}
