using BehaviorDesigner.Runtime.Tasks;

namespace SocialPresenceVR
{
    [TaskDescription("Devuelve si el jugador está apuntando a un objeto con el laser")]
    [TaskCategory("SocialPresenceVR/EnviromentInteractions/ContinuousActions")]
    public class IsPlayerPointingInteractable : IsPlayerInteracting
    {
        protected override void AddOnListener()
        {
            interactable.onFirstHoverEnter.AddListener(On);
        }

        protected override void AddOffListener()
        {
            interactable.onLastHoverExit.AddListener(Off);
        }

        protected override void RemoveListeners()
        {
            interactable.onFirstHoverEnter.RemoveListener(On);
            interactable.onLastHoverExit.RemoveListener(Off);
        }
    }
}