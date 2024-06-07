using System;
using UnityEngine.UIElements;

namespace LlamAcademy.Minigolf.UI
{
    public abstract class UIView : IDisposable
    {
        protected bool HideOnAwake { get; set; }
        protected bool IsOverlay { get; set; }
        public VisualElement Root { get; protected set; }
        public bool IsTransparent => IsOverlay;
        public bool IsHidden => Root.ClassListContains("hidden");

        public UIView() {}

        public UIView(VisualElement root)
        {
            Initialize(root);
        }

        public virtual void Initialize(VisualElement root)
        {
            Root = root ?? throw new ArgumentNullException(nameof(root));
            if (HideOnAwake)
            {
                Hide();
            }
            SetVisualElements();
            RegisterButtonCallbacks();
        }

        /// <summary>
        /// Sets up the VisualElements for the UI.
        /// Override in subclass for implementation.
        /// </summary>
        protected virtual void SetVisualElements() {}

        /// <summary>
        /// Registers callbacks for buttons in the UI.
        /// Override in subclass for implementation.
        /// </summary>
        protected virtual void RegisterButtonCallbacks() {}

        public virtual void Show()
        {
            Root.RemoveFromClassList("hidden");
            Root.AddToClassList("visible");
        }

        public virtual void Hide()
        {
            Root.RemoveFromClassList("visible");
            Root.AddToClassList("hidden");
        }

        public abstract void Dispose();
    }
}
