using System.Linq;
using LlamAcademy.Minigolf.LevelManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace LlamAcademy.Minigolf.UI.Components
{
    /// <summary>
    /// Backing class for Level component that is displayed on the UI.
    /// UXML is in Resources/UILevel.uxml
    /// </summary>
    public class UILevel : UIView
    {
        private Label LevelLabel;
        private Label ScoreLabel;
        private Label ParLabel;
        private LevelSO Level;
        private int? BestScore;

        public UILevel(VisualElement root, LevelSO level, int? bestScore = null)
        {
            Level = level;
            BestScore = bestScore;
            Initialize(root);
        }

        protected override void SetVisualElements()
        {
            LevelLabel = Root.Q<Label>("level");
            ScoreLabel = Root.Q<Label>("score");
            ParLabel = Root.Q<Label>("par");

            LevelLabel.text = Level.name;
            ScoreLabel.text = BestScore == null ? "-" : BestScore.ToString();

            if (Level.Par == null || Level.Par.Length != 4)
            {
                Debug.LogError($"Incorrect Par configured for {Level.name}! This is a configuration error that will break the game.");
                ParLabel.text = "ERROR";
            }
            else
            {
                ParLabel.text = Level.Par.Min(item => item.Strokes).ToString();
            }
        }

        public override void Dispose()
        {
        }
    }
}
