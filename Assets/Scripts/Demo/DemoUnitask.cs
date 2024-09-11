using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System;

public class DemoUnitask : MonoBehaviour
{
    private void Start()
    {
        
    }

    private void DoLog()
    {
        Debug.Log("This is log!");
        Debug.Log("Thread: " + Environment.CurrentManagedThreadId);
    }
}
