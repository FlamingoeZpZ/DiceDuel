using Game.Battle.ScriptableObjects;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponStats", menuName = "Scriptable Objects/WeaponStats")]
public class WeaponStats : ScriptableObject
{
    [Header("Weapon")]
    [SerializeField, TextArea] private string description; 
    [SerializeField] private AbilityStats[] attacks;

    [Header("Modifiers")]
    [SerializeField] private int offensiveModifier;
    [SerializeField] private int defensiveModifier;
    [SerializeField] private int staminaModifier;

}

