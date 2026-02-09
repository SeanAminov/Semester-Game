using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Sean.Combat
{
    public class PostureMeterUI : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private GameObject meterRoot;

        private float _displayedFill = 1f;

        private void OnEnable()
        {
            CombatEvents.OnPostureChanged += HandlePostureChanged;
            CombatEvents.OnCombatStarted += HandleCombatStarted;
        }

        private void OnDisable()
        {
            CombatEvents.OnPostureChanged -= HandlePostureChanged;
            CombatEvents.OnCombatStarted -= HandleCombatStarted;
        }

        private void HandleCombatStarted()
        {
            var config = CombatRuntimeConfig.Instance;
            if (meterRoot != null)
                meterRoot.SetActive(config != null && config.PostureEnabled);
        }

        private void HandlePostureChanged(int current, int max)
        {
            if (max <= 0) return;
            _displayedFill = (float)current / max;
            if (fillImage != null)
                fillImage.fillAmount = _displayedFill;
            if (valueText != null)
                valueText.text = $"{current} / {max}";
        }

        private void Update()
        {
            if (fillImage != null)
                fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, _displayedFill, Time.deltaTime * 8f);
        }
    }
}
