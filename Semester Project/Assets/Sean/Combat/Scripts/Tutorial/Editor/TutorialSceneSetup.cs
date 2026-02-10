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

        // ─── Create Tutorial Panel ───
        var tutorialPanel = CreatePanel(canvas.transform, "TutorialPanel");
        tutorialPanel.SetActive(false);

        // Background
        var bg = tutorialPanel.GetComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.7f);

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

        // ─── Create Tutorial Button on Customize Panel ───
        // Find the customize panel - look for it in the menuManager's serialized fields
        var customizePanel = FindCustomizePanel(menuManager);

        if (customizePanel != null)
        {
            CreateTutorialButton(customizePanel.transform, menuManager);
        }
        else
        {
            Debug.LogWarning("Could not find customize panel. You'll need to manually add a Tutorial button.");
        }

        // ─── Create CombatTutorialManager Component ───
        var tutorialManagerGO = new GameObject("CombatTutorialManager");
        tutorialManagerGO.transform.SetParent(canvas.transform.parent);
        var tutManager = tutorialManagerGO.AddComponent<CombatTutorialManager>();

        // Wire up serialized fields via SerializedObject
        var so = new SerializedObject(tutManager);

        // Find scene objects
        var playerEnergy = Object.FindObjectOfType<PlayerEnergy>();
        var enemyEnergy = Object.FindObjectOfType<EnemyEnergy>();
        var dirIndicator = Object.FindObjectOfType<DirectionIndicator>();

        // Find fighter visuals
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

        // Find enemy sprite renderer
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

        // Wire up CombatMenuManager
        var menuSO = new SerializedObject(menuManager);
        menuSO.FindProperty("tutorialPanel").objectReferenceValue = tutorialPanel;
        menuSO.FindProperty("tutorialManager").objectReferenceValue = tutManager;
        menuSO.ApplyModifiedProperties();

        // Also add Tutorial button to CombatHUD game over panel
        var hud = Object.FindObjectOfType<CombatHUD>();
        if (hud != null)
        {
            var gameOverPanel = FindChildRecursive(hud.transform, "GameOverPanel");
            if (gameOverPanel == null)
                gameOverPanel = hud.transform; // fallback

            // Check if a tutorial button already exists
            var existingBtn = FindChildRecursive(gameOverPanel, "TutorialButton");
            if (existingBtn == null)
            {
                var tutBtn = CreateButton(gameOverPanel, "TutorialButton", "TUTORIAL",
                    new Vector2(0, -120), new Vector2(200, 50));
                tutBtn.GetComponent<Button>().onClick.AddListener(() => menuManager.ShowTutorial());
            }
        }

        EditorUtility.SetDirty(menuManager);
        EditorUtility.SetDirty(tutManager);
        Debug.Log("Tutorial UI setup complete! Don't forget to save the scene.");
    }

    private static GameObject FindCustomizePanel(CombatMenuManager menuManager)
    {
        var so = new SerializedObject(menuManager);
        var prop = so.FindProperty("customizePanel");
        if (prop != null && prop.objectReferenceValue != null)
            return prop.objectReferenceValue as GameObject;
        return null;
    }

    private static void CreateTutorialButton(Transform parent, CombatMenuManager menuManager)
    {
        // Check if button already exists
        var existing = FindChildRecursive(parent, "TutorialButton");
        if (existing != null)
        {
            Debug.Log("Tutorial button already exists on customize panel.");
            return;
        }

        var btnGO = CreateButton(parent, "TutorialButton", "TUTORIAL",
            new Vector2(0, -60), new Vector2(250, 60));

        var btn = btnGO.GetComponent<Button>();
        UnityEditor.Events.UnityEventTools.AddPersistentListener(
            btn.onClick,
            new UnityEngine.Events.UnityAction(menuManager.ShowTutorial));

        EditorUtility.SetDirty(btn);
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
        string label, Vector2 anchoredPos, Vector2 size)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);

        var rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;

        var img = go.GetComponent<Image>();
        img.color = new Color(0.2f, 0.6f, 0.9f, 1f);

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
        tmp.fontSize = 24;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.fontStyle = FontStyles.Bold;

        return go;
    }
}
#endif
