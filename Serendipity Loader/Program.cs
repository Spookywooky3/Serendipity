using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Threading;

namespace Serendipity_Loader
{
    internal class Loader
    {
        static void Main()
        {
            try
            {
                var currentProcess = Process.GetCurrentProcess();
                Thread processThread = new Thread(new ThreadStart(ProcessCheck));
                processThread.Start();

                if (IsOnline() == false)
                {
                    Environment.Exit(0);
                }
                else if (IsOnline() == true)
                {
                    using (WebClient wc = new WebClient())
                    {
                        using (Functions.HWID hwidObj = new Functions.HWID())
                        {
                            string hwid = hwidObj.GetHwid();
                            string check = wc.DownloadString("https://localhost/php/hwidcheck.php?hwid=" + hwid);

                            if (check == "1")
                            {

                            }
                            else
                            {
                                Environment.Exit(0);
                            }
                        }
                    }
                }
                else
                {
                    Environment.Exit(0);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }
        }
        private static bool IsOnline()
        {
            try
            {
                Ping ping = new Ping();
                PingReply reply = ping.Send("8.8.8.8");

                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private static void ProcessCheck()
        {
            while (true)
            {
                Thread.Sleep(1000);
                string[] processList = { "dnspy", "cheatengine", "ida", "ghidra", "ollydbg" };
                foreach (var process in Process.GetProcesses())
                {
                    foreach (var pname in processList)
                    {
                        if (process.ProcessName.Contains(pname))
                        {
                            Environment.Exit(0);
                        }
                    }
                }
            }
        }



        //CREDIT:  https://github.com/malcomvetter/AntiDebug/blob/master/ProtectProcess/AntiDebug.cs
        //Rewritten to learn
        const int DBG_CONTINUE = 0x00010002;
        private static void DebuggerThread()
        {
            DEBUG_EVENT evt = new DEBUG_EVENT();
            evt.bytes = new byte[1024];

            while (true)
            {
                int continueFlag = DBG_CONTINUE;
                ContinueDebugEvent(evt.dwProcessId, evt.dwThreadId, continueFlag);
            }
        }
        private static void SelfDebug()
        {
            var p = Process.GetCurrentProcess();
            switch (p.Id)
            {
                case 0:
                    Process pDebug = Process.GetProcessById(p.Id);
                    new Thread(KillOnDebugExit)
                    {
                        IsBackground = true,
                        Name = "DieOnDebugExit"
                    }.Start(pDebug);
                    WaitForDebug();
                    DebuggerThread();
                    Environment.Exit(0);
                    break;
                default:
                    Process pParentDebug = new Process();
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = false;
                    p.StartInfo.ErrorDialog = false;
                    pParentDebug.Start();

                    new Thread(KillOnDebugExit)
                    {
                        IsBackground = true,
                        Name = "DieOnDebugExit"
                    }.Start(pParentDebug);

                    new Thread(DebuggerThread)
                    {
                        IsBackground = true,
                        Name = "DebugThread"
                    }.Start(pParentDebug);
                    WaitForDebug();
                    break;
            }
        }
        private static void KillOnDebugExit()
        {

        }
        private static void WaitForDebug()
        {
            while (!IsDebuggerPresent())
            {
                Thread.Sleep(1);
            }
        }

        enum DebugEventType : int
        {
            CREATE_PROCESS_DEBUG_EVENT = 3, //Reports a create-process debugging event. The value of u.CreateProcessInfo specifies a CREATE_PROCESS_DEBUG_INFO structure.
            CREATE_THREAD_DEBUG_EVENT = 2, //Reports a create-thread debugging event. The value of u.CreateThread specifies a CREATE_THREAD_DEBUG_INFO structure.
            EXCEPTION_DEBUG_EVENT = 1, //Reports an exception debugging event. The value of u.Exception specifies an EXCEPTION_DEBUG_INFO structure.
            EXIT_PROCESS_DEBUG_EVENT = 5, //Reports an exit-process debugging event. The value of u.ExitProcess specifies an EXIT_PROCESS_DEBUG_INFO structure.
            EXIT_THREAD_DEBUG_EVENT = 4, //Reports an exit-thread debugging event. The value of u.ExitThread specifies an EXIT_THREAD_DEBUG_INFO structure.
            LOAD_DLL_DEBUG_EVENT = 6, //Reports a load-dynamic-link-library (DLL) debugging event. The value of u.LoadDll specifies a LOAD_DLL_DEBUG_INFO structure.
            OUTPUT_DEBUG_STRING_EVENT = 8, //Reports an output-debugging-string debugging event. The value of u.DebugString specifies an OUTPUT_DEBUG_STRING_INFO structure.
            RIP_EVENT = 9, //Reports a RIP-debugging event (system debugging error). The value of u.RipInfo specifies a RIP_INFO structure.
            UNLOAD_DLL_DEBUG_EVENT = 7, //Reports an unload-DLL debugging event. The value of u.UnloadDll specifies an UNLOAD_DLL_DEBUG_INFO structure.
        }
        [StructLayout(LayoutKind.Sequential)]
        struct DEBUG_EVENT
        {
            [MarshalAs(UnmanagedType.I4)]
            public DebugEventType dwDebugEventCode;
            public int dwProcessId;
            public int dwThreadId;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public byte[] bytes;
        }

        [DllImport("Kernel32.dll", SetLastError = true)]
        static extern bool DebugActiveProcess(int dwProcessId);
        [DllImport("Kernel32.dll", SetLastError = true)]
        static extern bool WaitForDebugEvent([Out] out DEBUG_EVENT lpDebugEvent, int dwMilliseconds);
        [DllImport("Kernel32.dll", SetLastError = true)]
        static extern bool ContinueDebugEvent(int dwProcessId, int dwThreadId, int dwContinueStatus);
        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool IsDebuggerPresent();
    }
}