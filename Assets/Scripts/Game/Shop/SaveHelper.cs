using System;
using Managers.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Shop
{
    public class SaveHelper : MonoBehaviour
    {
        private Button _button;
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(SaveGame);
        }
        
        

        private async void SaveGame()
        {
            _button.interactable = false;
            await SaveManager.CurrentSave.SaveGame();
            _button.interactable = true;
        }
    }
}
