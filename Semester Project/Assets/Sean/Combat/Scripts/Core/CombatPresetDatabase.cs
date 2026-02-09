namespace Sean.Combat
{
    public static class CombatPresetDatabase
    {
        private static readonly CombatPreset[] _presets = new CombatPreset[]
        {
            // System 1: Parry Refill, Attack Cost
            new CombatPreset
            {
                Name = "System 1: Parry Refill + Attack Cost",
                PlayerStartingEnergy = 20,
                AttackDamage = 4,
                AttackEnergyCost = 2,
                AttackCooldown = 0.5f,
                ParryWindowDuration = 0.3f,
                ParryRefillsEnergy = true,
                ParryEnergyRefill = 7,
                DodgeEnabled = true,
                DodgeEnergyCost = 2,
                DodgeDuration = 0.4f,
                BlockEnabled = false,
                PostureEnabled = false,
                CritMeterEnabled = false,
                EnemyStartingEnergy = 50,
                EnemyAttackDamage = 5,
                TelegraphDurationMin = 0.5f,
                TelegraphDurationMax = 1.0f,
                EnemyAttackCooldownMin = 1.5f,
                EnemyAttackCooldownMax = 3.0f,
                ParryStunDuration = 1.0f,
                CombosEnabled = false,
            },

            // System 2: Parry Refill Only
            new CombatPreset
            {
                Name = "System 2: Parry Refill Only",
                PlayerStartingEnergy = 20,
                AttackDamage = 4,
                AttackEnergyCost = 0,
                AttackCooldown = 0.5f,
                ParryWindowDuration = 0.3f,
                ParryRefillsEnergy = true,
                ParryEnergyRefill = 7,
                DodgeEnabled = true,
                DodgeEnergyCost = 2,
                DodgeDuration = 0.4f,
                BlockEnabled = false,
                PostureEnabled = false,
                CritMeterEnabled = false,
                EnemyStartingEnergy = 50,
                EnemyAttackDamage = 5,
                ParryStunDuration = 1.0f,
                CombosEnabled = false,
            },

            // System 3: Stun System + Parry Energy Refill
            new CombatPreset
            {
                Name = "System 3: Stun + Parry Refill",
                PlayerStartingEnergy = 20,
                AttackDamage = 4,
                AttackEnergyCost = 2,
                AttackCooldown = 0.5f,
                ParryWindowDuration = 0.3f,
                ParryRefillsEnergy = true,
                ParryEnergyRefill = 7,
                DodgeEnabled = true,
                DodgeEnergyCost = 2,
                DodgeDuration = 0.4f,
                BlockEnabled = false,
                PostureEnabled = true,
                MaxPosture = 100,
                PostureDamageOnParry = 30,
                PostureDamageOnAttack = 10,
                PostureStunDuration = 5f,
                CritMeterEnabled = false,
                EnemyStartingEnergy = 50,
                EnemyAttackDamage = 5,
                ParryStunDuration = 1.0f,
                CombosEnabled = false,
            },

            // System 4: Stun System + Crit Energy Refill
            new CombatPreset
            {
                Name = "System 4: Stun + Crit Refill",
                PlayerStartingEnergy = 20,
                AttackDamage = 4,
                AttackEnergyCost = 0,
                AttackCooldown = 0.5f,
                ParryWindowDuration = 0.3f,
                ParryRefillsEnergy = false,
                ParryEnergyRefill = 0,
                DodgeEnabled = true,
                DodgeEnergyCost = 2,
                DodgeDuration = 0.4f,
                BlockEnabled = false,
                PostureEnabled = true,
                MaxPosture = 100,
                PostureDamageOnParry = 30,
                PostureDamageOnAttack = 10,
                PostureStunDuration = 5f,
                CritMeterEnabled = false,
                CritRestoresEnergy = true,
                CritEnergyRestore = 20,
                EnemyStartingEnergy = 50,
                EnemyAttackDamage = 5,
                ParryStunDuration = 1.0f,
                CombosEnabled = false,
            },

            // System 5: Crit System + Parry Energy Refill
            new CombatPreset
            {
                Name = "System 5: Crit + Parry Refill",
                PlayerStartingEnergy = 20,
                AttackDamage = 4,
                AttackEnergyCost = 2,
                AttackCooldown = 0.5f,
                ParryWindowDuration = 0.3f,
                ParryRefillsEnergy = true,
                ParryEnergyRefill = 7,
                DodgeEnabled = true,
                DodgeEnergyCost = 2,
                DodgeDuration = 0.4f,
                BlockEnabled = false,
                PostureEnabled = false,
                CritMeterEnabled = true,
                CritAutoActivate = true,
                CritMeterMax = 100,
                CritMeterGainOnParry = 25,
                CritMeterGainOnAttack = 10,
                CritDamage = 20,
                CritRestoresEnergy = false,
                CritDecreasesOnDefensive = false,
                EnemyStartingEnergy = 50,
                EnemyAttackDamage = 5,
                ParryStunDuration = 1.0f,
                CombosEnabled = false,
            },

            // System 6: Crit System + Crit Energy Refill
            new CombatPreset
            {
                Name = "System 6: Crit + Crit Refill",
                PlayerStartingEnergy = 20,
                AttackDamage = 4,
                AttackEnergyCost = 2,
                AttackCooldown = 0.5f,
                ParryWindowDuration = 0.3f,
                ParryRefillsEnergy = false,
                ParryEnergyRefill = 0,
                DodgeEnabled = true,
                DodgeEnergyCost = 2,
                DodgeDuration = 0.4f,
                BlockEnabled = false,
                PostureEnabled = false,
                CritMeterEnabled = true,
                CritAutoActivate = true,
                CritMeterMax = 100,
                CritMeterGainOnParry = 25,
                CritMeterGainOnAttack = 10,
                CritDamage = 20,
                CritRestoresEnergy = true,
                CritEnergyRestore = 20,
                CritDecreasesOnDefensive = false,
                EnemyStartingEnergy = 50,
                EnemyAttackDamage = 5,
                ParryStunDuration = 1.0f,
                CombosEnabled = false,
            },

            // System 7: Full System
            new CombatPreset
            {
                Name = "System 7: Full Combo",
                PlayerStartingEnergy = 20,
                AttackDamage = 4,
                AttackEnergyCost = 2,
                AttackCooldown = 0.5f,
                ParryWindowDuration = 0.3f,
                ParryRefillsEnergy = true,
                ParryEnergyRefill = 7,
                DodgeEnabled = true,
                DodgeEnergyCost = 2,
                DodgeDuration = 0.4f,
                BlockEnabled = false,
                PostureEnabled = true,
                MaxPosture = 100,
                PostureDamageOnParry = 30,
                PostureDamageOnAttack = 10,
                PostureStunDuration = 5f,
                CritMeterEnabled = true,
                CritAutoActivate = false,
                CritMeterMax = 100,
                CritMeterGainOnParry = 25,
                CritMeterGainOnAttack = 10,
                CritDamage = 20,
                CritRestoresEnergy = false,
                CritDecreasesOnDefensive = true,
                CritDecreaseAmount = 10,
                AttackDuringStunWithCritRefillsEnergy = true,
                StunAttackEnergyRefill = 7,
                EnemyStartingEnergy = 50,
                EnemyAttackDamage = 5,
                ParryStunDuration = 1.0f,
                CombosEnabled = false,
            },
        };

        public static int PresetCount => _presets.Length;

        public static CombatPreset GetPreset(int index)
        {
            if (index < 0 || index >= _presets.Length)
                return _presets[0];
            return _presets[index];
        }

        public static string[] GetPresetNames()
        {
            var names = new string[_presets.Length];
            for (int i = 0; i < _presets.Length; i++)
                names[i] = _presets[i].Name;
            return names;
        }
    }
}
