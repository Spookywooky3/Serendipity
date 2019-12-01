using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Windows;
using System.Text.RegularExpressions;

namespace Serendipity_HWID_Gen
{
    class HWID
    {
        static void Main(string[] args)
        {
            string cpuId = string.Empty;
            string winId = string.Empty;

            ManagementClass cpuMc = new ManagementClass("win32_processor");
            ManagementClass winMc = new ManagementClass("Win32_OperatingSystem");
            ManagementObjectCollection cpuMoc = cpuMc.GetInstances();
            ManagementObjectCollection winMoc = winMc.GetInstances();

            foreach (ManagementObject mo in cpuMoc)
            {
                if (cpuId == "")
                {
                    cpuId = mo.Properties["processorID"].Value.ToString();
                    break;
                }
            }
            foreach (ManagementObject mo in winMoc)
            {
                if (winId == "")
                {
                    winId = mo.Properties["SerialNumber"].Value.ToString();
                    break;
                }
            }
            string hwid = string.Concat(cpuId, winId);
            hwid = Regex.Replace(hwid, "[^A-Za-z0-9 ]", "");
            Console.WriteLine(hwid);
            Console.ReadKey();
        }
    }
}
