using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderText : MonoBehaviour
{
    private enum ESliderType
    {
        Percentage,
        Number,
        NumberWithMax
    }

    [SerializeField] private ESliderType sliderType;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI text;
    private float _maxValue;
    private float _currentValue;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void UpdateMax(float value)
    {
        _maxValue = value;
        UpdateCurrent(_currentValue);
    }

    public void UpdateCurrent(float value)
    {
        _currentValue = value;
        
        float percent = _currentValue / _maxValue;
        slider.value = percent;

        switch (sliderType)
        {
            case ESliderType.Percentage:
                text.text = ((int)(percent * 100)) + "%";
                break;
            case ESliderType.Number:
                text.text = _currentValue.ToString(CultureInfo.InvariantCulture);
                break;
            case ESliderType.NumberWithMax:
                text.text = _currentValue.ToString(CultureInfo.InvariantCulture) + "/" + _maxValue.ToString(CultureInfo.InvariantCulture);
                break;
        }

    }

}
