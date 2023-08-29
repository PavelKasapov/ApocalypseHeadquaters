using UnityEngine;

public class InputSystem : MonoBehaviour
{
    private PlayerControls _controls;
    [SerializeField] private PlayerControlSystem _playerControl;
    

    private void Awake()
    {
        _controls = new PlayerControls();

        _controls.Player.CameraMovement.performed += ctx => _playerControl.MoveCamera(ctx.ReadValue<Vector2>());
        _controls.Player.CameraMovement.canceled += ctx => _playerControl.MoveCamera(ctx.ReadValue<Vector2>());
        _controls.Player.LeftClick.performed += ctx => _playerControl.LeftClick(_controls.Player.MousePosition.ReadValue<Vector2>());
        _controls.Player.RightClick.performed += ctx => _playerControl.RightClick(_controls.Player.MousePosition.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }
}
