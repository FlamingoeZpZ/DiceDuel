using System;
using System.Threading.Tasks;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Battle.Attacks
{
    public class Dice : MonoBehaviour
    {
        //An event action is an action that can only be called privately.
        //An action is something that we can observe and notify other files. For instance, allows us to notify the DiceManager when the dice is done being rolled.
        public static event Action<EDiceType, Dice> OnDiceRolled;
        
        [SerializeField] private Sprite[] sprites;

        [SerializeField] private float minSpeed=5;
        [SerializeField] private float maxSpeed=10;
        [SerializeField, Range(0,1)] private float collisionSpeedLoss = 0.8f;
        
        [SerializeField] private AudioClip diceSound;
        [SerializeField] private AudioClip diceHitSound;
        [SerializeField] private ParticleSystem diceClinkParticles;
        
        private SpriteRenderer _spriteRenderer;
        private Rigidbody2D _rigidbody2D;
        
        private EDiceType _myType;
        private int _currentSpriteIndex;
        
        public Task<int> Roll(Color color, Vector2 direction)
        {
            _spriteRenderer.color = color;
            _rigidbody2D.AddForce(direction * Random.Range(minSpeed,maxSpeed), ForceMode2D.Impulse);
            
            
            //When the dice is rolled, first let's apply the color and direction
            
            ChooseNewSide();    
            //Ideally we would play this with a random pitch, but for our game this is fine enough
            AudioSource.PlayClipAtPoint(diceSound, transform.position, 0.2f);
            
            //Then let's choose some amount of time that the dice will spin for
            
            //Invoke on dice rolled.
            OnDiceRolled?.Invoke(_myType, this);
            
            return Task.FromResult(_currentSpriteIndex + 1);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            //If we're moving at some speed when we collide, we should choose a new side
            if(_rigidbody2D.linearVelocity.magnitude > 3)
              ChooseNewSide();
            
            //This is our hit impact
            Vector2 normal = other.contacts[0].normal;
            
            //An audiomanager and particle manager would be nice, but probably too much work
            AudioSource.PlayClipAtPoint(diceSound, transform.position, 0.2f);
            
            //Destroy the object after 3 secodns
            Destroy(Instantiate(diceClinkParticles, transform.position, Quaternion.LookRotation(normal, Vector3.forward)).gameObject, 3);

            _rigidbody2D.linearVelocity = normal * _rigidbody2D.linearVelocity * collisionSpeedLoss;

        }

        void ChooseNewSide()
        {
            //If we're at 5 out of 10, and we add from a random from 1,2,3,4,5,6,7,8,9 our next number could be: 6,7,8,9, 0,1,2,3,4 (not possible for 5)
            _currentSpriteIndex = (_currentSpriteIndex + Random.Range(1, sprites.Length)) % sprites.Length;
            _spriteRenderer.sprite = sprites[_currentSpriteIndex];
        
        }

        public void Initialize(EDiceType diceType)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _myType = diceType;
        }
    }
}