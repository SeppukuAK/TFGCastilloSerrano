/*
 * 3D Gesture Recognition - Unity Plug-In
 * 
 * Copyright (c) 2019 MARUI-PlugIn (inc.)
 * This software is free to use for non-commercial purposes.
 * You may use this software in part or in full for any project
 * that does not pursue financial gain, including free software 
 * and projectes completed for evaluation or educational purposes only.
 * Any use for commercial purposes is prohibited.
 * You may not sell or rent any software that includes
 * this software in part or in full, either in it's original form
 * or in altered form.
 * If you wish to use this software in a commercial application,
 * please contact us at support@marui-plugin.com to obtain
 * a commercial license.
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS 
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
 * THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
 * PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, 
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, 
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR 
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY 
 * OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.Networking;


public class Sample_Military : MonoBehaviour
{
    // Convenience ID's for the "left" and "right" sub-gestures.
    public const int Side_Left = 0;
    public const int Side_Right = 1;
    
    // Whether the user is currently pressing the contoller trigger.
    private bool trigger_pressed_left = false;
    private bool trigger_pressed_right = false;

    // Wether a gesture was already started
    private bool gesture_started = false;

    // The gesture recognition object for bimanual gestures.
    private GestureCombinations gc = new GestureCombinations(2);

    // The step in the tutorial where we currently are at
    private int current_tutorial_step = 0;

    // The text field to display instructions.
    private Text HUDText;

    // The canvas object to show illustrations.
    private GameObject illustration;

    // The texture of the illustration canvas.
    private RawImage illustration_image;

    // Temporary storage for objects to display the gesture stroke.
    List<string> stroke = new List<string>(); 

    // Temporary counter variable when creating objects for the stroke display:
    int stroke_index = 0;

#if UNITY_EDITOR
    private const string illustration_path = "Assets/GestureRecognition/Resources/Sample_Military_Illustration_";
#else
    private const string illustration_path = "Sample_Military_Illustration_";
#endif

    // List of the illustrations for the tutorial
    private string[] gestures = {
        "Assemble"
        ,
        "Disperse"
        ,
        "AirAttack"
        ,
        "TakeCover"
        ,
        "SpeedUp"
        ,
        "EnemySpotted"
        ,
        "Come"
    };

    // Initialization:
    void Start ()
    {
        // Set the welcome message.
        HUDText = GameObject.Find("HUDText").GetComponent<Text>();
        HUDText.text = "Welcome to 3D Gesture Recognition Plug-in!\n"
                      + "Press triggers of either or both controllers\n"
                      + "and perform the gesture displayed.";

        // Load the default set of gestures.
#if UNITY_EDITOR
        // When running the scene inside the Unity editor,
        // we can just load the file from the Assets/ folder:
        string gesture_file_path = "Assets/GestureRecognition";
#elif UNITY_ANDROID
        // On android, the file is in the .apk,
        // so we need to first "download" it to the apps' cache folder.
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        string gesture_file_path = activity.Call <AndroidJavaObject>("getCacheDir").Call<string>("getCanonicalPath");
        UnityWebRequest request = UnityWebRequest.Get(Application.streamingAssetsPath + "/Sample_Military_Gestures.dat");
        request.SendWebRequest();
        while (!request.isDone) {
            // wait for file extraction to finish
        }
        if (request.isNetworkError)
        {
            HUDText.text = "Failed to extract sample gesture database file from apk.\n";
            return;
        }
        File.WriteAllBytes(gesture_file_path + "/Sample_Military_Gestures.dat", request.downloadHandler.data);
#else
        // This will be the case when exporting a stand-alone PC app.
        // In this case, we can load the gesture database file from the streamingAssets folder.
        string gesture_file_path = Application.streamingAssetsPath;
#endif
        if (gc.loadFromFile(gesture_file_path + "/Sample_Military_Gestures.dat") == false)
        {
            HUDText.text = "Failed to load sample gesture database file.";
            return;
        }
        
        // Show the first gesture
        illustration = GameObject.Find("GestureIllustration");
        illustration_image = illustration.GetComponent<RawImage>();
#if UNITY_EDITOR
        illustration_image.texture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture>(illustration_path + gestures[0] + ".png");
#else
        illustration_image.texture = Resources.Load<Texture>(illustration_path + gestures[0]);
#endif

        // Hide unused models in the scene
        GameObject controller_oculus_left = GameObject.Find("controller_oculus_left");
        GameObject controller_oculus_right = GameObject.Find("controller_oculus_right");
        GameObject controller_vive_left = GameObject.Find("controller_vive_left");
        GameObject controller_vive_right = GameObject.Find("controller_vive_right");
        GameObject controller_microsoft_left = GameObject.Find("controller_microsoft_left");
        GameObject controller_microsoft_right = GameObject.Find("controller_microsoft_right");
        GameObject controller_dummy_left = GameObject.Find("controller_dummy_left");
        GameObject controller_dummy_right = GameObject.Find("controller_dummy_right");

        controller_oculus_left.SetActive(false);
        controller_oculus_right.SetActive(false);
        controller_vive_left.SetActive(false);
        controller_vive_right.SetActive(false);
        controller_microsoft_left.SetActive(false);
        controller_microsoft_right.SetActive(false);
        controller_dummy_left.SetActive(false);
        controller_dummy_right.SetActive(false);

        var input_devices = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevices(input_devices);
        String input_device = "";
        foreach (var device in input_devices)
        {
            if (device.characteristics.HasFlag(InputDeviceCharacteristics.HeadMounted))
            {
                input_device = device.name;
                break;
            }
        }

        if (input_device.Length >= 6 && input_device.Substring(0, 6) == "Oculus")
        {
            controller_oculus_left.SetActive(true);
            controller_oculus_right.SetActive(true);
        } else if (input_device.Length >= 4 && input_device.Substring(0, 4) == "Vive")
        {
            controller_vive_left.SetActive(true);
            controller_vive_right.SetActive(true);
        }
        else if (input_device.Length >= 4 && input_device.Substring(0, 4) == "DELL")
        {
            controller_microsoft_left.SetActive(true);
            controller_microsoft_right.SetActive(true);
        }
        else // 
        {
            controller_dummy_left.SetActive(true);
            controller_dummy_right.SetActive(true);
        }
        
        GameObject ball = GameObject.Find("ball");
        ball.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
    }
    

    // Update:
    void Update()
    {
        float escape = Input.GetAxis("escape");
        if (escape > 0.0f)
        {
            Application.Quit();
        }
        float trigger_left = Input.GetAxis("LeftControllerTrigger");
        float trigger_right = Input.GetAxis("RightControllerTrigger");
        
        // If the user presses either controller's trigger, we start a new gesture.
        if (trigger_pressed_left == false && trigger_left > 0.9) { 
            // Controller trigger pressed.
            trigger_pressed_left = true;
            GameObject hmd = GameObject.Find("Main Camera"); // alternative: Camera.main.gameObject
            Vector3 hmd_p = hmd.transform.localPosition;
            Quaternion hmd_q = hmd.transform.localRotation;
            gc.startStroke(Side_Left, hmd_p, hmd_q);
            gesture_started = true;
        }
        if (trigger_pressed_right == false && trigger_right > 0.9)
        {
            // Controller trigger pressed.
            trigger_pressed_right = true;
            GameObject hmd = GameObject.Find("Main Camera"); // alternative: Camera.main.gameObject
            Vector3 hmd_p = hmd.transform.localPosition;
            Quaternion hmd_q = hmd.transform.localRotation;
            gc.startStroke(Side_Right, hmd_p, hmd_q);
            gesture_started = true;
        }
        if (gesture_started == false) {
            // nothing to do.
            return;
        }

        // If we arrive here, the user is currently dragging with one of the controllers.
        if (trigger_pressed_left == true)
        {
            if (trigger_left < 0.8)
            {
                // User let go of a trigger and held controller still
                gc.endStroke(Side_Left);
                trigger_pressed_left = false;
            } else
            {
                // User still dragging or still moving after trigger pressed
                GameObject left_hand = GameObject.Find("Left Hand");
                gc.contdStrokeQ(Side_Left, left_hand.transform.position, left_hand.transform.rotation);
                // Show the stroke by instatiating new objects
                addToStrokeTrail(left_hand.transform.position);
            }
        }

        if (trigger_pressed_right == true)
        {
            if (trigger_right < 0.8)
            {
                // User let go of a trigger and held controller still
                gc.endStroke(Side_Right);
                trigger_pressed_right = false;
            }
            else
            {
                // User still dragging or still moving after trigger pressed
                GameObject right_hand = GameObject.Find("Right Hand");
                gc.contdStrokeQ(Side_Right, right_hand.transform.position, right_hand.transform.rotation);
                // Show the stroke by instatiating new objects
                addToStrokeTrail(right_hand.transform.position);
            }
        }

        if (trigger_pressed_left || trigger_pressed_right)
        {
            // User still dragging with either hand - nothing left to do
            return;
        }
        // else: if we arrive here, the user let go of both triggers, ending the gesture.
        gesture_started = false;

        // Delete the objectes that we used to display the gesture.
        foreach (string ball in stroke) {
            Destroy(GameObject.Find(ball));
            stroke_index = 0;
        }
        
        int gesture_combination_id = gc.identifyGestureCombination();
        if (gesture_combination_id < 0)
        {
            HUDText.text = "Failed to identify texture";
            return; // something went wrong
        }
        string gesture_combination_name = gc.getGestureCombinationName(gesture_combination_id);
        if (current_tutorial_step >=0)
        {
            // currently in tutorial mode
            if (String.Compare(gesture_combination_name, 0, gestures[current_tutorial_step], 0, gestures[current_tutorial_step].Length, true) != 0)
            {
                HUDText.text = "INCORRECT!\nPlease try again."; // \nRequested gesture was: '"+ gestures[current_tutorial_step]+"'\nDetected gesture was: '"+ gesture_combination_name+"'\nPlease try again.";
                
            }
            else
            {
                current_tutorial_step++;
                if (current_tutorial_step < gestures.Length)
                {
                    HUDText.text = "CORRECT!\nNext gesture:";
#if UNITY_EDITOR
                    illustration_image.texture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture>(illustration_path + gestures[current_tutorial_step] + ".png");
#else
                    illustration_image.texture = Resources.Load<Texture>(illustration_path + gestures[current_tutorial_step]);
#endif
                } else
                {
                    HUDText.text = "CORRECT!\nTutorial finished.\nFeel free to try out any of the gestures now.";
                    current_tutorial_step = -1;
                    illustration_image.texture = null;
                }
            }
            return;
        }
        // else: if we arrive here: the tutorial was finished
        // Show the gesture illustration
        int gesture_index = 0;
        for (; gesture_index < gestures.Length; gesture_index++)
        {
            if (String.Compare(gesture_combination_name, 0, gestures[gesture_index], 0, gestures[gesture_index].Length, true) == 0)
            {
                break;
            }
        }
        if (gesture_index >= gestures.Length)
        {
            return;
        }
#if UNITY_EDITOR
        illustration_image.texture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture>(illustration_path + gestures[gesture_index] + ".png");
#else
        illustration_image.texture = Resources.Load<Texture>(illustration_path + gestures[gesture_index]);
#endif
        HUDText.text = "Identified gesture:"; //  + gesture_combination_name;
    }


    // Helper function to add a new star to the stroke trail.
    public void addToStrokeTrail(Vector3 p)
    {
        GameObject ball_instance = Instantiate(GameObject.Find("ball"));
        GameObject ball = new GameObject("stroke_" + stroke_index++);
        ball_instance.name = ball.name + "_instance";
        ball_instance.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        ball_instance.transform.SetParent(ball.transform, false);
        System.Random random = new System.Random();
        ball.transform.localPosition = p;
        ball.transform.localRotation = new Quaternion((float)random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f).normalized;
        ball.transform.localScale    = new Vector3(0.5f, 0.5f, 0.5f);
        stroke.Add(ball.name);
    }
    
}
