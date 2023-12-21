using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace S3
{
    public class GameManager_ToggleMenu : MonoBehaviour
    {
        private GameManager_Master gameManagerMaster;
        public GameObject menu;

        // Start is called before the first frame update
        void Start()
        {
            ToggleMenu();
        }

        // Update is called once per frame
        void Update()
        {
            CheckForMenuToggleRequest();
        }

        void OnEnabled()
        {
            SetInitialReferences();
            gameManagerMaster.GameOverEvent += ToggleMenu;
        }

        void OnDisabled()
        {
            gameManagerMaster.GameOverEvent -= ToggleMenu;
        }

        void SetInitialReferences()
        {
            gameManagerMaster = GetComponent<GameManager_Master>(); 
        }

        void CheckForMenuToggleRequest()
        {
            if(Input.GetKeyUp(KeyCode.Escape) && !gameManagerMaster.isGameOver)
            {
                ToggleMenu();
                Debug.LogWarning("Need to assign a UI GameObject to the Toggle Menu script in the inspector.");
            }
        }

        void ToggleMenu()
        {
            if(menu != null)
            {
                menu.SetActive(!menu.activeSelf);
                gameManagerMaster.isMenuOn = !gameManagerMaster.isMenuOn;
                gameManagerMaster.CallEventMenuToggle();
            }
            else
            {
                Debug.LogWarning("Need to assign a UI GameObject to the Toggle Menu script in the inspector.");
            }
        }
    }
}


