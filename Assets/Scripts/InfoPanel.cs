using TMPro;
using UnityEngine;

public class InfoPanel : MonoBehaviour
{
    [SerializeField] new TextMeshProUGUI name;
    [SerializeField] StatusBar statusBar;
    [SerializeField] TextMeshProUGUI ammo;

    public Character character;

    public void SelectCharacter(Character character) 
    {
        if (character == this.character) return;

        UnselectCharacter();

        if (character != null)
        {
            this.character = character;
            name.text = character.transform.parent.name;

            character.SelectorHandler.MarkSelected(true);
            character.CombatSystem.OnAttackStatusChange += OnAttackStatusChange;
        }

        name.gameObject.SetActive(character != null);
        statusBar.gameObject.SetActive(character != null);
        ammo.gameObject.SetActive(character != null);
    }
    
    private void UnselectCharacter() 
    {
        if (character == null) return;

        character.SelectorHandler.MarkSelected(false);
        character.CombatSystem.OnAttackStatusChange -= OnAttackStatusChange;
    }

    private void OnAttackStatusChange(AttackStatus status, float duration)
    {
        statusBar.SetStatusText(status.ToString(), duration);
        ammo.gameObject.SetActive(character.CombatSystem.Attack is RangedAttack);
        if (status == AttackStatus.AttackCooldown && character.CombatSystem.Attack is RangedAttack rangedAttack)
        {
            ammo.text = $"Ammo: {rangedAttack.currentAmmo} / {rangedAttack.totalAmmo}";
        }
    }
}