using Cysharp.Threading.Tasks;
using Game.Battle.Interfaces;
using UnityEngine;

namespace Game.Battle.Character
{
    public class PlayerWarrior : BaseCharacter // A player is a warrior
    {
       //What does a player have?
       
       //A player has a set of attacks

       private AbilityController _abilityController;


       //Start because we're talking about other objects, and we would have to override awake.
       private void Start()
       {

           IAbility[] abilities = new IAbility[defaultAbilities.Length];

           for (int i = 0; i < defaultAbilities.Length; i++)
           {
               abilities[i] = defaultAbilities[i].;
           }

           _attacks = new IAbility[]
           {
               temp,
               temp,
               temp,
               temp,
               temp,
           };
           _abilityController = GetComponentInChildren<AbilityController>();
           _abilityController.SetAbilities();
       }

       public override async UniTask<IAbility> ChooseAttack()
       {
           return await _abilityController.SelectAbility();
       }

       public override Color GetTeamColor()
       {
           return SaveManager.SaveData.DiceColor;
       }
    }
}
