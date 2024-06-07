using LlamAcademy.Minigolf.UI.Modals.About;
using LlamAcademy.Minigolf.UI.Modals.LevelSelection;
using UnityEngine;
using UnityEngine.UIElements;

namespace LlamAcademy.Minigolf.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class MainMenu : MonoBehaviour
    {
        private UIDocument Document;
        private Button PlayGameButton => Document.rootVisualElement.Q<Button>("play-game-button");
        private Button AboutGameButton => Document.rootVisualElement.Q<Button>("about-game-button");
        private Button ExitGameButton => Document.rootVisualElement.Q<Button>("exit-game-button");

        private LevelSelection LevelSelectionModal;
        private About AboutModal;

        private void Awake()
        {
            Document = GetComponent<UIDocument>();

            PlayGameButton.RegisterCallback<ClickEvent>(ShowLevelPopup);
            AboutGameButton.RegisterCallback<ClickEvent>(ShowAboutGamePopup);
            ExitGameButton.RegisterCallback<ClickEvent>(ExitGame);

            LevelSelectionModal = new LevelSelection(Document.rootVisualElement.Q<VisualElement>("level-selection"));
            AboutModal = new About(Document.rootVisualElement.Q<VisualElement>("about-game"));
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
