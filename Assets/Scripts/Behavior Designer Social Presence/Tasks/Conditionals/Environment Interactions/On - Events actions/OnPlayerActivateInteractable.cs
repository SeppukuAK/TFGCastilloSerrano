using BehaviorDesigner.Runtime.Tasks;

namespace SocialPresenceVR
{
    [TaskDescription("Devuelve si el jugador ha usado un objeto")]
    [TaskCategory("SocialPresenceVR/EnviromentInteractions/EventActions")]
    public class OnPlayerActivateInteractable : OnPlayerInteraction
    {
        protected override void AddListener()
        {
            XRInteractable.Value.onActivate.AddListener(OnInteraction);
        }

        protected override void RemoveListener()
        {
            XRInteractable.Value.onActivate.RemoveListener(OnInteraction);
        }
    }
}