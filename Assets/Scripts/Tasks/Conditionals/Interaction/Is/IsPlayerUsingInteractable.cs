using BehaviorDesigner.Runtime.Tasks;

namespace TFG
{
    [TaskCategory("TFG")]
    public class IsPlayerUsingInteractable : IsPlayerInteracting
    {
        protected override void AddOnListener()
        {
            XRGrabInteractable.Value.onActivate.AddListener(On);
        }

        protected override void AddOffListener()
        {
            XRGrabInteractable.Value.onDeactivate.AddListener(Off);
        }
    }
}