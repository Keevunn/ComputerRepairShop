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
        
        private bool _playerInSight = false;
        public bool CanSeePlayer { get; private set; }
        public bool CanAttackPlayer { get; private set; }
        public bool CanHearPlayer => CanHear && CanAttackPlayer;
        [SerializeField] private LayerMask obstacleMask;
        
        public bool CanHear { get; set; }
        [SerializeField] private float fieldOfView = 180f;

        private void Awake()
        {
            _enemyRef = GetComponent<Enemy>();
            _seeingRadius = _enemyRef.GetSeeingRadius();
            _attackRadius = _enemyRef.GetAttackRadius();

            CanSeePlayer = false;
            CanAttackPlayer = false;

        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            _player = other.gameObject.transform.parent.gameObject;
            // In seeing radius - can see player 
            _playerInSight = true;
            
            float distance = Vector3.Distance(transform.position, other.ClosestPoint(transform.position));
            
            // In attack radius - can hear player
            if (!(distance <= _attackRadius)) return;
            CanAttackPlayer = true;

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
                if (distance >= _attackRadius) CanAttackPlayer = false;
                // Can't see player anymore
                if (distance >= _seeingRadius)
                {
                    _player = null;
                    _playerInSight = false;
                    CanSeePlayer = false;
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
            return CanAttackPlayer || CanSeePlayer;
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
                CanSeePlayer = false;
                return;
            }

            if ( 89f < Mathf.Abs(Vector3.Angle(_player.transform.up, transform.up)) && 
                 Mathf.Abs(Vector3.Angle(_player.transform.up, transform.up)) < 91f)
            {
                CanSeePlayer = true;
                return;
            }

            Vector3 dir = (_player.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dir);

            if (angle > fieldOfView / 2)
            {
                CanSeePlayer = false;
                return;
            }
            
            float distance = Vector3.Distance(transform.position, _player.transform.position);

            if (distance <= _seeingRadius && !Physics.Raycast(transform.position, dir, distance, obstacleMask)) CanSeePlayer = true;
            else if (CanSeePlayer) CanSeePlayer = false;
        }
    }
}