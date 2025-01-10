using System.Collections.Generic;
using Game.Battle.Character;
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


        public static DiceManager Instance { get; private set; }

        private readonly Dictionary<EDiceType, Queue<Dice>> _diceInstances = new Dictionary<EDiceType, Queue<Dice>>();

        //Singleton Design Pattern
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

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
        }



        //Idea of a factory design pattern, single place to create our objects where we hide creation
        public Dice CreateDice(EDiceType dice, bool isLeft, Color glowColor, Color numberColor)
        {
            //Just grab the dice, no need to check if it's valid.
            if (!_diceInstances[dice].TryDequeue(out Dice createdDice))
            {
                createdDice = AddDice(dice);
            }

            Transform tr = isLeft ? diceSpawnPointLeft : diceSpawnPointRight;
            createdDice.transform.position =tr.position;
            createdDice.transform.forward = tr.forward;
            createdDice.SetColor(glowColor, numberColor);
            return createdDice;
        }


        //Object pooling
        private Dice AddDice(EDiceType diceType)
        {
            int parent = (int)diceType + 1; // We can do this because enum are really just counters.
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

            createdDice.OnDiceRolled += ReturnDice;
            
            return createdDice;
        }
        
        public void ReturnDice(Dice dice)
        {
            _diceInstances[dice.Type].Enqueue(dice);
        }

        public static readonly Dictionary<EDiceType, DiceValue> DiceValues =
            new Dictionary<EDiceType, DiceValue>()
            {
                { EDiceType.Four, new DiceValue(1, 4) },
                { EDiceType.Six, new DiceValue(1, 6) },
                { EDiceType.Eight, new DiceValue(1, 8) },
                { EDiceType.Ten, new DiceValue(1, 10) },
                { EDiceType.Twenty, new DiceValue(1, 20) },
            };

        public readonly struct DiceValue
        {
            public readonly int Low;
            public readonly int High;

            public DiceValue(int low, int high)
            {
                Low = low;
                High = high;
            }
        }
    }
}