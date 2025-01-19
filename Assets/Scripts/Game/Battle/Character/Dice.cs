using System;
using Cysharp.Threading.Tasks;
using Managers;
using Managers.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Battle.Character
{

    
    [SelectionBase]
    public class Dice : MonoBehaviour
    {
        private static readonly int ColorID = Shader.PropertyToID("_Color");

        //An event action is an action that can only be called privately.
        //An action is something that we can observe and notify other files. For instance, allows us to notify the DiceManager when the dice is done being rolled.
        public Action<Dice> OnDiceRolled;
        
        //[SerializeField] private Sprite[] sprites;

        [SerializeField] private float minSpeed=5;
        [SerializeField] private float maxSpeed=10;
        [SerializeField] private EDiceType myType;
        private Color _myColor;

        
        //private SpriteRenderer _spriteRenderer;
        private Rigidbody _rigidbody;
        private Material _material;
        private Material _sharedMaterial;
        private Mesh _mesh;
        private int _currentValue;
        
        
        public int CurrentValue => _currentValue;
        public EDiceType Type => myType;

        private void Awake()
        {
            //_spriteRenderer = GetComponent<SpriteRenderer>();
            _rigidbody = GetComponent<Rigidbody>();
            
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

            //Clone and apply material
            _material = renderers[0].material;
            renderers[0].material = _material;
            
            _mesh = renderers[0].GetComponent<MeshFilter>().sharedMesh; // shared mesh to prevent duplicates

            //Clone the material
            _sharedMaterial = renderers[1].material;

            for (int index = 1; index < renderers.Length; index++)
            {
                //Apply to all other text objects
                renderers[index].sharedMaterial = _sharedMaterial;
            }
        }

        private void OnDisable()
        {
            EffectManager.Instance.PlayDissolve(transform.position, transform.rotation, _mesh, _myColor);
            
            //Reenable the rigidbody for future usage
            _rigidbody.isKinematic = false;
        }

        //We need to use UniTask to retrieve an int with low memory allocation, the alternatives will create lag or not work on webgl.
        public async UniTask<int> Roll(Vector2 direction)
        {
            gameObject.SetActive(true);
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
            OnDiceRolled?.Invoke(this);

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
                EffectManager.Instance.PlayDiceHitSound(0.1f);
                _rigidbody.AddForce(normal * Random.Range(minSpeed,maxSpeed), ForceMode.Impulse);
                EffectManager.Instance.PlaySparks(transform.position, Quaternion.LookRotation(normal), _myColor);
            }
        }

        public void SetColor(Color glowColor, Color numberColor)
        {
            _myColor = glowColor;
            _material.SetColor(ColorID, glowColor);
            _sharedMaterial.SetColor(ColorID, numberColor);
        }
    }
}