using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace Revo.Core
{
     /// <summary>
     /// Start,stop and check service status
     /// </summary>
    public class ServiceManager
    {
        public Dictionary<string, bool> CheckServicesRunning(Dictionary<string, string> serviceNames)
        {
            ServiceController serviceController = null;
            bool isRunning = false;
            Dictionary<string, bool> resultList = new Dictionary<string, bool>();
            foreach (string  serviceName in serviceNames.Keys)
            {
                try
                {
                    serviceController = new ServiceController(serviceNames[serviceName]);
                    isRunning = serviceController.Status == ServiceControllerStatus.Running;
                    resultList.Add(serviceName, isRunning);
                }
                catch (Exception ex)
                {
                    resultList.Add(serviceName, false);
                    LogManager.Instance.Log($"Check service installations:\n {ex}\n", LogType.Fatal);
                }
            }
            return resultList;
        }

        public bool CheckServiceRunning(string serviceName)
        {
            ServiceController sc = new ServiceController(serviceName);
            bool isRunning = sc.Status == ServiceControllerStatus.Running;
            return isRunning;
        }

        public async Task<bool> StartService(string serviceName, int tryAmount)
        {
            bool result = await Task.Run(() =>
            {
                bool isServiceRunning = false;
                do
                {
                    if (tryAmount == 0)
                    {
                        break;
                    }
                    tryAmount--;
                    try
                    {
                        ServiceController sc = new ServiceController(serviceName);
                        switch (sc.Status)
                        {
                            case ServiceControllerStatus.Running:
                                isServiceRunning = true;
                                break;
                            case ServiceControllerStatus.Stopped:
                                sc.Start();
                                sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(20));
                                break;
                            case ServiceControllerStatus.Paused:
                                sc.Continue();
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManager.Instance.Log($"Check service installations:\n {ex}\n", LogType.Fatal);
                    }
                    Thread.Sleep(100);
                } while (isServiceRunning == false);
                return isServiceRunning;
            });
            return result;
        }

        public async Task<bool> StopService(string serviceName, int tryAmount)
        {
            bool result = await Task.Run(() =>
            {
                bool isServiceStopped = false;
                do
                {
                    if (tryAmount == 0)
                    {
                        break;
                    }
                    tryAmount--;
                    ServiceController sc = new ServiceController(serviceName);
                    switch (sc.Status)
                    {
                        case ServiceControllerStatus.Running:
                            sc.Stop();
                            sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(20));
                            break;
                        case ServiceControllerStatus.Stopped:
                            isServiceStopped = true;
                            break;
                        case ServiceControllerStatus.Paused:
                            
                            break;
                    }
                    Thread.Sleep(100);
                } while (isServiceStopped == false);
                return isServiceStopped;
            });
            return result;
        }
    }
}
