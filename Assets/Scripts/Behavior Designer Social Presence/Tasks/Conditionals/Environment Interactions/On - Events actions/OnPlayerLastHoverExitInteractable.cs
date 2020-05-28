using BehaviorDesigner.Runtime.Tasks;

namespace SocialPresenceVR
{
    [TaskDescription("Devuelve si el jugador deja de apuntar a un objeto con el laser")]
    [TaskCategory("SocialPresenceVR/EnviromentInteractions/EventActions")]
    public class OnPlayerLastHoverExitInteractable : OnPlayerInteraction
    {
        protected override void AddListener()
        {
            XRInteractable.Value.onLastHoverExit.AddListener(OnInteraction);
        }

        protected override void RemoveListener()
        {
            XRInteractable.Value.onLastHoverExit.RemoveListener(OnInteraction);
        }
    }
}