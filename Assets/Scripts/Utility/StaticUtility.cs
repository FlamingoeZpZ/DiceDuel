using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Utility
{
    public static class StaticUtility
    {
        public static readonly int TimeID = Shader.PropertyToID("_T");
        public static async void InterpolateAlpha(this Graphic text, float time, float start = 0, float end = 1)
        {
            Color initialColor = text.color;
            initialColor.a = start;
            float elapsedTime = 0f;

            while (elapsedTime < time)
            {
                float t = elapsedTime / time;

                initialColor.a = Mathf.Lerp(start, end, t);
                text.color = initialColor;

                elapsedTime += Time.deltaTime;

                await UniTask.Yield();
            }
            
            initialColor.a = end;
            text.color = initialColor;
        }

    }
}