using System;
using System.Collections;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Enemy.Slug_Enemy
{
    public class Attack : IState
    {

        private SlugEnemy _enemyRef;
        private Vector3 _atkPoint;


        private GameObject _target;
        private Player3DController _playerController;

        private GameObject _bullet;
        private BulletControllerScript _bulletController;
        
        private readonly float _atkDmg, _shootDelay, _spread;
        private const float RotationSpeed = 0.1f;
        private const float LookDelay = 3f;
        private bool _isShooting;
        
        public Attack(SlugEnemy enemyRef, GameObject bulletRef, int atkDmg, int shootDelay, float spread)
        {
            _enemyRef = enemyRef;

            _bullet = Object.Instantiate(bulletRef, _atkPoint, Quaternion.identity);
            _bulletController = _bullet.GetComponent<BulletControllerScript>();
            _bulletController.IsEnemyBullet = true;
            _bullet.SetActive(false);
            
            _atkDmg = atkDmg;
            _shootDelay = shootDelay;
            _spread = spread;
        }
        
        public void Tick()
        {
            if (!_target || _playerController is null)
            {
                _target = _enemyRef.Target;
                _playerController = _enemyRef.GetPlayerScript(_target);
            } 
            _atkPoint = _enemyRef.transform.position + (_enemyRef.transform.forward * 0.7f) +  new Vector3(0f, 0.5f, 0f);
            _enemyRef.StartCoroutine(ResetLookDir(LookDelay));
            if (!_isShooting)
                Shoot();

        }

        public void FixedTick()
        {
            throw new NotImplementedException();
        }

        public void OnEnter()
        {
            Debug.Log("Attack State");
            if (_enemyRef.Target)
            {
                _target = _enemyRef.Target;
                _playerController = _enemyRef.GetPlayerScript(_target);
            }
            _bullet.SetActive(false);
            Shoot();
        }

        public void OnExit()
        {
            return;
        }
        
        private IEnumerator ResetShot(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            _isShooting = false;
        }
        
        private IEnumerator ResetLookDir(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            LookAtTarget(_target.transform.position);
        }

        private void Shoot()
        {
            _isShooting = true;
            
            //Finding hit position - TODO: shoot to the player directly - add delay so chance of missing player
            Ray ray = new Ray(_atkPoint, _enemyRef.transform.forward);

            //If ray hits something
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
            _bullet.SetActive(true);
            
            _bullet.transform.forward = dir;
            
            //Add force to bullet
            _bulletController.SetVelocity(dir, 50f);
            _bulletController.SetStartPoint(_atkPoint);
            
            _bulletController.AddForce();

            _enemyRef.StartCoroutine(ResetShot(_shootDelay));

        }
        
        //TODO: Rotation to fast and immediate - slow it down!
        private void LookAtTarget(Vector3 dest)
        {
            Vector3 dir = dest - _enemyRef.transform.position;
            
            dir.y = 0;
            Quaternion rot = Quaternion.LookRotation(dir);
            _enemyRef.transform.rotation = Quaternion.Slerp(_enemyRef.transform.rotation, rot, RotationSpeed);
        }
    }
}