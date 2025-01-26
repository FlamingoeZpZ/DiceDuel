using UnityEngine;

namespace Managers
{
    [RequireComponent(typeof(AudioSource))]
    public class EffectManager : MonoBehaviour
    {

        [SerializeField] private AudioClip[] diceDeploySound;
        [SerializeField] private AudioClip diceHitSound;
        [SerializeField] private AudioClip playBlockNoise;

        [SerializeField] private ParticleSystem sparkParticles;
        [SerializeField] private ParticleSystem diceDissolveParticles;
        public static EffectManager Instance { get; private set; }

        private AudioSource _source;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _source = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
        }

        public void PlayDiceHitSound(float pitchShift = 0)
        {
            _source.pitch =  1 + Random.Range(-pitchShift, pitchShift);
            _source.PlayOneShot(diceHitSound);
        }

        public void PlayBlockNoise(float pitchShift = 0)
        {
            _source.pitch =  1 + Random.Range(-pitchShift, pitchShift);
            _source.PlayOneShot(playBlockNoise);
        }

        public void PlayDiceDeploySound()
        {
            _source.pitch =1;
            _source.PlayOneShot(diceDeploySound[Random.Range(0, diceDeploySound.Length)]);
        }

        public void PlayDissolve(Vector3 position, Quaternion rotation, Mesh mesh, Color color)
        {
            // Set the particle system's transform position and rotation
            diceDissolveParticles.transform.SetPositionAndRotation(position, rotation);

            // Get the shape module to set the mesh
            var shapeModule = diceDissolveParticles.shape;
            shapeModule.shapeType = ParticleSystemShapeType.Mesh;
            shapeModule.mesh = mesh;

            // Get the main module to set the start color
            var mainModule = diceDissolveParticles.main;
            mainModule.startColor = color;

            // Play the particle system
            diceDissolveParticles.Play();
        }

        public void PlaySparks(Vector3 position, Quaternion rotation, Color color, float intensity = 1)
        {
            sparkParticles.transform.SetPositionAndRotation(position, rotation);
            
            // Get the main module to set the start color
            var mainModule = sparkParticles.main;
            mainModule.startColor = color;

            var b = sparkParticles.emission.GetBurst(0);
            var count = b.count;
            count.constantMax *= intensity;
            b.count = count;
            var curve = b.count;
            curve.constantMin *= intensity;
            b.count = curve;
            sparkParticles.emission.SetBurst(0, b);
            
            
            sparkParticles.Play();
        }
    }
}
