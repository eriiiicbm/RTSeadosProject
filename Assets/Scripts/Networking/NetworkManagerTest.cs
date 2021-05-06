using Mirror;
using UnityEngine;

namespace _Project.Scripts
{
    public class NetworkManagerTest : NetworkManager
    {
        [SerializeField] private GameObject unitSpawnerPrefab;
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);
         GameObject unitSpawnerInstance   = Instantiate(unitSpawnerPrefab, conn.identity.transform.position, conn.identity.transform.rotation);
         NetworkServer.Spawn(unitSpawnerInstance,conn);
            NetworkPlayer player =  conn.identity.GetComponent<NetworkPlayer>();
            player.SetDisplayName($"Player {numPlayers}");

            Color randomColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            player.SetPlayerColor(randomColor);
        }
    }
}
