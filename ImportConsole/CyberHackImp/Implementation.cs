using NetFrameworkLibrary.CyberArk.Common.BusinessLogic;
using NetFrameworkLibrary.CyberArk.Common.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportConsole.CyberHackImp
{
    public class Implementation
    {
        public ResponseObject GetConnectionCredentials(RequestDto model)
        {
            try
            {
                VaultDetailsRequest vaultDetailsRequest = new VaultDetailsRequest()
                {
                    AppId = model.AppID,
                    Safe = model.Safe,
                    Folder = model.Folder,
                    Object = model.Object,
                    Reason = model.Reason,
                    ConnectionPort = model.Port,
                    ConnectionTimeout = model.TimeOut
                };

                VaultDetailsManager vaultDetailsManager = new VaultDetailsManager();

                VaultDetailsResponse vaultDetailsResponse = new VaultDetailsResponse();

                vaultDetailsResponse = vaultDetailsManager.GetDetails(vaultDetailsRequest);

                ResponseObject response = new ResponseObject()
                {
                    Password = vaultDetailsResponse.Password,
                    IpAddress = vaultDetailsResponse.IpAddress,
                    UserId = vaultDetailsResponse.UserName
                };

                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}