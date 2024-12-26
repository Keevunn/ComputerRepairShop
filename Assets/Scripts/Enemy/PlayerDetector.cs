using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class PlayerDetector : MonoBehaviour
    {
        private GameObject _player;
        private Enemy _enemyRef;
        // Will only have max 2 colliders for player detection
        private float _seeingRadius = 100f;
        private float _attackRadius;
        
        private bool _canSeePlayer = false, _playerInSight = false, _canAttackPlayer = false;
        [SerializeField] private LayerMask obstacleMask;
        
        public bool CanHear { get; set; }
        [SerializeField] private float fieldOfView = 180f;

        private void Awake()
        {
            /*SphereCollider[] colliders = GetComponents<SphereCollider>();
            if (colliders.Length == 1) _hearingRadius = colliders[0].radius;
            else
            {
                _seeingRadius = Math.Max(colliders[0].radius, colliders[1].radius) + 0.4f;
                _hearingRadius = Math.Min(colliders[0].radius, colliders[1].radius) + 0.4f;
            }*/

            _enemyRef = GetComponent<Enemy>();
            _seeingRadius = _enemyRef.GetSeeingRadius();
            _attackRadius = _enemyRef.GetAttackRadius();


        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            _player = other.gameObject.transform.parent.gameObject;
            _playerInSight = true;
            float distance = Vector3.Distance(transform.position, other.ClosestPoint(transform.position));
            // In seeing radius - can see player 
            //if (distance > _attackRadius && distance <= _seeingRadius) _playerInSight = true;
            // In attack radius - can hear player
            if (!(distance <= _attackRadius)) return;
            
            _canAttackPlayer = true;

        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                float distance = Vector3.Distance(transform.position, other.bounds.center);
                StartCoroutine(ClearPlayerObj(distance));
            }
        }

        private IEnumerator ClearPlayerObj(float distance)
        {
            yield return new WaitForSeconds(0.75f);
            
            if (CanHear)
            {
                // Can't hear player anymore
                if (distance >= _attackRadius) _canAttackPlayer = false;
                // Can't see player anymore
                if (distance >= _seeingRadius)
                {
                    _player = null;
                    _playerInSight = false;
                    _canSeePlayer = false;
                }
            }
            else _player = null;
        }

        public float GetFoV()
        {
            return fieldOfView;
        }

        public bool PlayerInRange()
        {
            return _canAttackPlayer || _canSeePlayer;
        }

        public bool CanAttackPlayer()
        {
            /*if (!_player || !PlayerInRange()) return false;
            
            float distance = Vector3.Distance(transform.position, _player.transform.position);
            return (distance <= _seeingRadius);*/
            return _canAttackPlayer;
        }
        public bool CanSeePlayer()
        {
            return _canSeePlayer;
        }
        public bool CanHearPlayer()
        {
            return CanHear && _canAttackPlayer;
        }
        
        

        public Vector3 GetPlayerPosition()
        {
            return _player?.transform.position ?? Vector3.zero;
        }

        public GameObject GetPlayerRef()
        {
            return _player;
        }
        
        // FOV checks
        public IEnumerator FoVRoutine()
        {
            yield return new WaitForSeconds(0.15f);
            FoVCheck();
        }

        private void FoVCheck()
        {
            if (!_playerInSight || !_player)
            {
                _canSeePlayer = false;
                return;
            }

            Vector3 dir = (_player.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dir);

            if (angle > fieldOfView / 2)
            {
                _canSeePlayer = false;
                return;
            }
            
            float distance = Vector3.Distance(transform.position, _player.transform.position);

            if (distance <= _seeingRadius && !Physics.Raycast(transform.position, dir, distance, obstacleMask)) _canSeePlayer = true;
            else if (_canSeePlayer) _canSeePlayer = false;
            

        }
    }
}