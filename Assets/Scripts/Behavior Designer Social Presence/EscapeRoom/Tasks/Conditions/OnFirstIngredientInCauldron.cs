using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

namespace SocialPresenceVR
{
    /// <summary>
    /// TODO: No está hecho de la mejor manera, pero funciona.
    /// TODO: Deberia de hacerlo para cada vez que hay 1 ingrediente
    /// </summary>
    [TaskDescription("Devuelve true cuando se ha depositado el primer ingrediente en el caldero")]
    [TaskCategory("SocialPresenceVR/EscapeRoom")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}Play.png")]
    public class OnFirstIngredientInCauldron : Conditional
    {
        private bool done;

        private CauldronContent cauldron;

        /// <summary>
        /// Obtiene referencia al caldero e inicializa variables
        /// </summary>
        public override void OnAwake()
        {
            done = false;
            cauldron = Object.FindObjectOfType<CauldronContent>();

            if (!cauldron)
                Debug.LogError("cauldron no encontrado en la escena");

        }

        public override void OnEnd()
        {
            if (cauldron.GetCurrentIngredientsIn().Count > 0)
                done = true;
        }

        public override TaskStatus OnUpdate()
        {
            if (!done && cauldron.GetCurrentIngredientsIn().Count > 0)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }
    }
}