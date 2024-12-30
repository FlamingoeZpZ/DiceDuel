using Game.Battle.ScriptableObjects;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AbilityUI : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI abilityCost;
    [SerializeField] private TextMeshProUGUI effectiveRange;
    [SerializeField] private Image icon;

    private Button _button;
    private int _cost;
    
    //MUST BE AWAKE (pre-init)
    private void Awake()
    {
        _button = GetComponent<Button>();
    }
    
    public void SetState(int currentStamina)
    {
        _button.interactable = _cost < currentStamina;
    }
    
    public void Bind(AbilityController controller, AbilityBaseStats ability)
    {
        _button.onClick.RemoveAllListeners();

        icon.sprite = ability.Icon;
        backgroundImage.color = ability.GetColor;
        abilityCost.text = ability.StaminaCost.ToString();
        abilityCost.color = ability.StaminaCost >= 0 ? Color.green : Color.red;
        _cost = ability.StaminaCost;
        



        
        //It's possible to do fancy colour text / increase the size via rich text. 
        //For funsies, you can give these FUNCTIONS to students which is chatGPT generated.
        effectiveRange.text = GenerateFancyText(ability.MinRollValue, ability.MaxRollValue);
        
        _button.onClick.AddListener(() =>
        {
            controller.SetAttack(ability);
        });
    }

    #region GPTFancyText

    private const int ModerateValue = 25;
    private const int HighValue = 100;
    
    private static readonly Color HighNegativeValueColor = new Color(0.5f, 0f, 0f); // Dark Red (High Negative)
    private static readonly Color NegativeValueColor = new Color(1f, 0.6f, 0.6f); // Pale Red (Negative)
    private static readonly Color LowValueColor = new Color(1f, 1f, 0.7f); // Pale Yellow (Low Value)
    private static readonly Color ModerateValueColor = new Color(0.6f, 1f, 0.6f); // Pale Green (Moderate Value)
    private static readonly Color HighValueColor = new Color(0f, 0.2f, 0.5f); // Dark Blue (High Value)

    private string GenerateFancyText(int low, int high)
    {
        // If low and high values are the same, return just one formatted value
        if (low == high)
        {
            return Format(low);
        }
        // Otherwise, return a range with both formatted values
        return $"{Format(low)} ~ {Format(high)}";
    }
    
    // Helper method to determine the color based on the value
    string GetColor(int value)
    {
        // For values less than -100, we use HighNegativeValueColor
        if (value < -HighValue)
        {
            return ColorToHex(HighNegativeValueColor);
        }

        // For values between -100 and -25, interpolate from HighNegativeValueColor to NegativeValueColor
        if (value < -ModerateValue)
        {
            float t = Mathf.InverseLerp(-HighValue, -ModerateValue, value); // Normalize between -100 and -25
            return ColorToHex(Color.Lerp(HighNegativeValueColor, NegativeValueColor, t));
        }

        // For values between -25 and 25, interpolate from NegativeValueColor to LowValueColor
        if (value <= ModerateValue)
        {
            float t = Mathf.InverseLerp(-ModerateValue, ModerateValue, value); // Normalize between -25 and 25
            return ColorToHex(Color.Lerp(NegativeValueColor, LowValueColor, t));
        }

        // For values between 25 and 100, interpolate from LowValueColor to ModerateValueColor
        if (value <= HighValue)
        {
            float t = Mathf.InverseLerp(ModerateValue, HighValue, value); // Normalize between 25 and 100
            return ColorToHex(Color.Lerp(LowValueColor, ModerateValueColor, t));
        }

        // For values greater than 100, interpolate from ModerateValueColor to HighValueColor
        return ColorToHex(Color.Lerp(ModerateValueColor, HighValueColor, Mathf.InverseLerp(HighValue, float.MaxValue, value)));

    }

    // Helper method to determine the font size based on the value
    int GetFontSize(int value)
    {
        // Scale font size: 6 for value 0, up to 15 for value 100
        float size = Mathf.Lerp(6f, 15f, Mathf.Clamp01((float)Mathf.Abs(value) / HighValue));
        return Mathf.RoundToInt(size); // Round the value to get an integer font size
    }

    // Method to format the value with the appropriate color and font size for TMP
    string Format(int value)
    {
        // Apply size and color to the value
        return $"<size={GetFontSize(value)}><color={GetColor(value)}>{value}</color></size>";
    }

    private string ColorToHex(Color color)
    {
        // Convert the RGB color values to a hex string for TextMesh Pro
        int r = (int)(color.r * 255);  // Convert red component to range [0, 255]
        int g = (int)(color.g * 255);  // Convert green component to range [0, 255]
        int b = (int)(color.b * 255);  // Convert blue component to range [0, 255]

        // Return the hex string, ensuring the format is #RRGGBB
        return $"#{r:X2}{g:X2}{b:X2}";
    }
    #endregion


}
