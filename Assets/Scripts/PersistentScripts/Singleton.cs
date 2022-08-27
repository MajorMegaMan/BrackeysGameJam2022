using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LazyMonoSingletonBase<T> : MonoBehaviour where T : LazyMonoSingletonBase<T>
{
    private static readonly Lazy<T> _lazyInstance = new Lazy<T>(CreateSingleton);
    public static T instance { get { return _lazyInstance.Value; } }

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
        }

        DontDestroyOnLoad(ownerObject);
        return instance;
    }
}

public abstract class LazySingletonBase<T> where T : LazySingletonBase<T>
{
    private static readonly Lazy<T> _lazyInstance = new Lazy<T>(() => Activator.CreateInstance(typeof(T), true) as T);
    public static T instance { get { return _lazyInstance.Value; } }
}
