using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Prototype.NetworkLobby
{
    public class LobbyTopPanel : MonoBehaviour
    {
        public bool isInGame = false;

		public GameObject title;
		public Button backButton;
		public GameObject info;

        protected bool isDisplayed = true;
        protected Image panelImage;

        void Start()
        {
            panelImage = GetComponent<Image>();
        }


        void Update()
        {
            if (!isInGame)
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleVisibility(!isDisplayed);
            }

        }

        public void ToggleVisibility(bool visible)
        {
            isDisplayed = visible;
			backButton.gameObject.SetActive ((isInGame && Application.isMobilePlatform) || isDisplayed);
			title.gameObject.SetActive (!isInGame && isDisplayed);
			info.gameObject.SetActive (!isInGame && isDisplayed);
            if (panelImage != null)
            {
                panelImage.enabled = isDisplayed;
            }
        }
    }
}