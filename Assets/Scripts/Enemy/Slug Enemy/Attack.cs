using System;
using System.Collections;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Enemy.Slug_Enemy
{
    public class Attack : IState
    {

        private SlugEnemy _enemyRef;
        private Vector3 _atkPoint;

        private PlayerDetector _detector;

        private GameObject _target;

        private ObjectPool<Bullet> _pool;
        private readonly Bullet _bulletRef;
        
        private readonly float _atkDmg, _atkForce, _shootDelay, _spread;
        private const float RotationSpeed = 0.5f;
        private bool _isShooting;
        
        public Attack(SlugEnemy enemyRef, Bullet bulletRef, PlayerDetector detector, float atkDmg, float atkForce, int shootDelay, float spread)
        {
            _enemyRef = enemyRef;

            //_detector = detector;

            _bulletRef = bulletRef;

            // _bullet = Object.Instantiate(bulletRef, _atkPoint, Quaternion.identity);
            // _bulletController = _bullet.GetComponent<BulletControllerScript>();
            // _bulletController.IsEnemyBullet = true;
            // _bullet.SetActive(false);

            _pool = new ObjectPool<Bullet>(
                CreateBullet,
                OnTakeBallFromPool,
                OnReturnBallToPool,
                Object.Destroy,
                true, 5, 5);
            
            _atkDmg = atkDmg;
            _atkForce = atkForce;
            _shootDelay = shootDelay;
            _spread = spread;
        }

        #region Pool Functions
        private Bullet CreateBullet()
        {
            var bullet = Object.Instantiate(_bulletRef);
            bullet.SetPool(_pool);
            bullet.IsEnemyBullet = true;
            return bullet;
        }

        private void OnTakeBallFromPool(Bullet bullet) => bullet.gameObject.SetActive(true);
        private void OnReturnBallToPool(Bullet bullet) => bullet.gameObject.SetActive(false);
        #endregion

        public void Tick()
        {
            _atkPoint = _enemyRef.transform.position + (_enemyRef.transform.forward * 0.7f) +  new Vector3(0f, 0.5f, 0f);
            //_enemyRef.StartCoroutine(ResetLookDir(LookDelay));
            if (!_isShooting && _detector.CanSeePlayer)
            { 
                Shoot();
            }
            LookAtTarget(_target.transform);
        }

        public void FixedTick()
        {
            throw new NotImplementedException();
        }

        public void OnEnter()
        {
            Debug.Log("Attack State");
            if (_enemyRef.Target && !_target) _target = _enemyRef.Target;
            if (!_detector) _detector = _enemyRef.detectRange;
        }

        public void OnExit()
        {
            //_bullet.SetActive(false);
        }
        
        private IEnumerator ResetShot(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            _isShooting = false;
        }
        

        private void Shoot()
        {
            _isShooting = true;
            
            //Finding hit position - TODO: shoot to the player directly - add spread so chance of missing player
            Vector3 playerDir = _target.transform.position - _atkPoint;
            Ray ray = new Ray(_atkPoint, playerDir.normalized);

            //If ray hits something, otherwise just shoot forward 75 units
            Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : ray.GetPoint(75);

            //Direction of targetPoint
            Vector3 dir = targetPoint - _atkPoint;
            
            //Calculate spread
            float x = Random.Range(-_spread, _spread);
            float y = Random.Range(-_spread, _spread);
            
            //Direction of targetPoint with spread
            dir += new Vector3(x, y, 0);
            //Normalise direction
            dir = dir.normalized;
            
            
            //Shoot bullet in direction of dir
            //_bullet.SetActive(true);
            //GameObject bullet = ObjectPool.Instance.GetPooledObject();
            var bullet = _pool.Get();
            if (!bullet) return;
            
            
            bullet.transform.forward = dir;
            
            //Add force to bullet
            bullet.SetVelocity(dir, _atkForce);
            bullet.SetStartPoint(_atkPoint);
            
            bullet.AddForce();

            _enemyRef.StartCoroutine(ResetShot(_shootDelay));

        }
        
        private void LookAtTarget(Transform dest)
        {
            // Ray ray = new Ray(_enemyRef.transform.position, -_enemyRef.transform.up);
            // if (Physics.Raycast(ray, out RaycastHit hit)) _enemyRef.transform.up = hit.normal;
            
            Vector3 dir = dest.position - _enemyRef.transform.position;
            Quaternion rot = Quaternion.LookRotation(dir, _enemyRef.transform.up);
            
            _enemyRef.transform.rotation =
                Quaternion.Slerp(_enemyRef.transform.rotation, rot, RotationSpeed * Time.deltaTime);
        }
    }
}