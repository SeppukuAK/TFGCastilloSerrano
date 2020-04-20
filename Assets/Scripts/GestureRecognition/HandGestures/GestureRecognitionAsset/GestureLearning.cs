using UnityEngine;

public class GestureLearning : MonoBehaviour
{
    void Start()
    {
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
    }

}
