using Game.Battle.Interfaces;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AbilityUI : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image icon;

    private Button _button;
    
    //MUST BE AWAKE (pre-init)
    private void Awake()
    {
        _button = GetComponent<Button>();
    }
    public void Bind(AbilityController controller, IAbility ability)
    {
        _button.onClick.RemoveAllListeners();

        icon.sprite = ability.GetAttackStats().Icon;
        backgroundImage.color = ability.GetAttackStats().GetColor();
        
        _button.onClick.AddListener(() =>
        {
            controller.SetAttack(ability);
        });
    }
}
