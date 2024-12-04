using System.Collections.Concurrent;
using System;

public static class DebugLogForThread
{
    private static ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
    static public void Log(string _msg)
    {
        string _timeStemp = string.Format("[{0}:{1}:{2}:{3}]", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
        queue.Enqueue(_timeStemp + _msg);
    }    
    static public void LogFormat(string _format, params object[] args)
    {
        string _timeStemp = string.Format("[{0}:{1}:{2}:{3}]", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
        queue.Enqueue(_timeStemp + string.Format(_format, args));
    }
    static public void PrintDebugLog()
    {
        string _log = null;
        while(queue.TryDequeue(out _log))
        {
            UnityEngine.Debug.Log(_log);
        }
    }
}