using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControllerScript : MonoBehaviour
{
    private Rigidbody _rb;
    private Vector3 _velocity, _startPoint;
    private NPCControlScript _enemy;
    [SerializeField] private float deleteDistance = 30f;
    private float _distanceTravelled;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _velocity = transform.forward * 100f;
    }

    private void Update()
    {
        _distanceTravelled = Vector3.Distance(transform.position, _startPoint);
        if (_distanceTravelled >= deleteDistance) Destroy(gameObject);
    }

    private void FixedUpdate()
    {
       AddForce();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            _enemy = other.gameObject.GetComponent<NPCControlScript>();
            if (_enemy) _enemy.SetDamage(true);
        }
        Destroy(gameObject);
    }

    public void AddForce()
    {
        _rb.MovePosition(_rb.position + _velocity * Time.fixedDeltaTime);
    }

    public void SetVelocity(Vector3 dir, float force)
    {
        _velocity = dir * force;
    }

    public void SetStartPoint(Vector3 point)
    {
        _startPoint = point;
    }
}
