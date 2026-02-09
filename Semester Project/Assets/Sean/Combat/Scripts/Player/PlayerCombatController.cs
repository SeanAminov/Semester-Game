using System.Collections;
using UnityEngine;

namespace Sean.Combat
{
    public class PlayerCombatController : MonoBehaviour
    {
        [SerializeField] private PlayerEnergy energy;
        [SerializeField] private FighterVisual visual;

        [Header("--- Timing (seconds) ---")]
        [SerializeField] private float parryDuration = 0.3f;
        [SerializeField] private float dodgeDuration = 0.4f;
        [SerializeField] private float punchDuration = 0.3f;

        [Header("--- Energy Costs ---")]
        [SerializeField] private int punchEnergyCost = 3;
        [SerializeField] private int dodgeEnergyCost = 2;
        [SerializeField] private int parryEnergyGain = 5;
        [SerializeField] private int playerDamageTaken = 5;

        [Header("--- Damage ---")]
        [SerializeField] private int punchDamage = 3;

        [Header("--- Colors ---")]
        [SerializeField] private Color parryColor = Color.blue;
        [SerializeField] private Color dodgeColor = Color.green;
        [SerializeField] private Color punchColor = Color.yellow;
        [SerializeField] private Color hitColor = Color.red;
        [SerializeField] private float hitFlashDuration = 0.15f;

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
            visual.FlashColor(parryColor, parryDuration);
            StartCoroutine(ParryCoroutine());
        }

        private IEnumerator ParryCoroutine()
        {
            yield return new WaitForSeconds(parryDuration);
            if (_state == PlayerState.Parrying)
                _state = PlayerState.Idle;
        }

        private void TryDodge()
        {
            if (!_combatActive || _state != PlayerState.Idle) return;
            if (!energy.HasEnergy(dodgeEnergyCost)) return;

            _state = PlayerState.Dodging;
            energy.ModifyEnergy(-dodgeEnergyCost);
            visual.FlashColor(dodgeColor, dodgeDuration);
            StartCoroutine(DodgeCoroutine());
        }

        private IEnumerator DodgeCoroutine()
        {
            yield return new WaitForSeconds(dodgeDuration);
            if (_state == PlayerState.Dodging)
                _state = PlayerState.Idle;
        }

        private void TryPunch()
        {
            if (!_combatActive || _state != PlayerState.Idle) return;
            if (!energy.HasEnergy(punchEnergyCost)) return;

            _state = PlayerState.Punching;
            energy.ModifyEnergy(-punchEnergyCost);
            visual.FlashColor(punchColor, punchDuration);
            CombatEvents.RaisePlayerAttack(punchDamage);
            CombatEvents.RaiseNotification("PUNCH!", transform.position);
            StartCoroutine(PunchCoroutine());
        }

        private IEnumerator PunchCoroutine()
        {
            yield return new WaitForSeconds(punchDuration);
            if (_state == PlayerState.Punching)
                _state = PlayerState.Idle;
        }

        private void HandleEnemyAttack(AttackDirection direction)
        {
            if (!_combatActive) return;

            if (_state == PlayerState.Parrying && _parryDirection == direction)
            {
                energy.ModifyEnergy(parryEnergyGain);
                CombatEvents.RaiseDefenseResult(CombatResult.Parry, direction);
                CombatEvents.RaiseNotification("PARRY!", transform.position);
            }
            else if (_state == PlayerState.Dodging)
            {
                CombatEvents.RaiseDefenseResult(CombatResult.Dodge, direction);
                CombatEvents.RaiseNotification("DODGE!", transform.position);
            }
            else
            {
                energy.ModifyEnergy(-playerDamageTaken);
                visual.FlashColor(hitColor, hitFlashDuration);
                CombatEvents.RaiseDefenseResult(CombatResult.Hit, direction);
                CombatEvents.RaiseNotification("HIT!", transform.position);
            }
        }
    }
}
