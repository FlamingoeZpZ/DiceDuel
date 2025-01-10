using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace UI.DragAndDrop
{
    public class DragAndDropObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private LayerMask targetLayers;
        
        private Canvas _canvas;
        private RectTransform _canvasTransform;
        private DragAndDropZone _currentTarget;
        private DragAndDropZone _oldTarget;
        private bool _isBeingHeld;

        public UnityEvent onHover;
        public UnityEvent onStopHover;
        public UnityEvent onNewTarget;
        public UnityEvent onTargetRemoved;
        public UnityEvent<DragAndDropZone,DragAndDropZone> onApplyNewTarget;
        
        public static event Action<DragAndDropObject> OnDragItemChanged;
        
        
        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
            _canvasTransform =  _canvas.transform as RectTransform;
            _oldTarget = transform.parent.GetComponent<DragAndDropZone>();
        }


        private void Update()
        {
            if (!_isBeingHeld) return;
            //RectTransformUtility.screenpoint
            transform.position = Mouse.current.position.ReadValue();
        }
        
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_isBeingHeld) return;
            onHover?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_isBeingHeld) return;
            onStopHover?.Invoke();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isBeingHeld = true;
            transform.SetParent(_canvasTransform);
            OnDragItemChanged?.Invoke(this);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isBeingHeld = false;
            OnDragItemChanged?.Invoke(null); // There is no longer a current object
            
            if (_currentTarget)
            {
                SetParent(_currentTarget);
                _currentTarget = null;
            }
            else
            {
                transform.SetParent(_oldTarget.transform);
            }
        }

        public void SetParent(DragAndDropZone target)
        {
            transform.SetParent(target.transform);
            onApplyNewTarget?.Invoke(_oldTarget, target);
            _oldTarget = target;
        }

        public int GetLayers()
        {
            return targetLayers;
        }

        public void UnmarkTarget()
        {
            _currentTarget = null;
            onTargetRemoved?.Invoke();
        }

        public void MarkTarget(DragAndDropZone newTarget)
        {
            onNewTarget?.Invoke();
            if (_oldTarget == newTarget) return; // This is the same object
            _currentTarget = newTarget;
        }

        private void OnTransformParentChanged()
        {
            if (transform.parent.TryGetComponent(out DragAndDropZone target))
            {
                _currentTarget = null;
                SetParent(target);
            }
        }
    }
}
