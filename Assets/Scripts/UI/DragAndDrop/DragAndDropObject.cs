using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace UI.DragAndDrop
{
    public class DragAndDropObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private LayerMask targetLayers;
        private UIShaker _shaker;
        
        private Canvas _canvas;
        private RectTransform _canvasTransform;
        private RectTransform _currentTarget;
        private RectTransform _oldTarget;

        private bool _isBeingHeld;

        public static event Action<DragAndDropObject> OnDragItemChanged;
        
        
        private void Awake()
        {
            _shaker = GetComponent<UIShaker>();
            _canvas = GetComponentInParent<Canvas>();
            _canvasTransform =  _canvas.transform as RectTransform;
            _oldTarget = transform.parent as RectTransform;
        }


        private void Update()
        {
            if (!_isBeingHeld) return;
            //RectTransformUtility.screenpoint
            transform.position = Mouse.current.position.ReadValue();
        }
        
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            _shaker.enabled = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _shaker.enabled = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("OnPointerDown");
            _isBeingHeld = true;
            transform.SetParent(_canvasTransform);
            OnDragItemChanged?.Invoke(this);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("OnPointerUp");
            _isBeingHeld = false;
            OnDragItemChanged?.Invoke(null); // There is no longer a current object
            
            if (_currentTarget)
            {
                transform.SetParent(_currentTarget);
            }
            else
            {
                transform.SetParent(_oldTarget);
            }
        }

        public int GetLayers()
        {
            return targetLayers;
        }

        public void UnmarkTarget()
        {
            _currentTarget = null;
            _shaker.enabled = false;
        }

        public void MarkTarget(RectTransform rectTransform)
        {
            _shaker.enabled = true;
            if (_oldTarget == rectTransform) return; // This is the same object
            _currentTarget = rectTransform;
        }
    }
}
