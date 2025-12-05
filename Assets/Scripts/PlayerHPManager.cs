using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHPManager : MonoBehaviour
{
    public float maxHP = 3f;
    public float currentHP;

    public static PlayerHPManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        currentHP = maxHP;
    }

    void Update()
    {
        if (currentHP <= 0)
        {
            GameManager.instance.HandlePlayerDeath(gameObject);
        }
    }

    public void InflictDamage(float damage)
    {
        currentHP -= damage;
    }
}
