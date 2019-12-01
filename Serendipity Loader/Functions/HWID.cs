using System;
using System.Linq;
using System.Management;
using System.Net;
using System.Text.RegularExpressions;

namespace Serendipity_Loader.Functions
{
    public class HWID : IDisposable
    {
        bool disposed = false;
        private static ManagementClass cpuMc;
        private static ManagementClass winMc;
        public string GetHwid()
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

                cpuMc = new ManagementClass("win32_processor");
                winMc = new ManagementClass("Win32_OperatingSystem");
                ManagementObjectCollection cpuMoc = cpuMc.GetInstances();
                ManagementObjectCollection winMoc = winMc.GetInstances();

                var cpuId = from ManagementObject cpuObj in cpuMoc select cpuObj.Properties["processorID"].Value.ToString();
                var winId = from ManagementObject winObj in cpuMoc select winObj.Properties["SerialNumber"].Value.ToString();

                string hwid = string.Concat(cpuId.ToString(), winId.ToString());
                hwid = Regex.Replace(hwid, "[^A-Za-z0-9 ]", "");
                return hwid;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                cpuMc.Dispose();
                winMc.Dispose();
            }
            disposed = true;
        }
    }
}
