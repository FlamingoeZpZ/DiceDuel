using Game.Battle.ScriptableObjects;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponStats", menuName = "Scriptable Objects/WeaponStats")]
public class WeaponStats : ScriptableObject
{
    [Header("Weapon")]
    [SerializeField, TextArea] private string description; 
    [SerializeField] private AbilityBaseStats[] attacks;

    [Header("Modifiers")]
    [SerializeField] private int offensiveModifier;
    [SerializeField] private int defensiveModifier;
    [SerializeField] private int staminaModifier;
    
    public string Description => description;
    public AbilityBaseStats[] Attacks => attacks;
    public int OffensiveModifier => offensiveModifier;
    public int DefensiveModifier => defensiveModifier;
    public int StaminaModifier => staminaModifier;
    
}

