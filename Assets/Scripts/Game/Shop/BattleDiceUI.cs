using System.Collections.Generic;
using Managers.Core;
using UI.DragAndDrop;
using UnityEngine;

namespace Game.Shop
{
    public class BattleDiceUI : MonoBehaviour
    {
        [SerializeField] private DragAndDropZone dragAndDropZone;
        [SerializeField] private DragAndDropObject dragAndDropObjectPrefab;
        [SerializeField] private int maxDice = 12;
        private List<DragAndDropObject> _draggedObjects = new();
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            foreach(EDiceType dice in SaveManager.CurrentSave.BattleDice)
            {
                DragAndDropObject d = Instantiate(dragAndDropObjectPrefab, dragAndDropZone.transform);
                d.gameObject.AddComponent<DiceValue>().SetType(dice);
                _draggedObjects.Add(d);
                d.SetParent(dragAndDropZone);
            }

            //Note you can do without ITEM for add, but not remove because bool lol
            dragAndDropZone.OnGain += item => _draggedObjects.Add(item);
            dragAndDropZone.OnLost += item => _draggedObjects.Remove(item);

            dragAndDropZone.AcceptRules += AcceptRules;
        }

        private bool AcceptRules(DragAndDropObject arg)
        {
            return _draggedObjects.Count < maxDice;
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
            SaveManager.CurrentSave.BattleDice.Clear();
            foreach (DragAndDropObject obj in _draggedObjects)
            {
                SaveManager.CurrentSave.BattleDice.Add(obj.GetComponent<DiceValue>().DiceType);
            }
            
        }

    }
}
