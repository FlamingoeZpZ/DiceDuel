using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EffectManager : MonoBehaviour
{
    
    [SerializeField] private AudioClip diceSound;
    [SerializeField] private AudioClip diceHitSound;
    
    [SerializeField] private ParticleSystem sparkParticles;
    [SerializeField] private ParticleSystem diceDissolveParticles;
    
  
    
    public static EffectManager instance { get; private set; }

    private AudioSource _source;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        _source = GetComponent<AudioSource>();
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

    public void PlaySparks(Vector3 position, Quaternion rotation, Color color)
    {
        sparkParticles.transform.SetPositionAndRotation(position, rotation);
            
        // Get the main module to set the start color
        var mainModule = sparkParticles.main;
        mainModule.startColor = color;
            
        sparkParticles.Play();
    }
}
