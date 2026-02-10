using System.Collections;
using UnityEngine;

namespace Sean.Combat
{
    public class PlayerCombatController : MonoBehaviour
    {
        [SerializeField] private PlayerEnergy energy;
        [SerializeField] private FighterVisual visual;
        [SerializeField] private PlayerCritMeter critMeter;

        private CombatActions _input;
        private PlayerState _state = PlayerState.Idle;
        private AttackDirection _parryDirection;
        private bool _combatActive;
        private float _attackCooldownTimer;
        private bool _parryButtonHeld;
        private bool _enemyStunned;

        private CombatRuntimeConfig Config => CombatRuntimeConfig.Instance;

        private enum PlayerState { Idle, Parrying, Blocking, Dodging, Punching }

        private void OnEnable()
        {
            _input = new CombatActions();
            _input.Combat.Enable();

            _input.Combat.ParryUp.performed += _ => TryParry(AttackDirection.Up);
            _input.Combat.ParryDown.performed += _ => TryParry(AttackDirection.Down);
            _input.Combat.ParryLeft.performed += _ => TryParry(AttackDirection.Left);
            _input.Combat.ParryRight.performed += _ => TryParry(AttackDirection.Right);

            _input.Combat.ParryUp.canceled += _ => OnParryReleased();
            _input.Combat.ParryDown.canceled += _ => OnParryReleased();
            _input.Combat.ParryLeft.canceled += _ => OnParryReleased();
            _input.Combat.ParryRight.canceled += _ => OnParryReleased();

            _input.Combat.Punch.performed += _ => TryPunch();
            _input.Combat.Dodge.performed += _ => TryDodge();
            _input.Combat.Crit.performed += _ => TryActivateCrit();

            CombatEvents.OnEnemyAttackLand += HandleEnemyAttack;
            CombatEvents.OnCombatStarted += OnCombatStarted;
            CombatEvents.OnFighterDefeated += OnFighterDefeated;
            CombatEvents.OnEnemyStunned += OnEnemyStunned;
            CombatEvents.OnEnemyPostureBroken += OnEnemyPostureBroken;
        }

        private void OnDisable()
        {
            CombatEvents.OnEnemyAttackLand -= HandleEnemyAttack;
            CombatEvents.OnCombatStarted -= OnCombatStarted;
            CombatEvents.OnFighterDefeated -= OnFighterDefeated;
            CombatEvents.OnEnemyStunned -= OnEnemyStunned;
            CombatEvents.OnEnemyPostureBroken -= OnEnemyPostureBroken;

            _input.Combat.Disable();
            _input.Dispose();
        }

        private void Update()
        {
            if (_attackCooldownTimer > 0f)
                _attackCooldownTimer -= Time.deltaTime;
        }

        private void OnCombatStarted()
        {
            _combatActive = true;
            _state = PlayerState.Idle;
            _attackCooldownTimer = 0f;
            _enemyStunned = false;
        }

        private void OnFighterDefeated(FighterType type)
        {
            _combatActive = false;
            StopAllCoroutines();
            _state = PlayerState.Idle;
        }

        private void OnEnemyStunned(float duration)
        {
            StartCoroutine(EnemyStunTracker(duration));
        }

        private void OnEnemyPostureBroken(float stunDuration)
        {
            // Posture break also stuns the enemy
            StartCoroutine(EnemyStunTracker(stunDuration));
        }

        private IEnumerator EnemyStunTracker(float duration)
        {
            _enemyStunned = true;
            yield return new WaitForSeconds(duration);
            _enemyStunned = false;
        }

        private void TryParry(AttackDirection direction)
        {
            if (!_combatActive || _state != PlayerState.Idle) return;

            _state = PlayerState.Parrying;
            _parryDirection = direction;
            _parryButtonHeld = true;
            visual.FlashColor(Color.blue, Config.ParryWindowDuration);
            StartCoroutine(ParryCoroutine());
        }

        private void OnParryReleased()
        {
            _parryButtonHeld = false;
            if (_state == PlayerState.Blocking)
            {
                _state = PlayerState.Idle;
            }
        }

        private IEnumerator ParryCoroutine()
        {
            yield return new WaitForSeconds(Config.ParryWindowDuration);

            if (_state == PlayerState.Parrying)
            {
                // If block is enabled and button still held, transition to block
                if (Config.BlockEnabled && _parryButtonHeld)
                {
                    _state = PlayerState.Blocking;
                    visual.FlashColor(new Color(0.5f, 0.5f, 1f), 0.1f); // light blue for block
                    CombatEvents.RaiseBlockActivated();
                    CombatEvents.RaiseNotification("BLOCK!", transform.position);
                    // Block stays active until button released (handled in OnParryReleased)
                }
                else
                {
                    _state = PlayerState.Idle;
                }
            }
        }

        private void TryDodge()
        {
            if (!_combatActive || _state != PlayerState.Idle) return;
            if (!Config.DodgeEnabled) return;
            if (!energy.HasEnergy(Config.DodgeEnergyCost)) return;

            _state = PlayerState.Dodging;
            energy.ModifyEnergy(-Config.DodgeEnergyCost);
            visual.FlashColor(Color.green, Config.DodgeDuration);

            if (critMeter != null)
                critMeter.OnBlockPerformed(); // dodge also counts as defensive

            StartCoroutine(DodgeCoroutine());
        }

        private IEnumerator DodgeCoroutine()
        {
            yield return new WaitForSeconds(Config.DodgeDuration);
            if (_state == PlayerState.Dodging)
                _state = PlayerState.Idle;
        }

        private void TryPunch()
        {
            if (!_combatActive || _state != PlayerState.Idle) return;
            if (_attackCooldownTimer > 0f) return;

            int cost = Config.AttackEnergyCost;
            if (cost > 0 && !energy.HasEnergy(cost)) return;

            _state = PlayerState.Punching;
            if (cost > 0)
                energy.ModifyEnergy(-cost);

            // Check for crit
            bool isCrit = false;
            int damage = Config.AttackDamage;

            if (Config.CritMeterEnabled && critMeter != null && critMeter.CritReady && Config.CritAutoActivate)
            {
                isCrit = true;
                damage = Config.CritDamage;
                critMeter.ConsumeCrit();

                if (Config.CritRestoresEnergy)
                    energy.ModifyEnergy(Config.CritEnergyRestore);
            }

            // Check for stun + crit energy refill
            if (_enemyStunned && Config.AttackDuringStunWithCritRefillsEnergy &&
                Config.CritMeterEnabled && critMeter != null && critMeter.CritReady && !Config.CritAutoActivate)
            {
                energy.ModifyEnergy(Config.StunAttackEnergyRefill);
            }

            visual.FlashColor(isCrit ? new Color(1f, 0.5f, 0f) : Color.yellow, 0.3f);
            CombatEvents.RaisePlayerAttack(damage);
            CombatEvents.RaiseNotification(isCrit ? $"CRIT! -{damage}" : $"PUNCH! -{damage}", transform.position);

            _attackCooldownTimer = Config.AttackCooldown;
            StartCoroutine(PunchCoroutine());
        }

        // Manual crit activation (when CritAutoActivate is false) — called via dedicated key
        public void TryActivateCrit()
        {
            if (!_combatActive || !Config.CritMeterEnabled) return;
            if (critMeter == null || !critMeter.CritReady) return;
            if (Config.CritAutoActivate) return; // Auto mode handles it in TryPunch

            // If enemy is stunned and the special rule is active, enable energy refill on attacks
            // The crit itself doesn't attack — it activates a mode
            critMeter.ConsumeCrit();

            if (Config.CritRestoresEnergy)
                energy.ModifyEnergy(Config.CritEnergyRestore);

            CombatEvents.RaiseNotification("CRIT ACTIVATED!", transform.position);
        }

        private IEnumerator PunchCoroutine()
        {
            yield return new WaitForSeconds(0.3f);
            if (_state == PlayerState.Punching)
                _state = PlayerState.Idle;
        }

        private void HandleEnemyAttack(AttackDirection direction)
        {
            if (!_combatActive) return;

            if (_state == PlayerState.Parrying && _parryDirection == direction)
            {
                // Successful parry
                if (Config.ParryRefillsEnergy)
                    energy.ModifyEnergy(Config.ParryEnergyRefill);

                if (Config.ParryDrainsEnemyEnergy)
                    CombatEvents.RaiseParryEnemyEnergyDrain(Config.ParryEnemyEnergyDrain);

                CombatEvents.RaiseDefenseResult(CombatResult.Parry, direction);
                CombatEvents.RaiseNotification("PARRY!", transform.position);
            }
            else if (_state == PlayerState.Blocking)
            {
                // Block: reduce damage
                int reducedDamage = Mathf.Max(1, Mathf.RoundToInt(Config.EnemyAttackDamage * (1f - Config.BlockDamageReduction)));
                energy.ModifyEnergy(-reducedDamage);
                visual.FlashColor(new Color(0.5f, 0.5f, 1f), Config.HitFlashDuration);
                CombatEvents.RaiseNotification($"BLOCK! -{reducedDamage}", transform.position);

                if (critMeter != null)
                    critMeter.OnBlockPerformed();
            }
            else if (_state == PlayerState.Dodging)
            {
                CombatEvents.RaiseDefenseResult(CombatResult.Dodge, direction);
                CombatEvents.RaiseNotification("DODGE!", transform.position);
            }
            else
            {
                // Hit
                energy.ModifyEnergy(-Config.EnemyAttackDamage);
                visual.FlashColor(Color.red, Config.HitFlashDuration);
                CombatEvents.RaiseDefenseResult(CombatResult.Hit, direction);
                CombatEvents.RaiseNotification("HIT!", transform.position);
            }
        }
    }
}
