using LlamAcademy.Minigolf.LevelManagement;
using LlamAcademy.Minigolf.Persistence;
using LlamAcademy.Minigolf.UI.Modals.About;
using LlamAcademy.Minigolf.UI.Modals.LevelSelection;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace LlamAcademy.Minigolf.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera CinemachineCamera;
        [SerializeField] [Range(0.1f, 5f)] private float RotationSpeed = 1f;
        /// <summary>
        /// Both MainMenu and Game scenes should refer to this same single SO so they can use it to pass data between
        /// the scenes without static classes.
        /// </summary>
        [SerializeField] private LevelDataSO LevelData;
        private UIDocument Document;
        private Button PlayGameButton => Document.rootVisualElement.Q<Button>("play-game-button");
        private Button AboutGameButton => Document.rootVisualElement.Q<Button>("about-game-button");
        private Button TutorialButton => Document.rootVisualElement.Q<Button>("how-to-play-button");
        private Button ExitGameButton => Document.rootVisualElement.Q<Button>("exit-game-button");

        private LevelSelection LevelSelectionModal;
        private About AboutModal;
        private HowToPlay TutorialModal;
        private CinemachineOrbitalFollow Transposer;
        private PlayerLevelCompletionData LevelCompletionData;

        private void Awake()
        {
            Application.targetFrameRate = 30;
            Document = GetComponent<UIDocument>();

            PlayGameButton.RegisterCallback<ClickEvent>(ShowLevelPopup);
            AboutGameButton.RegisterCallback<ClickEvent>(ShowAboutGamePopup);
            TutorialButton.RegisterCallback<ClickEvent>(ShowTutorialPopup);
            ExitGameButton.RegisterCallback<ClickEvent>(ExitGame);

            LevelCompletionData = SavedDataService.LoadData();

            if (LevelData.AllLevels.Length == 0)
            {
                LevelData.AllLevels = Resources.LoadAll<LevelSO>("Levels/");
                System.Array.Sort(LevelData.AllLevels, (a,b) => a.name.CompareTo(b.name));
            }

            LevelSelectionModal = new LevelSelection(
                Document.rootVisualElement.Q<VisualElement>("level-selection"),
                LevelData.AllLevels,
                LevelCompletionData
            );
            AboutModal = new About(Document.rootVisualElement.Q<VisualElement>("about-game"));
            TutorialModal = new HowToPlay(Document.rootVisualElement.Q<VisualElement>("tutorial"));
            LevelSelectionModal.OnLevelSelected += OnLevelSelected;

            Transposer = CinemachineCamera.GetComponent<CinemachineOrbitalFollow>();
        }

        private void OnLevelSelected(LevelSO levelData)
        {
            LevelData.Level = levelData;
            SceneManager.LoadScene(Constants.GAME_SCENE_NAME);
        }

        private void Update()
        {
            Transposer.HorizontalAxis.Value += Time.deltaTime * RotationSpeed;
        }

        private void ShowLevelPopup(ClickEvent _ = null)
        {
            LevelSelectionModal.Show();
        }

        private void ShowTutorialPopup(ClickEvent _ = null)
        {
            TutorialModal.Show();
        }

        private void ShowAboutGamePopup(ClickEvent _ = null)
        {
            AboutModal.Show();
        }

        private void ExitGame(ClickEvent _ = null)
        {
            Application.Quit();
        }
    }
}
