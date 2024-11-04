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
    public void Bind(AbilityController controller, IAttack attack)
    {
        _button.onClick.RemoveAllListeners();

        icon.sprite = attack.GetAttackStats().Icon;
        backgroundImage.color = attack.GetAttackStats().GetColor();
        
        _button.onClick.AddListener(() =>
        {
            controller.SetAttack(attack);
        });
    }
}
