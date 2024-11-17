using PaymentGatewayAPI.Contracts.TransactionDetails;
using Dapper;
using System.Data;
using Microsoft.Extensions.Options;
using PaymentGatewayAPI.Contracts.MerchantSettings;
using System.Data.Common;
using PaymentGatewayAPI.Contracts.Order;

namespace PaymentGatewayAPI.Repository.MySQL
{
    public class SaveWithdrawTransactionDetailsService : ISaveWithdrawTransactionDetailsService
    {
        private readonly IDbConnection _connection;

        public SaveWithdrawTransactionDetailsService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async ValueTask<WithdrawTransactionDetails> SaveWithdrawTransactionDetails(Guid id, WithdrawRequest request)
        {
            try
            {
                var connection = _connection;

                connection.Open();

                string query = @"insert into WithdrawTransaction(OrderID,MerchantName,UserID,WalletAddress,TransactionID,Token,Network,TotalAmount,DateTime) 
                                                values(@OrderID,@MerchantName,@UserID,@WalletAddress,@TransactionID,@Token,@Network,@TotalAmount,@DateTime)";

                var parameters = new WithdrawTransactionDetails()
                {
                    OrderID = id,
                    MerchantName = request.MerchantName,
                    UserID = request.UserID,
                    WalletAddress = request.WalletAddress,
                    TransactionID = request.TransactionID,
                    Token = request.Token,
                    Network = request.Network,
                    TotalAmount = request.TotalAmount,
                    DateTime = request.DateTime
                };

                await connection.ExecuteAsync(query, parameters);

                connection.Close();

                return parameters;


            }
            catch (Exception ex)
            {
                throw new Exception(nameof(SaveWithdrawTransactionDetails), ex);

            }

        }
    }
}