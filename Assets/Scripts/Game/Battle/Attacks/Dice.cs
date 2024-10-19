using System;
using Cysharp.Threading.Tasks;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Battle.Attacks
{
    [SelectionBase]
    public class Dice : MonoBehaviour
    {
        private static readonly int Color1 = Shader.PropertyToID("_Color");

        //An event action is an action that can only be called privately.
        //An action is something that we can observe and notify other files. For instance, allows us to notify the DiceManager when the dice is done being rolled.
        public static event Action<EDiceType, Dice> OnDiceRolled;
        
        //[SerializeField] private Sprite[] sprites;

        [SerializeField] private float minSpeed=5;
        [SerializeField] private float maxSpeed=10;
        [SerializeField] private EDiceType myType;
        [SerializeField, ColorUsage(false, true)] private Color myColor;

        
        //private SpriteRenderer _spriteRenderer;
        private Rigidbody _rigidbody;
        private Material _material;
        private Mesh _mesh;
        
        private int _highestRoll;
        private int _currentValue;
        
        public int CurrentValue => _currentValue;
        public int HighestRoll => _highestRoll;
        
        //We need to use UniTask to retrieve an int with low memory allocation, the alternatives will create lag or not work on webgl.
        public async UniTask<int> Roll(Color color, Vector2 direction)
        {
            gameObject.SetActive(true);
            color.a = 1;
            myColor = color;
            _material.SetColor(Color1, color);
            _rigidbody.AddForce(direction * Random.Range(minSpeed, maxSpeed), ForceMode.Impulse);
            _rigidbody.AddTorque(Random.insideUnitSphere * Random.Range(minSpeed, maxSpeed), ForceMode.Impulse);

            const float stopThreshold = 0.1f;

            //We need both loops so that we get a fair chance to check
            do
            {
                // Wait until the dice has a velocity smaller than the threshold
                while (_rigidbody.linearVelocity.sqrMagnitude > stopThreshold)
                {
                    await UniTask.Delay(100); // Wait for 100ms
                }

                // Wait for an additional second to ensure it has settled
                await UniTask.Delay(1000); // Wait for 1 second

            } while (_rigidbody.linearVelocity.sqrMagnitude > stopThreshold);

            // Iterate through the children of the dice to determine which face is facing up
            float maxDot = float.NegativeInfinity;

            Transform dice = transform.GetChild(0);
            foreach (Transform child in dice)
            {
                float dotProduct = Vector3.Dot(child.forward, Vector3.forward);
                if (dotProduct > maxDot)
                {
                    maxDot = dotProduct;
                    _currentValue = child.GetSiblingIndex(); // Assume this gives the correct value
                }
            }

            _rigidbody.isKinematic = true; // Freeze the dice

            _currentValue += 1; // Adjust for your specific value range
            OnDiceRolled?.Invoke(myType, this);

            gameObject.SetActive(false);
            
            return _currentValue; // Return the rolled value
        }


        private void OnCollisionEnter(Collision other)
        {
            //If we're moving at some speed when we collide, we should choose a new side
            //if(_rigidbody.linearVelocity.magnitude > 3)
            //  ChooseNewSide();
            Rigidbody otherRigidbody = other.rigidbody;
            Vector3 normal = other.contacts[0].normal;
            //Bounce off other dice
            if (otherRigidbody && otherRigidbody.TryGetComponent(out Dice _))
            {
                _rigidbody.AddForce(normal * Random.Range(minSpeed,maxSpeed), ForceMode.Impulse);
                EffectManager.instance.PlaySparks(transform.position, Quaternion.LookRotation(normal), myColor);
            }

            //This is our hit impact
          
         
            //An audiomanager and particle manager would be nice, but probably too much work
           // AudioSource.PlayClipAtPoint(diceSound, transform.position, 0.2f);
            
            //Destroy the object after 3 secodns
           // Destroy(Instantiate(diceClinkParticles, transform.position, Quaternion.LookRotation(normal, Vector3.forward)).gameObject, 1);

           // _rigidbody.linearVelocity = normal * _rigidbody.linearVelocity * collisionSpeedLoss;

        }


        private async void Awake()
        {
            //_spriteRenderer = GetComponent<SpriteRenderer>();
            _rigidbody = GetComponent<Rigidbody>();
            _material = GetComponentInChildren<MeshRenderer>().material;
            _mesh = GetComponentInChildren<MeshFilter>().sharedMesh; // shared mesh to prevent duplicates
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
            int value = await Roll(myColor, Vector2.right);
            Debug.Log(value, gameObject);
        }

        private void OnDisable()
        {
            //The ? is null assertion and should be removed for release builds... Or use preprocess
            EffectManager.instance?.PlayDissolve(transform.position, transform.rotation, _mesh, myColor);
        }
    }
}