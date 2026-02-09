using System.Collections;
using UnityEngine;

namespace Sean.Combat
{
    public class EnemyAttackTelegraph : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private DirectionIndicator directionIndicator;

        private Color _originalColor;
        private Coroutine _telegraphCoroutine;

        private void Awake()
        {
            _originalColor = spriteRenderer.color;
        }

        private void OnEnable()
        {
            CombatEvents.OnEnemyTelegraph += HandleTelegraph;
            CombatEvents.OnEnemyAttackLand += HandleAttackLand;
        }

        private void OnDisable()
        {
            CombatEvents.OnEnemyTelegraph -= HandleTelegraph;
            CombatEvents.OnEnemyAttackLand -= HandleAttackLand;
        }

        private void HandleTelegraph(AttackDirection direction, float duration)
        {
            if (_telegraphCoroutine != null)
                StopCoroutine(_telegraphCoroutine);

            _telegraphCoroutine = StartCoroutine(TelegraphCoroutine(direction, duration));
        }

        private IEnumerator TelegraphCoroutine(AttackDirection direction, float duration)
        {
            // Show direction arrow
            directionIndicator.ShowDirection(direction);

            // Fade from original color to red over the telegraph duration
            float elapsed = 0f;
            Color startColor = _originalColor;
            Color endColor = Color.red;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                spriteRenderer.color = Color.Lerp(startColor, endColor, t);
                yield return null;
            }

            spriteRenderer.color = endColor;
        }

        private void HandleAttackLand(AttackDirection direction)
        {
            // Reset visuals after attack
            if (_telegraphCoroutine != null)
            {
                StopCoroutine(_telegraphCoroutine);
                _telegraphCoroutine = null;
            }

            directionIndicator.HideAll();
            spriteRenderer.color = _originalColor;
        }
    }
}
