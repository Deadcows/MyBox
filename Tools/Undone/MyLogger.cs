using System;
using System.IO;
using UnityEngine;

namespace MyBox.Internal
{
    public static class MyLogger
    {
        private static string LogFile = "customLog.txt";
        private static string TimeFormat = "MM-dd_HH-mm-ss";

        public static bool Disabled;

        public static string Session { get; private set; }
        public static string Version { get; private set; }


        private const int MaxMessageLength = 4000;

        static MyLogger()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) => LogException(args.ExceptionObject as Exception);
            Application.logMessageReceived += (condition, trace, type) => Log($"Console Log ({type}): {condition}{Environment.NewLine}{trace}");
        }

        public static void InitializeSession(string version = null, string filename = "customLog.txt", string timeFormat = "MM-dd_HH-mm-ss")
        {
            Session = Guid.NewGuid().ToString();
            Version = version ?? string.Empty;

            LogFile = filename;
            TimeFormat = timeFormat;
            
            Log("Initialized. " + version);
        } 
        
        public static void Log(string text, bool withStackTrace = false)
        {
            if (Application.isEditor) return;
            if (Disabled) return;

            string path = Path.Combine(Application.dataPath, LogFile);

            if (text.Length > MaxMessageLength) text = text.Substring(0, MaxMessageLength) + "...<trimmed>";
            if (withStackTrace) text += Environment.NewLine + Environment.StackTrace;
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
            
            string GetCurrentTime() => DateTime.Now.ToString(TimeFormat);
        }


        private static void LogException(Exception ex)
        {
            Log("Exception:" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
        }
    }
}