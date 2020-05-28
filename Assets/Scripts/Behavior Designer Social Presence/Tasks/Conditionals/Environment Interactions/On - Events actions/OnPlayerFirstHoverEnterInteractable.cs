using BehaviorDesigner.Runtime.Tasks;

namespace SocialPresenceVR
{
    [TaskDescription("Devuelve si el jugador apunta a un objeto con el laser")]
    [TaskCategory("SocialPresenceVR/EnviromentInteractions/EventActions")]
    public class OnPlayerFirstHoverEnterInteractable : OnPlayerInteraction
    {
        protected override void AddListener()
        {
            XRInteractable.Value.onFirstHoverEnter.AddListener(OnInteraction);
        }

        protected override void RemoveListener()
        {
            XRInteractable.Value.onFirstHoverEnter.RemoveListener(OnInteraction);
        }
    }
}