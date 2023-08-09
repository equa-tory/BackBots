using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public abstract class EnemyData : Damageable
{
    [Header("EnemyData")]
    public EnemyInfo info;
    // [HideInInspector] public SpawnManager sm;
    [SerializeField] public NavMeshAgent agent;
    public Transform target;
    
    public LayerMask playerMask;
    public LayerMask groundMask;

    [SerializeField] public bool playerInSightRange;
    [SerializeField] public bool playerInAttackRange;

    [SerializeField] public Vector3 walkPoint;
    [SerializeField] public bool stunned;
    [SerializeField] public bool stunFlag;
    [SerializeField] public bool canWalk = true;
    [SerializeField] public bool walkPointSet;
    [SerializeField] public bool alreadyAttacked;

    public AudioSource attackSound;
    public AudioSource damageSound;
    public AudioSource deathSound;

    public abstract void Attack();
}

public abstract class Enemy : EnemyData
{
    public abstract override void Attack();
}
