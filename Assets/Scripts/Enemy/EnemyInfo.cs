using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Scriptables/Enemy/New Enemy",fileName ="enemy_")]
public class EnemyInfo : ScriptableObject
{
    public string enemyName;
    public float damage;
    public float attackSpeed;
    public float attackCd;

    public float stunCd = 2f;


    public float attackRange;
    public float attackWide = 1;
    public float sightRange;

    public float walkPointRange;

    public float rotationSpeed;

    public float walkSpeed;
    public float runSpeed;
}
