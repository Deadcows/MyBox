using System;
using System.IO;
using UnityEngine;

namespace MyBox.Internal
{
    public static class MyLogger
    {
        //TODO: How to get logs on Mac?
        //TODO: Find better ways for logging?
        
        private static string LogFile = "customLog.txt";
        private static string TimeFormat = "MM-dd_HH-mm-ss";

        public static bool Disabled;

        public static string Session { get; private set; }
        public static string Version { get; private set; }
        
        public static bool LogToConsole { get; set; }

        public const string DefaultFilename = "customLog.txt";
        public const string DefaultTimeFormat = "MM-dd_HH-mm-ss";

        private const int MaxMessageLength = 4000;

        static MyLogger()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) 
                => LogException(args.ExceptionObject as Exception);
            Application.logMessageReceived += (condition, trace, type) 
                => Log($"Console Log ({type}): {condition}{Environment.NewLine}{trace}", false, false);
        }

        public static void InitializeSession(
            string version = null, string filename = DefaultFilename, 
            string timeFormat = DefaultTimeFormat, bool logToConsole = false)
        {
            Session = Guid.NewGuid().ToString();
            Version = version ?? string.Empty;

            LogFile = filename;
            TimeFormat = timeFormat;

            LogToConsole = logToConsole;
            Log("Initialized. " + version);
        } 
        
        public static void Log(string text, bool withStackTrace = false, bool logToConsole = true)
        {
            if (Application.isEditor && LogToConsole && logToConsole) 
                Debug.Log("Logger: ".Colored(Colors.brown) + text);
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
            => Log("Exception:" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace, false, false);
    }
}