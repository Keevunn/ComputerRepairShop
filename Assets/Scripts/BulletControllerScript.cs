using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControllerScript : MonoBehaviour
{
    private Rigidbody _rb;
    public bool IsEnemyBullet { get; set; }
    private Vector3 _velocity, _startPoint;
    private NPCController _enemy;
    private Player3DController _player;
    [SerializeField] private float deleteDistance = 30f;
    private float _distanceTravelled;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _velocity = transform.forward * 100f;
        IsEnemyBullet = false;
    }

    private void Update()
    {
        _distanceTravelled = Vector3.Distance(transform.position, _startPoint);
        if (_distanceTravelled >= deleteDistance) gameObject.SetActive(false);;
    }

    private void FixedUpdate()
    {
       AddForce();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsEnemyBullet && other.gameObject.CompareTag("Enemy"))
        {
            _enemy = other.gameObject.GetComponent<NPCController>();
            if (_enemy) _enemy.SetDamage(true);
            gameObject.SetActive(false);
        } if (IsEnemyBullet && other.CompareTag("Player") )
        {
            _player = other.gameObject.transform.parent.gameObject.GetComponent<Player3DController>();
            _player.IsTakingDamage = true;
            gameObject.SetActive(false);
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
