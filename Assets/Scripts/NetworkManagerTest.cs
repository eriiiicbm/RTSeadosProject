using Mirror;
using UnityEngine;

namespace _Project.Scripts
{
    public class NetworkManagerTest : NetworkManager
    {
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);
            NetworkPlayer player =  conn.identity.GetComponent<NetworkPlayer>();
            player.SetDisplayName($"Player {numPlayers}");

            Color randomColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            player.SetPlayerColor(randomColor);
        }
    }
}
