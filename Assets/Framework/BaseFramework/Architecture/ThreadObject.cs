

using System;
using System.Data;
using System.Diagnostics;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class ThreadObject
{
    public ThreadObject(ThreadProcFunc _func, int _fps = 60, string _name = "ThreadObject")
    {
        this.func = _func;
        this.threadName = _name;
        fixedDeltaTick = (long)((1.0 / (double)_fps) * 1000 * TimeSpan.TicksPerMillisecond);
    }    
    public  delegate bool ThreadProcFunc(float _deltaTime);
    public int              ThreadID {get; protected set;}
    private Thread          thread = null;
    private ThreadProcFunc  func = null;
    private bool            stopThreadFlag = false;
    private long            fixedDeltaTick = 0;
    private string          threadName = string.Empty;
    public void Start()
    {
        this.thread = new Thread(this.OnThreadProc);
        this.ThreadID = thread.ManagedThreadId;
        this.thread.Name = string.Format("{0}({1})", this.threadName, this.ThreadID);
        this.threadName = this.thread.Name;
        this.thread.Start();
    }

    public void Stop()
    {
        ThreadID = -1;
        if(thread.Join(1000) == false ) { thread.Abort(); }
        thread = null;
    }

    protected virtual void OnThreadProc()
    {
        var _timer = Stopwatch.StartNew();
        _timer.Start();

        var _curTime = DateTime.Now.Ticks;
        var _beforeTime = _curTime;

        try
        {
            while(!stopThreadFlag)
            {
                float _deltaTime = (float)((double)(_curTime - _beforeTime) / (1000 * TimeSpan.TicksPerMillisecond));
                
                _beforeTime = _curTime;
                _curTime = DateTime.Now.Ticks;

                if(!func(_deltaTime))
                    break;

                var _spanTime = Math.Min(fixedDeltaTick, (DateTime.Now.Ticks - _curTime));
                int _sleepTime = (int)((this.fixedDeltaTick - _spanTime) / TimeSpan.TicksPerMillisecond);

                DebugLogForThread.LogFormat("[{0}] Sleep Time = {1}", this.threadName, _sleepTime);

                Thread.Sleep(_sleepTime);
            }
        }
        catch (ThreadAbortException _error)
        {
            DebugLogForThread.Log(_error.ToString());
        }
    }
}