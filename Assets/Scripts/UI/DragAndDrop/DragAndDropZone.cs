using System;
using UnityEngine;

namespace UI.DragAndDrop
{
    public class DragAndDropZone : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private RectTransform _dragTransform;
        private DragAndDropObject _dragAndDropObject;
        private static DragAndDropZone _current;
        public event Func<DragAndDropObject,bool> AcceptRules; 
        
        private void Awake()
        {
            DragAndDropObject.OnDragItemChanged += OnItemChanged;
            _rectTransform = ((RectTransform)transform);
            gameObject.isStatic = true;
            OnItemChanged(null);
        }

        private void OnItemChanged(DragAndDropObject obj)
        {
            enabled = obj && (obj.GetLayers() & 1<<gameObject.layer) != 0;
            _dragAndDropObject = obj;
            if(enabled) _dragTransform = ((RectTransform)obj.transform);
        }

        private void Update()
        {
            if (_current == this)
            {
                //Inverse
                if (!RectTransformUtility.RectangleContainsScreenPoint(_rectTransform, _dragTransform.position))
                {
                    _current = null;
                    _dragAndDropObject.UnmarkTarget();
                }
            }
            else
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(_rectTransform, _dragTransform.position))
                {
                    _current = this;
                    _dragAndDropObject.MarkTarget(this);
                }
            }
        }

        public bool CanAcceptItem(DragAndDropObject obj)
        {
            if (AcceptRules == null) return true;
            return AcceptRules.Invoke(obj);
        }

    }
}
