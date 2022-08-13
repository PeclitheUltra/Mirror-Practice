using Mirror;
using UnityEngine;

internal class PlayerMovement : NetworkBehaviour
{
    public Vector3 LastFrameMovementDirection { get; private set; }
    [SerializeField] private Player _player;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private float _speed;

    private void Update()
    {
        if(hasAuthority)
        {
            Move(!_player.IsDashing && _player.AllowMovement, InputManager.Instance.MovementVector);
            Rotate(InputManager.Instance.CameraRotationVector.x);
        }
    }

    private void Move(bool allowMovement, Vector2 inputMovementVector)
    {
        if (!allowMovement)
            return;

        inputMovementVector.Normalize();
        inputMovementVector *= Time.deltaTime * _speed;
        Vector3 worldSpaceVector = _characterController.transform.forward * inputMovementVector.y + _characterController.transform.right * inputMovementVector.x;
        _characterController.Move(worldSpaceVector + Vector3.down * Time.deltaTime * 9.81f);


        LastFrameMovementDirection = worldSpaceVector.normalized;
    }

    private void Rotate(float cameraRotationX)
    {
        _characterController.transform.rotation *= Quaternion.Euler(0, cameraRotationX, 0);
    }
}