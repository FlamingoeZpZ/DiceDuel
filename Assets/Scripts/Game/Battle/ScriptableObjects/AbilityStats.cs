using Game.Battle.Attacks;
using Managers;
using UnityEngine;

namespace Game.Battle.ScriptableObjects
{
    [CreateAssetMenu(fileName = "AbilityStats", menuName = "Scriptable Objects/AbilityStats")]
    public class AbilityStats : ScriptableObject
    {
        [SerializeField] private int staminaCost;
        [SerializeField] private int baseValue;
        [SerializeField] private EDiceType[] dice;
        [SerializeField] private Sprite icon;
        [SerializeField] private EAttackType attackType;
        
        public int StaminaCost => staminaCost;
        public int BaseValue => baseValue;
        public Sprite Icon => icon;
        public EAttackType AttackType => attackType;
        public EDiceType[] Dice => dice;

        private static readonly Color[] AttackColors =
        {
            Color.red,
            Color.blue,
            Color.green,
        };
        
        public Color GetColor()
        {
            return AttackColors[(int)attackType];
        }
    }
    
    

    //Offensive plays after defensive
    //Support plays after offensive
    //Rest doesn't matter
    public enum EAttackType
    {
        Defensive,
        Offensive,
        Support,
    }
}
