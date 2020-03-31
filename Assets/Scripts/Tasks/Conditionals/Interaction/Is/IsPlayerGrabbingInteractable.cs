using BehaviorDesigner.Runtime.Tasks;

namespace TFG
{
    [TaskCategory("TFG")]
    public class IsPlayerGrabbingInteractable : IsPlayerInteracting
    {
        protected override void AddOnListener()
        {
            XRGrabInteractable.Value.onSelectEnter.AddListener(On);
        }

        protected override void AddOffListener()
        {
            XRGrabInteractable.Value.onSelectExit.AddListener(Off);
        }
    }
}