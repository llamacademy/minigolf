using UnityEngine.UIElements;

namespace LlamAcademy.Minigolf.UI.Modals.About
{
    public class HowToPlay : UIView
    {
        private Label CloseButton;

        public HowToPlay(VisualElement root)
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
        }

        protected override void SetVisualElements()
        {
            CloseButton = Root.Q<Label>("close-button");
        }

        private void UnregisterButtonCallbacks()
        {
            CloseButton.UnregisterCallback<ClickEvent>(Hide);
        }

        private void Hide(ClickEvent evt)
        {
            Hide();
        }
    }
}
