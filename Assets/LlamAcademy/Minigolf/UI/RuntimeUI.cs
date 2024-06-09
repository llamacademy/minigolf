using System.Collections;
using System.Linq;
using LlamAcademy.Minigolf.Bus;
using LlamAcademy.Minigolf.Bus.Events;
using LlamAcademy.Minigolf.LevelManagement;
using LlamAcademy.Minigolf.Persistence;
using LlamAcademy.Minigolf.UI.Modals.LevelSelection;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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
        private Button MainMenuButton => Document.rootVisualElement.Q<Button>("main-menu-button");
        private Button PauseButton => Document.rootVisualElement.Q<Button>("menu-button");

        private Label Stars3RatingLabel => Document.rootVisualElement.Q<Label>("stars-3-par");
        private Label Stars2RatingLabel => Document.rootVisualElement.Q<Label>("stars-2-par");
        private Label Stars1RatingLabel => Document.rootVisualElement.Q<Label>("stars-1-par");
        private Label Stars0RatingLabel => Document.rootVisualElement.Q<Label>("stars-0-par");

        private Label CurrentStrokesLabel => Document.rootVisualElement.Q<Label>("current-strokes");

        private LevelSelection LevelSelectionModal;

        /// <summary>
        /// Both MainMenu and Game scenes should refer to this same single SO so they can use it to pass data between
        /// the scenes without static classes.
        /// </summary>
        [SerializeField] private LevelDataSO LevelData;

        private int CurrentStrokes = 0;
        private bool BallIsInHole = false;

        private Par PerfectRating;
        private Par GoodRating;
        private Par OkRating;
        private Par FailRating;

        private PlayerLevelCompletionData LevelCompletionData;

        private void Awake()
        {
            Document = GetComponent<UIDocument>();

            LevelCompletionData = SavedDataService.LoadData();
            LevelSelectionModal = new LevelSelection(Document.rootVisualElement.Q<VisualElement>("level-selection"), LevelCompletionData);
            LevelSelectionModal.OnLevelSelected += OnLevelSelected;

            ResumeButton.RegisterCallback<ClickEvent>(ResumeGame);
            LevelSelectButton.RegisterCallback<ClickEvent>(ShowLevelSelection);
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

            EventBus<PlayerStrokeEvent>.OnEvent += HandlePlayerStroke;
            EventBus<BallInHoleEvent>.OnEvent += HandleEndLevel;
        }

        private void HandleEndLevel(BallInHoleEvent evt)
        {
            BallIsInHole = true;
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
            ResumeButton.RemoveFromHierarchy();
            ShowMenu(null);
        }

        private void UpdateSaveData(string levelName)
        {
            LevelCompletionData.LevelSaveData[levelName] = CurrentStrokes;
            LevelSelectionModal.UpdateLevelLabels(LevelCompletionData);
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

        private void ResumeGame(ClickEvent evt)
        {
            MenuContainer.AddToClassList("hidden");
            MenuContainer.RemoveFromClassList("visible");
            MenuContainer.pickingMode = PickingMode.Position;
            EventBus<ResumeEvent>.Raise(new ResumeEvent());
        }

        private void OnLevelSelected(LevelSO levelData)
        {
            LevelData.Level = levelData;
            SceneManager.LoadScene(Constants.GAME_SCENE_NAME);
        }
    }
}
