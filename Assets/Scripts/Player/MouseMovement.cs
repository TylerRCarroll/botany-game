using UnityEngine;
using UnityEngine.InputSystem;
public class MouseMovement : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    
    private PlayerInput _playerInput;
    private InputAction _lookAction;

    private float _xRotation = 0f;
    private float _yRotation = 0f;

    private readonly float _topClamp = 90f;
    private readonly float _bottomClamp = -90f;

    private void Awake()
    {
        _playerInput = GetComponentInParent<PlayerInput>();
        _lookAction = _playerInput.actions["Look"];
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //lock cursor for normal movement
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }
        
        Vector2 lookMovement = _lookAction.ReadValue<Vector2>();
        
        lookMovement.x *= mouseSensitivity * Time.deltaTime;
        lookMovement.y *= mouseSensitivity * Time.deltaTime;
        
        _xRotation -= lookMovement.y;
        
        _xRotation = Mathf.Clamp(_xRotation, _bottomClamp, _topClamp);
        
        _yRotation += lookMovement.x;
        
        transform.localRotation = Quaternion.Euler(_xRotation, _yRotation, 0f);
    }
}
