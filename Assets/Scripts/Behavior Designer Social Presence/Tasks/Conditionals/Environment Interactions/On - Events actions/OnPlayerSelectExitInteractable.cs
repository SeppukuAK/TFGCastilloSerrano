using BehaviorDesigner.Runtime.Tasks;

namespace SocialPresenceVR
{
    [TaskDescription("Devuelve si el jugador ha soltado un objeto o se ha teletransportado al objeto")]
    [TaskCategory("SocialPresenceVR/EnviromentInteractions/EventActions")]
    public class OnPlayerSelectExitInteractable : OnPlayerInteraction
    {
        protected override void AddListener()
        {
            XRInteractable.Value.onSelectExit.AddListener(OnInteraction);
        }
        protected override void RemoveListener()
        {
            XRInteractable.Value.onSelectExit.RemoveListener(OnInteraction);
        }
    }
}