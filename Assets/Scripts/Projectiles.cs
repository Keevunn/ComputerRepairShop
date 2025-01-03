using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Pool;
using GameObject = UnityEngine.GameObject;
using Random = UnityEngine.Random;

public class Projectiles : MonoBehaviour
{
    //Bullet
    public Bullet bulletRef;
    private int _currentBullet;
    private ObjectPool<Bullet> _pool;
    
    //Bullet Force
    [Header("Bullet Properties")]
    [SerializeField] private float shootForce;
    [SerializeField] private float upwardForce;

    //Gun Stats
    [Header("Gun Properties")]
    [SerializeField] private float shootingDeltaTime;
    [SerializeField] private float spread;
    [SerializeField] private float reloadTime;
    [SerializeField] private float shotsDeltaTime;
    [SerializeField] private int magSize;
    [SerializeField] private int bulletsPerTap;
    [SerializeField] private bool allowHold;
    private int _bulletsLeft, _bulletsShot;

    [Header("Keybinds")] 
    [SerializeField] private KeyCode shootKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode reloadKey = KeyCode.R;
    
    //bools
    private bool _shooting, _readyToShoot, _reloading, _readyToReload;
    
    //Graphics 
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;
        
    //Ref
    public Camera cam;
    public Transform attackPoint;

    #region Pool functions
    private Bullet CreateBullet()
    {
        var bullet = Instantiate(bulletRef);
        bullet.name = "Player Bullet";
        bullet.SetPool(_pool);
        bullet.IsEnemyBullet = false;
        return bullet;
    }

    private void OnTakeBallFromPool(Bullet bullet) => bullet.gameObject.SetActive(true);
    private void OnReturnBallToPool(Bullet bullet) => bullet.gameObject.SetActive(false);
    #endregion
    
    private void Awake()
    {
        _bulletsLeft = magSize;
        _readyToShoot = true;
        
        _pool = new ObjectPool<Bullet>(
            CreateBullet,
            OnTakeBallFromPool,
            OnReturnBallToPool,
            Destroy,
            true, 10, 15);
    }

    private void Start()
    {
        if (!attackPoint) attackPoint = transform.Find("Attack Point").transform;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();

        if (ammunitionDisplay) ammunitionDisplay.SetText(_bulletsLeft / bulletsPerTap + " / " + magSize / bulletsPerTap);
    }

    void FixedUpdate()
    {
        // Shooting
        if (_readyToShoot && _shooting && !_reloading && _bulletsLeft > 0)
        {
            _bulletsShot = 0;
            Shoot();
        }
        
        // Reloading
        if (_readyToReload && _bulletsLeft < magSize && !_reloading) StartCoroutine(Reload(reloadTime));
        if (_readyToShoot && _shooting && !_reloading && _bulletsLeft <= 0) StartCoroutine(Reload(reloadTime));
    }
    
    //Multiple bullets per shot
    private IEnumerator MultiShoot(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        Shoot();
    }
    
    private void PlayerInput()
    {
        _shooting = allowHold ? Input.GetKey(shootKey) : Input.GetKeyDown(shootKey);
        _readyToReload = Input.GetKeyDown(reloadKey);
    }
    
    private void Shoot()
    {
        _readyToShoot = false;
        
        //Finding hit position
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f,0.5f,0));

        //If ray hits something
        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : ray.GetPoint(75);
        
        /*
        //Close range debugging
        if (hit.collider)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                _enemy = hit.transform.gameObject.GetComponent<NPCControlScript>();
            }
        }*/
        
        //Direction of targetPoint
        Vector3 dir = targetPoint - attackPoint.position;
        
        //Calculate spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        
        //Direction of targetPoint with spread
        dir += new Vector3(x, y, 0);
        //Normalise direction
        dir = dir.normalized;
        
        //Spawn bullet - TODO: Can load the number of bullets per shot at start then no need to instantiate them everytime, just sleep them
        var bullet = _pool.Get();
        if (!bullet) return;
        
        //Shoot bullet in direction of dir
        bullet.transform.forward = dir;
        //Add force to bullet
        bullet.SetVelocity(dir, shootForce);
        bullet.SetStartPoint(attackPoint.position);
        bullet.AddForce();
        //For bouncing grenades
        //currentBulletrb.AddForce(cam.transform.up * upwardForce, ForceMode.Impulse);
        
        //Instantiate muzzle flash
        if (muzzleFlash) Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
        
        _bulletsLeft--;
        _bulletsShot++;

        StartCoroutine(ResetShot(shootingDeltaTime));
        
        //Multiple bullets - can turn this into timer method
        if (_currentBullet < bulletsPerTap) _currentBullet++;
        if (_currentBullet >= bulletsPerTap) _currentBullet = 0;
        if (_bulletsShot < bulletsPerTap && _bulletsLeft > 0) StartCoroutine(MultiShoot(shotsDeltaTime));
    }
    
    //Reset Shooting
    private IEnumerator ResetShot(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        _readyToShoot = true;
        //Destroy(obj);
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
