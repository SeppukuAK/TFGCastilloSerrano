using BehaviorDesigner.Runtime.Tasks;

namespace TFG
{
    [TaskDescription("Devuelve si el jugador ha dejado de usar un objeto")]
    [TaskCategory("TFG")]
    public class OnPlayerDeactivateInteractable : OnPlayerInteraction
    {
        protected override void AddListener()
        {
            XRInteractable.Value.onDeactivate.AddListener(OnInteraction);
        }
    }
}