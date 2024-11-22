using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour
{
    public float GravityScale { get; set;}
    private static float _globalGravity = -9.81f;
    private Vector3 _localGravity;
    private Rigidbody _rb;

    private void OnEnable()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
    }

    private void OnDisable()
    {
        _rb.useGravity = true;
    }

    private void FixedUpdate()
    {
        _localGravity = _globalGravity * GravityScale * Vector3.up;
        _rb.AddForce(_localGravity, ForceMode.Acceleration);
    }
}