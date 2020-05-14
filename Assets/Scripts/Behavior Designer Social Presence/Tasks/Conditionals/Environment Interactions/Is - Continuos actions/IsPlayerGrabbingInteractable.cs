using BehaviorDesigner.Runtime.Tasks;

namespace TFG
{
    [TaskDescription("Devuelve si el jugador está agarrando el objeto especificado")]
    [TaskCategory("TFG")]
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