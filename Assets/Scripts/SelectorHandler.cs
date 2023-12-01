using UnityEngine;

public class SelectorHandler : MonoBehaviour
{
    [SerializeField] private GameObject selectedCircle;
    public void MarkSelected(bool isSelected)
    {
        selectedCircle.SetActive(isSelected);
    }
}
