using Consumer.Contracts.Callback;
using Dapper;
using System.Data;

namespace Consumer.Repository.MySQL
{
    public class SaveDepositCallbackService : ISaveDepositCallbackService
    {
        private readonly IDbConnection _connection;

        public SaveDepositCallbackService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async ValueTask<DepositCallbackResponse> SaveDepositCallback(DepositCallbackResponse response, string merchantName)
        {
            try
            {
                var connection = _connection;

                connection.Open();

                string query = $"insert into {merchantName}DepositCallback(UserID,TransactionID,Address,DepositAmount,DepositFee,GivenCredit) " +
                    "values(@UserID,@TransactionID,@Address,@DepositAmount,@DepositFee,@GivenCredit)";

                var parameters = new DepositCallbackResponse
                {
                    UserID = response.UserID,
                    TransactionID = response.TransactionID,
                    Address = response.Address,
                    DepositAmount = response.DepositAmount,
                    DepositFee = response.DepositFee,
                    GivenCredit = response.GivenCredit
                };

                await connection.ExecuteAsync(query, parameters);

                connection.Close();

                return parameters;


            }
            catch (Exception ex)
            {
                throw new Exception(nameof(SaveDepositCallbackService), ex);
            }

        }
    }
}
