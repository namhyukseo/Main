using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPool : Framework.Singleton.Singleton<ObjectPool>
{
    protected override void OnInit()
    {
        base.OnInit();
    }

    protected override void OnRelease()
    {
        base.OnRelease();
    }

    public T Load<T>()
        where T : class, iPoolObject
    {
        T _ret = null;
        Queue<iPoolObject> _queue = null;

        if (!objects.TryGetValue(typeof(T), out _queue) || _queue.Count == 0)
        {
            return _ret;
        }

        _ret = _queue.Dequeue() as T;
        _ret.OnLoad();
        
        return _ret;
    }

    public  void Unload(iPoolObject _obj)
    {
        _obj.OnUnload();
        Queue<iPoolObject> _queue = null;
        if(!objects.TryGetValue(_obj.GetType(), out _queue))
        {
            _queue = new Queue<iPoolObject>();
            objects.Add(_obj.GetType(), _queue);
        }

        _queue.Enqueue(_obj);
    }

    protected Dictionary<Type, Queue<iPoolObject>> objects = new Dictionary<Type, Queue<iPoolObject>>();
}
/*
public class GameObjectPool : Framework.Singleton.Singleton<GameObjectPool>
{
    protected override void OnInit()
    {
        rootObject = new GameObject(string.Format("GameObjectPoo[{0}]", typeof(T).Name));
        rootObject.SetActive(false);
        GameObject.DontDestroyOnLoad(rootObject);
    }

    protected override void OnRelease()
    {
    }

    public T Load<T>() 
        where T : Component, iPoolObject
    {
        T _ret = objects.Dequeue();
        if (_ret != null)
        {
            _ret.OnLoadPoolObject();
        }
        return _ret;
    }

    public void Unload(iPoolObject _obj)
    {
        _obj.OnLoadPoolObject();
        
        Component _component = _obj as Component;

        if(_component != null)
        {
            _obj.transform.SetParent(rootObject.transform, false);
            objects.Enqueue(_obj);
        }
    }

    protected Queue<T> objects = new Queue<T>();
    protected GameObject    rootObject = null;
}
*/