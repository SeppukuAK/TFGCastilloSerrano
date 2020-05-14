using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace TFG
{
    [TaskDescription("El NPC coge un ingrediente")]
    [TaskCategory("TFG")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}Play.png")]
    public class PickUpObject : Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Ingrediente que se ha cogido")]
        public SharedGameObject Ingredient;

        private GameObject hand;//Mano del NPC
        private Rigidbody rb;//Rigidbody del ingrediente

        public override void OnAwake()
        {
            //Se obtiene la mano del NPC
            hand = GetComponent<NPC>().Hand;
        }
        public override void OnStart()
        {
            //Se obtiene el Rigidbody del ingrediente
            rb = Ingredient.Value.GetComponent<Rigidbody>();

            //Se une la posicion del objeto a la posicion de la mano         
            Ingredient.Value.transform.SetParent(hand.transform, true);

            //Se resetean la posicion y la rotación del ingrediente 
            Ingredient.Value.transform.localPosition = new Vector3(0, 0, 0);
            Ingredient.Value.transform.localRotation = new Quaternion(0, 0, 0, 1);

            //El ingrediente deja de responder a la física
            rb.isKinematic = true;
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
}
