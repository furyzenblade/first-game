using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierStorage : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        
    }

    public void X()
    {
        Event e = Event.current;

        if (e.isKey && e.type == EventType.KeyDown)
        {
            if (e.modifiers == EventModifiers.None)
            {
                Debug.Log("No modifier key is active.");
            }
            else
            {
                if ((e.modifiers & EventModifiers.Alt) != 0)
                {
                    Debug.Log("Alt key is active.");
                }

                if ((e.modifiers & EventModifiers.Control) != 0)
                {
                    Debug.Log("Control key is active.");
                }

                if ((e.modifiers & EventModifiers.Shift) != 0)
                {
                    Debug.Log("Shift key is active.");
                }

                if ((e.modifiers & EventModifiers.CapsLock) != 0)
                {
                    Debug.Log("Alt key is active.");
                }

                if ((e.modifiers & EventModifiers.Command) != 0)
                {
                    Debug.Log("Control key is active.");
                }
            }
        }
    }
}
