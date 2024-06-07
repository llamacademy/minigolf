using Cinemachine;
using LlamAcademy.Minigolf.UI.Modals.About;
using LlamAcademy.Minigolf.UI.Modals.LevelSelection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace LlamAcademy.Minigolf.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera VirtualCamera;
        [SerializeField] [Range(0.1f, 5f)] private float RotationSpeed = 1f;
        /// <summary>
        /// Both MainMenu and Game scenes should refer to this same single SO so they can use it to pass data between
        /// the scenes without static classes.
        /// </summary>
        [SerializeField] private LevelDataSO LevelData;
        private UIDocument Document;
        private Button PlayGameButton => Document.rootVisualElement.Q<Button>("play-game-button");
        private Button AboutGameButton => Document.rootVisualElement.Q<Button>("about-game-button");
        private Button ExitGameButton => Document.rootVisualElement.Q<Button>("exit-game-button");

        private LevelSelection LevelSelectionModal;
        private About AboutModal;
        private CinemachineOrbitalTransposer Transposer;

        private void Awake()
        {
            Document = GetComponent<UIDocument>();

            PlayGameButton.RegisterCallback<ClickEvent>(ShowLevelPopup);
            AboutGameButton.RegisterCallback<ClickEvent>(ShowAboutGamePopup);
            ExitGameButton.RegisterCallback<ClickEvent>(ExitGame);

            LevelSelectionModal = new LevelSelection(Document.rootVisualElement.Q<VisualElement>("level-selection"));
            AboutModal = new About(Document.rootVisualElement.Q<VisualElement>("about-game"));

            LevelSelectionModal.OnLevelSelected += OnLevelSelected;

            Transposer = VirtualCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
        }

        private void OnLevelSelected(LevelSO levelData)
        {
            LevelData.Level = levelData;
            SceneManager.LoadScene(Constants.GAME_SCENE_NAME);
        }

        private void Update()
        {
            Transposer.m_XAxis.Value += Time.deltaTime * RotationSpeed;
        }

        private void ShowLevelPopup(ClickEvent _ = null)
        {
            LevelSelectionModal.Show();
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
