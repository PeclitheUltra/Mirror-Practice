using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class Player : NetworkBehaviour
{
    [SyncVar]
    public bool AllowMovement = false;
    [SerializeField] private PlayerMovement _playerMovement;
    [HideInInspector] public UnityEvent OnScoreUpdate;
    [SerializeField] private CharacterController _controller;
    [SerializeField] private Transform _cameraAnchor;
    [SerializeField] private float _dashDistance, _invincibilityTime;
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private Material _default, _invincible;

    private List<Player> _playersHitDuringDash = new List<Player>();


    public bool IsDashing { get; private set; } = false;
    public bool IsAttackable { get; private set; } = true;

    [SyncVar]
    private int _score = 0;

    [SyncVar]
    private string _name = "";

    public string GetNickname() => _name;
    public int GetScore() => _score;
    public void ResetScore()
    {
        _score = 0;
        OnScoreUpdate.Invoke();
    }

    [Command]
    private void CmdAddScore()
    {
        _score++;
        OnScoreUpdate.Invoke();
    }

    [Command]
    private void CmdHurtPlayer(Player player)
    {
        player.RpcGetHit();
    }

    [ClientRpc]
    public async void RpcGetHit()
    {
        IsAttackable = false;
        _renderer.material = _invincible;
        await Task.Delay((int)(_invincibilityTime * 1000));
        IsAttackable = true;
        _renderer.material = _default;
    }

    [ClientRpc]
    public void RpcTeleport(Vector3 position)
    {
        if(hasAuthority)
        {
            transform.position = position;
        }
    }
    [Command]
    private void CmdUpdateName(string name)
    {
        _name = name;
    }

    private void Start()
    {
        if (!hasAuthority)
            return;

        InputManager.Instance.OnActionKeyDown.AddListener(TryDash);
        CmdUpdateName((NetworkManager.singleton as CustomRoomManager).GetNickname());
    }

    private void Update()
    {
        if (!hasAuthority)
            return;

        UpdateCamera(-InputManager.Instance.CameraRotationVector.y);
    }

    private void TryDash()
    {
        if (IsDashing || !AllowMovement)
            return;

        IsDashing = true;

        Vector3 dashVector = _playerMovement.LastFrameMovementDirection;
        if (dashVector == Vector3.zero)
            dashVector = _controller.transform.forward;

        StartCoroutine(DashCoroutine(dashVector, () => { IsDashing = false; _playersHitDuringDash.Clear(); }));
    }

    public IEnumerator DashCoroutine(Vector3 direction, Action onCompleteCallback)
    {
        float time = 0.3f;
        for (float t = 0; t < 1; t += Time.deltaTime / time)
        {
            _controller.Move(direction * Time.deltaTime * _dashDistance / time);
            yield return null;
        }
        onCompleteCallback.Invoke();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!hasAuthority)
            return;

        var player = hit.gameObject.GetComponent<Player>();

        if (player != null && player.IsAttackable && IsDashing && !_playersHitDuringDash.Contains(player))
        {
            _playersHitDuringDash.Add(player);
            CmdAddScore();
            CmdHurtPlayer(player);
        }
    }

    private void UpdateCamera(float cameraMovementY)
    {
        Vector3 currentRotation = _cameraAnchor.transform.rotation.eulerAngles;
        Vector3 newRotation = currentRotation + new Vector3(cameraMovementY, 0, 0);

        if (newRotation.x > 180)
        {
            newRotation.x = Mathf.Clamp(newRotation.x, 335f, 361f);
        }
        else
        {
            newRotation.x = Mathf.Clamp(newRotation.x, -1f, 25f);
        }

        _cameraAnchor.transform.rotation = Quaternion.Euler(newRotation);
    }

}
