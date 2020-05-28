using BehaviorDesigner.Runtime.Tasks;

namespace SocialPresenceVR
{
    [TaskDescription("Devuelve si el jugador ha dejado de usar un objeto")]
    [TaskCategory("SocialPresenceVR/EnviromentInteractions/EventActions")]
    public class OnPlayerDeactivateInteractable : OnPlayerInteraction
    {
        protected override void AddListener()
        {
            XRInteractable.Value.onDeactivate.AddListener(OnInteraction);
        }

        protected override void RemoveListener()
        {
            XRInteractable.Value.onDeactivate.RemoveListener(OnInteraction);
        }
    }
}