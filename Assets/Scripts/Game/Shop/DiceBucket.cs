using Managers.Core;
using UnityEngine;

namespace Game.Shop
{
    [RequireComponent(typeof(InfiniteDiceHolder)), DefaultExecutionOrder(1000)]
    public class DiceBucket : MonoBehaviour
    {
        [SerializeField] private EDiceType dice;
        private void Start()
        {
            InfiniteDiceHolder holder = GetComponent<InfiniteDiceHolder>();
            Debug.Log("Holder? " + (holder == null));
            holder.SetDiceType(dice, SaveManager.CurrentSave.StoredDice[(int)dice]);
        }
    }
}
