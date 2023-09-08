using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class InputActionHandler : MonoBehaviour
{
    [SerializeField] private CameraController cameraController;
    [Inject] private readonly SquadControlSystem squadControl;

    public void OnCameraInput(Vector2 direction)
    {
        cameraController.MoveCamera(direction);
    }

    public void SelectClick(Vector2 cursorPosition)
    {
        var target = cameraController.ClickRaycast(cursorPosition);

        if (target == null && !EventSystem.current.IsPointerOverGameObject())
        {
            squadControl.SelectCharacter(null);
            return;
        }

        switch (target?.EntityType)
        {
            case EntityType.Enemy:
                //Show UI Info
                break;

            case EntityType.SquadMember:
                var character = target as Character;
                squadControl.SelectCharacter(character);
                //Show UI Info
                break;

            default: break;
        }
    }

    public void InteractClick(Vector2 cursorPosition)
    {
        if (!EventSystem.current.IsPointerOverGameObject())
            return;

        var target = cameraController.ClickRaycast(cursorPosition);

        if (target == null)
        {
            squadControl.MoveToPoint(cameraController.ScreenToWorldPoint(cursorPosition));
            return;
        }

        switch (target?.EntityType)
        {
            case EntityType.Enemy:
                //Approach and attack
                break;

            case EntityType.SquadMember:
                //Follow or idk
                break;

            default: break;
        }
    }
}
