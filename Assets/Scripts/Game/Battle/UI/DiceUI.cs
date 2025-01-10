using UI.DragAndDrop;
using UnityEngine;
using Utility;

namespace Game.Battle.UI
{
    public class DiceUI : MonoBehaviour
    {
        [SerializeField] private AudioClip moveSound;
        [SerializeField] private AudioClip hoverSound;
        
        private SoundEscalator _soundEscalator;
        private void Awake()
        {
            DragAndDropObject dragAndDrop = GetComponent<DragAndDropObject>();
            _soundEscalator = GetComponent<SoundEscalator>();
        
            dragAndDrop.onHover.AddListener(PlaySound);
            dragAndDrop.onApplyNewTarget.AddListener(PlaySoundMoved);
        }

        private void PlaySoundMoved(DragAndDropZone oldZone, DragAndDropZone currentZone)
        {
            _soundEscalator.PlayManual(moveSound,transform.GetSiblingIndex()- 8);
        }

        private void PlaySound()
        {
            _soundEscalator.PlayManual(hoverSound,transform.GetSiblingIndex());
        }
    }
}
