using UnityEngine;

namespace Utility
{
    public class KnightAnimationDriver : MonoBehaviour
    {
        //Unfortunate necessity because of how the anims are handled.
        private SpriteRenderer[] _renderers;
        
        private void Awake()
        {
            _renderers = transform.GetChild(0).GetComponentsInChildren<SpriteRenderer>(true);
        }

        public void SetAllRenderersLayer(int layer)
        {
            foreach (SpriteRenderer rendering in _renderers)
            {
                rendering.sortingOrder = layer;
            }
        }
    }

}
