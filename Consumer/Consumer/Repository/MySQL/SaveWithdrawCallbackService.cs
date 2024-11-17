using Consumer.Contracts.Callback;
using System.Data;
using Dapper;

namespace Consumer.Repository.MySQL
{
    public class SaveWithdrawCallbackService : ISaveWithdrawCallbackService
    {
        private readonly IDbConnection _connection;

        public SaveWithdrawCallbackService(IDbConnection connection)
        {
            _connection = connection;
        }

        public async ValueTask<WithdrawCallbackResponse> SaveWithdrawCallback(WithdrawCallbackResponse response, string merchantName)
        {
            try
            {

                var connection = _connection;

                connection.Open();

                string query = $"insert into {merchantName}WithdrawCallback(UserID,TransactionID,Address,WithdrawAmount,WithdrawFee,Amount) " +
                    "values(@UserID,@TransactionID,@Address,@WithdrawAmount,@WithdrawFee,@Amount)";

                var parameters = new WithdrawCallbackResponse
                {
                    UserID = response.UserID,
                    TransactionID = response.TransactionID,
                    Address = response.Address,
                    WithdrawAmount = response.WithdrawAmount,
                    WithdrawFee = response.WithdrawFee,
                    Amount = response.Amount
                };

                await connection.ExecuteAsync(query, parameters);

                connection.Close();

                return parameters;


            }
            catch (Exception ex)
            {
                throw new Exception(nameof(SaveWithdrawCallbackService), ex);
            }
        }
    }
}
