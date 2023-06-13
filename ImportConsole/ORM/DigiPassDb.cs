using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ImportConsole.ORM
{
    public class DigiPassDB
    {
        static string internetBankingRIB = ConfigurationManager.AppSettings["InternetBankingDBRIB"].ToString();
        static string internetBankingCIB = ConfigurationManager.AppSettings["InternetBankingDBCIB"].ToString();
        static string CustomerTokenDB = ConfigurationManager.AppSettings["CustomTokenDb"].ToString();
        public static string UpdateTokenData(string mySerialNumber, string dpData)
        {
            
            string str1 = "";
            string str2 = internetBankingRIB;
            string str3 = internetBankingCIB;
            string str4 = CustomerTokenDB;
            List<string> stringList = new List<string>();
            stringList.Add(str2);
            stringList.Add(str3);
            stringList.Add(str4);
            try
            {
                foreach (string connectionString in stringList)
                {
                    string cmdText;
                    if (connectionString.Contains("CustomerTokenDB"))
                        cmdText = "UPDATE DigipassMain SET DpData ='" + dpData + "' where rtrim(ltrim(SerialNumber)) = '" + mySerialNumber + "'";
                    else
                        cmdText = "UPDATE Ribp_DigipassMain SET DpData ='" + dpData + "' where rtrim(ltrim(SerialNumber)) = '" + mySerialNumber + "'";
                    SqlConnection connection = new SqlConnection(connectionString);
                    SqlCommand sqlCommand = new SqlCommand(cmdText, connection);
                    sqlCommand.CommandType = CommandType.Text;
                    try
                    {
                        connection.Open();
                        sqlCommand.ExecuteNonQuery();
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        ProjectData.SetProjectError(ex);
                        ProjectData.ClearProjectError();
                    }
                }
            }
            finally
            {
                //List<string>.Enumerator enumerator;
                //enumerator.Dispose();
            }
            return str1;
        }

        public static string RetrievedpData(string mySerialNumber)
        {
            string str = "";
            string connectionString = CustomerTokenDB;
            string cmdText = "Select DpData from DigiPassMain where rtrim(Ltrim(SerialNumber)) = '" + mySerialNumber + "'";
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = new SqlCommand(cmdText, connection);
            sqlCommand.CommandType = CommandType.Text;
            try
            {
                connection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                while (sqlDataReader.Read())
                    str = Conversions.ToString(sqlDataReader[0]);
                sqlDataReader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                int num = (int)Interaction.MsgBox((object)ex.Message);
                ProjectData.ClearProjectError();
            }
            return str;
        }

        public static List<DigiPassDB.TokenInfo> GetAllTokens()
        {
            string connectionString = internetBankingRIB;
            string cmdText = "Select DpData, SerialNumber from Ribp_DigiPassMain ";
            List<DigiPassDB.TokenInfo> allTokens = new List<DigiPassDB.TokenInfo>();
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                SqlCommand sqlCommand = new SqlCommand(cmdText, connection);
                sqlCommand.CommandType = CommandType.Text;
                connection.Open();
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                if (sqlDataReader.HasRows)
                {
                    while (sqlDataReader.Read())
                        allTokens.Add(new DigiPassDB.TokenInfo()
                        {
                            dpData = Conversions.ToString(sqlDataReader[0]),
                            serialNo = Conversions.ToString(sqlDataReader[1])
                        });
                }
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                ProjectData.ClearProjectError();
            }
            return allTokens;
        }

        public class TokenInfo
        {
            public string dpData { get; set; }

            public string serialNo { get; set; }
        }
    }
}
