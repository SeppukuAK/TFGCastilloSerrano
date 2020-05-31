using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;

public class CreateFile : MonoBehaviour
{
    int tiempoJugado=0;//tiempo jugado

    void CreateText()
    {     
        //Path of the file
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TiempoJugado");

        //Create File if it doesn't exist
        //if (!File.Exists(path))
        //{
        //    File.WriteAllText(path, "Tiempo jugado: \n");
        //}

        //Content of the file
        string content = "Tiempo jugado: " + tiempoJugado + " minutos";

        //Add some to text to it
        File.AppendAllText(path,content);
    }
    private void Start()
    {
        CreateText();
    }
}
