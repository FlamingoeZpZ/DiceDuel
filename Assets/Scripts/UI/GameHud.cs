using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Battle.Attacks;
using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class GameHud : MonoBehaviour
    {
        private int _leftVal;
        private int _rightVal;

        [Header("Prefabs")] 
        [SerializeField] private TextMeshProUGUI diceScorePopup;
        [SerializeField] private AnimationCurve curve;
        [SerializeField] private AnimationCurve bubbleTextScale;
        [SerializeField] private float duration;
        [SerializeField] private float bubbleDuration = 0.4f;
    
        [Header("UI Elements")] 
        [SerializeField] private TextMeshProUGUI leftScore;
        [SerializeField] private TextMeshProUGUI rightScore;

        private float _textSize;

        private readonly Quaternion _cachedRotation = Quaternion.LookRotation(Vector3.down);
    
        private static GameHud _instance;
        
        private readonly List<ItemData> _items = new();

        private AudioSource _source;
        
        int _leftCounter;
        int _rightCounter;
        
        [SerializeField] private AudioClip escalationClip;
        
        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _textSize = leftScore.fontSize;
            _instance = this;
            _source = GetComponent<AudioSource>();
            Dice.OnDiceRolled += OnDiceRolled;

        }

        private void OnDestroy()
        {
            Dice.OnDiceRolled -= OnDiceRolled;
        }



        private async void OnDiceRolled(EDiceType type, Dice dice)
        {
            int value = dice.CurrentValue;
            Vector3 location = Camera.main!.WorldToScreenPoint(dice.transform.position);
            TextMeshProUGUI popup = Instantiate(diceScorePopup, location, Quaternion.identity, transform);
            popup.text = value.ToString();
            Color c =dice.MyColor / Mathf.Max(dice.MyColor.r, dice.MyColor.g, dice.MyColor.b);
            c.a = 1;
            popup.color = c;
            
            _items.Add(new ItemData
            {
                Value = value,
                Location = location,
                Popup = popup,
                Target = dice.IsLeftSide ? _instance.leftScore : _instance.rightScore,
                IsLeft = dice.IsLeftSide
            });
            await BubbleText(popup, popup.fontSize);
            

        }

        private async UniTask CreateAndHandlePopup(ItemData item, int addedDelay)
        {
            await UniTask.Delay(addedDelay);
            float life = 0;
            Vector3 target = item.Target.rectTransform.rect.center + (Vector2)item.Target.transform.position;
            Transform tr = item.Popup.transform; // yes this does optimize
            while (life < duration)
            {
                life += Time.deltaTime;
                tr.position = Vector3.Slerp(item.Location, target, curve.Evaluate(life / duration));
                await UniTask.Yield();
            }

            if (item.IsLeft)
            {
                _leftVal += item.Value;
                item.Target.text = _leftVal.ToString();
            }
            else
            {
                _rightVal += item.Value;
                item.Target.text = _rightVal.ToString();
            }
            
            Destroy(item.Popup.gameObject);

            target.y -= item.Target.rectTransform.rect.center.y;
            Vector3 loc = Camera.main.ScreenToWorldPoint(target);
            EffectManager.instance.PlaySparks(loc, _cachedRotation, Color.white);

            int side = item.IsLeft ? _leftCounter++ : _rightCounter++;

            _source.pitch = Mathf.Sqrt(side/3f);
            _source.Play();
            
             await BubbleText(item.Target, _textSize);
        }

        private async UniTask BubbleText(TextMeshProUGUI target, float originalScale)
        {
            
            float life = 0;
            target.fontSize = originalScale;
            while (life < bubbleDuration)
            {
                life += Time.deltaTime;
                target.fontSize = originalScale * bubbleTextScale.Evaluate(life / bubbleDuration);
                await UniTask.Yield();
            }
            target.fontSize = originalScale;
        }

        private struct ItemData
        {
            public bool IsLeft;
            public Vector3 Location;
            public int Value;
            public TextMeshProUGUI Popup;
            public TextMeshProUGUI Target;
        }

        public static async void DisplayDefaults(Color leftColor, int leftInitialValue, Color rightColor, int rightInitialValue)
        {
            _instance._leftCounter = 0;
            _instance._rightCounter = 0;
            leftColor/= Mathf.Max(leftColor.r,leftColor.g, leftColor.b);
            rightColor/= Mathf.Max(rightColor.r,rightColor.g, rightColor.b);
            leftColor.a = 1;
            rightColor.a = 1;
            _instance.leftScore.text = leftInitialValue.ToString();
            _instance.leftScore.color = leftColor;
            _instance.rightScore.text = rightInitialValue.ToString();
            _instance.rightScore.color = rightColor;
            _instance._leftVal = leftInitialValue;
            _instance._rightVal = rightInitialValue;
            //Play them simultaniously
            await UniTask.WhenAll(_instance.BubbleText(_instance.leftScore, _instance._textSize), _instance.BubbleText(_instance.rightScore, _instance._textSize));
        }

        public static async UniTask SendDice()
        {
            UniTask[] tasks = new UniTask[_instance._items.Count];
            for (int i = 0; i < _instance._items.Count; i++)
            {
                ItemData item = _instance._items[i];
               //Send each task to begin with a deviation on a log curve  
               tasks[i]= _instance.CreateAndHandlePopup(item, (int)(Mathf.Sqrt(i) * 750));
            }
            //Wait for all tasks to finish
            await UniTask.WhenAll(tasks); //The last one
            _instance._items.Clear();
        }

        public static void ResetHUD()
        {
            _instance.leftScore.text = "";
            _instance.rightScore.text = "";
        }
    }
}
