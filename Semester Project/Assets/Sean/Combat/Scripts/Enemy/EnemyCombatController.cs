using System.Collections;
using UnityEngine;

namespace Sean.Combat
{
    public class EnemyCombatController : MonoBehaviour
    {
        [SerializeField] private EnemyProfileSO profile;
        [SerializeField] private CombatConfigSO combatConfig;
        [SerializeField] private EnemyEnergy energy;
        [SerializeField] private FighterVisual visual;

        private EnemyState _state = EnemyState.Idle;
        private bool _combatActive;

        private enum EnemyState { Idle, Telegraphing, Attacking, Stunned, Defeated }

        private void OnEnable()
        {
            CombatEvents.OnPlayerDefenseResult += HandleDefenseResult;
            CombatEvents.OnPlayerAttackEnemy += HandlePlayerAttack;
            CombatEvents.OnCombatStarted += OnCombatStarted;
            CombatEvents.OnFighterDefeated += OnFighterDefeated;
        }

        private void OnDisable()
        {
            CombatEvents.OnPlayerDefenseResult -= HandleDefenseResult;
            CombatEvents.OnPlayerAttackEnemy -= HandlePlayerAttack;
            CombatEvents.OnCombatStarted -= OnCombatStarted;
            CombatEvents.OnFighterDefeated -= OnFighterDefeated;
        }

        private void OnCombatStarted()
        {
            _combatActive = true;
            _state = EnemyState.Idle;
            StartCoroutine(AttackLoop());
        }

        private void OnFighterDefeated(FighterType type)
        {
            _combatActive = false;
            StopAllCoroutines();
            if (type == FighterType.Enemy)
                _state = EnemyState.Defeated;
        }

        private IEnumerator AttackLoop()
        {
            // Initial delay before first attack
            yield return new WaitForSeconds(1.0f);

            while (_combatActive)
            {
                // Wait for cooldown
                float cooldown = Random.Range(profile.attackCooldownMin, profile.attackCooldownMax);
                yield return new WaitForSeconds(cooldown);

                if (!_combatActive || _state != EnemyState.Idle) continue;

                // Pick random direction
                AttackDirection direction = (AttackDirection)Random.Range(0, 4);
                float telegraphDuration = Random.Range(profile.telegraphDurationMin, profile.telegraphDurationMax);

                // Telegraph phase
                _state = EnemyState.Telegraphing;
                CombatEvents.RaiseTelegraph(direction, telegraphDuration);
                yield return new WaitForSeconds(telegraphDuration);

                if (!_combatActive) yield break;

                // Attack lands
                _state = EnemyState.Attacking;
                energy.ModifyEnergy(-profile.attackEnergyCost);
                CombatEvents.RaiseAttackLand(direction);

                // Brief vulnerability window after attack
                yield return new WaitForSeconds(combatConfig.enemyVulnerabilityWindow);

                if (_state == EnemyState.Attacking)
                    _state = EnemyState.Idle;
            }
        }

        private void HandleDefenseResult(CombatResult result, AttackDirection direction)
        {
            if (result == CombatResult.Parry)
            {
                // Enemy gets stunned on parry
                StopAllCoroutines();
                StartCoroutine(StunCoroutine(profile.parryStunDuration));
            }
        }

        private IEnumerator StunCoroutine(float duration)
        {
            _state = EnemyState.Stunned;
            CombatEvents.RaiseEnemyStunned(duration);
            visual.FlashColor(Color.white, duration); // flash white while stunned
            yield return new WaitForSeconds(duration);

            if (!_combatActive) yield break;

            _state = EnemyState.Idle;
            StartCoroutine(AttackLoop());
        }

        private void HandlePlayerAttack(int damage)
        {
            if (!_combatActive || _state == EnemyState.Defeated) return;

            energy.ModifyEnergy(-damage);
            visual.FlashColor(combatConfig.hitColor, combatConfig.hitFlashDuration);
            CombatEvents.RaiseNotification($"-{damage}", transform.position);
        }
    }
}
