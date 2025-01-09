using UnityEngine;

namespace Utility
{
    public static class StaticUtility
    {
        public static readonly int T = Shader.PropertyToID("_T");

        public static Rect WorldRect(this RectTransform rectTransform)
        {
            //Reconstruct in proper location
            return new Rect(rectTransform.position.x, rectTransform.position.y, rectTransform.rect.width, rectTransform.rect.height);
        }

    }
}