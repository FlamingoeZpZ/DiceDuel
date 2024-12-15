using System;
using UnityEngine;

namespace Utility
{
    public class KnightAnimationDriver : MonoBehaviour
    {
        public void SetAnimation(int num)
        {
            
        }
    }

    [Serializable]
    public struct AnimationSheet
    {
        [SerializeField] private Texture2D textureAtlas;
        [SerializeField] private int width;
        [SerializeField] private bool swordInFront;
    }
}
