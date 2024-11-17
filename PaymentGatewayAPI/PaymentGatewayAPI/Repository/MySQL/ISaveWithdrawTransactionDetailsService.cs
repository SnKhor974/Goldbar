using PaymentGatewayAPI.Contracts.Order;

using PaymentGatewayAPI.Contracts.TransactionDetails;
using System.Data;

namespace PaymentGatewayAPI.Repository.MySQL
{
    public interface ISaveWithdrawTransactionDetailsService
    {

        ValueTask<WithdrawTransactionDetails> SaveWithdrawTransactionDetails(Guid id, WithdrawRequest request);
    }
}
