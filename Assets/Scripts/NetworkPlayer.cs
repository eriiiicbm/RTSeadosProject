using UnityEngine;
using Mirror;
using TMPro;

namespace _Project.Scripts
{
    public class NetworkPlayer : NetworkBehaviour
    {
        private MaterialPropertyBlock _materialPropertyBlock;
        
        [SerializeField] private TMP_Text displayText;
        [SerializeField] private Renderer playerRenderer;
       
        [SyncVar(hook = nameof(HandleAndDisplayName))] [SerializeField] private string displayName = "MissingName";
        [SyncVar(hook = nameof(HandleAndDisplayColor))] [SerializeField] private Color playerColor;
        
        #region Server
        [Server]
        public void SetDisplayName(string newDisplayName)
        {
            displayName = newDisplayName;
        }
        
        [Server]
        public void SetPlayerColor(Color newColor)
        {
            playerColor = newColor;
        }

        [Command]
        private void CmdSetDisplayName(string newDisplayName)
        {
            RpcLogNewName(newDisplayName);
            SetDisplayName(newDisplayName); 
        }
        #endregion
        
        #region Client
        private void HandleAndDisplayName(string oldName, string newName)
        {
            displayText.text = newName;
        }
    
        private void HandleAndDisplayColor(Color oldColor, Color newColor)
        {
            playerRenderer.material.SetColor("_BaseColor", newColor);
        }

        [ContextMenu("Set name")]
        private void SetMyName()
        {
            CmdSetDisplayName("My new name");
        }
        
        [ClientRpc]
        private void RpcLogNewName(string newDisplayName)
        {
            Debug.Log(newDisplayName);
        }
        #endregion
    }   
}
