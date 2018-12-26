using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DistDataAquisition.Helpers
{
    public static class CasperJSHelper
    {
        public static string RunProcess(string[] parameters, string fileName, string processName)
        {
            string casperComp = ConfigurationManager.AppSettings["CasperComp"],
                casperPath = ConfigurationManager.AppSettings["CasperPath"],
                tempPath = ConfigurationManager.AppSettings["TempPath"],
                appDisk = ConfigurationManager.AppSettings["ApplicationDisk"],
                filePath = tempPath + fileName,
                json = string.Empty;

            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch { }
            }
            //Change to the application disk, go to the casper folder and then executes the .exe
            StringBuilder comand = new StringBuilder();
            comand.Append(appDisk);
            comand.Append(" & cd ");
            if (!string.IsNullOrEmpty(casperComp))
            {
                comand.Append(casperComp);
                comand.Append(" & cd ");
            }
            comand.Append(casperPath);
            comand.Append(" & casperjs ");
            comand.Append(string.Join(" ", parameters) + " " + filePath);


            System.Threading.Thread thread = new Thread(new ParameterizedThreadStart(s => ExecuteCommand(comand.ToString(), filePath)));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            if (!thread.Join(TimeSpan.FromSeconds(240)))
            {
                thread.Abort();

                if (!CheckAvailability(filePath))
                {

                }
            }


            //if (code != 1) throw new Exception("It was not possible to get the invoice details at CCW-R");

            json = File.ReadAllText(filePath);

            return json;
        }


        private static void ExecuteCommand(string command, string file)
        {
            ProcessStartInfo processInfo;
            processInfo = new ProcessStartInfo("cmd.exe", "/K " + command);
            processInfo.CreateNoWindow = false;
            processInfo.UseShellExecute = true;

            using (Process process = Process.Start(processInfo))
            {
                while (!CasperJSHelper.CheckAvailability(file)) { }
                process.Kill();
            }
        }


        private static bool CheckAvailability(string file)
        {


            try
            {
                using (var stream = File.Open(file, FileMode.Open, FileAccess.Read)) { }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
