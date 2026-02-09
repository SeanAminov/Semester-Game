using UnityEngine;

namespace Sean.Combat
{
    public class CombatRuntimeConfig : MonoBehaviour
    {
        public static CombatRuntimeConfig Instance { get; private set; }

        [Header("Player Core")]
        public int PlayerStartingEnergy = 20;
        public int AttackDamage = 4;
        public int AttackEnergyCost = 2;
        public float AttackCooldown = 0.5f;

        [Header("Parry")]
        public float ParryWindowDuration = 0.3f;
        public bool ParryRefillsEnergy = true;
        public int ParryEnergyRefill = 6;

        [Header("Dodge")]
        public bool DodgeEnabled = true;
        public int DodgeEnergyCost = 2;
        public float DodgeDuration = 0.4f;

        [Header("Block")]
        public bool BlockEnabled = false;
        [Range(0f, 1f)]
        public float BlockDamageReduction = 0.5f;

        [Header("Posture Meter (Enemy)")]
        public bool PostureEnabled = false;
        public int MaxPosture = 100;
        public int PostureDamageOnParry = 30;
        public int PostureDamageOnAttack = 10;
        public float PostureStunDuration = 5f;

        [Header("Crit Meter (Player)")]
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

        [Header("Enemy")]
        public int EnemyStartingEnergy = 50;
        public int EnemyAttackDamage = 5;
        public float TelegraphDurationMin = 0.5f;
        public float TelegraphDurationMax = 1.0f;
        public float EnemyAttackCooldownMin = 1.5f;
        public float EnemyAttackCooldownMax = 3.0f;
        public float ParryStunDuration = 1.0f;

        [Header("Enemy Combos")]
        public bool CombosEnabled = false;
        public EnemyComboData[] Combos = new EnemyComboData[3];

        [Header("Visual")]
        public float HitFlashDuration = 0.15f;
        public float NotificationDuration = 0.8f;
        public float NotificationFloatSpeed = 1.5f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDefaultCombos();
        }

        private void InitializeDefaultCombos()
        {
            if (Combos == null || Combos.Length == 0)
            {
                Combos = new EnemyComboData[3];
            }
            for (int i = 0; i < Combos.Length; i++)
            {
                if (Combos[i] == null)
                {
                    Combos[i] = new EnemyComboData
                    {
                        Name = $"Combo {i + 1}",
                        Enabled = false,
                        Attacks = new ComboAttack[]
                        {
                            new ComboAttack { Direction = AttackDirection.Left, TelegraphDuration = 0.5f, IntervalAfter = 0.3f },
                            new ComboAttack { Direction = AttackDirection.Right, TelegraphDuration = 0.4f, IntervalAfter = 0.3f },
                        }
                    };
                }
            }
        }

        public void LoadPreset(CombatPreset preset)
        {
            PlayerStartingEnergy = preset.PlayerStartingEnergy;
            AttackDamage = preset.AttackDamage;
            AttackEnergyCost = preset.AttackEnergyCost;
            AttackCooldown = preset.AttackCooldown;

            ParryWindowDuration = preset.ParryWindowDuration;
            ParryRefillsEnergy = preset.ParryRefillsEnergy;
            ParryEnergyRefill = preset.ParryEnergyRefill;

            DodgeEnabled = preset.DodgeEnabled;
            DodgeEnergyCost = preset.DodgeEnergyCost;
            DodgeDuration = preset.DodgeDuration;

            BlockEnabled = preset.BlockEnabled;
            BlockDamageReduction = preset.BlockDamageReduction;

            PostureEnabled = preset.PostureEnabled;
            MaxPosture = preset.MaxPosture;
            PostureDamageOnParry = preset.PostureDamageOnParry;
            PostureDamageOnAttack = preset.PostureDamageOnAttack;
            PostureStunDuration = preset.PostureStunDuration;

            CritMeterEnabled = preset.CritMeterEnabled;
            CritAutoActivate = preset.CritAutoActivate;
            CritMeterMax = preset.CritMeterMax;
            CritMeterGainOnParry = preset.CritMeterGainOnParry;
            CritMeterGainOnAttack = preset.CritMeterGainOnAttack;
            CritDamage = preset.CritDamage;
            CritRestoresEnergy = preset.CritRestoresEnergy;
            CritEnergyRestore = preset.CritEnergyRestore;
            CritDecreasesOnDefensive = preset.CritDecreasesOnDefensive;
            CritDecreaseAmount = preset.CritDecreaseAmount;
            AttackDuringStunWithCritRefillsEnergy = preset.AttackDuringStunWithCritRefillsEnergy;
            StunAttackEnergyRefill = preset.StunAttackEnergyRefill;

            EnemyStartingEnergy = preset.EnemyStartingEnergy;
            EnemyAttackDamage = preset.EnemyAttackDamage;
            TelegraphDurationMin = preset.TelegraphDurationMin;
            TelegraphDurationMax = preset.TelegraphDurationMax;
            EnemyAttackCooldownMin = preset.EnemyAttackCooldownMin;
            EnemyAttackCooldownMax = preset.EnemyAttackCooldownMax;
            ParryStunDuration = preset.ParryStunDuration;

            CombosEnabled = preset.CombosEnabled;
            if (preset.Combos != null && preset.Combos.Length > 0)
            {
                Combos = new EnemyComboData[preset.Combos.Length];
                for (int i = 0; i < preset.Combos.Length; i++)
                {
                    var src = preset.Combos[i];
                    Combos[i] = new EnemyComboData
                    {
                        Name = src.Name,
                        Enabled = src.Enabled,
                        Attacks = new ComboAttack[src.Attacks.Length]
                    };
                    for (int j = 0; j < src.Attacks.Length; j++)
                    {
                        Combos[i].Attacks[j] = new ComboAttack
                        {
                            Direction = src.Attacks[j].Direction,
                            TelegraphDuration = src.Attacks[j].TelegraphDuration,
                            IntervalAfter = src.Attacks[j].IntervalAfter
                        };
                    }
                }
            }
            else
            {
                InitializeDefaultCombos();
            }
        }

        public void ResetToDefaults()
        {
            LoadPreset(CombatPresetDatabase.GetPreset(0));
        }
    }
}
