using System.Collections;
using UnityEngine;

namespace Sean.Combat
{
    public class FighterVisual : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        private Color _originalColor;
        private Coroutine _flashCoroutine;

        private void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            _originalColor = spriteRenderer.color;
        }

        public void FlashColor(Color color, float duration)
        {
            if (_flashCoroutine != null)
                StopCoroutine(_flashCoroutine);

            _flashCoroutine = StartCoroutine(FlashCoroutine(color, duration));
        }

        private IEnumerator FlashCoroutine(Color color, float duration)
        {
            spriteRenderer.color = color;
            yield return new WaitForSeconds(duration);
            spriteRenderer.color = _originalColor;
            _flashCoroutine = null;
        }

        public void ResetColor()
        {
            if (_flashCoroutine != null)
            {
                StopCoroutine(_flashCoroutine);
                _flashCoroutine = null;
            }
            spriteRenderer.color = _originalColor;
        }
    }
}
