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
                var service = new ServiceController(serviceName);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
            }
            catch (Exception ex)
            {
                session.Log("CustomActionException: (StartService)" + ex.ToString());
                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult StopService(Session session)
        {
            try
            {
                var serviceName = session["START_SERVICE_NAME"];
                var service = new ServiceController(serviceName);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
            }
            catch (Exception ex)
            {
                session.Log("CustomActionException: (StopService)" + ex.ToString());
                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }
    }
}
