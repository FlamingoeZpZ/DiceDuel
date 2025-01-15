using Game.Battle.Character;
using TMPro;
using UI.DragAndDrop;
using UnityEngine;
using Utility;

namespace Game.Shop
{
    public class InfiniteDiceHolder : MonoBehaviour
    {
        [SerializeField] private RectTransform prefabParent;
        [SerializeField] private DragAndDropObject dragAndDropObjectPrefab;
        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] private EDiceType diceType;

        private DragAndDropObject _currentObject;
        private DragAndDropZone _dropZone;
        private int _currentAmount = 3;

        [SerializeField] private SoundEscalator escalator;

        private void Awake()
        {
            CreateNewObject();

            _dropZone = GetComponentInChildren<DragAndDropZone>();
            
            _dropZone.AcceptRules += AcceptRules;
            _dropZone.OnGain += DestroyItem;
            _dropZone.OnLost += SpawnItem;

             HandleEmpty();
             
        }

        private void SpawnItem(DragAndDropObject obj)
        {
            _currentAmount -= 1;
            escalator.Decrement();
            CreateNewObject();
            HandleEmpty();
        }

        private void HandleEmpty()
        {
            if (_currentAmount > 0) return;
            _currentObject.enabled = false;
        }
        

        private void DestroyItem(DragAndDropObject obj)
        {
            _currentAmount += 1;
            escalator.Increment();
            escalator.Play(false);
            if (_currentAmount > 0)  _currentObject.enabled = true;
            amountText.text = _currentAmount.ToString();
            Destroy(obj.gameObject);
        }


        private bool AcceptRules(DragAndDropObject diceObject)
        {
            return diceObject.TryGetComponent(out DiceValue dice) && dice.DiceType == diceType;
        }
        
        private void CreateNewObject()
        {
            _currentObject = Instantiate(dragAndDropObjectPrefab, prefabParent);
            _currentObject.gameObject.AddComponent<DiceValue>().SetType(diceType);
            amountText.text = _currentAmount.ToString();
        }
    }
}
