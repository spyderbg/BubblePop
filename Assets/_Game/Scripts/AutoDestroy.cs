using System;
using DarkTonic.PoolBoss;
using UnityEngine;
using Utils;

public class AutoDestroy : MonoBehaviour
{
    public float Delay = 1.0f;
    
    private void OnEnable()
    {
        World.Instance.Executor.DelayExecute(Delay, x => PoolBoss.Despawn(transform));
    }
}
