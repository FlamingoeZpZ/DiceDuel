using System;
using System.Threading.Tasks;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Battle.Attacks
{
    [SelectionBase]
    public class Dice : MonoBehaviour
    {
        //An event action is an action that can only be called privately.
        //An action is something that we can observe and notify other files. For instance, allows us to notify the DiceManager when the dice is done being rolled.
        public static event Action<EDiceType, Dice> OnDiceRolled;
        
        //[SerializeField] private Sprite[] sprites;

        [SerializeField] private float minSpeed=5;
        [SerializeField] private float maxSpeed=10;
        [SerializeField] private EDiceType myType;

        
        //private SpriteRenderer _spriteRenderer;
        private Rigidbody _rigidbody;
        
        private int _highestRoll;
        private int _currentValue;
        
        public Task<int> Roll(Color color, Vector2 direction)
        {
            //_spriteRenderer.color = color;
            _rigidbody.AddForce(direction * Random.Range(minSpeed,maxSpeed), ForceMode.Impulse);
            _rigidbody.AddTorque(Random.insideUnitSphere * Random.Range(minSpeed,maxSpeed), ForceMode.Impulse);
            
            //When the dice is rolled, first let's apply the color and direction
            
            ChooseNewSide();    
            //Ideally we would play this with a random pitch, but for our game this is fine enough
          //  AudioSource.PlayClipAtPoint(diceSound, transform.position, 0.2f);
            
            //Then let's choose some amount of time that the dice will spin for
            
            //Invoke on dice rolled.
            OnDiceRolled?.Invoke(myType, this);
            
            return Task.FromResult(_currentValue + 1);
        }

        private void OnCollisionEnter(Collision other)
        {
            //If we're moving at some speed when we collide, we should choose a new side
            if(_rigidbody.linearVelocity.magnitude > 3)
              ChooseNewSide();
            
            //This is our hit impact
            Vector2 normal = other.contacts[0].normal;
            
            //An audiomanager and particle manager would be nice, but probably too much work
           // AudioSource.PlayClipAtPoint(diceSound, transform.position, 0.2f);
            
            //Destroy the object after 3 secodns
           // Destroy(Instantiate(diceClinkParticles, transform.position, Quaternion.LookRotation(normal, Vector3.forward)).gameObject, 1);

           // _rigidbody.linearVelocity = normal * _rigidbody.linearVelocity * collisionSpeedLoss;

        }

        void ChooseNewSide()
        {
            //If we're at 5 out of 10, and we add from a random from 1,2,3,4,5,6,7,8,9 our next number could be: 6,7,8,9, 0,1,2,3,4 (not possible for 5)
            _currentValue = (_currentValue + Random.Range(1, _highestRoll)) % _highestRoll;
        
        }

        private async void Awake()
        {
            //_spriteRenderer = GetComponent<SpriteRenderer>();
            _rigidbody = GetComponent<Rigidbody>();
            
            switch (myType)
            {
                case EDiceType.Four:
                    _highestRoll = 4;
                    break;
                case EDiceType.Six:
                    _highestRoll = 6;
                    break;
                case EDiceType.Eight:
                    _highestRoll = 8;
                    break;
                case EDiceType.Ten:
                    _highestRoll = 10;
                    break;
                case EDiceType.Twenty:
                    _highestRoll = 20;
                    break;
            }
            
            int value = await Roll(Color.red, Vector2.right);
            
            print(value);
        }
        
        
    }
}