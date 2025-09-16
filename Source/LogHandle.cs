using FR_Operator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Timers;

internal class LogHandle
{
    private static string _externalLogPath = null;
    //private static bool _usePath = false;
    public static string ExternalLogDir
    {
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                //_usePath = true;
                _externalLogPath = value;
                _terminalLogFile = Path.Combine(_externalLogPath, "TerminalFnExchange.log");
                _fullPathToLog = Path.Combine(_externalLogPath, APP_LOG);
            }
        }
    }

    private const string APP_LOG = "FdEditor.log";
    static string _fullPathToLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, APP_LOG);
    static int counter = 0;
    static int fileChanger = 0;

    internal static void ol(string message)
    {
        if (++counter % 400 == 1)
        {
            if (counter == 1)
            {
                File.AppendAllText(_fullPathToLog, Environment.NewLine + Environment.NewLine);
            }
            ol("check log size");
            if (new FileInfo(_fullPathToLog).Length > 16777216)
            {
                File.Copy(_fullPathToLog, APP_LOG + fileChanger++ % 10, true);
                File.Delete(_fullPathToLog);
                ol("Log cleared");
            }
        }
        Console.WriteLine(message);

        if (message!= "Doc paid exactly") File.AppendAllText(_fullPathToLog, DateTime.Now.ToString("g") + ":\t" + message + Environment.NewLine);
    }


    public static async void olta(string uartEvent)
    {
        Console.WriteLine(uartEvent);
        uartEvents.Enqueue(uartEvent);
        if (_fileBuzy)
        {
            ActivateTimer();
        }
        else
        {
            _fileBuzy = true;
            Write(null,null);
        }
    }
    private static Queue<string> uartEvents = new Queue<string>();
    static string _terminalLogFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TerminalFnExchange.log");
    private static bool _fileBuzy = false; 
    
    private static System.Timers.Timer _timer=null;
    static void ActivateTimer()
    {
        if (_timer == null)
        {
            _timer = new System.Timers.Timer();
            _timer = new System.Timers.Timer();
            _timer.AutoReset = true;
            _timer.Elapsed += Write;
            _timer.Interval = 90;
            _timer.Enabled = true;
            _timer.Start();
        }
    }

    private static void Write(object sender, ElapsedEventArgs e)
    {
        _fileBuzy = true;
        StringBuilder stringBuilder = new StringBuilder();
        while (uartEvents.Count>0)
        {
            stringBuilder.AppendLine(uartEvents.Dequeue());
        }
        try
        {
            File.AppendAllText(_terminalLogFile, stringBuilder.ToString());
        }
        catch (Exception exc)
        {
            Debug.WriteLine(exc.Message);
        }
        
        _fileBuzy = false ;
        if(uartEvents.Count==0)
            _DeactivateTimer();
    }

    private static  void _DeactivateTimer()
    {
        if (_timer != null)
        {
            _timer.Stop();
            _timer.Dispose();
            _timer = null;
        }
    }
}
