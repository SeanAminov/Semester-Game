using UnityEngine;

namespace Sean.Combat
{
    [CreateAssetMenu(fileName = "CombatConfig", menuName = "Combat/Combat Config")]
    public class CombatConfigSO : ScriptableObject
    {
        [Header("Player Energy")]
        public int playerStartingEnergy = 20;
        public int punchEnergyCost = 3;
        public int dodgeEnergyCost = 2;
        public int parryEnergyGain = 5;
        public int playerDamageTaken = 5;

        [Header("Player Attack Damage")]
        public int punchDamage = 3;

        [Header("Timing Windows (seconds)")]
        public float parryDuration = 0.3f;
        public float dodgeDuration = 0.4f;
        public float enemyVulnerabilityWindow = 0.2f;
        public float punchDuration = 0.3f;

        [Header("Visual")]
        public float hitFlashDuration = 0.15f;
        public float notificationDuration = 0.8f;
        public float notificationFloatSpeed = 1.5f;

        [Header("Colors")]
        public Color parryColor = Color.blue;
        public Color dodgeColor = Color.green;
        public Color hitColor = Color.red;
        public Color punchColor = Color.yellow;
    }
}
