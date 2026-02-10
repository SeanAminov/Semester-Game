using System.Collections;
using UnityEngine;
using TMPro;

namespace Sean.Combat
{
    public class CombatTutorialManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerEnergy playerEnergy;
        [SerializeField] private EnemyEnergy enemyEnergy;
        [SerializeField] private FighterVisual playerVisual;
        [SerializeField] private FighterVisual enemyVisual;
        [SerializeField] private DirectionIndicator directionIndicator;
        [SerializeField] private SpriteRenderer enemySpriteRenderer;

        [Header("UI")]
        [SerializeField] private GameObject tutorialPanel;
        [SerializeField] private TextMeshProUGUI instructionText;
        [SerializeField] private TextMeshProUGUI promptText;
        [SerializeField] private GameObject continuePrompt; // "Press any key to continue"

        [Header("Settings")]
        [SerializeField] private float slowTimeScale = 0.15f;
        [SerializeField] private float telegraphDuration = 1.2f;
        [SerializeField] private float parryWindow = 0.5f;

        private CombatActions _input;
        private TutorialBeat _currentBeat;
        private bool _waitingForConfirm;
        private bool _waitingForPunch;
        private bool _waitingForParry;
        private int _punchCount;
        private int _parrySuccessCount;
        private int _requiredParries;
        private AttackDirection _expectedParryDir;
        private bool _parryActive;
        private AttackDirection _parryDirection;
        private bool _enemyAttackPending;
        private bool _tutorialActive;
        private Coroutine _currentDrill;
        private bool _punchAfterParry;
        private int _miniTestPhase; // 0=parry, 1=punch
        private Color _enemyOriginalColor;
        private bool _parryButtonHeld;

        private enum TutorialBeat
        {
            Entry,
            PunchLesson,
            PunchRepetition,
            ParryExplanation,
            ParrySingleDir,
            ParryMixedDir,
            EnergyReinforcement,
            MiniCombatTest,
            WrapUp,
            Done
        }

        private CombatRuntimeConfig Config => CombatRuntimeConfig.Instance;

        private void OnEnable()
        {
            _input = new CombatActions();
            _input.Combat.Enable();

            _input.Combat.Punch.performed += _ => OnPunchInput();
            _input.Combat.ParryUp.performed += _ => OnParryInput(AttackDirection.Up);
            _input.Combat.ParryDown.performed += _ => OnParryInput(AttackDirection.Down);
            _input.Combat.ParryLeft.performed += _ => OnParryInput(AttackDirection.Left);
            _input.Combat.ParryRight.performed += _ => OnParryInput(AttackDirection.Right);

            _input.Combat.ParryUp.canceled += _ => OnParryReleased();
            _input.Combat.ParryDown.canceled += _ => OnParryReleased();
            _input.Combat.ParryLeft.canceled += _ => OnParryReleased();
            _input.Combat.ParryRight.canceled += _ => OnParryReleased();
        }

        private void OnDisable()
        {
            _input.Combat.Disable();
            _input.Dispose();
        }

        public void StartTutorial()
        {
            _tutorialActive = true;
            tutorialPanel.SetActive(true);

            if (enemySpriteRenderer != null)
                _enemyOriginalColor = enemySpriteRenderer.color;

            // Set up tutorial-friendly config
            SetTutorialConfig();

            // Initialize energy
            playerEnergy.Initialize();
            enemyEnergy.Initialize();

            // Disable the normal enemy AI and player controllers
            var enemyAI = FindObjectOfType<EnemyCombatController>();
            if (enemyAI != null) enemyAI.enabled = false;

            var playerCtrl = FindObjectOfType<PlayerCombatController>();
            if (playerCtrl != null) playerCtrl.enabled = false;

            directionIndicator.HideAll();

            StartCoroutine(RunTutorial());
        }

        public void StopTutorial()
        {
            _tutorialActive = false;
            Time.timeScale = 1f;

            if (_currentDrill != null)
                StopCoroutine(_currentDrill);

            StopAllCoroutines();

            tutorialPanel.SetActive(false);
            directionIndicator.HideAll();
            ResetEnemyVisual();

            // Re-enable normal controllers
            var enemyAI = FindObjectOfType<EnemyCombatController>();
            if (enemyAI != null) enemyAI.enabled = true;

            var playerCtrl = FindObjectOfType<PlayerCombatController>();
            if (playerCtrl != null) playerCtrl.enabled = true;
        }

        private void SetTutorialConfig()
        {
            if (Config == null) return;
            Config.ParryWindowDuration = parryWindow;
            Config.ParryRefillsEnergy = true;
            Config.ParryEnergyRefill = 6;
            Config.EnemyAttackDamage = 5;
            Config.AttackDamage = 4;
            Config.AttackEnergyCost = 0;
            Config.PlayerStartingEnergy = 20;
            Config.EnemyStartingEnergy = 999;
            Config.DodgeEnabled = false;
            Config.BlockEnabled = false;
            Config.CritMeterEnabled = false;
            Config.PostureEnabled = false;
            Config.CombosEnabled = false;
        }

        // ─────────────────────── MAIN FLOW ───────────────────────

        private IEnumerator RunTutorial()
        {
            // Beat 0: Entry
            yield return Beat_Entry();

            // Beat 1: Punch lesson
            yield return Beat_PunchLesson();

            // Beat 2: Punch repetition
            yield return Beat_PunchRepetition();

            // Beat 3: Parry explanation
            yield return Beat_ParryExplanation();

            // Beat 4: Single direction parry drill
            yield return Beat_ParrySingleDir();

            // Beat 5: Mixed direction parry drill
            yield return Beat_ParryMixedDir();

            // Beat 6: Energy reinforcement
            yield return Beat_EnergyReinforcement();

            // Beat 7: Mini combat test
            yield return Beat_MiniCombatTest();

            // Beat 8: Wrap-up
            yield return Beat_WrapUp();
        }

        // ─────────────────── BEAT 0: ENTRY ───────────────────

        private IEnumerator Beat_Entry()
        {
            _currentBeat = TutorialBeat.Entry;
            Time.timeScale = slowTimeScale;

            SetInstruction("COMBAT TUTORIAL");
            SetPrompt(
                "Combat is about <b>reading tells</b>.\n" +
                "Parry direction must <b>match</b> the tell direction.\n" +
                "Energy reflects how well you fight."
            );
            ShowContinuePrompt(true);

            yield return WaitForAnyKey();
            ShowContinuePrompt(false);
            Time.timeScale = 1f;
        }

        // ─────────────────── BEAT 1: PUNCH LESSON ───────────────────

        private IEnumerator Beat_PunchLesson()
        {
            _currentBeat = TutorialBeat.PunchLesson;
            Time.timeScale = slowTimeScale;

            SetInstruction("PUNCH");
            SetPrompt("Press <b>Up Arrow</b> to punch.");

            _waitingForPunch = true;
            _punchCount = 0;

            while (_punchCount < 1)
                yield return null;

            _waitingForPunch = false;
            Time.timeScale = 1f;

            SetPrompt("Nice! Punching deals damage to the enemy.");
            yield return new WaitForSeconds(1.0f);
        }

        // ─────────────────── BEAT 2: PUNCH REPETITION ───────────────────

        private IEnumerator Beat_PunchRepetition()
        {
            _currentBeat = TutorialBeat.PunchRepetition;

            SetInstruction("KEEP PUNCHING");
            SetPrompt("Punch <b>3 more times</b>.");

            _waitingForPunch = true;
            _punchCount = 0;

            while (_punchCount < 3)
            {
                SetPrompt($"Punch <b>{3 - _punchCount} more time{(3 - _punchCount > 1 ? "s" : "")}</b>.");
                yield return null;
            }

            _waitingForPunch = false;

            SetPrompt("Good. Punching is your main way to deal damage.");
            yield return new WaitForSeconds(1.2f);
        }

        // ─────────────────── BEAT 3: PARRY EXPLANATION ───────────────────

        private IEnumerator Beat_ParryExplanation()
        {
            _currentBeat = TutorialBeat.ParryExplanation;
            Time.timeScale = slowTimeScale;

            SetInstruction("DIRECTIONAL PARRY");
            SetPrompt(
                "The enemy will show a <b>direction arrow</b> before attacking.\n" +
                "Press the <b>matching direction</b> (W/A/S/D) to parry.\n\n" +
                "<b>W</b> = Up   <b>A</b> = Left   <b>S</b> = Down   <b>D</b> = Right"
            );
            ShowContinuePrompt(true);

            yield return WaitForAnyKey();
            ShowContinuePrompt(false);
            Time.timeScale = 1f;
        }

        // ─────────────────── BEAT 4: SINGLE DIR PARRY ───────────────────

        private IEnumerator Beat_ParrySingleDir()
        {
            _currentBeat = TutorialBeat.ParrySingleDir;

            SetInstruction("PARRY DRILL - UP");
            SetPrompt("The enemy will attack from <b>UP</b>.\nPress <b>W</b> to parry when you see the arrow.");

            yield return new WaitForSeconds(1.0f);

            _requiredParries = 3;
            _parrySuccessCount = 0;

            while (_parrySuccessCount < _requiredParries)
            {
                SetPrompt($"Parry the UP attack! ({_parrySuccessCount}/{_requiredParries})");
                yield return RunParryDrill(AttackDirection.Up);
            }

            SetInstruction("WELL DONE!");
            SetPrompt("You parried all the attacks from above.");
            yield return new WaitForSeconds(1.2f);
        }

        // ─────────────────── BEAT 5: MIXED DIR PARRY ───────────────────

        private IEnumerator Beat_ParryMixedDir()
        {
            _currentBeat = TutorialBeat.ParryMixedDir;

            SetInstruction("MIXED PARRY DRILL");
            SetPrompt("Now the enemy will attack from <b>UP</b> or <b>LEFT</b>.\nWatch the arrow and match the direction!");

            yield return new WaitForSeconds(1.2f);

            _requiredParries = 4;
            _parrySuccessCount = 0;

            AttackDirection[] directions = { AttackDirection.Up, AttackDirection.Left };

            while (_parrySuccessCount < _requiredParries)
            {
                AttackDirection dir = directions[Random.Range(0, directions.Length)];
                SetPrompt($"Watch the tell! ({_parrySuccessCount}/{_requiredParries})");
                yield return RunParryDrill(dir);
            }

            SetInstruction("EXCELLENT!");
            SetPrompt("You can read tells and match the direction.");
            yield return new WaitForSeconds(1.2f);
        }

        // ─────────────────── BEAT 6: ENERGY REINFORCEMENT ───────────────────

        private IEnumerator Beat_EnergyReinforcement()
        {
            _currentBeat = TutorialBeat.EnergyReinforcement;
            Time.timeScale = slowTimeScale;

            SetInstruction("ENERGY");
            SetPrompt(
                "Notice your <b>energy bar</b>.\n\n" +
                "Correct parry <b>restores</b> energy.\n" +
                "Wrong parry or missed parry <b>costs</b> energy.\n" +
                "If energy hits 0, you lose."
            );
            ShowContinuePrompt(true);

            yield return WaitForAnyKey();
            ShowContinuePrompt(false);
            Time.timeScale = 1f;
        }

        // ─────────────────── BEAT 7: MINI COMBAT TEST ───────────────────

        private IEnumerator Beat_MiniCombatTest()
        {
            _currentBeat = TutorialBeat.MiniCombatTest;

            SetInstruction("COMBAT TEST");
            SetPrompt("Put it together: <b>Parry</b> the attack, then <b>Punch</b>.");

            yield return new WaitForSeconds(1.0f);

            int completedCycles = 0;
            int requiredCycles = 3;

            while (completedCycles < requiredCycles)
            {
                // Pick a random direction from all 4
                AttackDirection dir = (AttackDirection)Random.Range(0, 4);

                // Phase 1: Parry
                _miniTestPhase = 0;
                SetPrompt($"Incoming attack! Parry it! ({completedCycles}/{requiredCycles})");
                yield return RunParryDrillForTest(dir);

                if (!_tutorialActive) yield break;

                // If parry failed, loop back
                if (_miniTestPhase != 1) continue;

                // Phase 2: Punch
                SetPrompt("Now <b>PUNCH</b>! (Up Arrow)");
                _waitingForPunch = true;
                _punchCount = 0;

                float punchTimeout = 3f;
                float punchElapsed = 0f;
                while (_punchCount < 1 && punchElapsed < punchTimeout)
                {
                    punchElapsed += Time.deltaTime;
                    yield return null;
                }
                _waitingForPunch = false;

                if (_punchCount >= 1)
                {
                    completedCycles++;
                    CombatEvents.RaiseNotification("COMBO!", playerEnergy.transform.position);
                    yield return new WaitForSeconds(0.5f);
                }
            }

            SetInstruction("GREAT!");
            SetPrompt("You completed the parry-punch combo.");
            yield return new WaitForSeconds(1.2f);
        }

        // ─────────────────── BEAT 8: WRAP UP ───────────────────

        private IEnumerator Beat_WrapUp()
        {
            _currentBeat = TutorialBeat.WrapUp;
            Time.timeScale = slowTimeScale;

            SetInstruction("TUTORIAL COMPLETE");
            SetPrompt(
                "Remember:\n" +
                "  Read the <b>tell</b>.\n" +
                "  <b>Match</b> the direction.\n" +
                "  Parry <b>restores</b> energy.\n" +
                "  Punch to deal <b>damage</b>."
            );
            ShowContinuePrompt(true);

            yield return WaitForAnyKey();
            ShowContinuePrompt(false);
            Time.timeScale = 1f;

            _currentBeat = TutorialBeat.Done;

            // Return to menu
            var menuManager = FindObjectOfType<CombatMenuManager>();
            if (menuManager != null)
                menuManager.ShowCustomizeScreen();

            StopTutorial();
        }

        // ─────────────────── PARRY DRILL LOGIC ───────────────────

        private IEnumerator RunParryDrill(AttackDirection direction)
        {
            // Reset state
            _parryActive = false;
            _enemyAttackPending = false;

            yield return new WaitForSeconds(0.8f);

            // Telegraph
            directionIndicator.ShowDirection(direction);
            _expectedParryDir = direction;

            // Slow time during telegraph
            Time.timeScale = slowTimeScale;

            // Fade enemy to red
            yield return TelegraphFade(telegraphDuration);

            // Attack lands
            Time.timeScale = 1f;
            _enemyAttackPending = true;

            // Check if player was already parrying in correct direction
            bool success = _parryActive && _parryDirection == direction;

            // Brief window to still parry after attack
            if (!success)
            {
                _waitingForParry = true;
                float window = parryWindow;
                float elapsed = 0f;
                while (elapsed < window)
                {
                    if (_parryActive && _parryDirection == direction)
                    {
                        success = true;
                        break;
                    }
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                _waitingForParry = false;
            }

            _enemyAttackPending = false;
            directionIndicator.HideAll();
            ResetEnemyVisual();

            if (success)
            {
                _parrySuccessCount++;
                playerVisual.FlashColor(Color.blue, 0.3f);
                CombatEvents.RaiseNotification("PARRY!", playerEnergy.transform.position);

                if (Config.ParryRefillsEnergy)
                    playerEnergy.ModifyEnergy(Config.ParryEnergyRefill);
            }
            else
            {
                playerVisual.FlashColor(Color.red, 0.3f);
                playerEnergy.ModifyEnergy(-Config.EnemyAttackDamage);

                if (_parryActive && _parryDirection != direction)
                    CombatEvents.RaiseNotification("WRONG DIRECTION!", playerEnergy.transform.position);
                else
                    CombatEvents.RaiseNotification("TOO SLOW!", playerEnergy.transform.position);
            }

            // If player energy is too low, refill a bit so tutorial can continue
            if (playerEnergy.CurrentEnergy <= 5)
                playerEnergy.ModifyEnergy(10);

            yield return new WaitForSeconds(0.5f);
        }

        private IEnumerator RunParryDrillForTest(AttackDirection direction)
        {
            _parryActive = false;
            _enemyAttackPending = false;

            yield return new WaitForSeconds(0.8f);

            directionIndicator.ShowDirection(direction);
            _expectedParryDir = direction;

            // Only slow if this is the first attempt or player failed before
            Time.timeScale = slowTimeScale;
            yield return TelegraphFade(telegraphDuration);

            Time.timeScale = 1f;
            _enemyAttackPending = true;

            bool success = _parryActive && _parryDirection == direction;

            if (!success)
            {
                _waitingForParry = true;
                float window = parryWindow;
                float elapsed = 0f;
                while (elapsed < window)
                {
                    if (_parryActive && _parryDirection == direction)
                    {
                        success = true;
                        break;
                    }
                    elapsed += Time.deltaTime;
                    yield return null;
                }
                _waitingForParry = false;
            }

            _enemyAttackPending = false;
            directionIndicator.HideAll();
            ResetEnemyVisual();

            if (success)
            {
                _miniTestPhase = 1; // advance to punch phase
                playerVisual.FlashColor(Color.blue, 0.3f);
                CombatEvents.RaiseNotification("PARRY!", playerEnergy.transform.position);
                if (Config.ParryRefillsEnergy)
                    playerEnergy.ModifyEnergy(Config.ParryEnergyRefill);
            }
            else
            {
                _miniTestPhase = 0; // stay in parry phase
                playerVisual.FlashColor(Color.red, 0.3f);
                playerEnergy.ModifyEnergy(-Config.EnemyAttackDamage);

                if (_parryActive && _parryDirection != direction)
                    CombatEvents.RaiseNotification("WRONG DIRECTION!", playerEnergy.transform.position);
                else
                    CombatEvents.RaiseNotification("MISSED!", playerEnergy.transform.position);

                SetPrompt("Try again! Watch the arrow direction.");
            }

            if (playerEnergy.CurrentEnergy <= 5)
                playerEnergy.ModifyEnergy(10);

            yield return new WaitForSeconds(0.4f);
        }

        // ─────────────────── INPUT HANDLERS ───────────────────

        private void OnPunchInput()
        {
            if (!_tutorialActive) return;

            if (_waitingForPunch)
            {
                _punchCount++;
                playerVisual.FlashColor(Color.yellow, 0.3f);
                CombatEvents.RaisePlayerAttack(Config.AttackDamage);
                CombatEvents.RaiseNotification($"PUNCH! -{Config.AttackDamage}", playerEnergy.transform.position);
            }
        }

        private void OnParryInput(AttackDirection direction)
        {
            if (!_tutorialActive) return;

            _parryActive = true;
            _parryDirection = direction;
            _parryButtonHeld = true;
            playerVisual.FlashColor(Color.blue, Config.ParryWindowDuration);

            StartCoroutine(ParryWindowCoroutine());
        }

        private void OnParryReleased()
        {
            _parryButtonHeld = false;
        }

        private IEnumerator ParryWindowCoroutine()
        {
            yield return new WaitForSeconds(Config.ParryWindowDuration);
            if (!_enemyAttackPending)
                _parryActive = false;
        }

        // ─────────────────── UTILITY ───────────────────

        private IEnumerator WaitForAnyKey()
        {
            // Small delay to prevent accidental skip
            yield return new WaitForSecondsRealtime(0.3f);

            while (!Input.anyKeyDown)
                yield return null;
        }

        private IEnumerator TelegraphFade(float duration)
        {
            if (enemySpriteRenderer == null) yield break;

            float elapsed = 0f;
            Color startColor = _enemyOriginalColor;
            Color endColor = Color.red;

            // Use real time so the fade looks correct even when timeScale is low
            float realDuration = duration;

            while (elapsed < realDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / realDuration);
                enemySpriteRenderer.color = Color.Lerp(startColor, endColor, t);
                yield return null;
            }

            enemySpriteRenderer.color = endColor;
        }

        private void ResetEnemyVisual()
        {
            if (enemySpriteRenderer != null)
                enemySpriteRenderer.color = _enemyOriginalColor;
        }

        private void SetInstruction(string text)
        {
            if (instructionText != null)
                instructionText.text = text;
        }

        private void SetPrompt(string text)
        {
            if (promptText != null)
                promptText.text = text;
        }

        private void ShowContinuePrompt(bool show)
        {
            if (continuePrompt != null)
                continuePrompt.SetActive(show);
        }
    }
}
