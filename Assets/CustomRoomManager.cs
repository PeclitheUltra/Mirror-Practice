using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CustomRoomManager : NetworkRoomManager
{
    public UnityEvent OnServerReadyCallback;
    [SerializeField] private TMP_InputField _nicknameInput;
    private string _nickname;

    public string GetNickname() => _nickname;

    public override void OnRoomStartHost()
    {
        _nickname = _nickname = _nicknameInput.text;
    }

    public override void OnRoomStartClient()
    {
        _nickname = _nickname = _nicknameInput.text;
    }

    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);
        OnServerReadyCallback.Invoke();
    }
}
