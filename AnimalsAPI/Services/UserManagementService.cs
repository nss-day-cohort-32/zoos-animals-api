using AnimalsAPI.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace AnimalsAPI.Services
{
    public class UserManagementService : IUserManagementService
    {
        private string _connectionString;

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_connectionString);
            }
        }

        public bool IsValidUser(string connectionString, string userName, string password)
        {
            _connectionString = connectionString;

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Username, [Password]
                        FROM Users
                        WHERE Username = @username AND [Password] = @password";
                    cmd.Parameters.Add(new SqlParameter("@username", userName));
                    cmd.Parameters.Add(new SqlParameter("@password", password));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }

        }
    }

}
