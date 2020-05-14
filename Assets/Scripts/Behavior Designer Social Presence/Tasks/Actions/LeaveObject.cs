using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace TFG
{
    [TaskDescription("El NPC Suelta el ingrediente que sostiene en la mano")]
    [TaskCategory("TFG")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}Play.png")]
    public class LeaveObject : Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Objeto que representa al jugador")]
        public SharedGameObject targetObject;//Camara

        [BehaviorDesigner.Runtime.Tasks.Tooltip("Objeto que representa el ingrediente a soltar")]
        public SharedGameObject ingredient;//Ingrediente

        private Rigidbody rb;//Rigidbody del ingrediente
        private bool picked;//Booleana para comprobar que el jugador ha cogido el ingrediente (Simulación)

        public override void OnStart()
        {
            picked = false;

            //Se obtiene el Rigidbody del ingrediente
            //TODO: No se sabe si es necesario, puede que se haga internamente en el codigo de coger objeto con vr
            rb = ingredient.Value.GetComponent<Rigidbody>(); 
        }

        //Método encargado de desligar el ingrediente que sostiene el NPC en la mano
        public void DetachIngredient()
        {
            //*****TODO: ESTO ES PARA SIMULAR*****
            // El ingrediente ya no está atado a la mano del NPC         
            // ingredient.Value.transform.parent = null;

            //Se une la posicion del objeto a la posicion de la cámara(jugador)        
            ingredient.Value.transform.SetParent(targetObject.Value.transform, true);

            //Se resetean la posicion y la rotación del ingrediente para que estén delante del jugador
            ingredient.Value.transform.localPosition = new Vector3(0.0f, 0.0f, 2.0f);
            ingredient.Value.transform.localRotation = new Quaternion(0, 0, 0, 1);
            //*****TODO: ESTO ES PARA SIMULAR*****

        }

        public override TaskStatus OnUpdate()
        {
            //*****TODO: ESTO ES PARA SIMULAR*****
            //ESTO ES PARA SIMULAR QUE EL JUGADOR COGE EL INGREDIENTE CON LA TECLA P
            if (Input.GetKeyDown(KeyCode.P))
            {
                picked = !picked;
                if (picked)               
                    DetachIngredient();//Suelta el objeto
                                   
                return TaskStatus.Success;
            }
            else
                return TaskStatus.Running;
            //*****TODO: ESTO ES PARA SIMULAR*****


        }
    }
}

