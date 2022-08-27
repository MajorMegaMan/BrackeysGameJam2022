using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LazyMonoSingletonBase<T> : MonoBehaviour where T : LazyMonoSingletonBase<T>
{
    static T _instance = null;
    public static T instance { get { return _instanceGetter.Invoke(); } }


    delegate T InstanceGetter();
    static InstanceGetter _instanceGetter = CreateSingleton;

    private static T CreateSingleton()
    {
        var heirarchyArray = FindObjectsOfType<T>();

        GameObject ownerObject;
        T instance;
        if (heirarchyArray.Length > 0)
        {
            instance = heirarchyArray[0];
            ownerObject = instance.gameObject;
        }
        else
        {
            ownerObject = new GameObject($"{typeof(T).Name} (singleton)");
            instance = ownerObject.AddComponent<T>();
            instance.OnCreateInstance();
        }

        DontDestroyOnLoad(ownerObject);

        _instance = instance;
        _instanceGetter = ReturnSingleton;

        return instance;
    }

    private static T ReturnSingleton()
    {
        return _instance;
    }

    protected virtual void OnCreateInstance()
    {

    }
}

public abstract class LazySingletonBase<T> where T : LazySingletonBase<T>
{
    private static readonly Lazy<T> _lazyInstance = new Lazy<T>(() => Activator.CreateInstance(typeof(T), true) as T);
    public static T instance { get { return _lazyInstance.Value; } }
}
