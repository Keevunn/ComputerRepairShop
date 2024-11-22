using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_control : MonoBehaviour
{
    public float playerSpeed;
    private Rigidbody2D _rb;
    private Vector2 _movementDir;
    private string _horizontal = "Horizontal";
    private float _previousHorizontal = 0;
    private string _idleHorizontal = "Idle_horizontal";
    private string _vertical = "Vertical";
    private float _previousVertical = 0;
    private string _idleVertical = "Idle_vertical";
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        _movementDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        _animator.SetFloat(_horizontal, _movementDir.x);
        _animator.SetFloat(_idleHorizontal, _previousHorizontal);
        _animator.SetFloat(_vertical, _movementDir.y);
        _animator.SetFloat(_idleVertical, _previousVertical);
        if (_movementDir == Vector2.zero) return;
        _previousHorizontal = _movementDir.x;
        _previousVertical = _movementDir.y;
    }

    void FixedUpdate()
    {
        _rb.velocity = _movementDir * playerSpeed;
    }
}
