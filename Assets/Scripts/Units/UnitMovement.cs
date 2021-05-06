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
//        
        #region Server
        [Command] 
        public void CmdMove(Vector3 position)
        {
            navMeshAgent.SetDestination(position);
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
