using Mirror;
using TMPro;
using UnityEngine;

public class Scoreboard : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI[] _scores, _names;
    [SerializeField] private GameplayLoop _gameplayLoop;
    private Player[] _players;
    private bool _winnerIsDecided;

    private void Awake()
    {
        (CustomRoomManager.singleton as CustomRoomManager).OnServerReadyCallback.AddListener(InitializeScoreboard);
    }

    [Server]
    public void InitializeScoreboard()
    {
        Debug.Log("Initialize");
        _players = GameObject.FindObjectsOfType<Player>();
        RpcEnableScoreEntries(_players.Length);

        for (int i = 0; i < _players.Length; i++)
        {
            _players[i].ResetScore();
            RpcUpdateName(i, _players[i].GetNickname());
            RpcUpdateScore(i, _players[i].GetScore());
            _players[i].OnScoreUpdate.AddListener(UpdateScore);
        }
    }

    [ClientRpc]
    private void RpcEnableScoreEntries(int count)
    {
        for (int i = 0; i < count; i++)
        {
            _scores[i].gameObject.SetActive(true);
            _names[i].gameObject.SetActive(true);
        }
    }

    [ClientRpc]
    private void RpcUpdateName(int i, string name)
    {
        _names[i].text = name;
    }

    [ClientRpc]
    private void RpcUpdateScore(int i, int score)
    {
        _scores[i].text = score.ToString();
    }

    [Server]
    private void UpdateScore()
    {
        for (int i = 0; i < _players.Length; i++)
        {
            RpcUpdateScore(i, _players[i].GetScore());

            if(_players[i].GetScore() == 3 && !_winnerIsDecided)
            {
                _gameplayLoop.EndGame(_players[i]);
                _winnerIsDecided = true;
            }
        }
    }
}
