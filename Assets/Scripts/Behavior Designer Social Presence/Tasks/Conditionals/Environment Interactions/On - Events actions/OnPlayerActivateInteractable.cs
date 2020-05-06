using BehaviorDesigner.Runtime.Tasks;

namespace TFG
{
    [TaskDescription("Devuelve si el jugador ha usado un objeto")]
    [TaskCategory("TFG")]
    public class OnPlayerActivateInteractable : OnPlayerInteraction
    {
        protected override void AddListener()
        {
            XRInteractable.Value.onActivate.AddListener(OnInteraction);
        }
    }
}