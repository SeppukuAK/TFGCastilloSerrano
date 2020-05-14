using BehaviorDesigner.Runtime.Tasks;

namespace TFG
{
    [TaskDescription("Devuelve si el jugador ha cogido un objeto")]
    [TaskCategory("TFG")]
    public class OnPlayerSelectEnterInteractable : OnPlayerInteraction
    {
        protected override void AddListener()
        {
            XRInteractable.Value.onSelectEnter.AddListener(OnInteraction);
        }
    }
}