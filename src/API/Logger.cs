using System;
using System.IO;
using UnityEngine;

namespace Aotenjo
{
    public static class Logger
    {
        private static readonly string logDir;
        private static readonly string logFilePath;
        private static readonly object fileLock = new object();
        
        // 配置
        private const int KeepDays = 7;       // 保留最近 7 天日志
        private const int MaxFiles = 20;      // 最多保留 20 个文件（可选）

        static Logger()
        {
            logDir = Path.Combine(Application.persistentDataPath, "AML");
            if (!Directory.Exists(logDir))
                Directory.CreateDirectory(logDir);

            // 清理旧日志
            CleanupOldLogs();

            // 新建日志文件
            string fileName = $"AML_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log";
            logFilePath = Path.Combine(logDir, fileName);

            Log("==== Aotenjo ModLoader Logger Initialized ====");
        }

        public static void Log(string message)
        {
            string msg = $"[INFO] {message}";
            Debug.Log($"[AotenjoModLoader] {message}");
            WriteToFile(msg);
        }

        public static void LogWarning(string message)
        {
            string msg = $"[WARN] {message}";
            Debug.LogWarning($"[AotenjoModLoader] {message}");
            WriteToFile(msg);
        }

        public static void LogError(string message)
        {
            string msg = $"[ERROR] {message}";
            Debug.LogError($"[AotenjoModLoader] {message}");
            WriteToFile(msg);
        }

        private static void WriteToFile(string message)
        {
            lock (fileLock)
            {
                try
                {
                    File.AppendAllText(logFilePath, $"[{DateTime.Now:HH:mm:ss}] {message}\n");
                }
                catch (Exception e)
                {
                    Debug.LogError($"[AotenjoModLoader] Failed to write log file: {e}");
                }
            }
        }

        private static void CleanupOldLogs()
        {
            try
            {
                var files = new DirectoryInfo(logDir).GetFiles("modloader_*.log");

                // 1. 按日期清理
                foreach (var file in files)
                {
                    if (file.CreationTime < DateTime.Now.AddDays(-KeepDays))
                    {
                        file.Delete();
                    }
                }

                // 2. 按数量清理（只保留最新的 MaxFiles 个）
                files = new DirectoryInfo(logDir).GetFiles("modloader_*.log");
                if (files.Length > MaxFiles)
                {
                    Array.Sort(files, (a, b) => a.CreationTime.CompareTo(b.CreationTime));
                    for (int i = 0; i < files.Length - MaxFiles; i++)
                    {
                        files[i].Delete();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[AotenjoModLoader] Failed to clean old logs: {e}");
            }
        }
    }
}
