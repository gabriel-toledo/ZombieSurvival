using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace S3
{
    public class GameManager_Master : MonoBehaviour
    {

        public delegate void GameManagerEventHandler();
        public event GameManagerEventHandler MenuToggleEvent;
        public event GameManagerEventHandler RestartLevelEvent;
        public event GameManagerEventHandler GoToMenuSceneEvent;
        public event GameManagerEventHandler GameOverEvent;

        public bool isGameOver;
        public bool isMenuOn;

        public void CallEventMenuToggle()
        {
            if(MenuToggleEvent != null)
            {
                MenuToggleEvent();
            }
        }

        public void CallEventRestartLevel()
        {
            if(RestartLevelEvent != null)
            {
                RestartLevelEvent();
            }
        }

        public void CallEventGoToMenuScene()
        {
            if(GoToMenuSceneEvent != null)
            {
                GoToMenuSceneEvent();
            }
        }

        public void CallEventGameOver()
        {
            if(GameOverEvent != null)
            {
                isGameOver = true;
                GameOverEvent();
            }
        }
    }

}
