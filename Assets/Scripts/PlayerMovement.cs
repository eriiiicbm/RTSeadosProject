using System;
using Mirror;
using UnityEngine;
using UnityEngine.AI;

namespace _Project.Scripts
{
    public class PlayerMovement : NetworkBehaviour
    {
        [SerializeField] private NavMeshAgent navMeshAgent;
        private Camera _mainCamera;
        
        #region Server
        [Command] 
        private void CmdMove(Vector3 position)
        {
            navMeshAgent.SetDestination(position);
        }
        #endregion
        
        #region Client

        public override void OnStartAuthority()
        {
            _mainCamera = Camera.main;
        }

        [ClientCallback]
        private void Update()
        {
            if(!hasAuthority) return;
            
            if(!Input.GetMouseButtonDown(1)) return;
            
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) return;
            
            CmdMove(hit.point);
        }
        #endregion
    }
}
