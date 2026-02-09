using UnityEngine;

namespace Sean.Combat
{
    [CreateAssetMenu(fileName = "EnemyProfile", menuName = "Combat/Enemy Profile")]
    public class EnemyProfileSO : ScriptableObject
    {
        [Header("Energy")]
        public int startingEnergy = 50;
        public int attackEnergyCost = 3;

        [Header("Attack Timing")]
        public float telegraphDurationMin = 0.5f;
        public float telegraphDurationMax = 1.0f;
        public float attackCooldownMin = 1.5f;
        public float attackCooldownMax = 3.0f;

        [Header("Attack Damage")]
        public int attackDamage = 5;

        [Header("Stun")]
        public float parryStunDuration = 1.0f;
    }
}
