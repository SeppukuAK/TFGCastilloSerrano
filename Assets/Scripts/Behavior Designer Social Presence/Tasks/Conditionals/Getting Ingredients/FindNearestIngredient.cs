﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEditor;

namespace TFG
{
    [TaskDescription("Obtiene el ingrediente que forma parte de la misión más cercano")]
    [TaskCategory("TFG")]
    [TaskIcon("Assets/Behavior Designer Movement/Editor/Icons/{SkinColor}Play.png")]
    public class FindNearestIngredient : Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Distancia a la que se tiene que encontrar el ingrediente para ser detectado")]
        public SharedFloat magnitude = 5;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Ingrediente encontrado más cercano")]
        public SharedGameObject returnedObject;

        private List<GameObject> objects;//Lista de ingredientes

        //Variables para la detección de si un objeto está a la distancia establecida
        private float sqrMagnitude;
        private bool overlapCast = false;

        private CauldronContent cauldronContent;//Objeto de la clase CauldronContent necesario para obtener los objetos que se encuentran en el caldero

        public override void OnAwake()
        {
            var cauldronObject = Object.FindObjectOfType<CauldronContent>();
            cauldronContent = cauldronObject.GetComponent<CauldronContent>();
        }

        /// <summary>
        /// Método que comprueba si un objeto está en el caldero
        /// </summary>
        /// <param name="ingredientType"></param>
        /// <returns></returns>
        bool IsThisIngredientIn(string ingredientType)
        {
            List<string> aux = new List<string>(cauldronContent.GetCurrentIngredientsIn());

            //Comprobamos si el ingrediente está en la lista de objetos que están en el caldero
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
            
            //Se obtienen todos los objetos que tienen el componente de ingrediente
            var gameObjects = Object.FindObjectsOfType<CauldronIngredient>();

            for (int i = 0; i < gameObjects.Length; ++i)
            {
                //Variable auxiliar para obtener un ingrediente
                var cauldronIngredient = gameObjects[i].GetComponent<CauldronIngredient>();

                //Añadimos solamente los objetos que no están aún en el caldero y que forman parte de la misión(pluma, calabaza y filete)
                if (!IsThisIngredientIn(cauldronIngredient.IngredientType) && (cauldronIngredient.IngredientType == "feather" || cauldronIngredient.IngredientType == "pumpkin" || cauldronIngredient.IngredientType == "meat"))
                    objects.Add(gameObjects[i].gameObject);
            }
        }

        /// <summary>
        /// Método que devuelve éxito si cualquier objeto de la lista está a distancia establecida
        /// </summary>
        /// <returns></returns>
        public override TaskStatus OnUpdate()
        {
            if (transform == null || objects == null)
                return TaskStatus.Failure;

            //Se añaden a la lista los objetos que están a distancia
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

            // Se recorren todos los objetos, se devuelve éxito cuando se ha encontrado el más cercano 
            for (int i = 0; i < objects.Count; ++i)
            {
                if (objects[i] == null)                
                    continue;
                
                direction = objects[i].transform.position - (transform.position);

                // Se encuentra el objeto más cercano
                if (Vector3.SqrMagnitude(direction) < sqrMagnitude)
                {
                    returnedObject.Value = objects[i];
                    return TaskStatus.Success;
                }
            }
            // Si no hay objetos en la distancia establecida, se devuelve fallo
            return TaskStatus.Failure;
        }

        /// <summary>
        /// Método de dibujado para representar la distancia establecida
        /// </summary>
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

