using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Arkenstone.Logic.Logs
{
    public static class ClassLog
    {

        public static string logpath = "Applicatif/log/";

        public static void writeException(Exception ex)
        {
            string pathlog = logpath + DateTime.Now.ToString("yyyyMMdd") + ".log";

            string error = "";

            while (ex != null)
            {
                error += ex.GetType().FullName + "\n";
                error += "Message : " + ex.Message + "\n";
                error += "StackTrace : " + ex.StackTrace + "\n";
                ex = ex.InnerException;
            }

            writeLog(error);
        }

        static readonly object AppendAllTextLock = new object();
        
        public static void writeLog(string text)
        {
            lock (AppendAllTextLock)
            {

                if (!Directory.Exists(logpath))
                {
                    Directory.CreateDirectory(logpath);
                }

                string pathlog = logpath + DateTime.Now.ToString("yyyyMMdd") + ".log";
                File.AppendAllText(pathlog, DateTime.Now.ToString("HH mm ss ff") + "\t" + text + "\n");
            }
            Console.WriteLine(DateTime.Now.ToString("HH mm ss ff") + "\t" + text);
        }

        public static void purgelog()
        {
            DirectoryInfo dir = new DirectoryInfo(logpath);
            DateTime testDate = DateTime.Now.AddDays(-7);
            foreach (FileInfo f in dir.GetFiles())
            {
                DateTime fileAge = f.LastWriteTime;
                if (fileAge < testDate)
                {
                    File.Delete(f.FullName);
                    ClassLog.writeLog("File " + f.Name + " is older than today, deleted...");
                }
            }
        }
    }
}
