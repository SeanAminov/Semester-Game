using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sean.Combat
{
    public class EnemyCombatController : MonoBehaviour
    {
        [SerializeField] private EnemyEnergy energy;
        [SerializeField] private FighterVisual visual;

        private EnemyState _state = EnemyState.Idle;
        private bool _combatActive;

        private CombatRuntimeConfig Config => CombatRuntimeConfig.Instance;

        private enum EnemyState { Idle, Telegraphing, Attacking, Stunned, Defeated }

        private void OnEnable()
        {
            CombatEvents.OnPlayerDefenseResult += HandleDefenseResult;
            CombatEvents.OnPlayerAttackEnemy += HandlePlayerAttack;
            CombatEvents.OnCombatStarted += OnCombatStarted;
            CombatEvents.OnFighterDefeated += OnFighterDefeated;
            CombatEvents.OnEnemyPostureBroken += HandlePostureBroken;
        }

        private void OnDisable()
        {
            CombatEvents.OnPlayerDefenseResult -= HandleDefenseResult;
            CombatEvents.OnPlayerAttackEnemy -= HandlePlayerAttack;
            CombatEvents.OnCombatStarted -= OnCombatStarted;
            CombatEvents.OnFighterDefeated -= OnFighterDefeated;
            CombatEvents.OnEnemyPostureBroken -= HandlePostureBroken;
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
            yield return new WaitForSeconds(1.0f);

            while (_combatActive)
            {
                float cooldown = Random.Range(Config.EnemyAttackCooldownMin, Config.EnemyAttackCooldownMax);
                yield return new WaitForSeconds(cooldown);

                if (!_combatActive || _state != EnemyState.Idle) continue;

                // Decide: single attack or combo
                if (Config.CombosEnabled && TryPickCombo(out EnemyComboData combo))
                {
                    yield return ExecuteCombo(combo);
                }
                else
                {
                    yield return ExecuteSingleAttack();
                }
            }
        }

        private bool TryPickCombo(out EnemyComboData combo)
        {
            combo = null;
            if (Config.Combos == null || Config.Combos.Length == 0) return false;

            // Collect enabled combos
            var enabledCombos = new List<EnemyComboData>();
            for (int i = 0; i < Config.Combos.Length; i++)
            {
                if (Config.Combos[i] != null && Config.Combos[i].Enabled &&
                    Config.Combos[i].Attacks != null && Config.Combos[i].Attacks.Length > 0)
                {
                    enabledCombos.Add(Config.Combos[i]);
                }
            }

            if (enabledCombos.Count == 0) return false;

            // 40% chance to use a combo when combos are enabled
            if (Random.value > 0.4f) return false;

            combo = enabledCombos[Random.Range(0, enabledCombos.Count)];
            return true;
        }

        private IEnumerator ExecuteSingleAttack()
        {
            AttackDirection direction = (AttackDirection)Random.Range(0, 4);
            float telegraphDuration = Random.Range(Config.TelegraphDurationMin, Config.TelegraphDurationMax);

            _state = EnemyState.Telegraphing;
            CombatEvents.RaiseTelegraph(direction, telegraphDuration);
            yield return new WaitForSeconds(telegraphDuration);

            if (!_combatActive) yield break;

            _state = EnemyState.Attacking;
            CombatEvents.RaiseAttackLand(direction);

            yield return new WaitForSeconds(0.2f);

            if (_state == EnemyState.Attacking)
                _state = EnemyState.Idle;
        }

        private IEnumerator ExecuteCombo(EnemyComboData combo)
        {
            for (int i = 0; i < combo.Attacks.Length; i++)
            {
                if (!_combatActive || _state == EnemyState.Stunned || _state == EnemyState.Defeated)
                    yield break;

                var attack = combo.Attacks[i];

                _state = EnemyState.Telegraphing;
                CombatEvents.RaiseTelegraph(attack.Direction, attack.TelegraphDuration);
                yield return new WaitForSeconds(attack.TelegraphDuration);

                if (!_combatActive || _state == EnemyState.Stunned) yield break;

                _state = EnemyState.Attacking;
                CombatEvents.RaiseAttackLand(attack.Direction);

                yield return new WaitForSeconds(0.15f);

                if (!_combatActive) yield break;

                // Interval before next combo hit
                if (i < combo.Attacks.Length - 1)
                {
                    _state = EnemyState.Idle;
                    yield return new WaitForSeconds(attack.IntervalAfter);
                }
            }

            if (_state == EnemyState.Attacking)
                _state = EnemyState.Idle;
        }

        private void HandleDefenseResult(CombatResult result, AttackDirection direction)
        {
            if (result == CombatResult.Parry)
            {
                StopAllCoroutines();
                StartCoroutine(StunCoroutine(Config.ParryStunDuration));
            }
        }

        private void HandlePostureBroken(float stunDuration)
        {
            StopAllCoroutines();
            StartCoroutine(StunCoroutine(stunDuration));
        }

        private IEnumerator StunCoroutine(float duration)
        {
            _state = EnemyState.Stunned;
            CombatEvents.RaiseEnemyStunned(duration);
            visual.FlashColor(Color.white, duration);
            yield return new WaitForSeconds(duration);

            if (!_combatActive) yield break;

            _state = EnemyState.Idle;
            StartCoroutine(AttackLoop());
        }

        private void HandlePlayerAttack(int damage)
        {
            if (!_combatActive || _state == EnemyState.Defeated) return;

            energy.ModifyEnergy(-damage);
            visual.FlashColor(Color.red, Config.HitFlashDuration);
            CombatEvents.RaiseNotification($"-{damage}", transform.position);
        }
    }
}
