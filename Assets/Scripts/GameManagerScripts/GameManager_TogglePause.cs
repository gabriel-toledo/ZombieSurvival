using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace S3
{
    public class GameManager_TogglePause : MonoBehaviour
    {
        private GameManager_Master gameManagerMaster;
        private bool isPaused;

        void OnEnabled()
        {
            SetInitialReferences();
            gameManagerMaster.MenuToggleEvent += TogglePause;
        }

        void OnDisabled()
        {
            SetInitialReferences();
            gameManagerMaster.MenuToggleEvent -= TogglePause;
        }

        void SetInitialReferences()
        {
            gameManagerMaster = GetComponent<GameManager_Master>();
        }

        void TogglePause()
        {
            if(isPaused)
            {
                Time.timeScale=1;
                isPaused = false;
            }
            else
            {
                Time.timeScale = 0;
                isPaused = true;
            }
        }

    }
}


