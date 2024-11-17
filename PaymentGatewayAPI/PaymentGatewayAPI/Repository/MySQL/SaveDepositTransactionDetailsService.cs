using MySql.Data.MySqlClient;
using PaymentGatewayAPI.Contracts.Order;
using PaymentGatewayAPI.Contracts.TransactionDetails;
using Dapper;
using System.Data;
using Microsoft.Extensions.Options;
using PaymentGatewayAPI.Contracts.MerchantSettings;
using System.Data.Common;
using PaymentGatewayAPI.Contracts;

namespace PaymentGatewayAPI.Repository.MySQL
{
    public class SaveDepositTransactionDetailsService : ISaveDepositTransactionDetailsService
    {
        private readonly IDbConnection _connection;

        public SaveDepositTransactionDetailsService(IDbConnection connection)
        {
            _connection = connection;

        }
        public async ValueTask<DepositTransactionDetails> SaveDepositTransactionDetails(Guid orderID, DepositRequest request)
        {
            try
            {
                var connection = _connection;

                connection.Open();

                string query = @"insert into DepositTransaction(OrderID,MerchantName,UserID,WalletAddress,TransactionID,Currency,TotalAmount,DateTime) 
                                                values(@OrderID,@MerchantName,@UserID,@WalletAddress,@TransactionID,@Currency,@TotalAmount,@DateTime)";

                var parameters = new DepositTransactionDetails()
                {
                    OrderID = orderID,
                    MerchantName = request.MerchantName,
                    UserID = request.UserID,
                    WalletAddress = request.WalletAddress,
                    TransactionID = request.TransactionID,
                    Currency = request.Currency,
                    TotalAmount = request.TotalAmount,
                    DateTime = request.DateTime
                };

                await connection.ExecuteAsync(query, parameters);

                connection.Close();

                return parameters;


            }
            catch (Exception ex)
            {
                throw new Exception(nameof(SaveDepositTransactionDetails), ex);

            }

        }
    }
}