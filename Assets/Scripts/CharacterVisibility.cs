using UnityEngine;

public class CharacterVisibility : MonoBehaviour
{
    [SerializeField] GameObject[] objectsToHide;

    private void Start()
    {
        ChangeVisibility(false);
    }

    public void ChangeVisibility(bool isOn)
    {
        foreach (GameObject obj in objectsToHide)
        {
            obj.SetActive(isOn);
        }
    }
}
