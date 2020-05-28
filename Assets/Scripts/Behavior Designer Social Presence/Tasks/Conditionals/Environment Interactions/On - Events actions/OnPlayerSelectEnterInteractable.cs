using BehaviorDesigner.Runtime.Tasks;

namespace SocialPresenceVR
{
    [TaskDescription("Devuelve si el jugador ha cogido un objeto")]
    [TaskCategory("SocialPresenceVR/EnviromentInteractions/EventActions")]
    public class OnPlayerSelectEnterInteractable : OnPlayerInteraction
    {
        protected override void AddListener()
        {
            XRInteractable.Value.onSelectEnter.AddListener(OnInteraction);
        }

        protected override void RemoveListener()
        {
            XRInteractable.Value.onSelectEnter.RemoveListener(OnInteraction);
        }
    }
}