using BehaviorDesigner.Runtime.Tasks;

namespace TFG
{
    [TaskDescription("Devuelve si el jugador está apuntando a un objeto con el laser")]
    [TaskCategory("TFG")]
    public class IsPlayerPointingInteractable : IsPlayerInteracting
    {
        protected override void AddOnListener()
        {
            XRInteractable.Value.onFirstHoverEnter.AddListener(On);
        }

        protected override void AddOffListener()
        {
            XRInteractable.Value.onLastHoverExit.AddListener(Off);
        }
    }
}