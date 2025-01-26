using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicManager : MonoBehaviour
    {
        [SerializeField] private AudioClip battleMusic;
        [SerializeField] private AudioClip mainMusic;
        [SerializeField] private float transitionTime;
        public static MusicManager Instance { get; private set; }
        private AudioSource _audioSource;
        private bool _isInBattle;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            Instance = this;
            _audioSource = GetComponent<AudioSource>();
            SceneManager.sceneLoaded += OnSceneLoaded;
        
            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }

        private async void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.buildIndex == 3 && !_isInBattle)
            {

                _isInBattle = true;
                await TransitionMusic(battleMusic);
            }
            else
            {
                await TransitionMusic(mainMusic);
            }
        }

        private async UniTask TransitionMusic(AudioClip newMusic)
        {
            float t = _audioSource.time;
            float ct = 0;

            float startVolume = Mathf.Max(0.1f,_audioSource.volume);
            
            while (ct < transitionTime)
            {
                ct += Time.deltaTime;
                _audioSource.volume = Mathf.Lerp(startVolume, 0, ct/transitionTime);
                await UniTask.Yield();
            }

            _audioSource.clip = newMusic;
            _audioSource.Play();
            _audioSource.time = t;
        
            ct = 0;
            while (ct < transitionTime)
            {
                ct += Time.deltaTime;
                _audioSource.volume = Mathf.Lerp(0, startVolume, ct/transitionTime);
                await UniTask.Yield();
            }
            _audioSource.volume = startVolume;
        }

    }
}
