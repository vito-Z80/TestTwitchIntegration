using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class Log
{
    static readonly StringBuilder Text = new StringBuilder();

    static readonly string LogFilePath = Path.Combine(Application.persistentDataPath, "log.txt");

    public static void LogMessage(string message)
    {
        var logMessage = $"{DateTime.Now}: {message}\n";
        Debug.Log(message);
    }

    public static void SaveLog()
    {
        File.AppendAllText(LogFilePath, Text.ToString());
    }
}