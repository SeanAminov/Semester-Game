#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

namespace Sean.Combat.Editor
{
    public static class CombatDemoSceneBuilder
    {
        [MenuItem("Combat Demo/Build Scene")]
        public static void BuildCombatDemoScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // === Camera ===
            var cameraObj = new GameObject("Main Camera");
            var camera = cameraObj.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 5f;
            camera.transform.position = new Vector3(0, 0, -10);
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.15f, 0.15f, 0.2f);
            cameraObj.AddComponent<AudioListener>();
            cameraObj.AddComponent<UniversalAdditionalCameraData>();
            cameraObj.tag = "MainCamera";

            // === 2D Light ===
            var lightObj = new GameObject("Global Light 2D");
            var light2d = lightObj.AddComponent<Light2D>();
            light2d.lightType = Light2D.LightType.Global;

            // === Runtime Config (Singleton) ===
            var configObj = new GameObject("CombatRuntimeConfig");
            configObj.AddComponent<CombatRuntimeConfig>();

            // === Player (white square) ===
            var playerObj = new GameObject("Player");
            playerObj.transform.position = new Vector3(-3, 0, 0);
            var playerSR = playerObj.AddComponent<SpriteRenderer>();
            playerSR.sprite = CreateSquareSprite();
            playerSR.color = Color.white;
            playerSR.sortingOrder = 1;

            var playerEnergy = playerObj.AddComponent<PlayerEnergy>();
            var playerVisual = playerObj.AddComponent<FighterVisual>();
            var playerCritMeter = playerObj.AddComponent<PlayerCritMeter>();
            var playerController = playerObj.AddComponent<PlayerCombatController>();

            SetSerializedField(playerController, "energy", playerEnergy);
            SetSerializedField(playerController, "visual", playerVisual);
            SetSerializedField(playerController, "critMeter", playerCritMeter);
            SetSerializedField(playerVisual, "spriteRenderer", playerSR);

            // === Enemy (white circle) ===
            var enemyObj = new GameObject("Enemy");
            enemyObj.transform.position = new Vector3(3, 0, 0);
            var enemySR = enemyObj.AddComponent<SpriteRenderer>();
            enemySR.sprite = CreateCircleSprite();
            enemySR.color = Color.white;
            enemySR.sortingOrder = 1;

            var enemyEnergy = enemyObj.AddComponent<EnemyEnergy>();
            var enemyVisual = enemyObj.AddComponent<FighterVisual>();
            var enemyController = enemyObj.AddComponent<EnemyCombatController>();
            var telegraph = enemyObj.AddComponent<EnemyAttackTelegraph>();
            var dirIndicator = enemyObj.AddComponent<DirectionIndicator>();
            enemyObj.AddComponent<EnemyPostureMeter>();

            SetSerializedField(enemyController, "energy", enemyEnergy);
            SetSerializedField(enemyController, "visual", enemyVisual);
            SetSerializedField(enemyVisual, "spriteRenderer", enemySR);
            SetSerializedField(telegraph, "spriteRenderer", enemySR);
            SetSerializedField(telegraph, "directionIndicator", dirIndicator);

            // Direction arrows
            var arrowUp = CreateArrow("ArrowUp", enemyObj.transform, new Vector3(0, 1.5f, 0), 0f);
            var arrowDown = CreateArrow("ArrowDown", enemyObj.transform, new Vector3(0, -1.5f, 0), 180f);
            var arrowLeft = CreateArrow("ArrowLeft", enemyObj.transform, new Vector3(-1.5f, 0, 0), 90f);
            var arrowRight = CreateArrow("ArrowRight", enemyObj.transform, new Vector3(1.5f, 0, 0), -90f);

            SetSerializedField(dirIndicator, "arrowUp", arrowUp);
            SetSerializedField(dirIndicator, "arrowDown", arrowDown);
            SetSerializedField(dirIndicator, "arrowLeft", arrowLeft);
            SetSerializedField(dirIndicator, "arrowRight", arrowRight);

            // === Canvas ===
            var canvasObj = new GameObject("CombatCanvas");
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasObj.AddComponent<GraphicRaycaster>();

            // ==========================================
            // === PLAY PANEL (combat UI, initially hidden) ===
            // ==========================================
            var playPanel = new GameObject("PlayPanel");
            playPanel.transform.SetParent(canvasObj.transform, false);
            var playPanelRect = playPanel.AddComponent<RectTransform>();
            playPanelRect.anchorMin = Vector2.zero;
            playPanelRect.anchorMax = Vector2.one;
            playPanelRect.offsetMin = Vector2.zero;
            playPanelRect.offsetMax = Vector2.zero;

            // Player Energy Bar
            var playerBarObj = CreateEnergyBar("PlayerEnergyBar", playPanel.transform,
                new Color(0.2f, 0.8f, 1f), "PLAYER", FighterType.Player);
            var playerBarRect = playerBarObj.GetComponent<RectTransform>();
            playerBarRect.anchorMin = new Vector2(0, 0);
            playerBarRect.anchorMax = new Vector2(0, 0);
            playerBarRect.pivot = new Vector2(0, 0);
            playerBarRect.anchoredPosition = new Vector2(20, 20);
            playerBarRect.sizeDelta = new Vector2(350, 60);

            // Enemy Energy Bar
            var enemyBarObj = CreateEnergyBar("EnemyEnergyBar", playPanel.transform,
                new Color(1f, 0.3f, 0.3f), "ENEMY", FighterType.Enemy);
            var enemyBarRect = enemyBarObj.GetComponent<RectTransform>();
            enemyBarRect.anchorMin = new Vector2(1, 0);
            enemyBarRect.anchorMax = new Vector2(1, 0);
            enemyBarRect.pivot = new Vector2(1, 0);
            enemyBarRect.anchoredPosition = new Vector2(-20, 20);
            enemyBarRect.sizeDelta = new Vector2(350, 60);

            // === Posture Meter Bar ===
            var postureMeterObj = CreateMeterBar("PostureMeter", playPanel.transform,
                new Color(1f, 0.6f, 0.1f), "POSTURE");
            var postureMeterRect = postureMeterObj.GetComponent<RectTransform>();
            postureMeterRect.anchorMin = new Vector2(1, 0);
            postureMeterRect.anchorMax = new Vector2(1, 0);
            postureMeterRect.pivot = new Vector2(1, 0);
            postureMeterRect.anchoredPosition = new Vector2(-20, 90);
            postureMeterRect.sizeDelta = new Vector2(350, 45);

            var postureUI = postureMeterObj.AddComponent<PostureMeterUI>();
            SetSerializedField(postureUI, "fillImage", postureMeterObj.transform.Find("Fill").GetComponent<Image>());
            SetSerializedField(postureUI, "valueText", postureMeterObj.transform.Find("ValueText").GetComponent<TextMeshProUGUI>());
            SetSerializedField(postureUI, "meterRoot", postureMeterObj);

            // === Crit Meter Bar ===
            var critMeterObj = CreateMeterBar("CritMeter", playPanel.transform,
                new Color(1f, 0.85f, 0.2f), "CRIT");
            var critMeterRect = critMeterObj.GetComponent<RectTransform>();
            critMeterRect.anchorMin = new Vector2(0, 0);
            critMeterRect.anchorMax = new Vector2(0, 0);
            critMeterRect.pivot = new Vector2(0, 0);
            critMeterRect.anchoredPosition = new Vector2(20, 90);
            critMeterRect.sizeDelta = new Vector2(350, 45);

            var critUI = critMeterObj.AddComponent<CritMeterUI>();
            SetSerializedField(critUI, "fillImage", critMeterObj.transform.Find("Fill").GetComponent<Image>());
            SetSerializedField(critUI, "valueText", critMeterObj.transform.Find("ValueText").GetComponent<TextMeshProUGUI>());
            SetSerializedField(critUI, "meterRoot", critMeterObj);

            // Notification Container
            var notifContainer = new GameObject("NotificationContainer");
            notifContainer.transform.SetParent(playPanel.transform, false);
            var notifRect = notifContainer.AddComponent<RectTransform>();
            notifRect.anchorMin = Vector2.zero;
            notifRect.anchorMax = Vector2.one;
            notifRect.offsetMin = Vector2.zero;
            notifRect.offsetMax = Vector2.zero;

            var notifPrefab = new GameObject("NotificationText");
            notifPrefab.transform.SetParent(playPanel.transform, false);
            var notifTMP = notifPrefab.AddComponent<TextMeshProUGUI>();
            notifTMP.fontSize = 36;
            notifTMP.fontStyle = FontStyles.Bold;
            notifTMP.alignment = TextAlignmentOptions.Center;
            notifTMP.enableWordWrapping = false;
            notifTMP.raycastTarget = false;
            var notifPrefabRect = notifPrefab.GetComponent<RectTransform>();
            notifPrefabRect.sizeDelta = new Vector2(200, 50);
            notifPrefab.SetActive(false);

            var notifUI = notifContainer.AddComponent<CombatNotificationUI>();
            SetSerializedField(notifUI, "notificationPrefab", notifPrefab);
            SetSerializedField(notifUI, "canvas", canvas);

            // Game Over Panel
            var gameOverPanel = new GameObject("GameOverPanel");
            gameOverPanel.transform.SetParent(playPanel.transform, false);
            var goRect = gameOverPanel.AddComponent<RectTransform>();
            goRect.anchorMin = new Vector2(0.5f, 0.5f);
            goRect.anchorMax = new Vector2(0.5f, 0.5f);
            goRect.sizeDelta = new Vector2(450, 280);
            var goImage = gameOverPanel.AddComponent<Image>();
            goImage.color = new Color(0, 0, 0, 0.85f);

            var resultTextObj = new GameObject("ResultText");
            resultTextObj.transform.SetParent(gameOverPanel.transform, false);
            var resultTMP = resultTextObj.AddComponent<TextMeshProUGUI>();
            resultTMP.text = "VICTORY!";
            resultTMP.fontSize = 48;
            resultTMP.fontStyle = FontStyles.Bold;
            resultTMP.alignment = TextAlignmentOptions.Center;
            resultTMP.raycastTarget = false;
            var resultRect = resultTextObj.GetComponent<RectTransform>();
            resultRect.anchorMin = new Vector2(0, 0.55f);
            resultRect.anchorMax = new Vector2(1, 1);
            resultRect.offsetMin = new Vector2(10, 10);
            resultRect.offsetMax = new Vector2(-10, -10);

            // Restart button
            var restartObj = CreateUIButton("RestartButton", gameOverPanel.transform, "RESTART",
                new Vector2(0.05f, 0.1f), new Vector2(0.48f, 0.45f));
            var restartBtn = restartObj.GetComponent<Button>();

            // Customize button
            var customizeObj = CreateUIButton("CustomizeButton", gameOverPanel.transform, "CUSTOMIZE",
                new Vector2(0.52f, 0.1f), new Vector2(0.95f, 0.45f));
            var customizeBtn = customizeObj.GetComponent<Button>();

            // Controls hint
            var controlsObj = new GameObject("ControlsHint");
            controlsObj.transform.SetParent(playPanel.transform, false);
            var controlsTMP = controlsObj.AddComponent<TextMeshProUGUI>();
            controlsTMP.text = "WASD = Parry (directional)  |  Up Arrow = Punch  |  Space = Dodge";
            controlsTMP.fontSize = 18;
            controlsTMP.alignment = TextAlignmentOptions.Center;
            controlsTMP.color = new Color(1, 1, 1, 0.5f);
            controlsTMP.raycastTarget = false;
            var controlsRect = controlsObj.GetComponent<RectTransform>();
            controlsRect.anchorMin = new Vector2(0, 1);
            controlsRect.anchorMax = new Vector2(1, 1);
            controlsRect.pivot = new Vector2(0.5f, 1);
            controlsRect.anchoredPosition = new Vector2(0, -10);
            controlsRect.sizeDelta = new Vector2(0, 30);

            // ==========================================
            // === CUSTOMIZE PANEL (full opaque, initially visible) ===
            // ==========================================
            var customizePanel = new GameObject("CustomizePanel");
            customizePanel.transform.SetParent(canvasObj.transform, false);
            var custRect = customizePanel.AddComponent<RectTransform>();
            custRect.anchorMin = Vector2.zero;
            custRect.anchorMax = Vector2.one;
            custRect.offsetMin = Vector2.zero;
            custRect.offsetMax = Vector2.zero;

            // Dark background
            var custBg = customizePanel.AddComponent<Image>();
            custBg.color = new Color(0.1f, 0.1f, 0.15f, 1f);

            // Title
            var titleObj = new GameObject("Title");
            titleObj.transform.SetParent(customizePanel.transform, false);
            var titleTMP = titleObj.AddComponent<TextMeshProUGUI>();
            titleTMP.text = "COMBAT CUSTOMIZATION";
            titleTMP.fontSize = 36;
            titleTMP.fontStyle = FontStyles.Bold;
            titleTMP.color = Color.white;
            titleTMP.alignment = TextAlignmentOptions.Center;
            titleTMP.raycastTarget = false;
            var titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 1);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.pivot = new Vector2(0.5f, 1);
            titleRect.anchoredPosition = new Vector2(0, -15);
            titleRect.sizeDelta = new Vector2(0, 50);

            // Scroll View
            var scrollObj = new GameObject("ScrollView");
            scrollObj.transform.SetParent(customizePanel.transform, false);
            var scrollRect = scrollObj.AddComponent<RectTransform>();
            scrollRect.anchorMin = new Vector2(0.05f, 0.08f);
            scrollRect.anchorMax = new Vector2(0.95f, 0.92f);
            scrollRect.offsetMin = Vector2.zero;
            scrollRect.offsetMax = Vector2.zero;

            var scrollImage = scrollObj.AddComponent<Image>();
            scrollImage.color = new Color(0.12f, 0.12f, 0.18f, 0.5f);

            var scrollMask = scrollObj.AddComponent<Mask>();
            scrollMask.showMaskGraphic = true;

            var scrollComponent = scrollObj.AddComponent<ScrollRect>();
            scrollComponent.horizontal = false;
            scrollComponent.vertical = true;
            scrollComponent.movementType = ScrollRect.MovementType.Elastic;
            scrollComponent.scrollSensitivity = 30f;

            // Viewport
            var viewportObj = new GameObject("Viewport");
            viewportObj.transform.SetParent(scrollObj.transform, false);
            var viewportRect = viewportObj.AddComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;

            // Content (this is what CombatCustomizationUI populates)
            var contentObj = new GameObject("Content");
            contentObj.transform.SetParent(viewportObj.transform, false);
            var contentObjRect = contentObj.AddComponent<RectTransform>();
            contentObjRect.anchorMin = new Vector2(0, 1);
            contentObjRect.anchorMax = new Vector2(1, 1);
            contentObjRect.pivot = new Vector2(0.5f, 1);
            contentObjRect.sizeDelta = new Vector2(0, 0);

            var vertLayout = contentObj.AddComponent<VerticalLayoutGroup>();
            vertLayout.childControlWidth = true;
            vertLayout.childControlHeight = false;
            vertLayout.childForceExpandWidth = true;
            vertLayout.childForceExpandHeight = false;
            vertLayout.spacing = 3;
            vertLayout.padding = new RectOffset(10, 10, 10, 10);

            var contentFitter = contentObj.AddComponent<ContentSizeFitter>();
            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scrollComponent.content = contentObjRect;
            scrollComponent.viewport = viewportRect;

            // Play Button (bottom of customize panel)
            var playBtnObj = CreateUIButton("PlayButton", customizePanel.transform, "PLAY",
                new Vector2(0.35f, 0.01f), new Vector2(0.65f, 0.06f));
            var playBtnImage = playBtnObj.GetComponent<Image>();
            playBtnImage.color = new Color(0.2f, 0.6f, 0.2f);
            var playBtn = playBtnObj.GetComponent<Button>();

            // CombatCustomizationUI component
            var custUI = customizePanel.AddComponent<CombatCustomizationUI>();
            SetSerializedField(custUI, "contentParent", contentObj.transform);
            SetSerializedField(custUI, "playButton", playBtn);

            // === EventSystem ===
            var eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

            // === CombatManager ===
            var managerObj = new GameObject("CombatManager");
            var combatManager = managerObj.AddComponent<CombatManager>();
            SetSerializedField(combatManager, "playerEnergy", playerEnergy);
            SetSerializedField(combatManager, "enemyEnergy", enemyEnergy);

            // === CombatMenuManager ===
            var menuManagerObj = new GameObject("CombatMenuManager");
            var menuManager = menuManagerObj.AddComponent<CombatMenuManager>();
            SetSerializedField(menuManager, "customizePanel", customizePanel);
            SetSerializedField(menuManager, "playPanel", playPanel);
            SetSerializedField(menuManager, "combatManager", combatManager);

            // Wire CombatCustomizationUI -> menuManager
            SetSerializedField(custUI, "menuManager", menuManager);

            // === CombatHUD ===
            var hudComponent = playPanel.AddComponent<CombatHUD>();
            SetSerializedField(hudComponent, "gameOverPanel", gameOverPanel);
            SetSerializedField(hudComponent, "resultText", resultTMP);
            SetSerializedField(hudComponent, "restartButton", restartBtn);
            SetSerializedField(hudComponent, "customizeButton", customizeBtn);
            SetSerializedField(hudComponent, "combatManager", combatManager);
            SetSerializedField(hudComponent, "menuManager", menuManager);

            SetSerializedField(combatManager, "hud", hudComponent);

            // Save scene
            string scenePath = "Assets/Sean/Scenes/CombatDemo.unity";
            EditorSceneManager.SaveScene(scene, scenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Combat Demo scene built and saved to {scenePath}");
            Debug.Log("Use the Customize panel to adjust settings, then click PLAY!");
        }

        private static GameObject CreateUIButton(string name, Transform parent, string text,
            Vector2 anchorMin, Vector2 anchorMax)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var image = obj.AddComponent<Image>();
            image.color = new Color(0.3f, 0.3f, 0.4f);
            var btn = obj.AddComponent<Button>();
            btn.targetGraphic = image;

            var rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var textObj = new GameObject("Text");
            textObj.transform.SetParent(obj.transform, false);
            var tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 24;
            tmp.fontStyle = FontStyles.Bold;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            tmp.raycastTarget = false;
            var textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            return obj;
        }

        private static GameObject CreateMeterBar(string name, Transform parent,
            Color fillColor, string label)
        {
            var barObj = new GameObject(name);
            barObj.transform.SetParent(parent, false);
            barObj.AddComponent<RectTransform>();

            var bgObj = new GameObject("Background");
            bgObj.transform.SetParent(barObj.transform, false);
            var bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            var bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            var fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(barObj.transform, false);
            var fillImage = fillObj.AddComponent<Image>();
            fillImage.color = fillColor;
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillAmount = 1f;
            var fillRect = fillObj.GetComponent<RectTransform>();
            fillRect.anchorMin = new Vector2(0, 0);
            fillRect.anchorMax = new Vector2(1, 1);
            fillRect.offsetMin = new Vector2(4, 4);
            fillRect.offsetMax = new Vector2(-4, -16);

            var labelObj = new GameObject("Label");
            labelObj.transform.SetParent(barObj.transform, false);
            var labelTMP = labelObj.AddComponent<TextMeshProUGUI>();
            labelTMP.text = label;
            labelTMP.fontSize = 13;
            labelTMP.fontStyle = FontStyles.Bold;
            labelTMP.alignment = TextAlignmentOptions.Center;
            labelTMP.color = Color.white;
            labelTMP.raycastTarget = false;
            var labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0.55f);
            labelRect.anchorMax = new Vector2(1, 1);
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            var valueObj = new GameObject("ValueText");
            valueObj.transform.SetParent(barObj.transform, false);
            var valueTMP = valueObj.AddComponent<TextMeshProUGUI>();
            valueTMP.text = "0 / 0";
            valueTMP.fontSize = 16;
            valueTMP.fontStyle = FontStyles.Bold;
            valueTMP.alignment = TextAlignmentOptions.Center;
            valueTMP.color = Color.white;
            valueTMP.raycastTarget = false;
            var valueRect = valueObj.GetComponent<RectTransform>();
            valueRect.anchorMin = new Vector2(0, 0);
            valueRect.anchorMax = new Vector2(1, 0.6f);
            valueRect.offsetMin = Vector2.zero;
            valueRect.offsetMax = Vector2.zero;

            return barObj;
        }

        private static GameObject CreateArrow(string name, Transform parent, Vector3 localPos, float zRotation)
        {
            var arrowObj = new GameObject(name);
            arrowObj.transform.SetParent(parent, false);
            arrowObj.transform.localPosition = localPos;
            arrowObj.transform.localRotation = Quaternion.Euler(0, 0, zRotation);

            var sr = arrowObj.AddComponent<SpriteRenderer>();
            sr.sprite = CreateTriangleSprite();
            sr.color = Color.yellow;
            sr.sortingOrder = 2;
            arrowObj.transform.localScale = Vector3.one * 0.5f;
            arrowObj.SetActive(false);

            return arrowObj;
        }

        private static GameObject CreateEnergyBar(string name, Transform parent,
            Color fillColor, string label, FighterType fighterType)
        {
            var barObj = new GameObject(name);
            barObj.transform.SetParent(parent, false);
            barObj.AddComponent<RectTransform>();

            var bgObj = new GameObject("Background");
            bgObj.transform.SetParent(barObj.transform, false);
            var bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            var bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            var fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(barObj.transform, false);
            var fillImage = fillObj.AddComponent<Image>();
            fillImage.color = fillColor;
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillAmount = 1f;
            var fillRect = fillObj.GetComponent<RectTransform>();
            fillRect.anchorMin = new Vector2(0, 0);
            fillRect.anchorMax = new Vector2(1, 1);
            fillRect.offsetMin = new Vector2(4, 4);
            fillRect.offsetMax = new Vector2(-4, -20);

            var labelObj = new GameObject("Label");
            labelObj.transform.SetParent(barObj.transform, false);
            var labelTMP = labelObj.AddComponent<TextMeshProUGUI>();
            labelTMP.text = label;
            labelTMP.fontSize = 16;
            labelTMP.fontStyle = FontStyles.Bold;
            labelTMP.alignment = TextAlignmentOptions.Center;
            labelTMP.color = Color.white;
            labelTMP.raycastTarget = false;
            var labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0.6f);
            labelRect.anchorMax = new Vector2(1, 1);
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            var valueObj = new GameObject("ValueText");
            valueObj.transform.SetParent(barObj.transform, false);
            var valueTMP = valueObj.AddComponent<TextMeshProUGUI>();
            valueTMP.text = "0 / 0";
            valueTMP.fontSize = 20;
            valueTMP.fontStyle = FontStyles.Bold;
            valueTMP.alignment = TextAlignmentOptions.Center;
            valueTMP.color = Color.white;
            valueTMP.raycastTarget = false;
            var valueRect = valueObj.GetComponent<RectTransform>();
            valueRect.anchorMin = new Vector2(0, 0);
            valueRect.anchorMax = new Vector2(1, 0.65f);
            valueRect.offsetMin = Vector2.zero;
            valueRect.offsetMax = Vector2.zero;

            var barUI = barObj.AddComponent<EnergyBarUI>();
            SetSerializedField(barUI, "trackedFighter", (int)fighterType);
            SetSerializedField(barUI, "fillImage", fillImage);
            SetSerializedField(barUI, "valueText", valueTMP);

            return barObj;
        }

        private static Sprite CreateSquareSprite()
        {
            var builtIn = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            if (builtIn != null) return builtIn;

            var tex = new Texture2D(64, 64);
            var pixels = new Color[64 * 64];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
            tex.SetPixels(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64);
        }

        private static Sprite CreateCircleSprite()
        {
            int size = 64;
            var tex = new Texture2D(size, size);
            var pixels = new Color[size * size];
            float center = size / 2f;
            float radius = size / 2f - 1;

            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                    pixels[y * size + x] = dist <= radius ? Color.white : Color.clear;
                }

            tex.SetPixels(pixels);
            tex.Apply();
            tex.filterMode = FilterMode.Bilinear;
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        }

        private static Sprite CreateTriangleSprite()
        {
            int size = 32;
            var tex = new Texture2D(size, size);
            var pixels = new Color[size * size];

            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float halfWidth = (float)y / size * (size / 2f);
                    float center = size / 2f;
                    pixels[y * size + x] = (x >= center - halfWidth && x <= center + halfWidth)
                        ? Color.white : Color.clear;
                }

            tex.SetPixels(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        }

        private static void SetSerializedField(Object target, string fieldName, Object value)
        {
            var so = new SerializedObject(target);
            var prop = so.FindProperty(fieldName);
            if (prop != null)
            {
                prop.objectReferenceValue = value;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
            else
            {
                Debug.LogWarning($"Could not find serialized field '{fieldName}' on {target.GetType().Name}");
            }
        }

        private static void SetSerializedField(Object target, string fieldName, int value)
        {
            var so = new SerializedObject(target);
            var prop = so.FindProperty(fieldName);
            if (prop != null)
            {
                prop.enumValueIndex = value;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }
}
#endif
