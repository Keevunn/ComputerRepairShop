using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static System.Math;

public class NPCControlScript : MonoBehaviour
{
    [SerializeField] private int _health = 100;
    private bool _alive = true;
    [SerializeField] private TextMeshPro HealthDisplay;

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

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Bullet")) return;
        _health--;
        Destroy(other.gameObject);
        if (_health <= 0) _alive = false;
    }

    private void Update()
    {
        HealthDisplay.SetText(Max(_health, 0).ToString());
    }

    public void DestroyObj()
    {
        Destroy(gameObject);
    }
}
