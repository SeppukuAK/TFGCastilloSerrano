using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEditor;

namespace TFG
{
    [TaskDescription("Obtiene el ingrediente correcto más cercano")]
    [TaskCategory("TFG")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}Play.png")]
    public class FindNearestIngredient : Conditional
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The distance that the object needs to be within")]
        public SharedFloat magnitude = 5;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The object variable that will be set when a object is found what the object is")]
        public SharedGameObject returnedObject;

        private List<GameObject> objects;
        // distance * distance, optimization so we don't have to take the square root
        private float sqrMagnitude;
        private bool overlapCast = false;

        private CauldronContent cauldronContent;//Objeto de la clase CauldronContent
        private GameObject cauldronAux;//TODO:quitar

        public override void OnAwake()
        {
            //MAL SEGURAMENTE
            var cauldronObject = Object.FindObjectOfType<CauldronContent>();
            cauldronContent = cauldronObject.GetComponent<CauldronContent>();
        }

        //Método que comprueba si un objeto está ya en la lista de objetos que están en el caldero
        bool IsThisIngredientIn(string ingredientType)
        {
            List<string> aux = new List<string>(cauldronContent.GetCurrentIngredientsIn());

            //Comprobamos si está en la lista
            if (aux.Contains(ingredientType))
                return true;
            else
                return false;
        }

        public override void OnStart()
        {
            sqrMagnitude = magnitude.Value * magnitude.Value;

            //Si la lista de objetos no está vacía al inicio, se limpia
            if (objects != null)         
                objects.Clear();
            
            //Se crea la lista nueva
            else           
                objects = new List<GameObject>();
            
            //Se obtienen todos los objetos que tienen el componente de ingrediente AWAKE?
            var gameObjects = Object.FindObjectsOfType<CauldronIngredient>();


            for (int i = 0; i < gameObjects.Length; ++i)
            {
                //Variable auxiliar
                var cauldronIngredient = gameObjects[i].GetComponent<CauldronIngredient>();

                //Añadimos solamente los objetos que no están aún en el caldero y que forman parte de la misión
                if (!IsThisIngredientIn(cauldronIngredient.IngredientType) && (cauldronIngredient.IngredientType == "feather" || cauldronIngredient.IngredientType == "pumpkin" || cauldronIngredient.IngredientType == "meat"))
                    objects.Add(gameObjects[i].gameObject);
            }

        }

        // returns success if any object is within distance of the current object. Otherwise it will return failure
        public override TaskStatus OnUpdate()
        {
            if (transform == null || objects == null)
                return TaskStatus.Failure;

            if (overlapCast)
            {
                objects.Clear();

                var colliders = Physics2D.OverlapCircleAll(transform.position, magnitude.Value, 1);
                for (int i = 0; i < colliders.Length; ++i)
                {
                    objects.Add(colliders[i].gameObject);
                }
            }

            Vector3 direction;
            // check each object. All it takes is one object to be able to return success
            for (int i = 0; i < objects.Count; ++i)
            {
                if (objects[i] == null)
                {
                    continue;
                }
                direction = objects[i].transform.position - (transform.position);
                // check to see if the square magnitude is less than what is specified
                if (Vector3.SqrMagnitude(direction) < sqrMagnitude)
                {
                    // the object has a magnitude less than the specified magnitude. Set the object and return success
                    returnedObject.Value = objects[i];
                    Debug.Log("Objeto encontrado: " + objects[i].name);
                    return TaskStatus.Success;
                }
            }
            // no objects are within distance. Return failure
            return TaskStatus.Failure;
        }

        // Draw the seeing radius
        public override void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (Owner == null || magnitude == null)
            {
                return;
            }
            var oldColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.DrawWireDisc(Owner.transform.position, Owner.transform.up, magnitude.Value);
            UnityEditor.Handles.color = oldColor;
#endif
        }
    }
}

