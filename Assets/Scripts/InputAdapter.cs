using UnityEngine;

public class InputAdapter : MonoBehaviour
{
    private PlayerControls _controls;
    [SerializeField] private InputActionHandler _playerControl;

    private void Awake()
    {
        _controls = new PlayerControls();

        _controls.Player.CameraMovement.performed += 
            ctx => _playerControl.OnCameraInput(ctx.ReadValue<Vector2>());
        _controls.Player.CameraMovement.canceled += 
            ctx => _playerControl.OnCameraInput(ctx.ReadValue<Vector2>());
        _controls.Player.LeftClick.performed += 
            ctx => _playerControl.SelectClick(_controls.Player.MousePosition.ReadValue<Vector2>());
        _controls.Player.RightClick.performed += 
            ctx => _playerControl.InteractClick(_controls.Player.MousePosition.ReadValue<Vector2>());
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
