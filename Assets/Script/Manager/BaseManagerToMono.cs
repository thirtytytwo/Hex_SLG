using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManagerToMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T GetInstance()
    {

        if (instance == null)
        {
            GameObject obj = new GameObject();
            obj.name = typeof(T).ToString();
            instance = obj.AddComponent<T>();
        }

        DontDestroyOnLoad(instance);

        return instance;
    }
}
