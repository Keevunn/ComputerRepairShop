using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy.Slug_Enemy
{
    public class SlugEnemy : Enemy  
    {
        [SerializeField] private GameObject bulletPrefab;
        
        /*private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;

            Vector3 line1 = DirFromAngle(transform.eulerAngles.y + detectRange.GetFoV() / 2);
            Vector3 line2 = DirFromAngle(transform.eulerAngles.y - detectRange.GetFoV() / 2);
            
            Gizmos.DrawLine(transform.position, transform.position + line1 * radius);
            Gizmos.DrawLine(transform.position, transform.position + line2 * radius);

            if (detectRange.PlayerInRange())
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, detectRange.GetPlayerPosition());
            }
            
        }
        private Vector3 DirFromAngle(float angle)
        {
            return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
        }*/

        public override void Awake()
        {
            base.Awake();
            
            // STATES
            //Idle = new Idle(transform, agent, walkDistance);
            
            // TODO: ATTACK STATE SETUP
            Attack = new Attack(this, bulletPrefab, 1, 1, 0f);
            
            AT(MoveToPlayer, Attack, detectRange.CanAttackPlayer);
            AT(Attack, MoveToPlayer, () => !detectRange.CanAttackPlayer() && detectRange.CanSeePlayer());
            //AT(Attack, Idle, () => !detectRange.CanSeePlayer() && !detectRange.CanHearPlayer());
            //AT(Idle, Attack, detectRange.CanHearPlayer);
            
            
            // TRANSITIONS
            
            /*AT(Idle, MoveToPlayer, /* When player in FOV #1# );
            AT(Attack, MoveToPlayer, /* Player not in range, in FOV #1# );

            AAT(BeingAttacked, /* Enemy can't see player, but taking damage #1# );
            AAT(Dead, () => health == 0);*/
            
            StateMachine.SetState(Idle);
        }
        
        private void Update()
        {
            StartCoroutine(detectRange.FoVRoutine());
            if (detectRange.PlayerInRange()) Target = detectRange.GetPlayerRef(); 
            StateMachine.Tick();
        }

        public Player3DController GetPlayerScript(GameObject player)
        {
            return player.GetComponent<Player3DController>();
        }
        
    }
}