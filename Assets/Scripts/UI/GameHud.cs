using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(AudioSource))]
    public class GameHud : MonoBehaviour
    {
        private int _val;

        [Header("Prefabs")] 
        [SerializeField] private TextMeshProUGUI diceScorePopup;
        [SerializeField] private AnimationCurve curve;
        [SerializeField] private AnimationCurve bubbleTextScale;
        [SerializeField] private float duration;
        [SerializeField] private float bubbleDuration = 0.4f;
    
        [Header("UI Elements")] 
        [SerializeField] private TextMeshProUGUI score;
        [SerializeField] private AudioClip escalationClip;

        private float _textSize;
        private readonly Quaternion _cachedRotation = Quaternion.LookRotation(Vector3.down);
        private AudioSource _source;
        
        int _counter;
        private int _numCreated;

        private const int SemiTones = 1;
        private static readonly double PitchMultiplier = 1/Mathf.Pow(2f, SemiTones / 12f); // 2^(semitoneIncrease / 12)
        
        
        void Awake()
        {
            _textSize = score.fontSize;
            _source = GetComponent<AudioSource>();

        }


        public async void GeneratePopup(int value, Vector3 location)
        {
            TextMeshProUGUI popup = Instantiate(diceScorePopup, location, Quaternion.identity, transform);
            popup.text = value.ToString();
            popup.color = score.color;

            _numCreated += 1;
            await BubbleText(popup, popup.fontSize, bubbleTextScale, bubbleDuration);
            await CreateAndHandlePopup(value, location, popup);
            _numCreated -= 1;
        }

        private async UniTask CreateAndHandlePopup(int value, Vector3 location, TextMeshProUGUI popup)
        {
            await UniTask.Delay((int)(_counter++ * _source.clip.length*1000 * PitchMultiplier)); //Half of the clip length
            float life = 0;
            Vector3 target = score.rectTransform.rect.center + (Vector2)score.transform.position;
            Transform tr = popup.transform; // yes this does optimize
            while (life < duration)
            {
                life += Time.deltaTime;
                tr.position = Vector3.Slerp(location, target, curve.Evaluate(life / duration));
                await UniTask.Yield();
            }
            _val += value;
            score.text = _val.ToString();
            
            Destroy(popup.gameObject);

            target.y -=  score.rectTransform.rect.center.y;
            Vector3 loc = Camera.main!.ScreenToWorldPoint(target);
            EffectManager.instance.PlaySparks(loc, _cachedRotation, Color.white);
            
            _source.pitch = (float)(1 + (_counter * PitchMultiplier));
            _source.Play();
            
             await BubbleText(score, _textSize, bubbleTextScale, bubbleDuration);
        }

        public void SetColor(Color color)
        {
            score.color = color;
        }

        public async UniTask Display(int initialValue)
        {
            score.text = initialValue.ToString();
            score.gameObject.SetActive(true);
            _val = initialValue;
            await BubbleText(score, _textSize,  bubbleTextScale, bubbleDuration);
        }
        
        public async void Hide(float length = 0.3f)
        {
            _counter = 0;
            await FadeAwayAndDisable(score, length);
        }

       

        #region Should be in a UtilityFile
        //GPT generated cus lazy
        private async UniTask FadeAwayAndDisable(TextMeshProUGUI target, float length)
        {
            if (target == null) return;

            // Get the current color of the text
            Color originalColor = target.color;

            // Gradually fade out the alpha channel
            float elapsedTime = 0f;
            while (elapsedTime < length)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / length);
                target.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

                // Wait until the next frame
                await UniTask.Yield();
            }

            // Reset Alpha
            target.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);

            // Optionally disable the GameObject or component
            target.gameObject.SetActive(false);
        }
        
        private async UniTask BubbleText(TextMeshProUGUI target, float originalScale, AnimationCurve scaleCurve, float length)
        {
            float life = 0;
            target.fontSize = originalScale;
            while (life < length)
            {
                life += Time.deltaTime;
                target.fontSize = originalScale * scaleCurve.Evaluate(life / length);
                await UniTask.Yield();
            }
            target.fontSize = originalScale;
        }
        #endregion

        public UniTask AllDisabled()
        {
            return UniTask.WaitUntil(() => _numCreated == 0);
        }
    }
}



