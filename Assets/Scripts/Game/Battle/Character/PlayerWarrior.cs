using Managers;
using UnityEngine;
using UniTask = Cysharp.Threading.Tasks.UniTask;

namespace Game.Battle.Character
{
    public class PlayerWarrior : BaseCharacter // A player is a warrior
    {
       //What does a player have?
       
       //A player has a set of attacks

       private AbilityController _abilityController;


       //Start because we're talking about other objects, and we would have to override awake.
       protected override void Awake()
       {
           base.Awake();
           _abilityController = GetComponentInChildren<AbilityController>();
           _abilityController.ConstructAbilityController(this, abilities, characterStats.dice);

       }

       public override async UniTask ChooseAttacks()
       {
           await UniTask.WaitUntil(_abilityController.IsReady);

           DiceSets = _abilityController.DiceValues();

           _abilityController.DisableDice();
       }

       public override Color GetTeamColor()
       {
           return SaveManager.SaveData.DiceColor;
       }

       public override async void EndRound()
       {
           base.EndRound();
           await _abilityController.ReturnDice();
       }
    }
}
