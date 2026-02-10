#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;
using Sean.Combat;

public class TutorialSceneSetup : EditorWindow
{
    [MenuItem("Sean/Combat/Setup Tutorial UI")]
    public static void SetupTutorial()
    {
        // Find required scene objects
        var canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No Canvas found in scene. Please open the CombatDemo scene first.");
            return;
        }

        var menuManager = Object.FindObjectOfType<CombatMenuManager>();
        if (menuManager == null)
        {
            Debug.LogError("No CombatMenuManager found in scene.");
            return;
        }

        var menuSO = new SerializedObject(menuManager);

        // ─── Create Main Menu Panel (if it doesn't exist) ───
        var existingMainMenu = menuSO.FindProperty("mainMenuPanel").objectReferenceValue as GameObject;
        if (existingMainMenu == null)
        {
            var mainMenuPanel = CreatePanel(canvas.transform, "MainMenuPanel");

            // Dark background
            var bg = mainMenuPanel.GetComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);

            // Title
            CreateTextObj(mainMenuPanel.transform, "TitleText",
                "COMBAT DEMO", 52, TextAlignmentOptions.Center,
                new Vector2(0, 140), new Vector2(800, 80));

            // Play button
            var playBtn = CreateButton(mainMenuPanel.transform, "PlayButton", "PLAY",
                new Vector2(0, 30), new Vector2(300, 70),
                new Color(0.2f, 0.7f, 0.2f, 1f));
            playBtn.GetComponent<Button>().onClick.AddListener(() => { });
            UnityEditor.Events.UnityEventTools.AddPersistentListener(
                playBtn.GetComponent<Button>().onClick,
                new UnityEngine.Events.UnityAction(menuManager.ShowPlayScreen));

            // Customize button
            var customizeBtn = CreateButton(mainMenuPanel.transform, "CustomizeButton", "CUSTOMIZE",
                new Vector2(0, -60), new Vector2(300, 70),
                new Color(0.2f, 0.5f, 0.8f, 1f));
            customizeBtn.GetComponent<Button>().onClick.AddListener(() => { });
            UnityEditor.Events.UnityEventTools.AddPersistentListener(
                customizeBtn.GetComponent<Button>().onClick,
                new UnityEngine.Events.UnityAction(menuManager.ShowCustomizeScreen));

            // Tutorial button
            var tutorialBtn = CreateButton(mainMenuPanel.transform, "TutorialButton", "TUTORIAL",
                new Vector2(0, -150), new Vector2(300, 70),
                new Color(0.8f, 0.6f, 0.1f, 1f));
            tutorialBtn.GetComponent<Button>().onClick.AddListener(() => { });
            UnityEditor.Events.UnityEventTools.AddPersistentListener(
                tutorialBtn.GetComponent<Button>().onClick,
                new UnityEngine.Events.UnityAction(menuManager.ShowTutorial));

            menuSO.FindProperty("mainMenuPanel").objectReferenceValue = mainMenuPanel;
            menuSO.ApplyModifiedProperties();

            Debug.Log("Main menu panel created with Play, Customize, and Tutorial buttons.");
        }
        else
        {
            Debug.Log("Main menu panel already exists, skipping.");
        }

        // ─── Create Tutorial Panel (if it doesn't exist) ───
        var existingTutPanel = menuSO.FindProperty("tutorialPanel").objectReferenceValue as GameObject;
        if (existingTutPanel == null)
        {
            var tutorialPanel = CreatePanel(canvas.transform, "TutorialPanel");
            tutorialPanel.SetActive(false);

            var tbg = tutorialPanel.GetComponent<Image>();
            tbg.color = new Color(0, 0, 0, 0.7f);

            // Instruction text (top header)
            var instructionGO = CreateTextObj(tutorialPanel.transform, "InstructionText",
                "TUTORIAL", 42, TextAlignmentOptions.Top, new Vector2(0, 120), new Vector2(800, 80));
            var instructionText = instructionGO.GetComponent<TextMeshProUGUI>();
            instructionText.fontStyle = FontStyles.Bold;

            // Prompt text (main body)
            var promptGO = CreateTextObj(tutorialPanel.transform, "PromptText",
                "", 28, TextAlignmentOptions.Center, new Vector2(0, -20), new Vector2(800, 300));

            // Continue prompt
            var continueGO = CreateTextObj(tutorialPanel.transform, "ContinuePrompt",
                "[ Press any key to continue ]", 22, TextAlignmentOptions.Bottom, new Vector2(0, -180), new Vector2(600, 50));
            var continueText = continueGO.GetComponent<TextMeshProUGUI>();
            continueText.color = new Color(1f, 1f, 1f, 0.6f);
            continueText.fontStyle = FontStyles.Italic;

            // ─── Create CombatTutorialManager Component ───
            var tutorialManagerGO = new GameObject("CombatTutorialManager");
            tutorialManagerGO.transform.SetParent(canvas.transform.parent);
            var tutManager = tutorialManagerGO.AddComponent<CombatTutorialManager>();

            // Wire up serialized fields
            var so = new SerializedObject(tutManager);

            var playerEnergy = Object.FindObjectOfType<PlayerEnergy>();
            var enemyEnergy = Object.FindObjectOfType<EnemyEnergy>();
            var dirIndicator = Object.FindObjectOfType<DirectionIndicator>();

            FighterVisual playerVisual = null;
            FighterVisual enemyVisual = null;
            var allVisuals = Object.FindObjectsOfType<FighterVisual>();
            foreach (var v in allVisuals)
            {
                if (v.GetComponent<PlayerCombatController>() != null ||
                    v.GetComponentInParent<PlayerCombatController>() != null ||
                    (playerEnergy != null && v.gameObject == playerEnergy.gameObject))
                    playerVisual = v;
                else
                    enemyVisual = v;
            }

            SpriteRenderer enemySprite = null;
            var enemyAttackTelegraph = Object.FindObjectOfType<EnemyAttackTelegraph>();
            if (enemyAttackTelegraph != null)
                enemySprite = enemyAttackTelegraph.GetComponent<SpriteRenderer>();
            if (enemySprite == null && enemyVisual != null)
                enemySprite = enemyVisual.GetComponent<SpriteRenderer>();

            so.FindProperty("playerEnergy").objectReferenceValue = playerEnergy;
            so.FindProperty("enemyEnergy").objectReferenceValue = enemyEnergy;
            so.FindProperty("playerVisual").objectReferenceValue = playerVisual;
            so.FindProperty("enemyVisual").objectReferenceValue = enemyVisual;
            so.FindProperty("directionIndicator").objectReferenceValue = dirIndicator;
            so.FindProperty("enemySpriteRenderer").objectReferenceValue = enemySprite;
            so.FindProperty("tutorialPanel").objectReferenceValue = tutorialPanel;
            so.FindProperty("instructionText").objectReferenceValue = instructionText;
            so.FindProperty("promptText").objectReferenceValue = promptGO.GetComponent<TextMeshProUGUI>();
            so.FindProperty("continuePrompt").objectReferenceValue = continueGO;
            so.ApplyModifiedProperties();

            // Wire tutorial into menu manager
            menuSO = new SerializedObject(menuManager);
            menuSO.FindProperty("tutorialPanel").objectReferenceValue = tutorialPanel;
            menuSO.FindProperty("tutorialManager").objectReferenceValue = tutManager;
            menuSO.ApplyModifiedProperties();

            EditorUtility.SetDirty(tutManager);
            Debug.Log("Tutorial panel and CombatTutorialManager created.");
        }
        else
        {
            Debug.Log("Tutorial panel already exists, skipping.");
        }

        // ─── Add a "Back to Menu" button on the Customize panel ───
        var customizePanel = menuSO.FindProperty("customizePanel").objectReferenceValue as GameObject;
        if (customizePanel != null)
        {
            var existingBack = FindChildRecursive(customizePanel.transform, "BackToMenuButton");
            if (existingBack == null)
            {
                var backBtn = CreateButton(customizePanel.transform, "BackToMenuButton", "< BACK",
                    new Vector2(-300, -60), new Vector2(160, 50),
                    new Color(0.5f, 0.5f, 0.5f, 1f));
                UnityEditor.Events.UnityEventTools.AddPersistentListener(
                    backBtn.GetComponent<Button>().onClick,
                    new UnityEngine.Events.UnityAction(menuManager.ShowMainMenu));
                Debug.Log("Back button added to customize panel.");
            }
        }

        EditorUtility.SetDirty(menuManager);
        Debug.Log("Tutorial setup complete! Save the scene (Ctrl+S).");
    }

    private static Transform FindChildRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            var found = FindChildRecursive(child, name);
            if (found != null) return found;
        }
        return null;
    }

    private static GameObject CreatePanel(Transform parent, string name)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        return go;
    }

    private static GameObject CreateTextObj(Transform parent, string name,
        string text, int fontSize, TextAlignmentOptions alignment,
        Vector2 anchoredPos, Vector2 size)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);

        var rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;

        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.color = Color.white;
        tmp.enableWordWrapping = true;
        tmp.richText = true;

        return go;
    }

    private static GameObject CreateButton(Transform parent, string name,
        string label, Vector2 anchoredPos, Vector2 size,
        Color? buttonColor = null)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);

        var rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;

        var img = go.GetComponent<Image>();
        img.color = buttonColor ?? new Color(0.2f, 0.6f, 0.9f, 1f);

        // Label
        var labelGO = new GameObject("Label", typeof(RectTransform));
        labelGO.transform.SetParent(go.transform, false);

        var labelRT = labelGO.GetComponent<RectTransform>();
        labelRT.anchorMin = Vector2.zero;
        labelRT.anchorMax = Vector2.one;
        labelRT.offsetMin = Vector2.zero;
        labelRT.offsetMax = Vector2.zero;

        var tmp = labelGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 28;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.fontStyle = FontStyles.Bold;

        return go;
    }
}
#endif
