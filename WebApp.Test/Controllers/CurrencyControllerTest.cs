using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebApp.Controllers;
using WebApp.Interfaces;
using WebApp.Models;
using WebApp.Repositories;

namespace WebApp.Test.Controllers
{
    public class CurrencyControllerTest
    {
        private readonly Mock<ICurrenyConversionService> _currenyConversionService;
        private readonly Mock<IFetchRecordRepository> _fetchRecordRepository;
        private readonly Mock<ILogger<CurrencyController>> _logger;
        private readonly CurrencyController _currencyController;

        public CurrencyControllerTest()
        {
            _logger = new Mock<ILogger<CurrencyController>>();
            _currenyConversionService = new Mock<ICurrenyConversionService>();
            _fetchRecordRepository = new Mock<IFetchRecordRepository>();
            _currencyController = new CurrencyController(_logger.Object, _currenyConversionService.Object, _fetchRecordRepository.Object);
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

        [Fact]
        public async Task GetCurrencyFetchHistory_ShouldReturnBadRequestWhenInputDataIsInvalid()
        {
            // Arrange
            DateTime from = DateTime.Today;
            DateTime to = DateTime.Today.AddDays(-1);
            string currency = "USD";
            int offset = 0, limit = 10;

            // Act
            var response = await _currencyController.GetCurrencyFetchHistory(from, to, currency, offset, limit);

            // Assert
            Assert.IsType<BadRequestObjectResult>(response);

            // Arrange
            from = DateTime.Today;
            to = DateTime.Today;
            currency = "USD";
            offset = -1; 
            limit = 10;

            // Act
            response = await _currencyController.GetCurrencyFetchHistory(from, to, currency, offset, limit);

            // Assert
            Assert.IsType<BadRequestObjectResult>(response);

            // Arrange
            from = DateTime.Today;
            to = DateTime.Today;
            currency = "USD";
            offset = 0;
            limit = 0;

            // Act
            response = await _currencyController.GetCurrencyFetchHistory(from, to, currency, offset, limit);

            // Assert
            Assert.IsType<BadRequestObjectResult>(response);
        }

        [Fact]
        public async Task GetCurrencyFetchHistory_ShouldReturnListWhenInputDataIsProvided()
        {
            // Arrange
            DateTime from = DateTime.Today;
            DateTime to = DateTime.Today;
            string currency = "USD";
            int offset = 0, limit = 10;
            _fetchRecordRepository.Setup(x => x.GetCurrencyRecordData(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(GetConvertedRecords());

            // Act
            var response = await _currencyController.GetCurrencyFetchHistory(from, to, currency, offset, limit);

            // Assert
            Assert.IsType<OkObjectResult>(response);
        }

        private static List<ConvertedRecord> GetConvertedRecords()
        {
            return new List<ConvertedRecord> {
                new ConvertedRecord {
                    Currency = "USD",
                    Rate = 10,
                    DateTime = DateTime.Now
                },
                new ConvertedRecord {
                    Currency = "INR",
                    Rate = 30,
                    DateTime = DateTime.Now
                },
                new ConvertedRecord {
                    Currency = "AUD",
                    Rate = 20,
                    DateTime = DateTime.Now
                }
            };
        }
    }
}
