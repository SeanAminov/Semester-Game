using System.Collections;
using UnityEngine;

namespace Sean.Combat
{
    public class PlayerCombatController : MonoBehaviour
    {
        [SerializeField] private CombatConfigSO config;
        [SerializeField] private PlayerEnergy energy;
        [SerializeField] private FighterVisual visual;

        private CombatActions _input;
        private PlayerState _state = PlayerState.Idle;
        private AttackDirection _parryDirection;
        private bool _combatActive;

        private enum PlayerState { Idle, Parrying, Dodging, Punching }

        private void OnEnable()
        {
            _input = new CombatActions();
            _input.Combat.Enable();

            _input.Combat.ParryUp.performed += _ => TryParry(AttackDirection.Up);
            _input.Combat.ParryDown.performed += _ => TryParry(AttackDirection.Down);
            _input.Combat.ParryLeft.performed += _ => TryParry(AttackDirection.Left);
            _input.Combat.ParryRight.performed += _ => TryParry(AttackDirection.Right);
            _input.Combat.Punch.performed += _ => TryPunch();
            _input.Combat.Dodge.performed += _ => TryDodge();

            CombatEvents.OnEnemyAttackLand += HandleEnemyAttack;
            CombatEvents.OnCombatStarted += OnCombatStarted;
            CombatEvents.OnFighterDefeated += OnFighterDefeated;
        }

        private void OnDisable()
        {
            CombatEvents.OnEnemyAttackLand -= HandleEnemyAttack;
            CombatEvents.OnCombatStarted -= OnCombatStarted;
            CombatEvents.OnFighterDefeated -= OnFighterDefeated;

            _input.Combat.Disable();
            _input.Dispose();
        }

        private void OnCombatStarted()
        {
            _combatActive = true;
            _state = PlayerState.Idle;
        }

        private void OnFighterDefeated(FighterType type)
        {
            _combatActive = false;
            StopAllCoroutines();
            _state = PlayerState.Idle;
        }

        private void TryParry(AttackDirection direction)
        {
            if (!_combatActive || _state != PlayerState.Idle) return;

            _state = PlayerState.Parrying;
            _parryDirection = direction;
            visual.FlashColor(config.parryColor, config.parryDuration);
            StartCoroutine(ParryCoroutine());
        }

        private IEnumerator ParryCoroutine()
        {
            yield return new WaitForSeconds(config.parryDuration);
            if (_state == PlayerState.Parrying)
                _state = PlayerState.Idle;
        }

        private void TryDodge()
        {
            if (!_combatActive || _state != PlayerState.Idle) return;
            if (!energy.HasEnergy(config.dodgeEnergyCost)) return;

            _state = PlayerState.Dodging;
            energy.ModifyEnergy(-config.dodgeEnergyCost);
            visual.FlashColor(config.dodgeColor, config.dodgeDuration);
            StartCoroutine(DodgeCoroutine());
        }

        private IEnumerator DodgeCoroutine()
        {
            yield return new WaitForSeconds(config.dodgeDuration);
            if (_state == PlayerState.Dodging)
                _state = PlayerState.Idle;
        }

        private void TryPunch()
        {
            if (!_combatActive || _state != PlayerState.Idle) return;
            if (!energy.HasEnergy(config.punchEnergyCost)) return;

            _state = PlayerState.Punching;
            energy.ModifyEnergy(-config.punchEnergyCost);
            visual.FlashColor(config.punchColor, config.punchDuration);
            CombatEvents.RaisePlayerAttack(config.punchDamage);
            CombatEvents.RaiseNotification("PUNCH!", transform.position);
            StartCoroutine(PunchCoroutine());
        }

        private IEnumerator PunchCoroutine()
        {
            yield return new WaitForSeconds(config.punchDuration);
            if (_state == PlayerState.Punching)
                _state = PlayerState.Idle;
        }

        private void HandleEnemyAttack(AttackDirection direction)
        {
            if (!_combatActive) return;

            if (_state == PlayerState.Parrying && _parryDirection == direction)
            {
                // Successful parry
                energy.ModifyEnergy(config.parryEnergyGain);
                CombatEvents.RaiseDefenseResult(CombatResult.Parry, direction);
                CombatEvents.RaiseNotification("PARRY!", transform.position);
            }
            else if (_state == PlayerState.Dodging)
            {
                // Successful dodge
                CombatEvents.RaiseDefenseResult(CombatResult.Dodge, direction);
                CombatEvents.RaiseNotification("DODGE!", transform.position);
            }
            else
            {
                // Player gets hit
                energy.ModifyEnergy(-config.playerDamageTaken);
                visual.FlashColor(config.hitColor, config.hitFlashDuration);
                CombatEvents.RaiseDefenseResult(CombatResult.Hit, direction);
                CombatEvents.RaiseNotification("HIT!", transform.position);
            }
        }
    }
}
