using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using WebApp.Exceptions;
using WebApp.Interfaces;
using WebApp.Models;
using WebApp.Repositories;
using WebApp.Services;

namespace WebApp.Test.Services
{
    public class CurrencyConversionServiceTest
    {
        private readonly Mock<ILogger<CurrencyConversionService>> _logger;
        private readonly Mock<IDKBankService> _dKBankService;
        private readonly CurrencyConversionService _currencyConversionService;

        public CurrencyConversionServiceTest()
        {
            _logger = new Mock<ILogger<CurrencyConversionService>>();
            _dKBankService = new Mock<IDKBankService>();

            var mem = new Mock<IMemoryCache>();
            var cacheRepository = new CacheRepository(mem.Object);
            
            var fileLogger = new Mock<ILogger<FileRepository>>();
            var fileRepository = new FileRepository(fileLogger.Object);
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMemoryCache();
            serviceCollection.AddSingleton(cacheRepository);
            serviceCollection.AddSingleton(fileRepository);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            _currencyConversionService = new CurrencyConversionService(_logger.Object, serviceProvider, _dKBankService.Object);
        }

        [Theory]
        [InlineData("ABC", 10)]
        public async Task GetDKKEquivalentOf_ShouldReturnItemNotFoundExceptionWhenCurrencyIsInvalid(string currency, long value)
        {
            // Arrange
            CancellationToken cancellationToken = default;

            // Act
            try
            {
                var result = await _currencyConversionService.GetDKKEquivalentOf(currency, value, cancellationToken);
            }
            catch (Exception ex)
            {
                // Assert
                Assert.IsType<ItemNotFoundException>(ex);
            }
        }

        [Theory]
        [InlineData("USD", 10)]
        public async Task GetDKKEquivalentOf_ShouldReturnDataWhenCurrencyIsValid(string currency, long value)
        {
            // Arrange
            CancellationToken cancellationToken = default;

            // Act
            double result = 0;
            try
            {
                result = await _currencyConversionService.GetDKKEquivalentOf(currency, value, cancellationToken);
            }
            catch (Exception)
            {
                // Assert
                Assert.Fail("Should not throw any exception.");
            }
            Assert.IsType<double>(result);
        }
    }
}
