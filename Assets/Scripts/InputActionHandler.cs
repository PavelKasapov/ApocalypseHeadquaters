using System.Collections;
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
            /*case EntityType.Ground:
                squadControl.SelectCharacter(null);
                break;*/
        }


        //ClickRaycast(cursorPosition)?.OnLeftClick(mainCamera.ScreenToWorldPoint(cursorPosition));
    }

    public void InteractClick(Vector2 cursorPosition)
    {
        /*if (EventSystem.current.IsPointerOverGameObject()) 
        {
            squadControl.MoveToPoint(cameraController.ScreenToWorldPoint(cursorPosition));
            return;
        }*/

        var target = cameraController.ClickRaycast(cursorPosition);

        if (target == null && !EventSystem.current.IsPointerOverGameObject())
        {
            squadControl.MoveToPoint(cameraController.ScreenToWorldPoint(cursorPosition));
            return;
        }

        //if (target == null)  return;

        switch (target?.EntityType)
        {
            case EntityType.Enemy:
                //Approach and attack
                break;

            case EntityType.SquadMember:
                //Follow or idk
                break;

            default: break;
            /*case EntityType.Ground:
                squadControl.MoveToPoint(cameraController.ScreenToWorldPoint(cursorPosition));
                break;*/
        }
        //ClickRaycast(cursorPosition)?.OnRightClick(mainCamera.ScreenToWorldPoint(cursorPosition));
    }
}