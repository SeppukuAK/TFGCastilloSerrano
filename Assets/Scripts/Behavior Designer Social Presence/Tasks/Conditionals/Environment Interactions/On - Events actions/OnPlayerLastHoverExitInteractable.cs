using BehaviorDesigner.Runtime.Tasks;

namespace TFG
{
    [TaskDescription("Devuelve si el jugador deja de apuntar a un objeto con el laser")]
    [TaskCategory("TFG")]
    public class OnPlayerLastHoverExitInteractable : OnPlayerInteraction
    {
        protected override void AddListener()
        {
            XRInteractable.Value.onLastHoverExit.AddListener(OnInteraction);
        }
    }
}