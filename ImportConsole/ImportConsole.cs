using ImportConsole.CyberHackImp;
using ImportConsole.ORM;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vasco;
using static ImportConsole.ORM.ClassLibrary;

namespace ImportConsole
{

    [StandardModule]
    internal sealed class Module1
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static Implementation imp = new Implementation();
        [STAThread]
        public static void Main()
        {
            Console.WriteLine("Hi What do you want to do today ? ");
            Console.WriteLine("1 => Upload Tokens ");
            Console.WriteLine("2 => Reset 1 Token ");
            Console.Write("3 => Reset All Tokens Across the channels RIB, BBG e.t.c. ");
            string Left = Console.ReadLine();
            if (Microsoft.VisualBasic.CompilerServices.Operators.CompareString(Left, "1", false) == 0)
            {
                Console.WriteLine("Enter Folder Path :");
                string str1 = Console.ReadLine();
                Module1.logger.Info(str1);
                Console.WriteLine("Enter DPKey :");
                string str2 = Console.ReadLine();
                Module1.logger.Info(str2);
                try
                {
                    foreach (string file in Microsoft.VisualBasic.FileIO.FileSystem.GetFiles(str1))
                    {
                        if (file.Contains("dpx"))
                            Module1.ImportToken(str2, file);
                    }
                }
                finally
                {
                    //IEnumerator<string> enumerator;
                    //enumerator?.Dispose();
                }
                Console.WriteLine("Completed :");
                Console.ReadLine();
            }
            else if (Microsoft.VisualBasic.CompilerServices.Operators.CompareString(Left, "2", false) == 0)
            {
                Console.WriteLine("Enter Token Serial Number :");
                Module1.ResetToken_Click(Console.ReadLine());
            }
            else if (Microsoft.VisualBasic.CompilerServices.Operators.CompareString(Left, "3", false) == 0)
                Module1.ResetAllTokens_Click();
            else
                Console.WriteLine("*****Please ender a valid Number*****");
        }

        public static void ImportToken(string DPKey, string path)
        {
            int DigipassCount = 0;
            int AppCount = 0;
            try
            {
                AAL2Wrap aaL2Wrap = new AAL2Wrap();
                AAL2Wrap.TDigipass tdigipass = new AAL2Wrap.TDigipass();
                string[] strArray = aaL2Wrap.AAL2DPXInit(path, DPKey, ref AppCount, ref DigipassCount);
                int num1 = DigipassCount;
                int num2 = 1;
                while (num2 <= num1)
                {
                    AAL2Wrap.TDigipass token = aaL2Wrap.AAL2DPXGetToken(strArray[0]);
                    int num3 = Module1.AddNew(token);
                    Console.WriteLine(token.SerialNumber + Conversions.ToString(num3));
                    Module1.logger.Info(token.SerialNumber + Conversions.ToString(num3));
                    checked { ++num2; }
                }
                aaL2Wrap.AAL2DPXClose();
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Exception exception = ex;
                Module1.logger.Info("ImportToken: " + exception.ToString());
                int num = (int)Interaction.MsgBox((object)exception.Message);
                ProjectData.ClearProjectError();
            }
        }

        public static int AddNew(AAL2Wrap.TDigipass myDigiPass)
        {
            int num = 0;
            try
            {
                RequestDto RIBrequestDto = new RequestDto
                {
                    AppID = ConfigurationManager.AppSettings["InternetBankingDBRIBCyberHackAppID"].ToString(),
                    Safe = ConfigurationManager.AppSettings["InternetBankingDBRIBCyberHackSafe"].ToString(),
                    Folder = ConfigurationManager.AppSettings["InternetBankingDBRIBCyberHackFolder"].ToString(),
                    Object = ConfigurationManager.AppSettings["InternetBankingDBRIBCyberHackObject"].ToString(),
                    Reason = ConfigurationManager.AppSettings["InternetBankingDBRIBCyberHackReason"].ToString(),
                    Port = int.Parse(ConfigurationManager.AppSettings["InternetBankingDBRIBCyberHackConnectionPort"]),
                    TimeOut = int.Parse(ConfigurationManager.AppSettings["InternetBankingDBRIBCyberHackConnectionTimeout"])
                };

                var RIBDetail = imp.GetConnectionCredentials(RIBrequestDto); 
                RequestDto BBGrequestDto = new RequestDto
                {
                    AppID = ConfigurationManager.AppSettings["InternetBankingDBBBGCyberHackAppID"].ToString(),
                    Safe = ConfigurationManager.AppSettings["InternetBankingDBBBGCyberHackSafe"].ToString(),
                    Folder = ConfigurationManager.AppSettings["InternetBankingDBBBGCyberHackFolder"].ToString(),
                    Object = ConfigurationManager.AppSettings["InternetBankingDBBBGCyberHackObject"].ToString(),
                    Reason = ConfigurationManager.AppSettings["InternetBankingDBBBGCyberHackReason"].ToString(),
                    Port = int.Parse(ConfigurationManager.AppSettings["InternetBankingDBBBGCyberHackConnectionPort"]),
                    TimeOut = int.Parse(ConfigurationManager.AppSettings["InternetBankingDBBBGCyberHackConnectionTimeout"])
                };

                var BBGDetail = imp.GetConnectionCredentials(BBGrequestDto);
                RequestDto CustomerTokenrequestDto = new RequestDto
                {
                    AppID = ConfigurationManager.AppSettings["CustomerTokenCyberHackAppID"].ToString(),
                    Safe = ConfigurationManager.AppSettings["CustomerTokenCyberHackSafe"].ToString(),
                    Folder = ConfigurationManager.AppSettings["CustomerTokenCyberHackFolder"].ToString(),
                    Object = ConfigurationManager.AppSettings["CustomerTokenCyberHackObject"].ToString(),
                    Reason = ConfigurationManager.AppSettings["CustomerTokenCyberHackReason"].ToString(),
                    Port = int.Parse(ConfigurationManager.AppSettings["CustomTokenCyberHackConnectionPort"]),
                    TimeOut = int.Parse(ConfigurationManager.AppSettings["CustomTokenCyberHackConnectionTimeout"])
                };

                var CustomerTokenDetail = imp.GetConnectionCredentials(CustomerTokenrequestDto);
                string str1 = ConfigurationManager.AppSettings["RIB"].ToString().Replace("@@uid@@", RIBDetail.UserId).Replace("@@server@@", RIBDetail.IpAddress).Replace("@@password@@", RIBDetail.Password);
                string str2 = ConfigurationManager.AppSettings["BBG"].ToString().Replace("@@uid@@", BBGDetail.UserId).Replace("@@server@@", BBGDetail.IpAddress).Replace("@@password@@", BBGDetail.Password);
                string str3 = ConfigurationManager.AppSettings["CustomerToken"].ToString().Replace("@@uid@@", CustomerTokenDetail.UserId).Replace("@@server@@", CustomerTokenDetail.IpAddress).Replace("@@password@@", CustomerTokenDetail.Password);
                List<string> stringList = new List<string>();
                stringList.Add(str1);
                stringList.Add(str2);
                stringList.Add(str3);
                try
                {
                    foreach (string connectionString in stringList)
                    {
                        try
                        {
                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                SqlCommand sqlCommand = new SqlCommand("[RIBP_DigipassMain_AddNew]", connection);
                                sqlCommand.CommandType = CommandType.StoredProcedure;
                                sqlCommand.Parameters.AddWithValue("@DigipassData", (object)myDigiPass.bDpData);
                                sqlCommand.Parameters.AddWithValue("@SerialNumber", (object)myDigiPass.SerialNumber);
                                sqlCommand.Parameters.AddWithValue("@Type", (object)myDigiPass.Type);
                                sqlCommand.Parameters.AddWithValue("@ApplicationMode", (object)myDigiPass.Mode);
                                sqlCommand.Parameters.AddWithValue("@DpData", (object)myDigiPass.DpData);
                                connection.Open();
                                sqlCommand.ExecuteScalar();
                                connection.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            ProjectData.SetProjectError(ex);
                            Exception exception = ex;
                            Console.WriteLine("AddNew Error 1: " + exception.Message);
                            Module1.logger.Info("AddNew Error 1:  " + exception.ToString());
                            ProjectData.ClearProjectError();
                        }
                    }
                }
                finally
                {
                    //List<string>.Enumerator enumerator;
                    //enumerator.Dispose();
                }
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Exception exception = ex;
                Console.WriteLine("AddNew Error2: " + exception.Message);
                Module1.logger.Info("AddNew Error 2:  " + exception.ToString());
                ProjectData.ClearProjectError();
            }
            return num;
        }

        public static void ResetFCMBUKTokens()
        {
            List<TokenInfo> allTokensTkUtility = getAllTokensTkUtility();
            try
            {
                foreach (TokenInfo tokenInfo in allTokensTkUtility)
                {
                    AAL2Wrap aaL2Wrap = new AAL2Wrap();
                    try
                    {
                        string DpData = DigiPassDB.RetrievedpData(tokenInfo.SerialNumber + "GO3DEFAULT  ");
                        aaL2Wrap.KParams.ITimeWindow = 3;
                        aaL2Wrap.KParams.SyncWindow = 1;
                        string syncWindow = aaL2Wrap.AAL2GetTokenInfoEx(DpData).SyncWindow;
                        aaL2Wrap.AAL2ResetTokenInfo(ref DpData);
                        DigiPassDB.UpdateTokenData(tokenInfo.SerialNumber + "GO3DEFAULT  ", DpData);
                        Console.WriteLine(tokenInfo.SerialNumber + " AAL2VerifyPassword returns " + aaL2Wrap.getLastError());
                        Module1.logger.Info(tokenInfo.SerialNumber + " AAL2VerifyPassword returns " + aaL2Wrap.getLastError());
                    }
                    catch (Exception ex)
                    {
                        ProjectData.SetProjectError(ex);
                        Exception exception = ex;
                        Console.WriteLine(tokenInfo.SerialNumber + " Error: " + exception.Message);
                        Module1.logger.Info(tokenInfo.SerialNumber + " Error: " + exception.Message);
                        ProjectData.ClearProjectError();
                    }
                }
            }
            finally
            {
                //List<TokenInfo>.Enumerator enumerator;
                //enumerator.Dispose();
            }
            Console.ReadLine();
        }

        private static void ResetToken_Click(string serialNumber)
        {
            AAL2Wrap aaL2Wrap = new AAL2Wrap();
            try
            {
                string DpData = DigiPassDB.RetrievedpData(serialNumber + "GO3DEFAULT  ");
                aaL2Wrap.KParams.ITimeWindow = 3;
                aaL2Wrap.KParams.SyncWindow = 1;
                string syncWindow = aaL2Wrap.AAL2GetTokenInfoEx(DpData).SyncWindow;
                aaL2Wrap.AAL2ResetTokenInfo(ref DpData);
                DigiPassDB.UpdateTokenData(serialNumber + "GO3DEFAULT", DpData);
                Console.WriteLine("AAL2VerifyPassword returns " + aaL2Wrap.getLastError());
                Module1.logger.Info("AAL2VerifyPassword returns " + aaL2Wrap.getLastError());
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Exception exception = ex;
                Console.WriteLine(serialNumber + " Error: " + exception.Message);
                Module1.logger.Info(serialNumber + " Error: " + exception.Message);
                Console.ReadLine();
                ProjectData.ClearProjectError();
            }
        }

        private static void ResetAllTokens_Click()
        {
            AAL2Wrap aaL2Wrap = new AAL2Wrap();
            List<DigiPassDB.TokenInfo> tokenInfoList = new List<DigiPassDB.TokenInfo>();
            List<DigiPassDB.TokenInfo> allTokens = DigiPassDB.GetAllTokens();
            Console.WriteLine(string.Format("Got all the Tokens :{0}", (object)allTokens.Count));
            Module1.logger.Info(string.Format("Got all the Tokens :{0}", (object)allTokens.Count));
            if (allTokens.Count > 0)
                Parallel.ForEach<DigiPassDB.TokenInfo>((IEnumerable<DigiPassDB.TokenInfo>)allTokens, new ParallelOptions()
                {
                    MaxDegreeOfParallelism = 5
                }, (Action<DigiPassDB.TokenInfo>)(o =>
                {
                    try
                    {
                        aaL2Wrap.KParams.ITimeWindow = 3;
                        aaL2Wrap.KParams.SyncWindow = 1;
                        string syncWindow = aaL2Wrap.AAL2GetTokenInfoEx(o.dpData).SyncWindow;
                        AAL2Wrap aaL2Wrap1 = aaL2Wrap;
                        DigiPassDB.TokenInfo tokenInfo;
                        string dpData = (tokenInfo = o).dpData;
                        ref string local = ref dpData;
                        aaL2Wrap1.AAL2ResetTokenInfo(ref local);
                        tokenInfo.dpData = dpData;
                        DigiPassDB.UpdateTokenData(o.serialNo, o.dpData);
                        Console.WriteLine(string.Format(" {0} Reset Successfully :  {1}", (object)o.serialNo, (object)aaL2Wrap.getLastError()));
                        Module1.logger.Info(string.Format(" {0} Reset Successfully :  {1}", (object)o.serialNo, (object)aaL2Wrap.getLastError()));
                    }
                    catch (Exception ex)
                    {
                        ProjectData.SetProjectError(ex);
                        Exception exception = ex;
                        Console.WriteLine(string.Format("{0} Reset Failed : {1} ", (object)o.serialNo, (object)exception.Message));
                        Module1.logger.Info(string.Format("{0} Reset Failed : {1} ", (object)o.serialNo, (object)exception.Message));
                        Console.ReadLine();
                        ProjectData.ClearProjectError();
                    }
                }));
            Console.WriteLine("Completed");
            Module1.logger.Info("Completed");
            Console.ReadLine();
        }
    }
}
