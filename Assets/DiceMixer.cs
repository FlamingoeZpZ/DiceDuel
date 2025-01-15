using System.Collections.Generic;
using Game.Battle.Character;
using Game.Shop;
using UI.DragAndDrop;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utility;

[RequireComponent(typeof(DragAndDropZone))]
public class DiceMixer : MonoBehaviour
{
    [SerializeField] private SoundEscalator escalator;
    [SerializeField] private AudioClip acceptSound;
    
    [SerializeField] private Sprite[] sprites;
    private DragAndDropZone _dragAndDropZone;
    
    private readonly List<RectTransform> _fours = new List<RectTransform>();
    private readonly List<RectTransform> _sixes = new List<RectTransform>();
    private readonly List<RectTransform> _eights = new List<RectTransform>();
    private readonly List<RectTransform> _tens = new List<RectTransform>();
    
    
    private void Awake()
    {
        _dragAndDropZone = GetComponent<DragAndDropZone>();
        _dragAndDropZone.OnGain += OnItemGained;
        _dragAndDropZone.OnLost += OnItemLost;
    }

    private void OnItemLost(DragAndDropObject obj)
    {
        if (!obj.TryGetComponent(out DiceValue value) || value.DiceType == EDiceType.Twenty) return;
        
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
        }
        return null;
    }

    private void OnItemGained(DragAndDropObject obj)
    {
        if (!obj.TryGetComponent(out DiceValue value)  || value.DiceType == EDiceType.Twenty) return;
        
        List<RectTransform> dice = GetList(value.DiceType);
        RectTransform rt = (RectTransform)obj.transform;

        for (int i = 0; i < dice.Count; i++)
        {
            RectTransform r = dice[i];
            if (RectTransformUtility.RectangleContainsScreenPoint(r, rt.position))
            {
                dice.RemoveAt(i);
                Destroy(r.gameObject);
                
                Debug.Log("Merged a dice");
                
                int type = (int)value.DiceType + 1;
                value.SetType((EDiceType)type);
                rt.GetComponent<Image>().sprite = sprites[type];
                escalator.PlayManual(acceptSound, type);

                if (value.DiceType == EDiceType.Twenty) return;
                List<RectTransform> newSet = GetList(value.DiceType); 
                newSet.Add(rt);
                return;
            }
        }
        dice.Add(rt);
        Debug.Log("Added a new dice");
    }
}
