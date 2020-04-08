using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureGame : MonoBehaviour
{
    public int NumTrainingSamples = 20;

    private GestureRecognition gestureRecognition;

    int testGesture;

    private bool recording;

    private GameObject leftHand;

    void Start()
    {
        recording = false;

        string GesturesFilePath = "Assets/GestureRecognition";
        string LoadGesturesFile = "Sample_OneHanded_Gestures.dat";

        //This object provides all the functions for recording, identifying and saving gestures.
        gestureRecognition = new GestureRecognition();
        //GestureCombinations gc = new GestureCombinations();

        //Carga los gestos de un archivo de base de datos de gestos
        if (gestureRecognition.loadFromFile(GesturesFilePath + "/" + LoadGesturesFile) == false)
        {
            Debug.LogError("Error cargando gestos");
        }

        //Guarda los gestos en un archivo de base de datos de gestos
        //gestureRecognition.saveToFile("C:/myGestures.dat");

        //Creación de un gesto para reconocerlo posteriormente
        //testGesture = gestureRecognition.createGesture("GestureTest");


        //Empieza el proceso de entrenamiento
        //gestureRecognition.setMaxTrainingTime(10);  //10 segundos
        //gestureRecognition.startTraining();

        //Se puede registrar a un callback para recibir actualizaciones del proceso de entrenamiento
        //gestureRecognition.setTrainingUpdateCallback();
        //gestureRecognition.setTrainingFinishCallback();

        //Para el proceso de entrenamiento
        //gestureRecognition.stopTraining();

        //Debug.Log("Gesture Identification Performance (Valor de 1 significa reconocimiento 100% correcto) " + gestureRecognition.recognitionScore());

        //Documentación del asset lo hace así
        leftHand = GameObject.Find("LeftHand Controller");
        if(leftHand == null)
        {
            Debug.LogError("Mano izquierda no encontrada");
        }

    }

    /// <summary>
    /// Grabar 'X' ejemplos del gesto.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            recording = !recording;

            //Empieza a grabar
            if (recording)
            {
                Vector3 headsetPos = Camera.main.gameObject.transform.position;
                Quaternion headsetRotation = Camera.main.gameObject.transform.rotation;
                gestureRecognition.startStroke(headsetPos, headsetRotation);
            }

            //Para de grabar
            else
            {
                double similarity = 0;  // [0, 1]

                int identifiedGesture = gestureRecognition.endStroke(ref similarity);

                if (identifiedGesture <0)
                    Debug.Log("NINGUN GESTO IDENTIFICADO");

                else if (identifiedGesture == 0) //Circulo
                {
                    Debug.Log("CIRCULO, Similarity: " + similarity);
                    // ...
                }

                else if (identifiedGesture == 1) //SWIPE LEFT
                {
                    Debug.Log("SWIPE LEFT, Similarity: " + similarity);
                    // ...
                }                else
                    Debug.Log("GESTO: " + identifiedGesture + " SIMILARITY: " + similarity);


            }

        }


        if (recording)
        {
            //Instruciones mientras se realiza la grabación
            Vector3 controllerPos = leftHand.transform.position;
            Quaternion controllerRotation = leftHand.transform.rotation;
            gestureRecognition.contdStrokeQ(controllerPos, controllerRotation);
        }

    }
}
