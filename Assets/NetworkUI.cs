using Unity.Netcode;
using UnityEngine;

public class NetworkUI : MonoBehaviour
{
    public GameObject player1Prefab;
    public GameObject player2Prefab;

    void Start()
    {
        // Hook into connection event
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    void OnGUI()
    {
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            if (GUI.Button(new Rect(10, 10, 100, 30), "Host"))
            {
                NetworkManager.Singleton.StartHost();
                SpawnCustomPlayer(NetworkManager.Singleton.LocalClientId, player1Prefab);
            }

            if (GUI.Button(new Rect(10, 50, 100, 30), "Client"))
            {
                NetworkManager.Singleton.StartClient();
                // Host will spawn Player2 when this client connects
            }

            if (GUI.Button(new Rect(10, 90, 100, 30), "Server"))
            {
                NetworkManager.Singleton.StartServer();
            }
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer && clientId != NetworkManager.Singleton.LocalClientId)
        {
            // First client gets Player2
            SpawnCustomPlayer(clientId, player2Prefab);
        }
    }

    private void SpawnCustomPlayer(ulong clientId, GameObject prefab)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            GameObject player = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        }
    }
}
