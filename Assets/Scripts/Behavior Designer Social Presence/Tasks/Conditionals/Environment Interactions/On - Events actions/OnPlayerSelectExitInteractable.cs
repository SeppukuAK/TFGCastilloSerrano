using BehaviorDesigner.Runtime.Tasks;

namespace TFG
{
    [TaskDescription("Devuelve si el jugador ha soltado un objeto o se ha teletransportado al objeto")]
    [TaskCategory("TFG")]
    public class OnPlayerSelectExitInteractable : OnPlayerInteraction
    {
        protected override void AddListener()
        {
            XRInteractable.Value.onSelectExit.AddListener(OnInteraction);
        }
    }
}