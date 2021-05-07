using System;
using Mirror;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace _Project.Scripts
{
    public class UnitMovement : NetworkBehaviour
    {
        [SerializeField] private NavMeshAgent navMeshAgent;
        [SerializeField] private float chaseRange = 10f;
        [SerializeField] private Targeter targeter;
//        

        #region Server

        [ServerCallback]
        private void Update()
        {
            Targetable target = targeter.GetTarget();

            if (target != null)
            {
                if ((target.transform.position-transform.position).sqrMagnitude> chaseRange*chaseRange)
                {
                    navMeshAgent.SetDestination(target.transform.position);
                }
                else if (navMeshAgent.hasPath)
                {
                    navMeshAgent.ResetPath();
                }

                return;
            }

            if (!navMeshAgent.hasPath)
            {
                return;
            }

            if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            {
                return;
            }

            navMeshAgent.ResetPath();
        }

        [Command]
        public void CmdMove(Vector3 position)
        {
            targeter.ClearTarget();
            if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                return;
            }

            navMeshAgent.SetDestination(hit.position);
            Debug.Log("Moving");
        }

        #endregion

        /*     #region Client
     
             public override void OnStartAuthority()
             {
                 _mainCamera = Camera.main;
             }
     
             [ClientCallback]
             private void Update()
             {
                 if(!hasAuthority) return;
                 
                 if(!Mouse.current.rightButton.wasPressedThisFrame) return;
                 
                 Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
                 
                 if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) return;
                 
                 CmdMove(hit.point);
             }
             #endregion*/
    }
}