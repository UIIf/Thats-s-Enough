using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemy view")]
    

    [SerializeField] Transform enemyTarget;
    public Transform trarget
    {
        get => enemyTarget;
    }

    [Tooltip("Count of fixedUpdate before next view")]
    [SerializeField] uint fixedUpdateCount = 100;
    public uint maxFixedUpdateCount
    {
        get => fixedUpdateCount;
    }

    uint fixedUpdateCounter = 0;
    public uint fUCounter
    {
        get => fixedUpdateCounter;
    }

    [Min(0.1f)]
    [SerializeField] float lookingAroundTimer;
    public float lookArT
    {
        get => lookingAroundTimer;
    }

    private void FixedUpdate()
    {
        fixedUpdateCounter = ++fixedUpdateCounter % fixedUpdateCount;
    }
}
