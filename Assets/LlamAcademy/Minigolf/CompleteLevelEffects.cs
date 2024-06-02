using LlamAcademy.Minigolf.Bus;
using LlamAcademy.Minigolf.Bus.Events;
using UnityEngine;

namespace LlamAcademy.Minigolf
{
    public class CompleteLevelEffects : MonoBehaviour
    {
        [SerializeField] private Renderer[] Effects;

        private void OnEnable()
        {
            EventBus<BallInHoleEvent>.OnEvent += HandleOnBallInHole;
        }

        private void HandleOnBallInHole(BallInHoleEvent evt)
        {
            int scoreEffectCount = (int)evt.PlayerScore;
            for (int i = 0; i < scoreEffectCount; i++)
            {
                Effects[i].enabled = true;
            }
        }

        private void OnDisable()
        {
            EventBus<BallInHoleEvent>.OnEvent -= HandleOnBallInHole;
        }
    }
}