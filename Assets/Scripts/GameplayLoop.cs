using Mirror;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class GameplayLoop : NetworkBehaviour
{
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private TextMeshProUGUI _endingText;
    private Player[] _players;

    private void Awake()
    {
        (CustomRoomManager.singleton as CustomRoomManager).OnServerReadyCallback.AddListener(MovePlayersToSpawnPoints);
    }

    [Server]
    public void MovePlayersToSpawnPoints()
    {
        _players = GameObject.FindObjectsOfType<Player>();
        var unusedSpawnPoints = _spawnPoints.ToList();
        for (int i = 0; i < _players.Length; i++)
        {
            var spawnPoint = unusedSpawnPoints[UnityEngine.Random.Range(0, unusedSpawnPoints.Count)];
            unusedSpawnPoints.Remove(spawnPoint);
            _players[i].RpcTeleport(spawnPoint.position);
            _players[i].AllowMovement = true;
            Debug.Log($"player {i} teleported to {spawnPoint.position}");
        }
    }

    [Server]
    public async void EndGame(Player winner)
    {
        RpcShowWinningMessage(winner.GetNickname());
        for (int i = 0; i < _players.Length; i++)
        {
            _players[i].AllowMovement = false;
        }
        await Task.Delay(5000);
        (NetworkManager.singleton as NetworkRoomManager).ServerChangeScene((NetworkManager.singleton as NetworkRoomManager).RoomScene);
    }

    [ClientRpc]
    private void RpcShowWinningMessage(string winnerName)
    {
        _endingText.gameObject.SetActive(true);
        _endingText.text = _endingText.text.Replace("%", winnerName);
    }
}
