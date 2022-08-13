using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public Vector2 MovementVector { get; private set; }
    public Vector2 CameraRotationVector { get; private set; }
    public UnityEvent OnActionKeyDown;

    private void Start()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        MovementVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        CameraRotationVector = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        if (Input.GetMouseButtonDown(0))
        {
            OnActionKeyDown.Invoke();
        }
    }

    
}
