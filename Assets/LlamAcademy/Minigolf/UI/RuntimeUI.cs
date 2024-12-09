using System.Collections;
using System.Linq;
using LlamAcademy.Minigolf.Bus;
using LlamAcademy.Minigolf.Bus.Events;
using LlamAcademy.Minigolf.LevelManagement;
using LlamAcademy.Minigolf.Persistence;
using LlamAcademy.Minigolf.UI.Modals.LevelSelection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

namespace LlamAcademy.Minigolf.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class RuntimeUI : MonoBehaviour
    {
        private UIDocument Document;

        private VisualElement MenuContainer => Document.rootVisualElement.Q("menu-container");
        private Button ResumeButton => Document.rootVisualElement.Q<Button>("resume-button");
        private Button ResetButton => Document.rootVisualElement.Q<Button>("reset-button");
        private Button LevelSelectButton => Document.rootVisualElement.Q<Button>("level-selection-button");
        private Button NextLevelButton => Document.rootVisualElement.Q<Button>("next-level-button");
        private Button MainMenuButton => Document.rootVisualElement.Q<Button>("main-menu-button");
        private Button PauseButton => Document.rootVisualElement.Q<Button>("menu-button");

        private Label Stars3RatingLabel => Document.rootVisualElement.Q<Label>("stars-3-par");
        private Label Stars2RatingLabel => Document.rootVisualElement.Q<Label>("stars-2-par");
        private Label Stars1RatingLabel => Document.rootVisualElement.Q<Label>("stars-1-par");
        private Label Stars0RatingLabel => Document.rootVisualElement.Q<Label>("stars-0-par");
        private Label LevelLabel => Document.rootVisualElement.Q<Label>("level-label");

        private Label CurrentStrokesLabel => Document.rootVisualElement.Q<Label>("current-strokes");

        /// <summary>
        /// Both MainMenu and Game scenes should refer to this same single SO so they can use it to pass data between
        /// the scenes without static classes.
        /// </summary>
        [SerializeField] private LevelDataSO LevelData;

        private int CurrentStrokes = 0;
        private bool BallIsInHole = false;
        private LevelSelection LevelSelectionModal;

        private Par PerfectRating;
        private Par GoodRating;
        private Par OkRating;
        private Par FailRating;

        private PlayerLevelCompletionData LevelCompletionData;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            Document = GetComponent<UIDocument>();

            LevelCompletionData = SavedDataService.LoadData();

            LevelSelectionModal = new LevelSelection(
                Document.rootVisualElement.Q<VisualElement>("level-selection"),
                LevelData.AllLevels,
                LevelCompletionData
            );
            LevelSelectionModal.OnLevelSelected += OnLevelSelected;

            ResumeButton.RegisterCallback<ClickEvent>(ResumeGame);
            LevelSelectButton.RegisterCallback<ClickEvent>(ShowLevelSelection);
            NextLevelButton.RegisterCallback<ClickEvent>(GoToNextLevel);
            MainMenuButton.RegisterCallback<ClickEvent>(ReturnToMainMenu);
            PauseButton.RegisterCallback<ClickEvent>(ShowMenu);
            ResetButton.RegisterCallback<ClickEvent>(ResetLevel);

            PerfectRating = LevelData.Level.Par.First(par => par.Rating == Rating.Perfect);
            GoodRating = LevelData.Level.Par.First(par => par.Rating == Rating.Good);
            OkRating = LevelData.Level.Par.First(par => par.Rating == Rating.Ok);
            FailRating = LevelData.Level.Par.First(par => par.Rating == Rating.Fail);

            if (PerfectRating == null || GoodRating == null || OkRating == null || FailRating == null)
            {
                Debug.LogError($"Level {LevelData.Level.name} is misconfigured! There are not all 4 par ratings available!");
                SceneManager.LoadScene(Constants.MAIN_MENU_SCENE_NAME);
                return;
            }

            Stars3RatingLabel.text = PerfectRating.Strokes.ToString();
            Stars2RatingLabel.text = GoodRating.Strokes.ToString();
            Stars1RatingLabel.text = OkRating.Strokes.ToString();
            Stars0RatingLabel.text = FailRating.Strokes.ToString();

            LevelLabel.text = $"Level <b>{LevelData.Level.name}</b>";

            if (GetNextLevel() == null)
            {
                NextLevelButton.RemoveFromHierarchy();
            }

            EventBus<PlayerStrokeEvent>.OnEvent += HandlePlayerStroke;
            EventBus<BallInHoleEvent>.OnEvent += HandleEndLevel;
        }

        private LevelSO GetNextLevel()
        {
            int level = int.Parse(LevelData.Level.name);
            int nextLevel = level + 1;
            return System.Array.Find(LevelData.AllLevels, (levelData) => levelData.name == nextLevel.ToString());
        }

        private void GoToNextLevel(ClickEvent evt)
        {
            LevelSO nextLevelData = GetNextLevel();

            if (nextLevelData == null)
            {
                Debug.LogError("Player completed the game but next level button was still shown!");
                SceneManager.LoadScene(Constants.MAIN_MENU_SCENE_NAME);
            }
            else
            {
                LevelData.Level = nextLevelData;
                SceneManager.LoadScene(Constants.GAME_SCENE_NAME);
            }
        }

        private void HandleEndLevel(BallInHoleEvent evt)
        {
            BallIsInHole = true;

            if (NextLevelButton != null)
            {
                NextLevelButton.AddToClassList("primary");
                NextLevelButton.RemoveFromClassList("secondary");
            }

            StartCoroutine(HandleEndLevel());
        }

        private IEnumerator HandleEndLevel()
        {
            // Small delay for the win effect to play
            yield return new WaitForSeconds(1f);

            string levelName = LevelData.Level.name;
            if (LevelCompletionData.LevelSaveData.TryGetValue(levelName, out int bestScore) && bestScore > CurrentStrokes)
            {
                UpdateSaveData(levelName);
            }
            else if (LevelCompletionData.LevelSaveData.TryAdd(levelName, CurrentStrokes))
            {
                UpdateSaveData(levelName);
            }

            // you can add a more fancy end game screen, for our microgame we'll just show the pause menu without
            // a resume option
            if (ResumeButton != null)
            {
                ResumeButton.RemoveFromHierarchy();
                ShowMenu(null);
            }
        }

        private void UpdateSaveData(string levelName)
        {
            LevelCompletionData.LevelSaveData[levelName] = CurrentStrokes;
            SavedDataService.SaveData(LevelCompletionData);
        }

        private void ResetLevel(ClickEvent evt)
        {
            SceneManager.LoadScene(Constants.GAME_SCENE_NAME);
        }

        private void HandlePlayerStroke(PlayerStrokeEvent evt)
        {
            CurrentStrokes++;
            CurrentStrokesLabel.text = CurrentStrokes.ToString();

            if (CurrentStrokes == FailRating.Strokes)
            {
                EventBus<BallSettledEvent>.OnEvent -= HandleFinalBallSettle;
                EventBus<BallSettledEvent>.OnEvent += HandleFinalBallSettle;
            }
        }

        private void HandleFinalBallSettle(BallSettledEvent evt)
        {
            EventBus<BallSettledEvent>.OnEvent -= HandleFinalBallSettle;
            if (!BallIsInHole)
            {
                ResumeButton.RemoveFromHierarchy();
                ShowMenu(null);
            }
        }

        private void OnDestroy()
        {
            EventBus<PlayerStrokeEvent>.OnEvent -= HandlePlayerStroke;
            EventBus<BallInHoleEvent>.OnEvent -= HandleEndLevel;
            EventBus<BallSettledEvent>.OnEvent -= HandleFinalBallSettle;
        }

        private void ReturnToMainMenu(ClickEvent evt)
        {
            SceneManager.LoadScene(Constants.MAIN_MENU_SCENE_NAME);
        }

        private void ShowMenu(ClickEvent evt)
        {
            MenuContainer.AddToClassList("visible");
            MenuContainer.RemoveFromClassList("hidden");
            MenuContainer.pickingMode = PickingMode.Position;

            EventBus<PauseEvent>.Raise(new PauseEvent());
        }

        private void ShowLevelSelection(ClickEvent evt)
        {
            LevelSelectionModal.Show();
        }

        private void OnLevelSelected(LevelSO levelData)
        {
            LevelData.Level = levelData;
            SceneManager.LoadScene(Constants.GAME_SCENE_NAME);
        }

        private void ResumeGame(ClickEvent evt)
        {
            MenuContainer.AddToClassList("hidden");
            MenuContainer.RemoveFromClassList("visible");
            MenuContainer.pickingMode = PickingMode.Position;
            EventBus<ResumeEvent>.Raise(new ResumeEvent());
        }
    }
}
