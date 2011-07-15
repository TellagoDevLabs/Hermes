using System;
using System.ComponentModel;
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


                session.Log(service.Status.ToString());
                if (service.Status == ServiceControllerStatus.StartPending ||
                    service.Status == ServiceControllerStatus.Running)
                {
                    service.Stop();
                }

                service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
            }
            catch (Exception ex)
            {
                session.Log("CustomActionException: (StopService)" + ex.ToString());
                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult StopMongoService(Session session)
        {
            try
            {
                var serviceName = session["START_SERVICE_NAME"];
                var service = new ServiceController(serviceName);

                session.Log("SAM");
                session.Log(service.Status.ToString());
                if (service.Status == ServiceControllerStatus.StartPending ||
                    service.Status == ServiceControllerStatus.Running)
                {
                    service.Stop();
                }

                service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
            }
            catch (Win32Exception w32Ex)
            {
                session.Log("Win32exception: Code={0}, Native={1}, Message={2}", w32Ex.ErrorCode, w32Ex.NativeErrorCode, w32Ex.Message);

                if (w32Ex.ErrorCode == 109 || w32Ex.NativeErrorCode == 109)
                {
                    // it's OK
                    return ActionResult.Success;
                }
                session.Log("CustomActionException: (StopService)" + w32Ex.ToString());
                return ActionResult.Failure;
            }
            catch (Exception ex)
            {
                session.Log(ex.GetType().ToString());
                session.Log("CustomActionException: (StopService)" + ex.ToString());
                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }
    }
}
