using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using UnityEngine.XR.Interaction.Toolkit;


namespace SocialPresenceVR
{
    [TaskDescription("Comprueba si se ha hecho la poción, en ese caso el robot reacciona de una forma u otra")]
    [TaskCategory("SocialPresenceVR/EscapeRoom")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}Play.png")]
    public class OnBrewPotion : Conditional
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Estado de la poción (Correcta o no)")]
        public SharedBool CorrectPotion;

        private CauldronContent cauldronContent;
        private bool potionBrewed;

        public override void OnAwake()
        {
            cauldronContent = Object.FindObjectOfType<CauldronContent>();
        }

        // Start is called before the first frame update
        public override void OnStart ()
        {        
            cauldronContent.OnBrew.AddListener(OnBrew);
        }

        public override TaskStatus OnUpdate()
        {
            if (potionBrewed)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }

        void OnBrew(CauldronContent.Recipe recipe)
        {
            //Robot feliz
            if (recipe != null)
            {
                CorrectPotion.Value = true;
            }
            //Robot triste
            else
            {
                CorrectPotion.Value = false;
            }

            //La poción se ha cocinado
            potionBrewed = true;
        }

        public override void OnEnd()
        {
            potionBrewed = false;
        }
    }
}
