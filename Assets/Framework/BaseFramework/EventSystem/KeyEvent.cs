
using Mono.Cecil;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

static class KeyEventHandler
{
    public interface IKeyDownEvent
    {
        bool OnKeyDown(KeyCode _keyCode, bool _alt, bool _ctrl, bool _shift);
    }
    public interface IKeyUpEvent
    {
        bool OnKeyUp(KeyCode _keyCode, bool _alt, bool _ctrl, bool _shift);
    }

    public delegate bool KeyEventFunc(KeyCode _key, bool _alt, bool _ctrl, bool _shift);
    static Dictionary<EventType, List<KeyEventFunc>> eventHandlers = new Dictionary<EventType, List<KeyEventFunc>>();
    static Event keyEvent = new Event();

    public static void RegistKeyEventFunc(EventType _type, KeyEventFunc _func)
    {
        List<KeyEventFunc> keyEventFuncs = null;

        if(eventHandlers.TryGetValue(_type, out keyEventFuncs) == false)
        {
            keyEventFuncs = new List<KeyEventFunc>();
            eventHandlers.Add(_type, keyEventFuncs);  
        }

        keyEventFuncs.Add(_func);
    }

    public static void UnregistKeyEventFunc(EventType _type, KeyEventFunc _func)
    {
        List<KeyEventFunc> keyEventFuncs = null;

        if (eventHandlers.TryGetValue(_type, out keyEventFuncs) == false)
        {
            return;
        }

        keyEventFuncs.Remove(_func);
    }

    public static void OnKeyProcess(Event _event)
    {
        if(_event != null) 
        {
            List<KeyEventFunc> eventHandler = null;
            if (eventHandlers.TryGetValue(_event.type, out eventHandler) == false)
            {
                return;
            }

            for (int j = eventHandler.Count - 1; j >= 0; --j)
            {
                var _eventFunc = eventHandler[j];
                if (_eventFunc != null && _eventFunc(_event.keyCode, _event.alt, _event.control, _event.shift))
                {
                    _event.Use();
                    break;
                }
            }
        }
    }
}