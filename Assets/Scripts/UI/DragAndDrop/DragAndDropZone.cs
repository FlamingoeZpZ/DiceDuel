using UnityEngine;
using UnityEngine.EventSystems;
using Utility;

namespace UI.DragAndDrop
{
    public class DragAndDropZone : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private RectTransform _dragTransform;
        private DragAndDropObject _dragAndDropObject;
        private static DragAndDropZone _current;
        
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
                    Debug.Log("Stopped Overlapping", gameObject);
                    _current = null;
                    _dragAndDropObject.UnmarkTarget();
                }
            }
            else
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(_rectTransform, _dragTransform.position))
                {
                    Debug.Log("Overlapping", gameObject);
                    _current = this;
                    _dragAndDropObject.MarkTarget(_rectTransform);
                }
            }
        }

    }
}
