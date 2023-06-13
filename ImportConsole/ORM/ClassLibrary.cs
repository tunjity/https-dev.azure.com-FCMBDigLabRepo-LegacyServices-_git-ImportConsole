using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ImportConsole.ORM
{
    class ClassLibrary
    {
        public static List<TokenInfo> getAllTokensTkUtility()
        {
            List<TokenInfo> allTokensTkUtility = new List<TokenInfo>();
            var tokenDb = ConfigurationManager.AppSettings["CustomTokenDb"].ToString();
            using (SqlConnection connection = new SqlConnection(tokenDb))
            {
                using (SqlCommand sqlCommand = new SqlCommand("SELECT REPLACE([TokenSerial], 'GO3DEFAULT', '') [TokenSerial], CustomerID  FROM[CustomerTokenDB].[dbo].[CustomerToken]  where AppID <> 1234324", connection))
                {
                    connection.Open();
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    while (sqlDataReader.Read())
                    {
                        if (sqlDataReader.HasRows)
                            allTokensTkUtility.Add(new TokenInfo()
                            {
                                CustomerId = sqlDataReader.GetString(1),
                                SerialNumber = sqlDataReader.GetString(0)
                            });
                    }
                }
            }
            return allTokensTkUtility;
        }

        public class TokenInfo
        {
            public string SerialNumber { get; set; }

            public string CustomerId { get; set; }
        }
    }
}
