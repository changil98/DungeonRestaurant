using System;
using UnityEngine;

[Serializable]
public class StatBase
{
    [SerializeField] protected int hp;
    public int HP { get { return hp; } set { hp = value; } }

    [SerializeField] protected int mp;
    public int MP { get { return mp; } set { mp = value; } }
    [SerializeField] protected int atk;
    public int ATK { get { return atk; } set { atk = value; } }
    [SerializeField] protected int def;
    public int DEF { get { return def; } set { def = value; } }
    [SerializeField] protected int resistance;
    public int Resistance { get { return resistance; } set { resistance = value; } }
    [SerializeField] protected float moveSpeed;
    public float MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }
    [SerializeField] protected float attackSpeed;
    public float AttackSpeed { get { return attackSpeed; } set { attackSpeed = value; } }
    [SerializeField] protected float range;
    public float Range { get { return range; } set { range = value; } }

    [SerializeField] protected float criticalPercent;
    public float CriticalPercent { get { return criticalPercent; } set { criticalPercent = value; } }
    [SerializeField] protected float criticalDamage;
    public float CriticalDamage { get { return criticalDamage; } set { criticalDamage = value; } }
}
