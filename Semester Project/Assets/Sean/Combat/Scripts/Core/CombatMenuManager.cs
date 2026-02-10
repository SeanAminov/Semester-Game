using UnityEngine;

namespace Sean.Combat
{
    public class CombatMenuManager : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject customizePanel;
        [SerializeField] private GameObject playPanel;
        [SerializeField] private GameObject tutorialPanel;

        [Header("References")]
        [SerializeField] private CombatManager combatManager;
        [SerializeField] private CombatHUD hud;
        [SerializeField] private CombatTutorialManager tutorialManager;

        private void Start()
        {
            ShowMainMenu();
        }

        /// <summary>
        /// First screen the player sees: Play, Customize, Tutorial.
        /// </summary>
        public void ShowMainMenu()
        {
            Time.timeScale = 1f;

            if (hud != null) hud.HideGameOver();

            CleanupVisuals();

            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
            if (customizePanel != null) customizePanel.SetActive(false);
            if (playPanel != null) playPanel.SetActive(false);
            if (tutorialPanel != null) tutorialPanel.SetActive(false);

            if (tutorialManager != null)
                tutorialManager.StopTutorial();
        }

        public void ShowCustomizeScreen()
        {
            Time.timeScale = 1f;

            if (hud != null) hud.HideGameOver();

            CleanupVisuals();

            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (customizePanel != null) customizePanel.SetActive(true);
            if (playPanel != null) playPanel.SetActive(false);
            if (tutorialPanel != null) tutorialPanel.SetActive(false);

            if (tutorialManager != null)
                tutorialManager.StopTutorial();
        }

        public void ShowPlayScreen()
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (customizePanel != null) customizePanel.SetActive(false);
            if (playPanel != null) playPanel.SetActive(true);
            if (tutorialPanel != null) tutorialPanel.SetActive(false);
            if (combatManager != null) combatManager.StartCombat();
        }

        public void ShowTutorial()
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (customizePanel != null) customizePanel.SetActive(false);
            if (playPanel != null) playPanel.SetActive(false);
            if (tutorialPanel != null) tutorialPanel.SetActive(true);

            if (hud != null) hud.HideGameOver();

            CleanupVisuals();

            if (tutorialManager != null)
                tutorialManager.StartTutorial();
        }

        private void CleanupVisuals()
        {
            var notifUI = FindObjectOfType<CombatNotificationUI>();
            if (notifUI != null) notifUI.ClearAllNotifications();

            var visuals = FindObjectsOfType<FighterVisual>();
            foreach (var v in visuals) v.ResetColor();
        }
    }
}
