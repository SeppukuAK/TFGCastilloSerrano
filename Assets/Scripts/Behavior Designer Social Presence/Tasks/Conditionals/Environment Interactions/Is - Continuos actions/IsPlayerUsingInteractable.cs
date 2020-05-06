using BehaviorDesigner.Runtime.Tasks;

namespace TFG
{
    [TaskDescription("Devuelve si el jugador está usando el objeto especificado")]
    [TaskCategory("TFG")]
    public class IsPlayerUsingInteractable : IsPlayerInteracting
    {
        protected override void AddOnListener()
        {
            XRInteractable.Value.onActivate.AddListener(On);
        }

        protected override void AddOffListener()
        {
            XRInteractable.Value.onDeactivate.AddListener(Off);
        }
    }
}