

using System;
using System.Data;
using System.Diagnostics;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class ThreadObject
{
    public ThreadObject(ThreadProcFunc _func, int _fps)
    {
        func = _func;
        fixedDeltaTime = 1.0f / (float)_fps;
    }    
    public  delegate bool ThreadProcFunc(float _deltaTime);
    public int ThreadID {get; protected set;}
    private Thread thread = null;
    private ThreadProcFunc  func = null;
    private bool stopThreadFlag = false;
    private float fixedDeltaTime = 0.0f;
    public void Start()
    {
        thread = new Thread(this.OnThreadProc);
        this.ThreadID = thread.ManagedThreadId;        
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

        float _fixedDeltaTime = fixedDeltaTime * 0.001f;

        try
        {
            while(!stopThreadFlag)
            {
                float _deltaTime = (float)((_curTime - _beforeTime) / 10000000.0);
                if(!func(_deltaTime))
                    break;
                Thread.Sleep((int)Math.Min(this.fixedDeltaTime * 1000, (Math.Max(0,(this.fixedDeltaTime - _deltaTime) * 1000))));
                _beforeTime = _curTime;
                _curTime = DateTime.Now.Ticks;                
            }
        }
        catch (ThreadAbortException _error)
        {
            UnityEngine.Debug.LogException( _error );
        }
    }
}