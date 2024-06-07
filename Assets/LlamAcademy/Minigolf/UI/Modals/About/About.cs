using UnityEngine.Device;
using UnityEngine.UIElements;

namespace LlamAcademy.Minigolf.UI.Modals.About
{
    public class About : UIView
    {
        private Label CloseButton;
        private Button PatreonButton;
        private Button TutorialsButton;
        private Button CodeButton;

        public About(VisualElement root)
        {
            IsOverlay = true;
            HideOnAwake = true;
            Initialize(root);
        }

        public override void Dispose()
        {
            UnregisterButtonCallbacks();
        }

        protected override void RegisterButtonCallbacks()
        {
            CloseButton.RegisterCallback<ClickEvent>(Hide);
            PatreonButton.RegisterCallback<ClickEvent>(OpenPatreonPage);
            TutorialsButton.RegisterCallback<ClickEvent>(OpenYouTubePage);
            CodeButton.RegisterCallback<ClickEvent>(OpenGitHubPage);
        }

        protected override void SetVisualElements()
        {
            CloseButton = Root.Q<Label>("close-button");
            PatreonButton = Root.Q<Button>("patreon-button");
            TutorialsButton = Root.Q<Button>("tutorials-button");
            CodeButton = Root.Q<Button>("code-button");
        }

        private void UnregisterButtonCallbacks()
        {
            CloseButton.UnregisterCallback<ClickEvent>(Hide);
            PatreonButton.UnregisterCallback<ClickEvent>(OpenPatreonPage);
            TutorialsButton.UnregisterCallback<ClickEvent>(OpenYouTubePage);
            CodeButton.UnregisterCallback<ClickEvent>(OpenGitHubPage);
        }

        private void Hide(ClickEvent evt)
        {
            Hide();
        }

        private void OpenGitHubPage(ClickEvent evt)
        {
            Application.OpenURL("https://github.com/llamacademy/minigolf");
        }

        private void OpenYouTubePage(ClickEvent evt)
        {
            Application.OpenURL("https://youtube.com/@llamacademy");
        }

        private void OpenPatreonPage(ClickEvent evt)
        {
            Application.OpenURL("https://patreon.com/llamacademy");
        }
    }
}
