using UnityEngine;

namespace Sean.Combat
{
    public class CombatMenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject customizePanel;
        [SerializeField] private GameObject playPanel;
        [SerializeField] private GameObject tutorialPanel;
        [SerializeField] private CombatManager combatManager;
        [SerializeField] private CombatHUD hud;
        [SerializeField] private CombatTutorialManager tutorialManager;

        private void Start()
        {
            ShowCustomizeScreen();
        }

        public void ShowCustomizeScreen()
        {
            Time.timeScale = 1f;

            // Hide game over panel first
            if (hud != null) hud.HideGameOver();

            // Clean up any lingering combat visuals
            var notifUI = FindObjectOfType<CombatNotificationUI>();
            if (notifUI != null) notifUI.ClearAllNotifications();

            var visuals = FindObjectsOfType<FighterVisual>();
            foreach (var v in visuals) v.ResetColor();

            if (customizePanel != null) customizePanel.SetActive(true);
            if (playPanel != null) playPanel.SetActive(false);
            if (tutorialPanel != null) tutorialPanel.SetActive(false);

            // Stop tutorial if it was running
            if (tutorialManager != null)
                tutorialManager.StopTutorial();
        }

        public void ShowPlayScreen()
        {
            if (customizePanel != null) customizePanel.SetActive(false);
            if (playPanel != null) playPanel.SetActive(true);
            if (tutorialPanel != null) tutorialPanel.SetActive(false);
            if (combatManager != null) combatManager.StartCombat();
        }

        public void ShowTutorial()
        {
            if (customizePanel != null) customizePanel.SetActive(false);
            if (playPanel != null) playPanel.SetActive(false);
            if (tutorialPanel != null) tutorialPanel.SetActive(true);

            // Hide game over panel
            if (hud != null) hud.HideGameOver();

            // Clean up visuals
            var notifUI = FindObjectOfType<CombatNotificationUI>();
            if (notifUI != null) notifUI.ClearAllNotifications();

            var visuals = FindObjectsOfType<FighterVisual>();
            foreach (var v in visuals) v.ResetColor();

            if (tutorialManager != null)
                tutorialManager.StartTutorial();
        }
    }
}
