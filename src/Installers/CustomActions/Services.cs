using System;
using System.ServiceProcess;
using Microsoft.Deployment.WindowsInstaller;

namespace TellagoStudios.Hermes.CustomActions
{
    public class Services
    {
        [CustomAction]
        public static ActionResult StartService(Session session)
        {
            try
            {
                var serviceName = session["START_SERVICE_NAME"];
                ServiceController service = new ServiceController(serviceName);

                TimeSpan timeout = TimeSpan.FromSeconds(3);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
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
