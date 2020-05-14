using BehaviorDesigner.Runtime.Tasks;

namespace SocialPresenceVR
{
    [TaskDescription("Devuelve si el jugador está agarrando el objeto especificado")]
    [TaskCategory("SocialPresenceVR/EnviromentInteractions/ContinuousActions")]
    public class IsPlayerGrabbingInteractable : IsPlayerInteracting
    {
        protected override void AddOnListener()
        {
            XRInteractable.Value.onSelectEnter.AddListener(On);
        }

        protected override void AddOffListener()
        {
            XRInteractable.Value.onSelectExit.AddListener(Off);
        }
    }
}