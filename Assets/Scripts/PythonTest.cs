using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Scripting.Python;
using Python.Runtime;
using System;

public class PythonTest : MonoBehaviour
{
    private void Awake()
    {
        PythonRunner.EnsureInitialized();
        using (Py.GIL())
        {
            try
            {
                dynamic importlib = Py.Import("importlib");
                dynamic Unity_Test = Py.Import("Unity_Test");
                importlib.reload(Unity_Test);
                dynamic result = Unity_Test.hello_world();
                Debug.Log(result);
            }
            catch (PythonException e)
            {
                Debug.LogException(e);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
