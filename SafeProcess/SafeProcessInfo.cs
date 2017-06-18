using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SafeProcess
{
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