using UnityEngine;
using UnityEngine.UI;

public class ColorSelector : MonoBehaviour
{

    [Header("Sliders")]
    [SerializeField] private Slider hue;
    [SerializeField] private Slider saturation;
    [SerializeField] private Slider value;
    [SerializeField] private Slider glow;
    
    [Header("Target")]
    [SerializeField] private Material[] targetMaterials;
    [SerializeField] private string propertyName;

    //For optimization
    private int _hashedProperty;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _hashedProperty = Shader.PropertyToID(propertyName);
        hue.onValueChanged.AddListener(_ => ModifyColor());
        saturation.onValueChanged.AddListener(_ => ModifyColor());
        value.onValueChanged.AddListener(_ => ModifyColor());
        glow.onValueChanged.AddListener(_ => ModifyColor());
    }

    void ModifyColor()
    {
        foreach (Material targetMaterial in targetMaterials)
        {
            targetMaterial.SetColor(_hashedProperty,
                Color.HSVToRGB(hue.value * glow.value, saturation.value * glow.value, value.value * glow.value, true));
        }
    }
}
