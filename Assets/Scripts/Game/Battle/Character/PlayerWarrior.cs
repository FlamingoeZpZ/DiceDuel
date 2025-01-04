using Cysharp.Threading.Tasks;
using Managers;
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
           _abilityController = GetComponentInChildren<AbilityController>();
           _abilityController.SetAbilities(abilities);
           _abilityController.UpdateAbilities(CurrentStamina);

       }

       public override async UniTask<AbilityBaseStats> ChooseAttack()
       {
           _abilityController.UpdateAbilities(CurrentStamina);
           return await _abilityController.SelectAbility();
       }

       public override Color GetTeamColor()
       {
           return SaveManager.SaveData.DiceColor;
       }
    }
}
