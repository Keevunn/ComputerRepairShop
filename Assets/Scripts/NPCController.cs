using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static System.Math;

public class NPCController : MonoBehaviour
{
    [SerializeField] private int _health = 10;
    private bool _alive = true;
    private bool _isTakingDamage;
    [SerializeField] private TextMeshPro HealthDisplay;

    public void Awake()
    {
        HealthDisplay.SetText(Max(_health, 0).ToString());
    }
    public int GetHealth()
    {
        return _health;
    }

    public void SetHealth(int health)
    {
        _health = health;
    }
    public bool IsAlive()
    {
        return _alive;
    }
    
    
    /*private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Bullet")) return;
        _isTakingDamage = true;
        Destroy(other.gameObject);
    }*/

    public void SetDamage(bool status)
    {
        _isTakingDamage = status;
    }

    public void TakeDamage(int dmg)
    {
        _health--;
        if (_health <= 0) _alive = false;
        _isTakingDamage = false;
    }

    private void Update()
    {
        if (_isTakingDamage) TakeDamage(1);
        HealthDisplay.SetText(Max(_health, 0).ToString());
    }

    public void DestroyObj()
    {
        Destroy(gameObject);
    }
}
