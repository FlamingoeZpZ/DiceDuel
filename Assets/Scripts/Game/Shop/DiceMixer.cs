using System.Collections.Generic;
using Managers;
using Managers.Core;
using UI.DragAndDrop;
using UnityEngine;
using Utility;

namespace Game.Shop
{
    [RequireComponent(typeof(DragAndDropZone))]
    public class DiceMixer : MonoBehaviour
    {
        [SerializeField] private SoundEscalator escalator;
        [SerializeField] private AudioClip acceptSound;
    
        private DragAndDropZone _dragAndDropZone;
    
        private readonly List<RectTransform> _fours = new List<RectTransform>();
        private readonly List<RectTransform> _sixes = new List<RectTransform>();
        private readonly List<RectTransform> _eights = new List<RectTransform>();
        private readonly List<RectTransform> _tens = new List<RectTransform>();
        private readonly List<RectTransform> _twenties = new List<RectTransform>();
    
    
        private void Awake()
        {
            _dragAndDropZone = GetComponent<DragAndDropZone>();
            _dragAndDropZone.OnGain += OnItemGained;
            _dragAndDropZone.OnLost += OnItemLost;
        }

        private void OnItemLost(DragAndDropObject obj)
        {
            if (!obj.TryGetComponent(out DiceValue value)) return;
        
            List<RectTransform> dice = GetList(value.DiceType);
            RectTransform rt = (RectTransform)obj.transform;

            dice.Remove(rt);
            Debug.Log("Removed a dice");
        }

        private List<RectTransform> GetList(EDiceType value)
        {
            switch (value)
            {
                case EDiceType.Four:
                    return _fours;
                case EDiceType.Six:
                    return _sixes;
                case EDiceType.Eight:
                    return _eights;
                case EDiceType.Ten:
                    return _tens;
                default:
                    return _twenties;
            }
        }

        private void OnItemGained(DragAndDropObject obj)
        {
            
            if (!obj.TryGetComponent(out DiceValue value)) return;
        
            List<RectTransform> dice = GetList(value.DiceType);
            RectTransform rt = (RectTransform)obj.transform;

            if (value.DiceType == EDiceType.Twenty)
            {
                dice.Add(rt);
                return;
            }
         

            for (int i = 0; i < dice.Count; i++)
            {
                RectTransform r = dice[i];
                if (RectTransformUtility.RectangleContainsScreenPoint(r, rt.position))
                {
                    dice.RemoveAt(i);
                    Destroy(r.gameObject);
                
                    Debug.Log("Merged a dice");
                
                    int type = (int)value.DiceType + 1;
                    value.SetType(type);
                    escalator.PlayManual(acceptSound, type);

                    List<RectTransform> newSet = GetList(value.DiceType); 
                    newSet.Add(rt);
                    Vector3 loc = Camera.main.ScreenToWorldPoint(rt.position);
                    loc.z = 0;
                    EffectManager.Instance.PlaySparks(loc, Quaternion.identity, DataManager.Instance.diceColors[(int)value.DiceType] , 1); 
                    
                    //SaveManager.CurrentSave.Merge(value.DiceType - 1, value.DiceType);
                    return;
                }
            }
            dice.Add(rt);
            Debug.Log("Added a new dice");
        }
        private void OnEnable()
        {
            SaveManager.OnPreSave += ApplySaveInfo;
        }
        
        private void OnDisable()
        {
            SaveManager.OnPreSave -= ApplySaveInfo;
            ApplySaveInfo();
        }
        
        
        private void ApplySaveInfo()
        {
            SaveManager.CurrentSave.AddDiceToStorage(EDiceType.Four,_fours.Count);
            SaveManager.CurrentSave.AddDiceToStorage(EDiceType.Six, _sixes.Count);
            SaveManager.CurrentSave.AddDiceToStorage(EDiceType.Eight, _eights.Count);
            SaveManager.CurrentSave.AddDiceToStorage(EDiceType.Ten, _tens.Count);
            SaveManager.CurrentSave.AddDiceToStorage(EDiceType.Twenty, _twenties.Count);

            foreach (RectTransform rt in _fours)
            {
                Destroy(rt.gameObject);
            }
            foreach (RectTransform rt in _sixes)
            {
                Destroy(rt.gameObject);
            }
            foreach (RectTransform rt in _eights)
            {
                Destroy(rt.gameObject);
            }
            foreach (RectTransform rt in _tens)
            {
                Destroy(rt.gameObject);
            }
            foreach (RectTransform rt in _twenties)
            {
                Destroy(rt.gameObject);
            }
            
            _fours.Clear();
            _sixes.Clear();
            _eights.Clear();
            _tens.Clear();
            _twenties.Clear();
        }
    }
}
