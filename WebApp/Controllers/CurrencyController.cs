using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Constants;
using WebApp.Exceptions;
using WebApp.Interfaces;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class CurrencyController : Controller
    {
        private readonly ILogger<CurrencyController> _logger;
        private readonly ICurrenyConversionService _currenyService;
        private readonly IFetchRecordRepository _fetchRecordRepository;
        public CurrencyController(ILogger<CurrencyController> logger, ICurrenyConversionService currenyService, IFetchRecordRepository fetchRecordRepository)
        {
            _logger = logger;
            _currenyService = currenyService;
            _fetchRecordRepository = fetchRecordRepository;
        }

        [Authorize]
        [HttpGet("/Currency/GetDKKEquivalent/{currency}")]
        public async Task<ActionResult> GetDKKEquivalent([FromRoute]string currency, [FromQuery]long value, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(currency))
            {
                return new BadRequestObjectResult("Currency parameter is required.");
            }
            double result;
            try
            {
                // service call
                result = await _currenyService.GetDKKEquivalentOf(currency, value, cancellationToken);
                await _fetchRecordRepository.StoreCurrencyRecordData(
                    new ConvertedRecord { Currency = currency, DateTime = DateTime.Now, Rate = result }, 
                    cancellationToken
                );
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while fetching DKK equivalent data");
                return HandleException(e);
            }
            return Ok(
                new
                {
                    Currency = currency,
                    DKKRate = result
                }
            );
        }

        [Authorize(AuthorizationPolicyConstants.AdminPolicyName)]
        [HttpGet("/Currency/GetCurrencyRates")]
        public async Task<ActionResult> GetAllCurrencyRates(CancellationToken cancellationToken = default)
        {
            try
            {
                var list = await _currenyService.GetAllCurrenciesAsync(cancellationToken);
                return Ok(list);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while fetching Currency Rates data");
                return HandleException(e);
            }
        }


        [Authorize]
        [HttpGet("/Currency/GetCurrencyFetchHistory")]
        public async Task<ActionResult> GetCurrencyFetchHistory(
            DateTime from, 
            DateTime to,
            string currency,
            int offset = 0,
            int limit = 0,
            CancellationToken cancellationToken = default
        ) {
            try
            {
                if (from > to)
                {
                    return new BadRequestObjectResult("From date should not be greater than to date.");
                }
                if (offset < 0)
                {
                    return new BadRequestObjectResult("Invalid Offset, should be atleast 0 or greater than 0.");
                }
                if (limit < 1)
                {
                    return new BadRequestObjectResult("Invalid limit. should be atleast 1 or greater than 1");
                }
                var list = await _fetchRecordRepository.GetCurrencyRecordData(from, to, currency, offset, limit, cancellationToken);
                if (list != null && list.Count > 0)
                {
                    return Ok(list);
                }
                return new NotFoundObjectResult("Couldn't find any data for the input filter, please try again with different input combinations.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while fetching Currency Rates history data");
                return HandleException(e);
            }
        }

        private ActionResult HandleException(Exception exception)
        {
            return exception switch
            {
                ItemNotFoundException ex => StatusCode(400, "Currency conversion not supported."),
                IOException ex => ReturnInternalError(),
                OverflowException ex => ReturnInternalError(),
                Exception ex => ReturnInternalError()
            };
        }

        private ObjectResult ReturnInternalError()
            => StatusCode(500, "Something went wrong.");

    }
}
