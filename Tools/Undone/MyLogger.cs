using System;
using System.IO;
using UnityEngine;

namespace MyBox.Internal
{
    public static class MyLogger
    {
        private const string LogFile = "customLog.txt";
        private const string TimeFormat = "MM-dd_HH-mm-ss";

        public static bool Disabled;

        public static string Session { get; private set; }
        public static string Version { get; private set; }


        private const int MaxMessageLength = 4000;

        static MyLogger()
        {
            Session = Guid.NewGuid().ToString();
            Version = "Version not initiated";

            AppDomain.CurrentDomain.UnhandledException += (sender, args) => Log(args.ExceptionObject as Exception);
            Application.logMessageReceived +=
                (condition, trace, type) => Log(string.Format("Console Log ({0}): {1}{2}{3}", type, condition, Environment.NewLine, trace));
        }


        public static void Log(string text)
        {
            if (Application.isEditor) return;
            if (Disabled) return;

            string path = Path.Combine(Application.dataPath, LogFile);

            if (text.Length > MaxMessageLength) text = text.Substring(0, MaxMessageLength) + "...<trimmed>";

            try
            {
                if (!File.Exists(path))
                {
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(GetCurrentTime() + " || Log created" + Environment.NewLine);
                        sw.WriteLine(GetCurrentTime() + ": " + text);
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(path))
                    {
                        sw.WriteLine(GetCurrentTime() + ": " + text);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private static string GetCurrentTime()
        {
            return DateTime.Now.ToString(TimeFormat);
        }

        private static void Log(Exception ex)
        {
            Log("Exception:" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
        }
    }
}