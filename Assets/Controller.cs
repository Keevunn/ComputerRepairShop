using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] private float speed = 5.0f;
    private float _translation;
    private float _strafe;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _translation = Input.GetAxis("Vertical")* speed * Time.deltaTime;
        _strafe = Input.GetAxis("Horizontal")* speed * Time.deltaTime;
        transform.Translate(_strafe,0, _translation);
        if (Input.GetKeyDown("escape")) Cursor.lockState = CursorLockMode.None;
    }

    private void FixedUpdate()
    {
        
    }
}
