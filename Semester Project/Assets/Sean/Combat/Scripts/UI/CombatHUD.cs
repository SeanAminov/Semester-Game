using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Sean.Combat
{
    public class CombatHUD : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI resultText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button customizeButton;
        [SerializeField] private CombatManager combatManager;
        [SerializeField] private CombatMenuManager menuManager;

        private void Awake()
        {
            gameOverPanel.SetActive(false);

            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartClicked);

            if (customizeButton != null)
                customizeButton.onClick.AddListener(OnCustomizeClicked);
        }

        public void ShowGameOver(bool playerWon)
        {
            gameOverPanel.SetActive(true);
            resultText.text = playerWon ? "VICTORY!" : "DEFEAT!";
            resultText.color = playerWon ? Color.green : Color.red;
        }

        public void HideGameOver()
        {
            gameOverPanel.SetActive(false);
        }

        private void OnRestartClicked()
        {
            Time.timeScale = 1f;
            combatManager.RestartCombat();
        }

        private void OnCustomizeClicked()
        {
            Time.timeScale = 1f;
            if (menuManager != null)
                menuManager.ShowMainMenu();
        }
    }
}
