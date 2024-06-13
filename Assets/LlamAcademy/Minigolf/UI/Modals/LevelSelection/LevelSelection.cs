using System.Collections.Generic;
using LlamAcademy.Minigolf.LevelManagement;
using LlamAcademy.Minigolf.UI.Components;
using UnityEngine;
using UnityEngine.UIElements;

namespace LlamAcademy.Minigolf.UI.Modals.LevelSelection
{
    public class LevelSelection : UIView
    {
        private ScrollView ScrollView;
        private Label CloseButton;
        private VisualTreeAsset LevelPrefab;
        private LevelSO[] AvailableLevels;

        public delegate void LevelSelectedEvent(LevelSO levelData);
        public event LevelSelectedEvent OnLevelSelected;

        private List<UILevel> UILevels;
        private bool IsDragging;
        private PlayerLevelCompletionData LevelCompletionData;

        public LevelSelection(VisualElement root, LevelSO[] availableLevels, PlayerLevelCompletionData data)
        {
            HideOnAwake = true;
            IsOverlay = true;

            LevelPrefab = Resources.Load<VisualTreeAsset>("UILevel");
            AvailableLevels = availableLevels;

            LevelCompletionData = data;
            Initialize(root);
        }

        protected override void SetVisualElements()
        {
            UILevels = new List<UILevel>(AvailableLevels.Length);
            ScrollView = Root.Q<ScrollView>();
            CloseButton = Root.Q<Label>("close-button");

            foreach (LevelSO level in AvailableLevels)
            {
                VisualElement uiLevelRoot = new();
                LevelPrefab.CloneTree(uiLevelRoot);
                int? bestScore = null;

                if (LevelCompletionData.LevelSaveData.TryGetValue(level.name, out int levelScore))
                {
                    bestScore = levelScore;
                }

                UILevel uiLevel = new(uiLevelRoot, level, bestScore);

                uiLevel.Root.RegisterCallback<ClickEvent, LevelSO>(HandleUIClick, level);

                uiLevel.Root.RegisterCallback<MouseDownEvent>(HandleMouseDownDrag);
                uiLevel.Root.RegisterCallback<MouseUpEvent>(HandleMouseUpDrag);
                uiLevel.Root.RegisterCallback<MouseMoveEvent>(HandleMouseMove);

                ScrollView.Add(uiLevel.Root);
                UILevels.Add(uiLevel);
            }

            ScrollView.ScrollTo(UILevels[0].Root);
        }

        public void UpdateLevelLabels(PlayerLevelCompletionData data)
        {
            LevelCompletionData = data;

            UILevels.Clear();
            ScrollView.Clear();
            SetVisualElements();
        }

        private void HandleUIClick(ClickEvent evt, LevelSO levelData)
        {
            OnLevelSelected?.Invoke(levelData);
        }

        public override void Show()
        {
            // appears to be some bug in Unity 6 / UITK that makes the visual elements sometimes...not show??
            UILevels.Clear();
            ScrollView.Clear();
            SetVisualElements();

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
            ScrollView.RegisterCallback<PointerLeaveEvent>(HandleLoseMouse);
        }

        private void HandleLoseMouse(PointerLeaveEvent evt) => IsDragging = false;
        private void HandleMouseUpDrag(MouseUpEvent evt) => IsDragging = false;
        private void HandleMouseDownDrag(MouseDownEvent evt) => IsDragging = true;

        private void HandleMouseMove(MouseMoveEvent evt)
        {
            if (!IsDragging) return;

            // ideally we'd use some kind of velocity on this, but this gets us started
            ScrollView.scrollOffset -= evt.mouseDelta;
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
                uiLevel.Root.UnregisterCallback<ClickEvent, LevelSO>(HandleUIClick);
            }

            CloseButton.UnregisterCallback<ClickEvent>(Hide);
        }
    }
}
