using PaymentGatewayAPI.Contracts;
using PaymentGatewayAPI.Contracts.TransactionDetails;
using System.Data;

namespace PaymentGatewayAPI.Repository.MySQL
{
    public interface ISaveDepositTransactionDetailsService
    {
        ValueTask<DepositTransactionDetails> SaveDepositTransactionDetails(Guid orderID, DepositRequest request);
    }
}
