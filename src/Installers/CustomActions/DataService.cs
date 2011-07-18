using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using System.Web;
using System.Net;

namespace TellagoStudios.Hermes.CustomActions
{
    public class DataServiceServer
    {
        [CustomAction]
        public static ActionResult CheckDataServiceConnection(Session session)
        {
            try
            {
                session["APPLICATIONREPOSITORY_TESTRESULT"] = "Not Checked";

                var url = session["APPLICATIONREPOSITORY_URL"];

                var request = HttpWebRequest.Create(url + "/Hermes");
				request.UseDefaultCredentials = true;

                var response = (HttpWebResponse)request.GetResponse();

                using (var stream = response.GetResponseStream())
                {
                    var buffer = new byte[4096];
                    while (stream.Read(buffer, 0, 4096) > 0) ;
                }
                session["APPLICATIONREPOSITORY_TESTRESULT"] = "1";
            }
            catch (Exception ex)
            {
                session["APPLICATIONREPOSITORY_TESTRESULT"] = "0";
                session.Log("EXCEPTION:" + Environment.NewLine + ex.ToString());
            }

            session.Log("4 ");
            return ActionResult.Success;
        }
    }
}
