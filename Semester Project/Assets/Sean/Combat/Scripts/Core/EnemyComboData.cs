using UnityEngine;

namespace Sean.Combat
{
    [System.Serializable]
    public class ComboAttack
    {
        public AttackDirection Direction = AttackDirection.Up;
        public float TelegraphDuration = 0.5f;
        public float IntervalAfter = 0.3f;
    }

    [System.Serializable]
    public class EnemyComboData
    {
        public string Name = "Combo";
        public bool Enabled = true;
        public ComboAttack[] Attacks = new ComboAttack[]
        {
            new ComboAttack { Direction = AttackDirection.Left, TelegraphDuration = 0.5f, IntervalAfter = 0.3f },
            new ComboAttack { Direction = AttackDirection.Right, TelegraphDuration = 0.4f, IntervalAfter = 0.3f },
        };
    }
}
