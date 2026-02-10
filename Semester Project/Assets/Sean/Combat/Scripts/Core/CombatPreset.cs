namespace Sean.Combat
{
    [System.Serializable]
    public class CombatPreset
    {
        public string Name;

        // Player Core
        public int PlayerStartingEnergy = 20;
        public int AttackDamage = 4;
        public int AttackEnergyCost = 2;
        public float AttackCooldown = 0.5f;

        // Parry
        public float ParryWindowDuration = 0.3f;
        public bool ParryRefillsEnergy = true;
        public int ParryEnergyRefill = 6;
        public bool ParryDrainsEnemyEnergy = false;
        public int ParryEnemyEnergyDrain = 3;

        // Dodge
        public bool DodgeEnabled = true;
        public int DodgeEnergyCost = 2;
        public float DodgeDuration = 0.4f;

        // Block
        public bool BlockEnabled = false;
        public float BlockDamageReduction = 0.5f;

        // Posture (Enemy)
        public bool PostureEnabled = false;
        public int MaxPosture = 100;
        public int PostureDamageOnParry = 30;
        public int PostureDamageOnAttack = 10;
        public float PostureStunDuration = 5f;

        // Crit (Player)
        public bool CritMeterEnabled = false;
        public bool CritAutoActivate = true;
        public int CritMeterMax = 100;
        public int CritMeterGainOnParry = 25;
        public int CritMeterGainOnAttack = 10;
        public int CritDamage = 20;
        public bool CritRestoresEnergy = false;
        public int CritEnergyRestore = 20;
        public bool CritDecreasesOnDefensive = false;
        public int CritDecreaseAmount = 10;
        public bool AttackDuringStunWithCritRefillsEnergy = false;
        public int StunAttackEnergyRefill = 6;

        // Enemy
        public int EnemyStartingEnergy = 50;
        public int EnemyAttackDamage = 5;
        public float TelegraphDurationMin = 0.5f;
        public float TelegraphDurationMax = 1.0f;
        public float EnemyAttackCooldownMin = 1.5f;
        public float EnemyAttackCooldownMax = 3.0f;
        public float ParryStunDuration = 1.0f;

        // Enemy Combos
        public bool CombosEnabled = false;
        public EnemyComboData[] Combos = new EnemyComboData[0];
    }
}
