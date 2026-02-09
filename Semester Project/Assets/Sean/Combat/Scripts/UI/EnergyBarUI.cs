using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace Sean.Combat
{
    public class EnergyBarUI : MonoBehaviour
    {
        [SerializeField] private FighterType trackedFighter;
        [SerializeField] private Image fillImage;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private float lerpSpeed = 5f;

        private float _targetFill = 1f;

        private void OnEnable()
        {
            CombatEvents.OnEnergyChanged += HandleEnergyChanged;
        }

        private void OnDisable()
        {
            CombatEvents.OnEnergyChanged -= HandleEnergyChanged;
        }

        private void Update()
        {
            if (fillImage != null)
            {
                fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, _targetFill, Time.deltaTime * lerpSpeed);
            }
        }

        private void HandleEnergyChanged(FighterType type, int current, int max)
        {
            if (type != trackedFighter) return;

            _targetFill = max > 0 ? (float)current / max : 0f;

            if (valueText != null)
            {
                valueText.text = $"{current} / {max}";
            }
        }
    }
}
