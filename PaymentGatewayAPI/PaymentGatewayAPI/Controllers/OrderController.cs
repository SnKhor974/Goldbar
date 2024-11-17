using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using PaymentGatewayAPI.Services.Deposit.Interfaces;
using PaymentGatewayAPI.Services.Withdraw.Interfaces;
using Microsoft.Extensions.Options;
using PaymentGatewayAPI.Contracts.MerchantSettings;
using PaymentGatewayAPI.Contracts.Order;
using PaymentGatewayAPI.Services.Deposit.Coin2Pay;
using PaymentGatewayAPI.Contracts.Enumerables;
using PaymentGatewayAPI.Contracts;
using PaymentGatewayAPI.Repository.MySQL;
using PaymentGatewayAPI.Services.Withdraw.Coin2Pay.Coin2PayExtensions.Contract;
using PaymentGatewayAPI.Services.Withdraw.Coin2Pay.Coin2PayExtensions.Interface;


namespace PaymentGatewayAPI.Controllers
{

    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IEnumerable<ICreateDepositService> _depositServicesEnum;
        private readonly IEnumerable<ICreateWithdrawService> _withdrawServicesEnum;

 
        private readonly ISaveDepositTransactionDetailsService _saveDepositTransactionDetails;
        private readonly ISaveWithdrawTransactionDetailsService _saveWithdrawTransactionDetails;
        private readonly ILogger<OrderController> _logger;
        private readonly IOptions<List<MerchantSettings>> _options;

        private readonly ICoin2PayWithdrawalEstimationAsset _coin2PayWithdrawalEstimationAsset;

        public OrderController(ICoin2PayWithdrawalEstimationAsset coin2PayWithdrawalEstimationAsset, ILogger<OrderController> logger, ISaveWithdrawTransactionDetailsService saveWithdrawTransactionDetails, ISaveDepositTransactionDetailsService saveDepositTransactionDetails, IOptions<List<MerchantSettings>> options, IEnumerable<ICreateDepositService> depositServicesEnum, IEnumerable<ICreateWithdrawService> withdrawServicesEnum)
        {
            
            _depositServicesEnum = depositServicesEnum;
            _withdrawServicesEnum = withdrawServicesEnum;
            _logger = logger;
            _saveDepositTransactionDetails = saveDepositTransactionDetails;
            _saveWithdrawTransactionDetails = saveWithdrawTransactionDetails;
            _options = options;
            _coin2PayWithdrawalEstimationAsset = coin2PayWithdrawalEstimationAsset;
        }

        [HttpPost("/CreateDepositOrder/{merchantName}")]
        public async ValueTask<IActionResult> CreateDepositOrder(DepositRequest request, string merchantName)
        {
            //<summary>
            //type = MerchantSettings.cs (class or json) / null
            //"_options.value" are values stored in appsettings.json, the lambda iterates through the values of "MerchantName" in appsettings.json
            //to match the "MerchantName" from the request
            //</summary>
            var merchantSettings = _options.Value.FirstOrDefault(name => name.MerchantName == merchantName);
            if (merchantSettings == null) return NotFound();

            Guid orderID = Guid.NewGuid();
          

            //<summary>
            //type = DepositTransactionDetails.cs (class) / exception
            //save web request into db
            //</summary>
            var save = await _saveDepositTransactionDetails.SaveDepositTransactionDetails(orderID, request);

            //<summary>
            //type = ICreateDepositService.cs (interface) / null
            //"_depositServiceEnum" are classes that implemented "ICreateDepositService", the lambda iterates through the values of attribute "Merchant" to
            //match the "MerchantName" from the request
            //</summary>
            var selectMerchant = _depositServicesEnum.FirstOrDefault(name => name.Merchants.ToString() == merchantName);
            if (selectMerchant == null) return NotFound();

            //<summary>
            //type = DepositResponse.cs (class or json)
            //create deposit with selected merchant
            //</summary>
            var result = await selectMerchant.CreateDepositOrder(orderID, request, merchantSettings);
            
            return Ok(result);
        }



        [HttpPost("/CreateWithdrawOrder/{merchantName}")]
        public async ValueTask<IActionResult> CreateWithdrawOrder(WithdrawRequest request, string merchantName)
        {

            var merchantSettings = _options.Value.FirstOrDefault(name => name.MerchantName == merchantName);
            if (merchantSettings == null) return NotFound();

            Guid id = Guid.NewGuid();

            var save = await _saveWithdrawTransactionDetails.SaveWithdrawTransactionDetails(id, request);


            var selectMerchant = _withdrawServicesEnum.FirstOrDefault(name => name.Merchants.ToString() == merchantName);
            if (selectMerchant == null) return NotFound();


            var result = await selectMerchant.CreateWithdrawOrder(id, request, merchantSettings);

            return Ok(result);
        }

        //Coin2Pay credit estimation
        [HttpPost("/WithdrawalEstimationAsset/Coin2Pay")]
        public async ValueTask<IActionResult> VerifyWithdraw(WithdrawalEstimationAssetRequest request)
        {
            var merchantSettings = _options.Value.FirstOrDefault(name => name.MerchantName == "Coin2Pay");
            if (merchantSettings == null) return NotFound();

            var result = await _coin2PayWithdrawalEstimationAsset.WithdrawalEstimationAsset(request, merchantSettings);

            return Ok(result);
        }



    }
}

