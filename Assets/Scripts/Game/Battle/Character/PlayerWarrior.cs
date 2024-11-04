using System;
using Cysharp.Threading.Tasks;
using Game.Battle.Interfaces;
using UnityEngine;

namespace Game.Battle.Character
{
    [SelectionBase]
    public class PlayerWarrior : BaseCharacter // A player is a warrior
    {
       //What does a player have?
       
       //A player has a set of attacks
       private IAttack[] _attacks;

       public SwordAttack temp;
       
       private AbilityController _abilityController;


       //Start because we're talking about other objects, and we would have to override awake.
       private void Start()
       {
           _attacks = new IAttack[]
           {
               temp,
               temp,
               temp,
               temp,
               temp,
           };
           _abilityController = GetComponentInChildren<AbilityController>();
           _abilityController.SetAbilities(_attacks);
       }

       public override async UniTask<IAttack> ChooseAttack()
       {
           return await _abilityController.SelectAbility();
       }
    }
}
