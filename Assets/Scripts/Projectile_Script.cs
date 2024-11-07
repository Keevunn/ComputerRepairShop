using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class Projectile_Script : MonoBehaviour
{
    //Bullet
    public GameObject bullet;
    
    //Bullet Force
    [SerializeField] private float shootForce, upwardForce=0;
    
    //Gun Stats
    [SerializeField] private float shootingDeltaTime, spread, reloadTime, shotsDeltaTime;
    [SerializeField] private int magSize, bulletsPerTap;
    [SerializeField] private bool allowHold;
    private int _bulletsLeft, _bulletsShot;
    
    //bools
    private bool _shooting, _readyToShoot, _reloading;
    
    //Graphics TODO: make UI element later
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;
    
    //Ref
    public Camera cam;
    public Transform attackPoint;

    private void Awake()
    {
        _bulletsLeft = magSize;
        _readyToShoot = true;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();

        if (ammunitionDisplay) ammunitionDisplay.SetText(_bulletsLeft / bulletsPerTap + " / " + magSize / bulletsPerTap);
    }
    
    //Multiple bullets per shot
    private IEnumerator MultiShoot(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        Shoot();
    }
    
    private void PlayerInput()
    {
        _shooting = allowHold ? Input.GetKey(KeyCode.Mouse0) : Input.GetKeyDown(KeyCode.Mouse0);
        
        // Shooting
        if (_readyToShoot && _shooting && !_reloading && _bulletsLeft > 0)
        {
            _bulletsShot = 0;
            Shoot();
        }
        
        // Reloading
        if (Input.GetKeyDown(KeyCode.R) && _bulletsLeft < magSize && !_reloading) StartCoroutine(Reload(reloadTime));
        if (_readyToShoot && _shooting && !_reloading && _bulletsLeft <= 0) StartCoroutine(Reload(reloadTime));
    }
    
    private void Shoot()
    {
        _readyToShoot = false;
        
        //Finding hit position
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f,0.5f,0));
        RaycastHit hit;
        
        //If ray hits something
        Vector3 targetPoint;
        targetPoint = Physics.Raycast(ray, out hit) ? hit.point : ray.GetPoint(75);
        
        //Direction of targetPoint
        Vector3 dir = targetPoint - attackPoint.position;
        
        //Calculate spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        
        //Direction of targetPoint with spread
        dir += new Vector3(x, y, 0);
        //Normalise direction
        dir = dir.normalized;
        
        //Spawn bullet
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        //Shoot bullet in direction of dir
        currentBullet.transform.forward = dir;
        //Add force to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(dir * shootForce, ForceMode.Impulse);
        //For bouncing grenades
        currentBullet.GetComponent<Rigidbody>().AddForce(cam.transform.up * upwardForce, ForceMode.Impulse);
        
        //Instantiate muzzle flash
        if (muzzleFlash) Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
        
        _bulletsLeft--;
        _bulletsShot++;

        StartCoroutine(ResetShot(shootingDeltaTime, currentBullet));
        
        //Multiple bullets - can turn this into timer method
        if (_bulletsShot < bulletsPerTap && _bulletsLeft > 0) StartCoroutine(MultiShoot(shotsDeltaTime));
    }
    
    //Reset Shooting
    private IEnumerator ResetShot(float delayTime, GameObject obj)
    {
        yield return new WaitForSeconds(delayTime);
        _readyToShoot = true;
        Destroy(obj);
    }

    //Reload timer
    private IEnumerator Reload(float delayTime)
    {
        _reloading = true;
        yield return new WaitForSeconds(delayTime);
        _bulletsLeft = magSize;
        _reloading = false;
    }
}
