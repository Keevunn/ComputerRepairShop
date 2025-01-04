using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    private Rigidbody _rb;
    public bool IsEnemyBullet { get; set; }
    private Vector3 _velocity, _startPoint;
    [SerializeField] private float deleteDistance = 50f;
    private float _distanceTravelled;

    private IObjectPool<Bullet> _pool;

    public void SetPool(IObjectPool<Bullet> pool) => _pool = pool;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _velocity = transform.forward * 100f;
        IsEnemyBullet = false;
    }

    private void Update()
    {
        if (_pool == null) Destroy(this);
        
        _distanceTravelled = Vector3.Distance(transform.position, _startPoint);
        
        if (_distanceTravelled >= deleteDistance) _pool?.Release(this);
        
    }

    private void FixedUpdate()
    {
       AddForce();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsEnemyBullet && other.gameObject.CompareTag("Enemy"))
        {
            Enemy.Enemy enemy = other.transform.parent.GetComponent<Enemy.Enemy>();
            enemy?.TakeDamage(1);
            _pool.Release(this);
        } if (IsEnemyBullet && other.CompareTag("Player") )
        {
            Player3DController player = other.gameObject.transform.parent.gameObject.GetComponent<Player3DController>();
            if (player)
                player.IsTakingDamage = true;
            _pool.Release(this);
        } 
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
        transform.position = _startPoint;
    }

    public void Reset()
    {
        _velocity = Vector3.zero;
    }
}
