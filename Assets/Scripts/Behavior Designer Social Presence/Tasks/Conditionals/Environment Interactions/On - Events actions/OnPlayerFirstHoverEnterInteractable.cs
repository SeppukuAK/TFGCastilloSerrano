using BehaviorDesigner.Runtime.Tasks;

namespace TFG
{
    [TaskDescription("Devuelve si el jugador apunta a un objeto con el laser")]
    [TaskCategory("TFG")]
    public class OnPlayerFirstHoverEnterInteractable : OnPlayerInteraction
    {
        protected override void AddListener()
        {
            XRInteractable.Value.onFirstHoverEnter.AddListener(OnInteraction);
        }
    }
}