using UnityEngine;

namespace Sean.Combat
{
    public class CombatMenuManager : MonoBehaviour
    {
        [SerializeField] private GameObject customizePanel;
        [SerializeField] private GameObject playPanel;
        [SerializeField] private CombatManager combatManager;

        private void Start()
        {
            ShowCustomizeScreen();
        }

        public void ShowCustomizeScreen()
        {
            Time.timeScale = 1f;
            if (customizePanel != null) customizePanel.SetActive(true);
            if (playPanel != null) playPanel.SetActive(false);
        }

        public void ShowPlayScreen()
        {
            if (customizePanel != null) customizePanel.SetActive(false);
            if (playPanel != null) playPanel.SetActive(true);
            if (combatManager != null) combatManager.StartCombat();
        }
    }
}
