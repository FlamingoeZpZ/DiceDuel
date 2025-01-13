using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Battle.Character
{
    [SelectionBase]
    public class AIWarrior : BaseCharacter
    {
        [SerializeField] private EAIType aiType;
        [SerializeField] private EDiceType[] myDice;

        /*
         * 1) Based on some arbitrary difficulty level, we should generate some amount of dice.
         * 2) We then want to allocate the dice to different abilities depending on what our AI type is.
         * But how do we do this? How do we determine that we are:
         * 1. Spending an appropriate amount of dice
         * 2. Not putting too much into attacking
         * 3. Managing our resources correctly.
         *
         * Aggressive:
         * Rush generating stamina
         * If stamina > totalDiceCost OR (stamina > 10 and opponent health < 20)
         * Place the highest value dice into attack
         * if stamina is > 20, put the remainder into defense
         * else remainder into stamina
         *
         */

        protected override void Awake()
        {
            base.Awake();
            int difficulty = Random.Range(1, 100);
            Debug.Log("Difficulty: "+ difficulty);
            myDice = GenerateDiceSet(difficulty, aiType);
        }

        private EDiceType[] _myDice;

        //Based on difficulty
        private EDiceType[] GenerateDiceSet(int difficulty, EAIType ai)
        {
            //Let's say there's a max difficulty of 100. What does that look like?
            //The AI should have 16D20, 4D4, 2D6, 1D8, 1D10 
            //Let's say the minimum difficulty is 10. What does that look like?
            //Let's double the AI's difficulty and use that as our value.
            
            //An aggressive AI will take higher value dice, a defensive AI will take more lower value dice.
            //How do we calculate these formulae?
            //For both AI types, start by choosing a reasonable number of dice.
          
            //When difficulty is over 80 the AI will have all 24 dice. When the difficulty is 40, the AI will have ~13.
            //We then add random, which means that at difficulty 50/70 --> 18, 50/80 --> 16, 50/90 --> 14
            //Essentially as difficulty increases there is more variety in what the AI will have
            int numDice = (int)Mathf.Lerp(3,12,Mathf.Clamp01(difficulty/Random.Range(60f,80f)));
            
            //We also want to reduce our operations given that all items are arbitrary.
            if(ai is EAIType.Aggressive) return AIUtility.GenerateDice(numDice, difficulty, 20, 10, 40, 10);
            if(ai is EAIType.Defensive) return AIUtility.GenerateDice(numDice, difficulty, 30, 15, 50, 15);
            return AIUtility.GenerateDice(numDice, difficulty, 20, 15, 70, 20);
        }
        
        
        public override async UniTask ChooseAttacks()
        {
            await UniTask.Delay(100);

            switch (aiType)
            {
                case EAIType.Aggressive:
                    AggressiveAI();
                    break;
                case EAIType.Defensive:
                    DefensiveAI();
                    break;
                case EAIType.Balanced:
                    BalancedAI();
                    break;
            }
            
            for (int i = 0; i < DiceSets.Length; i++)
            {
                DiceSets[i] = new EDiceType[]
                {
                    EDiceType.Six,
                    EDiceType.Six
                };
            }
            Debug.LogWarning("AIWarrior ChooseAttacks needs to be completed");
        }

        public override Color GetTeamColor()
        {
            return Color.white;
        }

        private void AggressiveAI()
        {
            
        }

        private void DefensiveAI()
        {
            
        }

        private void BalancedAI()
        {
            
        }

    }
    
    public enum EAIType
    {
        Aggressive,
        Defensive,
        Balanced
    }
}
