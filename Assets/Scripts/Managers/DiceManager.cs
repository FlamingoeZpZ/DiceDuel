using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Battle.Attacks;
using UnityEngine;

namespace Managers
{
    public class DiceManager : MonoBehaviour
    {
        [SerializeField] private Dice fourSidedDice;
        [SerializeField] private Dice sixSidedDice;
        [SerializeField] private Dice eightSidedDice;
        [SerializeField] private Dice tenSidedDice;
        [SerializeField] private Dice twentySidedDice;

        [SerializeField] private Transform diceSpawnPointLeft;
        [SerializeField] private Transform diceSpawnPointRight;

        
        private static DiceManager _instance;

        private readonly Dictionary<EDiceType, Queue<Dice>> _diceInstances = new Dictionary<EDiceType, Queue<Dice>>();

        //Singleton Design Pattern
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            //Add each of the queues as empty by default.
            _diceInstances.Add(EDiceType.Four, new Queue<Dice>());
            _diceInstances.Add(EDiceType.Six, new Queue<Dice>());
            _diceInstances.Add(EDiceType.Eight, new Queue<Dice>());
            _diceInstances.Add(EDiceType.Ten, new Queue<Dice>());
            _diceInstances.Add(EDiceType.Twenty, new Queue<Dice>());

            //Let's also create an empty gameObject to store all of these

            foreach (EDiceType pair in _diceInstances.Keys)
            {
                GameObject go = new GameObject(pair.ToString());
                go.transform.parent = transform;
            }

            //Static Observer
            Dice.OnDiceRolled += AddDiceBack;
        }

        private void OnDestroy()
        {
            //Make sure to unsubscribe on destroy to stop observing.
            Dice.OnDiceRolled -= AddDiceBack;
        }

        //Idea of a factory design pattern, single place to create our objects where we hide creation
        public static async UniTask<int> CreateDice(EDiceType dice, Color color, Vector2 direction)
        {
            //Just grab the dice, no need to check if it's valid.
            if (!_instance._diceInstances[dice].TryDequeue(out Dice createdDice))
            {
                createdDice = _instance.AddDice(dice);
            }

            if (direction.x < 0) createdDice.transform.position = _instance.diceSpawnPointLeft.position;
            else createdDice.transform.position = _instance.diceSpawnPointRight.position;
            int val = await createdDice.Roll(color, direction);

            return val;
        }


        //Object pooling
        private Dice AddDice(EDiceType diceType)
        {
            int parent = (int)diceType; // We can do this because enum are really just counters.
            Dice createdDice;

            switch (diceType)
            {
                case EDiceType.Four:
                    createdDice = Instantiate(fourSidedDice, transform.GetChild(parent));
                    break;
                case EDiceType.Six:
                    createdDice = Instantiate(sixSidedDice, transform.GetChild(parent));
                    break;
                case EDiceType.Eight:
                    createdDice = Instantiate(eightSidedDice, transform.GetChild(parent));
                    break;
                case EDiceType.Ten:
                    createdDice = Instantiate(tenSidedDice, transform.GetChild(parent));
                    break;
                default: // We say default so the compiler doesn't think the object is null.
                    createdDice = Instantiate(twentySidedDice, transform.GetChild(parent));
                    break;
            }

            return createdDice;
        }


        private void AddDiceBack(EDiceType type, Dice dice)
        {
            _diceInstances[type].Enqueue(dice);
            
        }

       
    }

    public enum EDiceType
    {
        Four,
        Six,
        Eight,
        Ten,
        Twenty
    }
}