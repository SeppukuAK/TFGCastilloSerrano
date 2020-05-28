using BehaviorDesigner.Runtime.Tasks;

namespace SocialPresenceVR
{
    [TaskDescription("Devuelve si el jugador está usando el objeto especificado")]
    [TaskCategory("SocialPresenceVR/EnviromentInteractions/ContinuousActions")]
    public class IsPlayerUsingInteractable : IsPlayerInteracting
    {
        protected override void AddOnListener()
        {
            interactable.onActivate.AddListener(On);
        }

        protected override void AddOffListener()
        {
            interactable.onDeactivate.AddListener(Off);
        }

        protected override void RemoveListeners()
        {
            interactable.onActivate.RemoveListener(On);
            interactable.onDeactivate.RemoveListener(Off);
        }
    }
}