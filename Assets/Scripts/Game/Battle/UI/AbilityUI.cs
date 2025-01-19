using System.Collections.Generic;
using Game.Battle.ScriptableObjects;
using Managers.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Battle.UI
{
    public class AbilityUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI effectiveRange;
        [SerializeField] private Image icon;
        private int _low;
        private int _high;
        private readonly List<EDiceType> _dice = new();
        public AbilityBaseStats AbilityBaseStats { get; private set; }


        public void Bind(AbilityBaseStats ability)
        {
            icon.sprite = ability.Icon;
            AbilityBaseStats = ability;
            ResetDice();
        }

        public void AddDice(EDiceType value)
        {
            _dice.Add(value);
            _low += 1;
            _high += DataManager.DiceValues[value].High;
            effectiveRange.text = GenerateFancyText(_low,_high);
        }

        public void RemoveDice(EDiceType value)
        {
            _dice.Remove(value);
            _low -= 1;
            _high -= DataManager.DiceValues[value].High;
            effectiveRange.text = GenerateFancyText(_low,_high);

        }

        public EDiceType[] GetDice()
        {
            return _dice.ToArray();
        }


        public void ResetDice()
        {
            _low = 0;
            _high = 0;
            _dice.Clear();
            effectiveRange.text = GenerateFancyText(_low,_high);
        }

        //Generated with chatGPT, optional way to make text fancier
        #region GPTFancyText

        private const int ModerateValue = 30;
        private const int HighValue = 100;
        private const float MinFontSize = 16f;
        private const float MaxFontSize = 24f;
    
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
            return $"{Format(low)} {Format((low+high)/2,"~")} {Format(high)}";
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
        float GetFontSize(int value)
        {
            // Scale font size: 6 for value 0, up to 15 for value 100
            float size = Mathf.Lerp(MinFontSize, MaxFontSize, Mathf.Clamp01((float)Mathf.Abs(value) / HighValue));
            return size;
        }

        // Method to format the value with the appropriate color and font size for TMP
        string Format(int value)
        {
            // Calculate font size
            float fontSize = GetFontSize(value);

            // Calculate a vertical offset to center smaller text
            float verticalOffset = 0.5f * (1.0f - fontSize); // Adjust offset based on font size

            // Apply size, color, and vertical offset to the value
            return $"<align=center><voffset={verticalOffset}em><size={fontSize}><color={GetColor(value)}>{value}</color></size></voffset></align>";
        }
        
        // Method to format the value with the appropriate color and font size for TMP
        string Format(int value, string text)
        {
            // Calculate font size
            float fontSize = GetFontSize(value);

            // Calculate a vertical offset to center smaller text
            float verticalOffset = 0.5f * (1.0f - fontSize); // Adjust offset based on font size

            // Apply size, color, and vertical offset to the value
            return $"<align=center><voffset={verticalOffset}em><size={fontSize}><color={GetColor(value)}>{text}</color></size></voffset></align>";
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
}
