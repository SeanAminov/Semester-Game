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
        [SerializeField] private CombatManager combatManager;

        private void Awake()
        {
            gameOverPanel.SetActive(false);

            if (restartButton != null)
            {
                restartButton.onClick.AddListener(OnRestartClicked);
            }
        }

        public void ShowGameOver(bool playerWon)
        {
            gameOverPanel.SetActive(true);
            resultText.text = playerWon ? "VICTORY!" : "DEFEAT!";
            resultText.color = playerWon ? Color.green : Color.red;
        }

        private void OnRestartClicked()
        {
            combatManager.RestartCombat();
        }
    }
}
