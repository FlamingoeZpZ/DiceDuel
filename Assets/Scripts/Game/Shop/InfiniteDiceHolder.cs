using System;
using Managers.Core;
using TMPro;
using UI.DragAndDrop;
using UnityEngine;
using Utility;

namespace Game.Shop
{
    
    public class InfiniteDiceHolder : MonoBehaviour
    {

        private enum EEmptyResponse
        {
            Keep,
            Destroy
        }
        
        [SerializeField] private RectTransform prefabParent;
        [SerializeField] private DragAndDropObject dragAndDropObjectPrefab;
        [SerializeField] private TextMeshProUGUI amountText;
        [SerializeField] private bool takeAnyType;
        [SerializeField] private EEmptyResponse emptyResponse;
        private bool _hasAType;
        
        private EDiceType _diceType;

        private DragAndDropObject _currentObject;
        private DragAndDropZone _dropZone;
        private int _currentAmount = 0;

        public EDiceType DiceType => _diceType;
        public int CurrentAmount => _currentAmount;
        
        public event Action OnDiceRemoved;
        public event Action OnDiceAdded;

        [SerializeField] private SoundEscalator escalator;

        private void OnEnable()
        {
            
            //??= only do if null.
            _dropZone ??= GetComponentInChildren<DragAndDropZone>();
            _dropZone.enabled = true;

            CreateNewObject();
            
            _dropZone.AcceptRules += AcceptRules;
            _dropZone.OnGain += DestroyItem;
            _dropZone.OnLost += SpawnItem;
            
            HandleEmpty();
            
        

        }

        private void OnDisable()
        {
            _dropZone.enabled = false;
            _dropZone.AcceptRules -= AcceptRules;
            _dropZone.OnGain -= DestroyItem;
            _dropZone.OnLost -= SpawnItem;

           
        }

        private void SpawnItem(DragAndDropObject obj)
        {
            _currentAmount -= 1;
            escalator.Decrement();
            CreateNewObject();
            HandleEmpty();
            OnDiceRemoved?.Invoke();
        }

        private void HandleEmpty()
        {
            if (_currentAmount > 0) return;
            _currentObject.enabled = false;
            switch (emptyResponse)
            {
                case EEmptyResponse.Keep:
                    break;
                case EEmptyResponse.Destroy:
                    _hasAType = false;
                    if(_currentObject) Destroy(_currentObject.gameObject);
                    break;
            }
        }
        

        private void DestroyItem(DragAndDropObject obj)
        {
            _currentAmount += 1;
            escalator.Increment();
            escalator.Play(false);
            if (_currentAmount > 0)  _currentObject.enabled = true;
            amountText.text = _currentAmount.ToString();
            Destroy(obj.gameObject);
            OnDiceAdded?.Invoke();
        }


        private bool AcceptRules(DragAndDropObject diceObject)
        {
            if (diceObject.TryGetComponent(out DiceValue dice))
            {
                if (takeAnyType && !_hasAType)
                {
                    _hasAType = true;
                    _diceType = dice.DiceType;
                    if(_currentObject) Destroy(_currentObject.gameObject);
                    CreateNewObject();
                    return true;
                }
                return dice.DiceType == _diceType;
            }

            return false;
        }
        
        private void CreateNewObject()
        {
            _currentObject = Instantiate(dragAndDropObjectPrefab, prefabParent);
            DiceValue dv = _currentObject.gameObject.AddComponent<DiceValue>();
            if(_hasAType) dv.SetType(_diceType);
            else dv.SetInvalid();
            amountText.text = _currentAmount.ToString();
        }

        public void SetDiceType(EDiceType diceType, int amount)
        {
            _hasAType = true;
            _diceType = diceType;
            _currentAmount = amount;
            amountText.text = _currentAmount.ToString();

            if(_currentObject) Destroy(_currentObject.gameObject);
            CreateNewObject();
            HandleEmpty();
        }

        public void ClearDiceType()
        {
            _hasAType = false;
        }
    }
}
