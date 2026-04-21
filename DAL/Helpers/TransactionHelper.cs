using System;
using System.Data.SqlClient;

namespace MySellerApp.DAL.Helpers
{
    public static class TransactionHelper
    {
        public static void Execute(string connString, Action<SqlConnection, SqlTransaction> action)
        {
            using (var con = new SqlConnection(connString))
            {
                con.Open();
                using (var tran = con.BeginTransaction())
                {
                    try
                    {
                        action(con, tran);
                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        public static int GetIdentity(SqlCommand cmd)
        {
            cmd.CommandText += "; SELECT SCOPE_IDENTITY();";
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
    }
}