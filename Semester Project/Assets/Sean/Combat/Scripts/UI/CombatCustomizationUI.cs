using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace Sean.Combat
{
    public class CombatCustomizationUI : MonoBehaviour
    {
        [SerializeField] private CombatMenuManager menuManager;
        [SerializeField] private Transform contentParent;
        [SerializeField] private Button playButton;

        private CombatRuntimeConfig Config => CombatRuntimeConfig.Instance;
        private readonly List<System.Action> _refreshActions = new List<System.Action>();

        private void Start()
        {
            if (playButton != null)
                playButton.onClick.AddListener(() => menuManager.ShowPlayScreen());

            BuildUI();
        }

        private void BuildUI()
        {
            if (contentParent == null || Config == null) return;

            // === PRESETS ===
            CreateSectionHeader("PRESETS");
            var presetNames = CombatPresetDatabase.GetPresetNames();
            for (int i = 0; i < presetNames.Length; i++)
            {
                int index = i;
                CreateButton(presetNames[i], () =>
                {
                    Config.LoadPreset(CombatPresetDatabase.GetPreset(index));
                    RefreshAllFields();
                });
            }

            CreateSpacer();

            // === CORE SETTINGS ===
            CreateSectionHeader("PLAYER - CORE");
            CreateIntSlider("Starting Energy", 1, 100, () => Config.PlayerStartingEnergy, v => Config.PlayerStartingEnergy = v);
            CreateIntSlider("Attack Damage", 0, 50, () => Config.AttackDamage, v => Config.AttackDamage = v);
            CreateIntSlider("Attack Energy Cost", 0, 20, () => Config.AttackEnergyCost, v => Config.AttackEnergyCost = v);
            CreateFloatSlider("Attack Cooldown (s)", 0f, 2f, () => Config.AttackCooldown, v => Config.AttackCooldown = v);

            CreateSpacer();

            // === PARRY ===
            CreateSectionHeader("PARRY");
            CreateFloatSlider("Parry Window (s)", 0.1f, 1f, () => Config.ParryWindowDuration, v => Config.ParryWindowDuration = v);
            CreateToggle("Parry Refills Energy", () => Config.ParryRefillsEnergy, v => Config.ParryRefillsEnergy = v);
            CreateIntSlider("Parry Energy Refill", 0, 20, () => Config.ParryEnergyRefill, v => Config.ParryEnergyRefill = v);

            CreateSpacer();

            // === DODGE ===
            CreateSectionHeader("DODGE");
            CreateToggle("Dodge Enabled", () => Config.DodgeEnabled, v => Config.DodgeEnabled = v);
            CreateIntSlider("Dodge Energy Cost", 0, 10, () => Config.DodgeEnergyCost, v => Config.DodgeEnergyCost = v);
            CreateFloatSlider("Dodge Duration (s)", 0.1f, 1f, () => Config.DodgeDuration, v => Config.DodgeDuration = v);

            CreateSpacer();

            // === BLOCK ===
            CreateSectionHeader("BLOCK (HOLD PARRY)");
            CreateToggle("Block Enabled", () => Config.BlockEnabled, v => Config.BlockEnabled = v);
            CreateFloatSlider("Damage Reduction %", 0f, 1f, () => Config.BlockDamageReduction, v => Config.BlockDamageReduction = v);

            CreateSpacer();

            // === POSTURE METER ===
            CreateSectionHeader("POSTURE METER (ENEMY)");
            CreateToggle("Posture Enabled", () => Config.PostureEnabled, v => Config.PostureEnabled = v);
            CreateIntSlider("Max Posture", 10, 500, () => Config.MaxPosture, v => Config.MaxPosture = v);
            CreateIntSlider("Posture Dmg on Parry", 0, 100, () => Config.PostureDamageOnParry, v => Config.PostureDamageOnParry = v);
            CreateIntSlider("Posture Dmg on Attack", 0, 100, () => Config.PostureDamageOnAttack, v => Config.PostureDamageOnAttack = v);
            CreateFloatSlider("Posture Stun Duration (s)", 0.5f, 15f, () => Config.PostureStunDuration, v => Config.PostureStunDuration = v);

            CreateSpacer();

            // === CRIT METER ===
            CreateSectionHeader("CRIT METER (PLAYER)");
            CreateToggle("Crit Meter Enabled", () => Config.CritMeterEnabled, v => Config.CritMeterEnabled = v);
            CreateToggle("Auto-Activate (next attack)", () => Config.CritAutoActivate, v => Config.CritAutoActivate = v);
            CreateIntSlider("Crit Meter Max", 10, 500, () => Config.CritMeterMax, v => Config.CritMeterMax = v);
            CreateIntSlider("Gain on Parry", 0, 100, () => Config.CritMeterGainOnParry, v => Config.CritMeterGainOnParry = v);
            CreateIntSlider("Gain on Attack", 0, 100, () => Config.CritMeterGainOnAttack, v => Config.CritMeterGainOnAttack = v);
            CreateIntSlider("Crit Damage", 0, 100, () => Config.CritDamage, v => Config.CritDamage = v);
            CreateToggle("Crit Restores Energy", () => Config.CritRestoresEnergy, v => Config.CritRestoresEnergy = v);
            CreateIntSlider("Energy Restored", 0, 50, () => Config.CritEnergyRestore, v => Config.CritEnergyRestore = v);
            CreateToggle("Decreases on Block/Dodge", () => Config.CritDecreasesOnDefensive, v => Config.CritDecreasesOnDefensive = v);
            CreateIntSlider("Decrease Amount", 0, 50, () => Config.CritDecreaseAmount, v => Config.CritDecreaseAmount = v);
            CreateToggle("Stun+Crit: Attacks Refill Energy", () => Config.AttackDuringStunWithCritRefillsEnergy, v => Config.AttackDuringStunWithCritRefillsEnergy = v);
            CreateIntSlider("Stun Attack Energy Refill", 0, 20, () => Config.StunAttackEnergyRefill, v => Config.StunAttackEnergyRefill = v);

            CreateSpacer();

            // === ENEMY ===
            CreateSectionHeader("ENEMY");
            CreateIntSlider("Starting Energy", 1, 200, () => Config.EnemyStartingEnergy, v => Config.EnemyStartingEnergy = v);
            CreateIntSlider("Attack Damage", 0, 50, () => Config.EnemyAttackDamage, v => Config.EnemyAttackDamage = v);
            CreateFloatSlider("Telegraph Min (s)", 0.1f, 3f, () => Config.TelegraphDurationMin, v => Config.TelegraphDurationMin = v);
            CreateFloatSlider("Telegraph Max (s)", 0.1f, 3f, () => Config.TelegraphDurationMax, v => Config.TelegraphDurationMax = v);
            CreateFloatSlider("Cooldown Min (s)", 0.1f, 5f, () => Config.EnemyAttackCooldownMin, v => Config.EnemyAttackCooldownMin = v);
            CreateFloatSlider("Cooldown Max (s)", 0.1f, 5f, () => Config.EnemyAttackCooldownMax, v => Config.EnemyAttackCooldownMax = v);
            CreateFloatSlider("Parry Stun Duration (s)", 0.1f, 5f, () => Config.ParryStunDuration, v => Config.ParryStunDuration = v);

            CreateSpacer();

            // === ENEMY COMBOS ===
            CreateSectionHeader("ENEMY COMBOS");
            CreateToggle("Combos Enabled", () => Config.CombosEnabled, v => Config.CombosEnabled = v);

            for (int c = 0; c < Config.Combos.Length; c++)
            {
                int comboIndex = c;
                var combo = Config.Combos[comboIndex];
                CreateSectionHeader($"  Combo {c + 1}");
                CreateToggle($"Combo {c + 1} Enabled", () => Config.Combos[comboIndex].Enabled, v => Config.Combos[comboIndex].Enabled = v);

                // Show attacks in this combo
                for (int a = 0; a < combo.Attacks.Length; a++)
                {
                    int attackIndex = a;
                    CreateLabel($"    Hit {a + 1}");
                    CreateDirectionDropdown($"      Direction", () => Config.Combos[comboIndex].Attacks[attackIndex].Direction, v => Config.Combos[comboIndex].Attacks[attackIndex].Direction = v);
                    CreateFloatSlider($"      Telegraph (s)", 0.1f, 2f, () => Config.Combos[comboIndex].Attacks[attackIndex].TelegraphDuration, v => Config.Combos[comboIndex].Attacks[attackIndex].TelegraphDuration = v);
                    CreateFloatSlider($"      Interval After (s)", 0.1f, 2f, () => Config.Combos[comboIndex].Attacks[attackIndex].IntervalAfter, v => Config.Combos[comboIndex].Attacks[attackIndex].IntervalAfter = v);
                }

                // Add/Remove hit buttons
                CreateButton($"+ Add Hit to Combo {c + 1}", () =>
                {
                    var current = Config.Combos[comboIndex].Attacks;
                    if (current.Length >= 5) return;
                    var newAttacks = new ComboAttack[current.Length + 1];
                    System.Array.Copy(current, newAttacks, current.Length);
                    newAttacks[current.Length] = new ComboAttack();
                    Config.Combos[comboIndex].Attacks = newAttacks;
                    RebuildUI();
                });
                CreateButton($"- Remove Last Hit from Combo {c + 1}", () =>
                {
                    var current = Config.Combos[comboIndex].Attacks;
                    if (current.Length <= 1) return;
                    var newAttacks = new ComboAttack[current.Length - 1];
                    System.Array.Copy(current, newAttacks, current.Length - 1);
                    Config.Combos[comboIndex].Attacks = newAttacks;
                    RebuildUI();
                });
            }

            CreateSpacer();
            CreateSpacer();
        }

        private void RebuildUI()
        {
            // Destroy all children of content parent
            for (int i = contentParent.childCount - 1; i >= 0; i--)
                Destroy(contentParent.GetChild(i).gameObject);
            _refreshActions.Clear();
            BuildUI();
        }

        private void RefreshAllFields()
        {
            foreach (var action in _refreshActions)
                action?.Invoke();
        }

        // === UI Factory Methods ===

        private void CreateSectionHeader(string title)
        {
            var obj = new GameObject("Header_" + title);
            obj.transform.SetParent(contentParent, false);
            var rect = obj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 40);
            var layout = obj.AddComponent<LayoutElement>();
            layout.minHeight = 40;
            layout.preferredHeight = 40;

            var tmp = obj.AddComponent<TextMeshProUGUI>();
            tmp.text = title;
            tmp.fontSize = 22;
            tmp.fontStyle = FontStyles.Bold;
            tmp.color = new Color(1f, 0.8f, 0.2f);
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            tmp.margin = new Vector4(10, 0, 0, 0);
            tmp.raycastTarget = false;
        }

        private void CreateLabel(string text)
        {
            var obj = new GameObject("Label");
            obj.transform.SetParent(contentParent, false);
            var rect = obj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 25);
            var layout = obj.AddComponent<LayoutElement>();
            layout.minHeight = 25;

            var tmp = obj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 16;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            tmp.margin = new Vector4(10, 0, 0, 0);
            tmp.raycastTarget = false;
        }

        private void CreateSpacer()
        {
            var obj = new GameObject("Spacer");
            obj.transform.SetParent(contentParent, false);
            obj.AddComponent<RectTransform>();
            var layout = obj.AddComponent<LayoutElement>();
            layout.minHeight = 15;
            layout.preferredHeight = 15;
        }

        private void CreateIntSlider(string label, int min, int max, System.Func<int> getter, System.Action<int> setter)
        {
            var row = CreateRow(label);

            var sliderObj = new GameObject("Slider");
            sliderObj.transform.SetParent(row.transform, false);
            var sliderRect = sliderObj.AddComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0.45f, 0.1f);
            sliderRect.anchorMax = new Vector2(0.85f, 0.9f);
            sliderRect.offsetMin = Vector2.zero;
            sliderRect.offsetMax = Vector2.zero;

            // Slider background
            var bgObj = new GameObject("Background");
            bgObj.transform.SetParent(sliderObj.transform, false);
            var bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.3f, 0.3f, 0.3f);
            var bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0, 0.4f);
            bgRect.anchorMax = new Vector2(1, 0.6f);
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            // Fill area
            var fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sliderObj.transform, false);
            var fillAreaRect = fillArea.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0, 0.35f);
            fillAreaRect.anchorMax = new Vector2(1, 0.65f);
            fillAreaRect.offsetMin = Vector2.zero;
            fillAreaRect.offsetMax = Vector2.zero;

            var fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(fillArea.transform, false);
            var fillImage = fillObj.AddComponent<Image>();
            fillImage.color = new Color(0.3f, 0.7f, 1f);
            var fillRect = fillObj.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            // Handle
            var handleArea = new GameObject("Handle Slide Area");
            handleArea.transform.SetParent(sliderObj.transform, false);
            var handleAreaRect = handleArea.AddComponent<RectTransform>();
            handleAreaRect.anchorMin = Vector2.zero;
            handleAreaRect.anchorMax = Vector2.one;
            handleAreaRect.offsetMin = Vector2.zero;
            handleAreaRect.offsetMax = Vector2.zero;

            var handleObj = new GameObject("Handle");
            handleObj.transform.SetParent(handleArea.transform, false);
            var handleImage = handleObj.AddComponent<Image>();
            handleImage.color = Color.white;
            var handleRect = handleObj.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(15, 0);

            var slider = sliderObj.AddComponent<Slider>();
            slider.minValue = min;
            slider.maxValue = max;
            slider.wholeNumbers = true;
            slider.fillRect = fillRect;
            slider.handleRect = handleRect;
            slider.targetGraphic = handleImage;
            slider.value = getter();

            // Value text
            var valueObj = new GameObject("Value");
            valueObj.transform.SetParent(row.transform, false);
            var valueRect = valueObj.AddComponent<RectTransform>();
            valueRect.anchorMin = new Vector2(0.87f, 0);
            valueRect.anchorMax = new Vector2(1f, 1);
            valueRect.offsetMin = Vector2.zero;
            valueRect.offsetMax = Vector2.zero;
            var valueTMP = valueObj.AddComponent<TextMeshProUGUI>();
            valueTMP.text = getter().ToString();
            valueTMP.fontSize = 18;
            valueTMP.color = Color.white;
            valueTMP.alignment = TextAlignmentOptions.Center;
            valueTMP.raycastTarget = false;

            slider.onValueChanged.AddListener(v =>
            {
                setter((int)v);
                valueTMP.text = ((int)v).ToString();
            });

            _refreshActions.Add(() =>
            {
                slider.value = getter();
                valueTMP.text = getter().ToString();
            });
        }

        private void CreateFloatSlider(string label, float min, float max, System.Func<float> getter, System.Action<float> setter)
        {
            var row = CreateRow(label);

            var sliderObj = new GameObject("Slider");
            sliderObj.transform.SetParent(row.transform, false);
            var sliderRect = sliderObj.AddComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0.45f, 0.1f);
            sliderRect.anchorMax = new Vector2(0.85f, 0.9f);
            sliderRect.offsetMin = Vector2.zero;
            sliderRect.offsetMax = Vector2.zero;

            var bgObj = new GameObject("Background");
            bgObj.transform.SetParent(sliderObj.transform, false);
            var bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.3f, 0.3f, 0.3f);
            var bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0, 0.4f);
            bgRect.anchorMax = new Vector2(1, 0.6f);
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            var fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(sliderObj.transform, false);
            var fillAreaRect = fillArea.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0, 0.35f);
            fillAreaRect.anchorMax = new Vector2(1, 0.65f);
            fillAreaRect.offsetMin = Vector2.zero;
            fillAreaRect.offsetMax = Vector2.zero;

            var fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(fillArea.transform, false);
            var fillImage = fillObj.AddComponent<Image>();
            fillImage.color = new Color(0.3f, 0.7f, 1f);
            var fillRect = fillObj.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            var handleArea = new GameObject("Handle Slide Area");
            handleArea.transform.SetParent(sliderObj.transform, false);
            var handleAreaRect = handleArea.AddComponent<RectTransform>();
            handleAreaRect.anchorMin = Vector2.zero;
            handleAreaRect.anchorMax = Vector2.one;
            handleAreaRect.offsetMin = Vector2.zero;
            handleAreaRect.offsetMax = Vector2.zero;

            var handleObj = new GameObject("Handle");
            handleObj.transform.SetParent(handleArea.transform, false);
            var handleImage = handleObj.AddComponent<Image>();
            handleImage.color = Color.white;
            var handleRect = handleObj.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(15, 0);

            var slider = sliderObj.AddComponent<Slider>();
            slider.minValue = min;
            slider.maxValue = max;
            slider.wholeNumbers = false;
            slider.fillRect = fillRect;
            slider.handleRect = handleRect;
            slider.targetGraphic = handleImage;
            slider.value = getter();

            var valueObj = new GameObject("Value");
            valueObj.transform.SetParent(row.transform, false);
            var valueRect = valueObj.AddComponent<RectTransform>();
            valueRect.anchorMin = new Vector2(0.87f, 0);
            valueRect.anchorMax = new Vector2(1f, 1);
            valueRect.offsetMin = Vector2.zero;
            valueRect.offsetMax = Vector2.zero;
            var valueTMP = valueObj.AddComponent<TextMeshProUGUI>();
            valueTMP.text = getter().ToString("F2");
            valueTMP.fontSize = 18;
            valueTMP.color = Color.white;
            valueTMP.alignment = TextAlignmentOptions.Center;
            valueTMP.raycastTarget = false;

            slider.onValueChanged.AddListener(v =>
            {
                setter(v);
                valueTMP.text = v.ToString("F2");
            });

            _refreshActions.Add(() =>
            {
                slider.value = getter();
                valueTMP.text = getter().ToString("F2");
            });
        }

        private void CreateToggle(string label, System.Func<bool> getter, System.Action<bool> setter)
        {
            var row = CreateRow(label);

            var toggleObj = new GameObject("Toggle");
            toggleObj.transform.SetParent(row.transform, false);
            var toggleRect = toggleObj.AddComponent<RectTransform>();
            toggleRect.anchorMin = new Vector2(0.45f, 0.15f);
            toggleRect.anchorMax = new Vector2(0.52f, 0.85f);
            toggleRect.offsetMin = Vector2.zero;
            toggleRect.offsetMax = Vector2.zero;

            // Checkbox background
            var checkBg = new GameObject("Background");
            checkBg.transform.SetParent(toggleObj.transform, false);
            var checkBgImage = checkBg.AddComponent<Image>();
            checkBgImage.color = new Color(0.3f, 0.3f, 0.3f);
            var checkBgRect = checkBg.GetComponent<RectTransform>();
            checkBgRect.anchorMin = Vector2.zero;
            checkBgRect.anchorMax = Vector2.one;
            checkBgRect.offsetMin = Vector2.zero;
            checkBgRect.offsetMax = Vector2.zero;

            // Checkmark
            var checkmark = new GameObject("Checkmark");
            checkmark.transform.SetParent(checkBg.transform, false);
            var checkImage = checkmark.AddComponent<Image>();
            checkImage.color = new Color(0.3f, 0.8f, 0.3f);
            var checkRect = checkmark.GetComponent<RectTransform>();
            checkRect.anchorMin = new Vector2(0.15f, 0.15f);
            checkRect.anchorMax = new Vector2(0.85f, 0.85f);
            checkRect.offsetMin = Vector2.zero;
            checkRect.offsetMax = Vector2.zero;

            var toggle = toggleObj.AddComponent<Toggle>();
            toggle.targetGraphic = checkBgImage;
            toggle.graphic = checkImage;
            toggle.isOn = getter();

            toggle.onValueChanged.AddListener(v => setter(v));

            _refreshActions.Add(() => toggle.isOn = getter());
        }

        private void CreateDirectionDropdown(string label, System.Func<AttackDirection> getter, System.Action<AttackDirection> setter)
        {
            var row = CreateRow(label);

            var ddObj = new GameObject("Dropdown");
            ddObj.transform.SetParent(row.transform, false);
            var ddRect = ddObj.AddComponent<RectTransform>();
            ddRect.anchorMin = new Vector2(0.45f, 0.1f);
            ddRect.anchorMax = new Vector2(0.75f, 0.9f);
            ddRect.offsetMin = Vector2.zero;
            ddRect.offsetMax = Vector2.zero;

            var ddImage = ddObj.AddComponent<Image>();
            ddImage.color = new Color(0.25f, 0.25f, 0.3f);

            // Label text for dropdown
            var ddLabelObj = new GameObject("Label");
            ddLabelObj.transform.SetParent(ddObj.transform, false);
            var ddLabelTMP = ddLabelObj.AddComponent<TextMeshProUGUI>();
            ddLabelTMP.fontSize = 16;
            ddLabelTMP.color = Color.white;
            ddLabelTMP.alignment = TextAlignmentOptions.Center;
            var ddLabelRect = ddLabelObj.GetComponent<RectTransform>();
            ddLabelRect.anchorMin = Vector2.zero;
            ddLabelRect.anchorMax = Vector2.one;
            ddLabelRect.offsetMin = new Vector2(5, 0);
            ddLabelRect.offsetMax = new Vector2(-5, 0);

            // Template (required for TMP_Dropdown)
            var templateObj = new GameObject("Template");
            templateObj.transform.SetParent(ddObj.transform, false);
            var templateRect = templateObj.AddComponent<RectTransform>();
            templateRect.anchorMin = new Vector2(0, 0);
            templateRect.anchorMax = new Vector2(1, 0);
            templateRect.pivot = new Vector2(0.5f, 1f);
            templateRect.sizeDelta = new Vector2(0, 120);
            var templateImage = templateObj.AddComponent<Image>();
            templateImage.color = new Color(0.2f, 0.2f, 0.25f);
            var templateScroll = templateObj.AddComponent<ScrollRect>();

            var viewportObj = new GameObject("Viewport");
            viewportObj.transform.SetParent(templateObj.transform, false);
            var viewportRect = viewportObj.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;
            viewportObj.AddComponent<Image>().color = new Color(0.2f, 0.2f, 0.25f);
            viewportObj.AddComponent<Mask>().showMaskGraphic = false;

            var contentObj = new GameObject("Content");
            contentObj.transform.SetParent(viewportObj.transform, false);
            var contentRect = contentObj.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.sizeDelta = new Vector2(0, 30);

            templateScroll.content = contentRect;
            templateScroll.viewport = viewportRect;

            // Item template
            var itemObj = new GameObject("Item");
            itemObj.transform.SetParent(contentObj.transform, false);
            var itemRect = itemObj.AddComponent<RectTransform>();
            itemRect.anchorMin = new Vector2(0, 0.5f);
            itemRect.anchorMax = new Vector2(1, 0.5f);
            itemRect.sizeDelta = new Vector2(0, 30);
            var itemToggle = itemObj.AddComponent<Toggle>();

            var itemBg = new GameObject("Item Background");
            itemBg.transform.SetParent(itemObj.transform, false);
            var itemBgImage = itemBg.AddComponent<Image>();
            itemBgImage.color = new Color(0.25f, 0.25f, 0.3f);
            var itemBgRect = itemBg.GetComponent<RectTransform>();
            itemBgRect.anchorMin = Vector2.zero;
            itemBgRect.anchorMax = Vector2.one;
            itemBgRect.offsetMin = Vector2.zero;
            itemBgRect.offsetMax = Vector2.zero;

            var itemCheckmark = new GameObject("Item Checkmark");
            itemCheckmark.transform.SetParent(itemObj.transform, false);
            var itemCheckImage = itemCheckmark.AddComponent<Image>();
            itemCheckImage.color = new Color(0.3f, 0.7f, 1f);
            var itemCheckRect = itemCheckmark.GetComponent<RectTransform>();
            itemCheckRect.anchorMin = new Vector2(0, 0);
            itemCheckRect.anchorMax = new Vector2(0, 1);
            itemCheckRect.sizeDelta = new Vector2(5, 0);
            itemCheckRect.anchoredPosition = new Vector2(2.5f, 0);

            var itemLabelObj = new GameObject("Item Label");
            itemLabelObj.transform.SetParent(itemObj.transform, false);
            var itemLabelTMP = itemLabelObj.AddComponent<TextMeshProUGUI>();
            itemLabelTMP.fontSize = 14;
            itemLabelTMP.color = Color.white;
            itemLabelTMP.alignment = TextAlignmentOptions.MidlineLeft;
            var itemLabelRect = itemLabelObj.GetComponent<RectTransform>();
            itemLabelRect.anchorMin = Vector2.zero;
            itemLabelRect.anchorMax = Vector2.one;
            itemLabelRect.offsetMin = new Vector2(10, 0);
            itemLabelRect.offsetMax = Vector2.zero;

            itemToggle.targetGraphic = itemBgImage;
            itemToggle.graphic = itemCheckImage;

            templateObj.SetActive(false);

            var dropdown = ddObj.AddComponent<TMP_Dropdown>();
            dropdown.template = templateRect;
            dropdown.captionText = ddLabelTMP;
            dropdown.itemText = itemLabelTMP;
            dropdown.targetGraphic = ddImage;

            dropdown.options.Clear();
            dropdown.options.Add(new TMP_Dropdown.OptionData("Up"));
            dropdown.options.Add(new TMP_Dropdown.OptionData("Down"));
            dropdown.options.Add(new TMP_Dropdown.OptionData("Left"));
            dropdown.options.Add(new TMP_Dropdown.OptionData("Right"));
            dropdown.value = (int)getter();
            dropdown.RefreshShownValue();

            dropdown.onValueChanged.AddListener(v => setter((AttackDirection)v));

            _refreshActions.Add(() =>
            {
                dropdown.value = (int)getter();
                dropdown.RefreshShownValue();
            });
        }

        private void CreateButton(string text, UnityEngine.Events.UnityAction onClick)
        {
            var obj = new GameObject("Button_" + text);
            obj.transform.SetParent(contentParent, false);
            var rect = obj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 35);
            var layout = obj.AddComponent<LayoutElement>();
            layout.minHeight = 35;
            layout.preferredHeight = 35;

            var image = obj.AddComponent<Image>();
            image.color = new Color(0.25f, 0.35f, 0.5f);

            var btn = obj.AddComponent<Button>();
            btn.targetGraphic = image;
            btn.onClick.AddListener(onClick);

            var colors = btn.colors;
            colors.highlightedColor = new Color(0.35f, 0.45f, 0.65f);
            colors.pressedColor = new Color(0.2f, 0.3f, 0.4f);
            btn.colors = colors;

            var textObj = new GameObject("Text");
            textObj.transform.SetParent(obj.transform, false);
            var tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 16;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = false;
            var textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(10, 0);
            textRect.offsetMax = new Vector2(-10, 0);
        }

        private GameObject CreateRow(string label)
        {
            var obj = new GameObject("Row_" + label);
            obj.transform.SetParent(contentParent, false);
            var rect = obj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 35);
            var layout = obj.AddComponent<LayoutElement>();
            layout.minHeight = 35;
            layout.preferredHeight = 35;

            var labelObj = new GameObject("Label");
            labelObj.transform.SetParent(obj.transform, false);
            var labelRect = labelObj.AddComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0);
            labelRect.anchorMax = new Vector2(0.44f, 1);
            labelRect.offsetMin = new Vector2(20, 0);
            labelRect.offsetMax = Vector2.zero;

            var tmp = labelObj.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 16;
            tmp.color = new Color(0.85f, 0.85f, 0.85f);
            tmp.alignment = TextAlignmentOptions.MidlineLeft;
            tmp.raycastTarget = false;

            return obj;
        }
    }
}
