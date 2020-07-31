using UnityEngine;

/// <summary>
/// Quickly turns any Unity MonoBehaviour into a singleton.
/// 
/// Example of how to implement. Change the class declaration at the top
/// of a new Unity C# script to look similar to this ('SpawnManager' is
/// just an example, it should be whatever you've named the class):
/// 
///   public class SpawnManager : MonoSingleton<SpawnManager>
/// 
/// Credit for this code goes to Jonathan Weinberger. This is copied from
/// his Unity C# Survival Guide. The original tutorial can be found at:
/// 
/// https://learn.unity.com/tutorial/monosingleton?uv=2018.4&courseId=5cf06bd1edbc2a58d7fc3209&projectId=5d63c601edbc2a001ffa567e#
/// 
/// </summary>
/// <typeparam name="T">The name of the class we're turning into a MonoSingleton.</typeparam>
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError(typeof(T).ToString() + " is NULL.");

            return _instance;
        }
    }

    /// <summary>
    /// Since we have claimed Awake() and derived classes might need it, we
    /// provide Init() below as a substitute for Awake() when actually using
    /// the MonoSingleton.
    /// </summary>
    private void Awake() 
    {
        _instance = this as T;
        Init();
    }

    /// <summary>
    /// Optional to override Init.
    /// 
    /// To override, use the following code in your new class:
    /// 
    /// public override void Init()
    /// {
    ///     base.Init();    
    ///     <your code...>
    /// }
    /// </summary>
    public virtual void Init()
    {

    }
}

