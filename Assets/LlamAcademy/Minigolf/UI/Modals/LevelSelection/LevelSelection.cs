using System.Collections.Generic;
using LlamAcademy.Minigolf.UI.Components;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace LlamAcademy.Minigolf.UI.Modals.LevelSelection
{
    public class LevelSelection : UIView
    {
        private ScrollView ScrollView;
        private Label CloseButton;
        private VisualTreeAsset LevelPrefab;
        private List<Level> AvailableLevels;

        private List<UILevel> UILevels;

        public LevelSelection(VisualElement root)
        {
            HideOnAwake = true;
            IsOverlay = true;

            LevelPrefab = Resources.Load<VisualTreeAsset>("UILevel");
            AvailableLevels = new List<Level>(Resources.LoadAll<Level>("Levels/"));

            Initialize(root);
        }

        protected override void SetVisualElements()
        {
            UILevels = new List<UILevel>(AvailableLevels.Count);
            ScrollView = Root.Q<ScrollView>();
            CloseButton = Root.Q<Label>("close-button");

            foreach (Level level in AvailableLevels)
            {
                VisualElement uiLevelRoot = new();
                LevelPrefab.CloneTree(uiLevelRoot);
                UILevel uiLevel = new(uiLevelRoot, level);

                uiLevel.Root.RegisterCallback<ClickEvent, Level>(HandleUIClick, level);

                ScrollView.Add(uiLevel.Root);
                UILevels.Add(uiLevel);
            }
        }

        private void HandleUIClick(ClickEvent evt, Level levelData)
        {
            SceneManager.LoadScene("LlamAcademy/Minigolf/Scenes/Game");
        }

        public override void Show()
        {
            base.Show();
            Root.pickingMode = PickingMode.Position;
        }

        public override void Hide()
        {
            base.Hide();
            Root.pickingMode = PickingMode.Ignore;
        }

        protected override void RegisterButtonCallbacks()
        {
            CloseButton.RegisterCallback<ClickEvent>(Hide);
        }

        private void Hide(ClickEvent evt)
        {
            Hide();
        }

        public override void Dispose()
        {
            UnregisterButtonCallbacks();
            UILevels.Clear();
            ScrollView.Clear();
        }

        private void UnregisterButtonCallbacks()
        {
            foreach (UILevel uiLevel in UILevels)
            {
                uiLevel.Root.UnregisterCallback<ClickEvent, Level>(HandleUIClick);
            }

            CloseButton.UnregisterCallback<ClickEvent>(Hide);
        }
    }
}
