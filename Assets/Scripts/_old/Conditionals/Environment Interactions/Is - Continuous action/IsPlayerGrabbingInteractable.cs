//using BehaviorDesigner.Runtime.Tasks;

//namespace SocialPresenceVR
//{
//    [TaskDescription("Devuelve si el jugador está agarrando el objeto especificado")]
//    [TaskCategory("SocialPresenceVR/EnviromentInteractions/ContinuousActions")]
//    public class IsPlayerGrabbingInteractable : IsPlayerInteracting
//    {
//        protected override void AddOnListener()
//        {
//            interactable.onSelectEnter.AddListener(On);
//        }

//        protected override void AddOffListener()
//        {
//            interactable.onSelectExit.AddListener(Off);
//        }

//        protected override void RemoveListeners()
//        {
//            interactable.onSelectEnter.RemoveListener(On);
//            interactable.onSelectExit.RemoveListener(Off);
//        }
//    }
//}