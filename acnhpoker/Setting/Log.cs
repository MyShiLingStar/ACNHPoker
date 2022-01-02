using System;
using System.IO;

namespace ACNHPoker
{

    class Log
    {
        public Log()
        {
        }

        public static void logEvent(string Location, string Message)
        {
            if (!File.Exists(Utilities.logPath))
            {
                string logheader = "Timestamp" + "," + "Form" + "," + "Message";

                string directoryPath = Directory.GetCurrentDirectory() + "\\" + Utilities.saveFolder;

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                using (StreamWriter sw = File.CreateText(Utilities.logPath))
                {
                    sw.WriteLine(logheader);
                }
            }

            DateTime localDate = DateTime.Now;

            string newLog = localDate.ToString() + "," + Location + "," + Message;

            if (File.Exists(Utilities.logPath))
            {
                using (StreamWriter sw = File.AppendText(Utilities.logPath))
                {
                    sw.WriteLine(newLog);
                }
            }
        }
    }
}
