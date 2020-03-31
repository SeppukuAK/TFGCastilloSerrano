using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.XR.Interaction.Toolkit;

namespace TFG
{
    public abstract class IsPlayerInteracting : Conditional
    {
        public SharedXRGrabInteractable XRGrabInteractable;

        private bool interaction;

        protected abstract void AddOnListener();

        protected abstract void AddOffListener();

        public override void OnAwake()
        {
            interaction = false;
            AddOnListener();
            AddOffListener();
        }

        protected void On(XRBaseInteractor arg0)
        {
            interaction = true;
        }

        protected void Off(XRBaseInteractor arg0)
        {
            interaction = false;
        }

        public override TaskStatus OnUpdate()
        {
            if (interaction)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }

    }
}