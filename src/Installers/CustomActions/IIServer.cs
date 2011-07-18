using System;
using Microsoft.Deployment.WindowsInstaller;
using System.DirectoryServices;

namespace TellagoStudios.Hermes.CustomActions
{
    public class IIServer
    {
        [CustomAction]
        public static ActionResult GetWebSites(Session session)
        {
            try
            {
                View listBoxView = session.Database.OpenView("select * from ListBox");
                View availableWSView = session.Database.OpenView("select * from AvailableWebSites");

                DirectoryEntry iisRoot = new DirectoryEntry("IIS://localhost/W3SVC");

                int order = 1;
                foreach (DirectoryEntry webSite in iisRoot.Children)
                {
                    if (webSite.SchemaClassName.ToLower() == "iiswebserver" &&
                        webSite.Name.ToLower() != "administration web site")
                    {
                        StoreWebSiteDataInListBoxTable(webSite, order, listBoxView);
                        StoreWebSiteDataInAvailableWebSitesTable(webSite, availableWSView);
                        order++;
                    }
                }
            }
            catch (Exception ex)
            {
                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }

        private static void StoreWebSiteDataInListBoxTable(DirectoryEntry webSite, int order, View listBoxView)
        {
            Record newListBoxRecord = new Record(4);
            newListBoxRecord[1] = "WEBSITE";
            newListBoxRecord[2] = order;
            newListBoxRecord[3] = webSite.Name;
            newListBoxRecord[4] = webSite.Properties["ServerComment"].Value;

            listBoxView.Modify(ViewModifyMode.InsertTemporary, newListBoxRecord);
        }

        private static void StoreWebSiteDataInAvailableWebSitesTable(DirectoryEntry webSite, View availableWSView)
        {
            //Get Ip, Port and Header from server bindings
            string[] serverBindings = ((string)webSite.Properties["ServerBindings"].Value).Split(':');
            string ip = serverBindings[0];
            string port = serverBindings[1];
            string header = serverBindings[2];

            Record newFoundWebSiteRecord = new Record(5);
            newFoundWebSiteRecord[1] = webSite.Name;
            newFoundWebSiteRecord[2] = webSite.Properties["ServerComment"].Value;
            newFoundWebSiteRecord[3] = port;
            newFoundWebSiteRecord[4] = ip;
            newFoundWebSiteRecord[5] = header;

            availableWSView.Modify(ViewModifyMode.InsertTemporary, newFoundWebSiteRecord);
        }

        [CustomAction]
        public static ActionResult UpdatePropsWithSelectedWebSite(Session session)
        {
            try
            {
                string selectedWebSiteId = session["WEBSITE"];
                session.Log("CA: Found web site id: " + selectedWebSiteId);

                View availableWebSitesView = session.Database.OpenView("Select * from AvailableWebSites where WebSiteNo=" + selectedWebSiteId);
                availableWebSitesView.Execute();

                Record record = availableWebSitesView.Fetch();
                if ((record[1].ToString()) == selectedWebSiteId)
                {
                    session["WEBSITE_DESCRIPTION"] = (string)record[2];
                    session["WEBSITE_PORT"] = (string)record[3];
                    session["WEBSITE_IP"] = (string)record[4];
                    session["WEBSITE_HEADER"] = (string)record[5];
                }
            }
            catch (Exception ex)
            {
                session.Log("CustomActionException: " + ex.ToString());
                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }
    }
}
