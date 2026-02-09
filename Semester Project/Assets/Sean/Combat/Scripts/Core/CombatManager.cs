using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sean.Combat
{
    public class CombatManager : MonoBehaviour
    {
        [SerializeField] private CombatConfigSO config;
        [SerializeField] private EnemyProfileSO enemyProfile;
        [SerializeField] private PlayerEnergy playerEnergy;
        [SerializeField] private EnemyEnergy enemyEnergy;
        [SerializeField] private CombatHUD hud;

        private bool _combatOver;

        private void OnEnable()
        {
            CombatEvents.OnFighterDefeated += HandleFighterDefeated;
        }

        private void OnDisable()
        {
            CombatEvents.OnFighterDefeated -= HandleFighterDefeated;
        }

        private void Start()
        {
            StartCombat();
        }

        public void StartCombat()
        {
            _combatOver = false;
            Time.timeScale = 1f;

            playerEnergy.Initialize(config);
            enemyEnergy.Initialize(enemyProfile);

            CombatEvents.RaiseCombatStarted();
        }

        private void HandleFighterDefeated(FighterType type)
        {
            if (_combatOver) return;
            _combatOver = true;

            bool playerWon = type == FighterType.Enemy;
            hud.ShowGameOver(playerWon);
            Time.timeScale = 0f;
        }

        public void RestartCombat()
        {
            Time.timeScale = 1f;
            CombatEvents.ClearAll();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
