using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
   private Targetable target;

  public Targetable GetTarget()
  {
      return target;
  }

  #region Server

  public override void OnStartServer()
  {
      GameOverHandler.ServerOnGameOver += ServerHandleGameOver;

  }

  public override void OnStopServer()
  {

      GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;  }



  [Command]
    public void CmdSetTarget(GameObject targetGameObject)
    {
        if (!targetGameObject.TryGetComponent<Targetable>(out Targetable newTarget ))
        {
        return;}

        this.target = newTarget;

        
    }
    
[Server]
    public void ClearTarget()
    {
        target = null;
    }
    [Server]
    private void ServerHandleGameOver()
    {
        ClearTarget();
    }
    #endregion

    #region Client
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
