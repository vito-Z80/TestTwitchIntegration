using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class Log
{
    static readonly StringBuilder Text = new();

    static readonly string LogFilePath = Path.Combine(Application.persistentDataPath, "log.txt");

    public static void LogMessage(string message)
    {
        Text.Append($"{DateTime.Now}: {message}\n");
        Debug.Log(message);
    }

    public static void SaveLog()
    {
        // File.AppendAllText(LogFilePath, Text.ToString());
        File.WriteAllText(LogFilePath, Text.ToString());
    }
}