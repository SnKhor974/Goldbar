using System.Globalization;

namespace Consumer.Contracts.Callback
{
    /// <summary>
    /// deposit callback response for frontend
    /// </summary>
    public class DepositCallbackResponse
    {
        public string UserID { get; set; } = string.Empty;
        public string TransactionID { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public double DepositAmount { get; set; }
        public double DepositFee { get; set; }
        public double GivenCredit { get; set; }

    }
}
