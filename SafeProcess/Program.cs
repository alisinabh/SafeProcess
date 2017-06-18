using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SafeProcess
{
    class Program
    {
        public const string ConfigFileName = "safeprocess.conf";

        private readonly List<SafeProcessInfo> _processes = new List<SafeProcessInfo>();

        public Program()
        {
            if (!File.Exists(ConfigFileName))
            {
                Console.WriteLine($"Config file {ConfigFileName} not found!");
                return;
            }

            ReadFile(ConfigFileName);
        }

        private void ReadFile(string configFileName)
        {
            Console.WriteLine("Reading configuration");

            var confData = File.ReadAllLines(configFileName);

            int confs = 0;

            foreach (var s in confData)
            {
                if (s.Trim().StartsWith("#") || string.IsNullOrEmpty(s.Trim()))
                    continue;

                var confParams = s.Trim().Split('|');

                _processes.Add(new SafeProcessInfo
                {
                    ProcessName = confParams[0],
                    ExecutablePath = confParams[1]
                });

                confs++;
            }

            Console.WriteLine($"Configuration loaded with {confs} process bindins.");

            Console.WriteLine("Starting monitors...");

            foreach (var safeProcessInfo in _processes)
            {
                Thread td = new Thread(MonitorProcess) {Priority = ThreadPriority.Highest};

                td.Start(safeProcessInfo);
            }

        }

        private void MonitorProcess(object procObj)
        {
            var proc = (SafeProcessInfo) procObj;

            Console.WriteLine($"Initiating monitor for: {proc.ProcessName}");

            var preProc = Process.GetProcesses().FirstOrDefault(s => s.ProcessName == proc.ProcessName);

            if (preProc != null)
            {
                // Monitor pre executed process

                while (true)
                {
                    if (preProc.WaitForExit(10000))
                    {
                        // Process Crashed!
                        proc.Crashed(0);
                        break;
                    }
                    Thread.Sleep(1000);
                }
                
            }

            // Either process crashed or not executed at all

            while (true)
            {
                try
                {
                    var newProc = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = proc.ExecutablePath
                        }
                    };

                    newProc.Start();

                    while (true)
                    {
                        if (newProc.WaitForExit(10000))
                        {
                            // Process has crashed!
                            proc.Crashed(newProc.ExitCode);
                            break;
                        }
                        Thread.Sleep(1000);
                    }

                    // Process has crashed! Recovery prcedure
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    Thread.Sleep(1000);
                }
            }

        }

        static void Main(string[] args) => new Program();
    }

    public class SafeProcessInfo
    {
        public string ProcessName { get; set; }

        public string ExecutablePath { get; set; }

        public List<DateTime> CrashTimes { get; } = new List<DateTime>();

        private const string LogDirName = "SafeProcessLogs/";

        public void Crashed(int exitCode)
        {
            var crashDate = DateTime.Now;
            CrashTimes.Add(crashDate);

            try
            {
                Console.WriteLine($"Process {ProcessName} has crashed at {crashDate} for {CrashTimes.Count} time!");

                if (!Directory.Exists(LogDirName))
                    Directory.CreateDirectory(LogDirName);

                string fileName =
                    (long)crashDate.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds + ".log";

                StreamWriter sw = new StreamWriter(Path.Combine(LogDirName, ProcessName + fileName));

                sw.WriteLine(DateTime.Now.ToString(CultureInfo.InvariantCulture));
                sw.WriteLine($"Process {ProcessName} crashed with exit code: {exitCode}");

                sw.Flush();

                sw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Log crash exception:");
                Console.WriteLine(ex);
            }
        }
    }
}
