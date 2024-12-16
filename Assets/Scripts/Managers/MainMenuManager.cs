using UnityEngine;

namespace Managers
{
    public class MainMenuManager : MonoBehaviour
    {

        public void Quit()
        {
            Application.Quit();
        }

        public void LoadLevel(int id)
        {
            LoadingManager.instance.LoadLevelById(id);
        }


    }
}