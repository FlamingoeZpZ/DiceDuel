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

       public override async UniTask<IAttack> ChooseAttack()
       {
           await UniTask.Delay(100);
           return temp;
       }
    }
}
