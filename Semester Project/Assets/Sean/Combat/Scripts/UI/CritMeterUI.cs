using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Sean.Combat
{
    public class CritMeterUI : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private GameObject meterRoot;

        private float _displayedFill;

        private void OnEnable()
        {
            CombatEvents.OnCritMeterChanged += HandleCritMeterChanged;
            CombatEvents.OnCombatStarted += HandleCombatStarted;
        }

        private void OnDisable()
        {
            CombatEvents.OnCritMeterChanged -= HandleCritMeterChanged;
            CombatEvents.OnCombatStarted -= HandleCombatStarted;
        }

        private void HandleCombatStarted()
        {
            var config = CombatRuntimeConfig.Instance;
            if (meterRoot != null)
                meterRoot.SetActive(config != null && config.CritMeterEnabled);
        }

        private void HandleCritMeterChanged(int current, int max)
        {
            if (max <= 0) return;
            _displayedFill = (float)current / max;
            if (fillImage != null)
            {
                fillImage.fillAmount = _displayedFill;
                fillImage.color = current >= max ? new Color(1f, 0.5f, 0f) : new Color(1f, 0.85f, 0.2f);
            }
            if (valueText != null)
            {
                valueText.text = current >= max ? "CRIT READY!" : $"{current} / {max}";
            }
        }

        private void Update()
        {
            if (fillImage != null)
                fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, _displayedFill, Time.deltaTime * 8f);
        }
    }
}
