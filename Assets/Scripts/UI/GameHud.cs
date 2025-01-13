using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace UI
{
    [RequireComponent(typeof(AudioSource))]
    public class GameHud : MonoBehaviour
    {
        private int _val;

        [Header("Prefabs")] 
        [SerializeField] private Image displayIcon;
        [SerializeField] private TextMeshProUGUI diceScorePopup;
        [SerializeField] private AnimationCurve curve;
        [SerializeField] private AnimationCurve bubbleTextScale;
        [SerializeField] private float duration;
        [SerializeField] private float bubbleDuration = 0.4f;
    
        [Header("UI Elements")] 
        [SerializeField] private TextMeshProUGUI score;

        private float _textSize;
        private readonly Quaternion _cachedRotation = Quaternion.LookRotation(Vector3.down);
        private SoundEscalator _soundEscalator;
        
        private int _numCreated;
        private Camera _camera;
        private Transform _canvas;
        
        void Awake()
        {
            _textSize = score.fontSize;
            _canvas = GetComponentInParent<Canvas>().transform;
            _soundEscalator = GetComponent<SoundEscalator>();
            _camera = Camera.main;
        }


        public async void GeneratePopup(int value, Vector3 location)
        {
            location = _camera.WorldToScreenPoint(location);
            //location = _camera.WorldToScreenPoint(location);
            TextMeshProUGUI popup = Instantiate(diceScorePopup, location, Quaternion.identity, _canvas);
            popup.text = value.ToString();
            popup.color = score.color;

            _numCreated += 1;
            await BubbleText(popup, popup.fontSize, bubbleTextScale, bubbleDuration);
            await CreateAndHandlePopup(value, location, popup);
            _numCreated -= 1;
        }

        private async UniTask CreateAndHandlePopup(int value, Vector3 location, TextMeshProUGUI popup)
        {
             _soundEscalator.Play();
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
             await BubbleText(score, _textSize, bubbleTextScale, bubbleDuration);
        }

        public async UniTask Display(int initialValue)
        {
            _soundEscalator.ResetProgression();
            score.text = initialValue.ToString();
            _val = initialValue;
            await BubbleText(score, _textSize,  bubbleTextScale, bubbleDuration);
        }
        
        
        #region Should be in a UtilityFile
        //GPT generated cus lazy
        /*
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
        */
        
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

        private bool AreAllDisabled()
        {
            return _numCreated == 0;
        }

        public UniTask AllDisabled()
        {
            return UniTask.WaitUntil(AreAllDisabled);
        }

        public void SetIcon(Sprite icon)
        {
            displayIcon.sprite = icon;
        }
    }
}



