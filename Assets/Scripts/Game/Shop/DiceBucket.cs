using Managers.Core;
using UnityEngine;

namespace Game.Shop
{
    [RequireComponent(typeof(InfiniteDiceHolder)), DefaultExecutionOrder(1000)]
    public class DiceBucket : MonoBehaviour
    {
        [SerializeField] private EDiceType dice;
        private InfiniteDiceHolder _holder;
        private void Start()
        {
            _holder = GetComponent<InfiniteDiceHolder>();
            Debug.Log("Holder? " + (_holder == null));
            SetDice();

            _holder.OnDiceRemoved += () => SaveManager.CurrentSave.RemoveStoredDice(dice, 1);
            _holder.OnDiceAdded += () => SaveManager.CurrentSave.AddDiceToStorage(dice, 1);
            
           

        }

        private void OnEnable()
        {
            SaveManager.OnPreSave += SetDice;
        }

        private void OnDisable()
        {
            SaveManager.OnPreSave -= SetDice;
        }

        private void SetDice()
        {
            _holder.SetDiceType(dice, SaveManager.CurrentSave.StoredDice[(int)dice]);
        }

}
}
