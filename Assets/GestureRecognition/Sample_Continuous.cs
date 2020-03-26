/*
 * Advaced Gesture Recognition - Unity Plug-In
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
using AOT;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.Networking;

public class Sample_Continuous : MonoBehaviour
{
    // The interval in seconds at which to perform the
    // identification (or recording) of the gesture.
    [SerializeField] private float RecognitionInterval;

    // The approximate duration in seconds of the gestures to be
    // detected.
    [SerializeField] private float GesturePeriod;

    // The file from which to load gestures on startup.
    // For example: "Assets/GestureRecognition/sample_gestures.dat"
    [SerializeField] private string LoadGesturesFile;

    // File where to save recorded gestures.
    // For example: "Assets/GestureRecognition/my_custom_gestures.dat"
    [SerializeField] private string SaveGesturesFile;

    // The gesture recognition object:
    // You can have as many of these as you want simultaneously.
    private GestureRecognition gr = new GestureRecognition();

    // The text field to display instructions.
    private Text HUDText;

    // The game object associated with the currently active controller (if any):
    private GameObject active_controller = null;

    // The game object associated with the currently active controller (if any):
    private bool button_a_pressed = false;

    // ID of the gesture currently being recorded,
    // or: -1 if not currently recording a new gesture,
    // or: -2 if the AI is currently trying to learn to identify gestures
    // or: -3 if the AI has recently finished learning to identify gestures
    private int recording_gesture = -1; 

    // Last reported recognition performance (during training).
    // 0 = 0% correctly recognized, 1 = 100% correctly recognized.
    private double last_performance_report = 0;

    // Last timestamp when a gesture recognition was performed.
    private float last_recognition_time = 0.0f;

    // Timestamp when a gesture stroke was started.
    private float stroke_start_time = 0.0f;

    public class StrokePoint
    {
        public float time; // Time when the star object was created.
        public string name; // Name of the created star object.
        public static int stroke_index = 0;
        public StrokePoint(Vector3 p)
        {
            GameObject star_instance = Instantiate(GameObject.Find("star"));
            GameObject star = new GameObject("stroke_" + stroke_index++);
            star_instance.name = star.name + "_instance";
            star_instance.transform.SetParent(star.transform, false);
            System.Random random = new System.Random();
            star.transform.localPosition = new Vector3(p.x + (float)random.NextDouble() / 80, p.y + (float)random.NextDouble() / 80, p.z + (float)random.NextDouble() / 80);
            star.transform.localRotation = new Quaternion((float)random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f).normalized;
            float star_scale = (float)random.NextDouble() + 0.3f;
            star.transform.localScale = new Vector3(star_scale, star_scale, star_scale);
            this.time = Time.time;
            this.name = star.name;
        }
    };

    // Temporary storage for objects to display the gesture stroke.
    List<StrokePoint> stroke = new List<StrokePoint>(); 

    // List of Objects created with gestures:
    List<GameObject> created_objects = new List<GameObject>();

    // Handle to this object/script instance, so that callbacks from the plug-in arrive at the correct instance.
    GCHandle me;

    // Initialization:
    void Start ()
    {
        me = GCHandle.Alloc(this);
        
        if (RecognitionInterval == 0)
        {
            RecognitionInterval = 0.1f;
        }
        if (GesturePeriod == 0)
        {
            GesturePeriod = 1.0f;
        }

        // Load the set of gestures.
        if (LoadGesturesFile == null)
        {
            LoadGesturesFile = "Sample_Continuous_Gestures.dat";
        }

        // Find the location for the gesture database (.dat) file
#if UNITY_EDITOR
        // When running the scene inside the Unity editor,
        // we can just load the file from the Assets/ folder:
        string GesturesFilePath = "Assets/GestureRecognition";
#elif UNITY_ANDROID
        // On android, the file is in the .apk,
        // so we need to first "download" it to the apps' cache folder.
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        string GesturesFilePath = activity.Call <AndroidJavaObject>("getCacheDir").Call<string>("getCanonicalPath");
        UnityWebRequest request = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + LoadGesturesFile);
        request.SendWebRequest();
        while (!request.isDone) {
            // wait for file extraction to finish
        }
        if (request.isNetworkError)
        {
            HUDText.text = "Failed to extract sample gesture database file from apk.";
            return;
        }
        File.WriteAllBytes(GesturesFilePath + "/" + LoadGesturesFile, request.downloadHandler.data);
#else
        // This will be the case when exporting a stand-alone PC app.
        // In this case, we can load the gesture database file from the streamingAssets folder.
        string GesturesFilePath = Application.streamingAssetsPath;
#endif
        if (gr.loadFromFile(GesturesFilePath + "/" + LoadGesturesFile) == false)
        {
            HUDText.text = "Failed to load sample gesture database file\n";
            return;
        }
        gr.contdIdentificationSmoothing = 5;
        
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
        
        GameObject star = GameObject.Find("star");
        star.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        GameObject controller_dummy = GameObject.Find("controller_dummy");
        controller_dummy.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);


        // Set the welcome message.
        HUDText = GameObject.Find("HUDText").GetComponent<Text>();
        HUDText.text = "Welcome to MARUI Gesture Plug-in!\n"
                      + "Hold the trigger to draw gestures.\nAvailable gestures:";
        for (int i=0; i<gr.numberOfGestures(); i++)
        {
            HUDText.text += "\n" + (i + 1) + " : " + gr.getGestureName(i);
        }
        HUDText.text += "\nor: press 'A'/'X'/Menu button\nto create new gesture.";
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

        bool button_a_left = Input.GetButton("LeftControllerButtonA");
        bool button_a_right = Input.GetButton("RightControllerButtonA");
        if (button_a_pressed)
        {
            if (!button_a_left && !button_a_right)
            {
                button_a_pressed = false;
            }
            return;
        }

        // If recording_gesture is -3, that means that the AI has recently finished learning a new gesture.
        if (recording_gesture == -3) {
            // Show "finished" message.
            double performance = gr.recognitionScore();
            HUDText.text = "Training finished!\n(Final recognition performance = " + (performance * 100.0) + "%)\nFeel free to use your new gesture.";
            // Set recording_gesture to -1 to indicate normal operation (learning finished).
            recording_gesture = -1;
            return;
        }
        // If recording_gesture is -2, that means that the AI is currently learning a new gesture.
        if (recording_gesture == -2) {
            // Show "please wait" message
            HUDText.text = "...training...\n(Current recognition performance = " + (last_performance_report * 100.0) + "%)\nPress the 'A'/'X'/Menu button to stop training.";
            // In this mode, the user may press the "A/X/menu" button to cancel the learning process.
            if (button_a_left || button_a_right) {
                // Button pressed: stop the learning process.
                gr.stopTraining();
                recording_gesture = -3;
                button_a_pressed = true;
            }
            return;
		}
        // Else: if we arrive here, we're not in training/learning mode,
        // so the user can draw gestures.

        if (button_a_left || button_a_right)
        {
            button_a_pressed = true;
            // If recording_gesture is -1, we're currently not recording a new gesture.
            if (recording_gesture == -1) {
                // In this mode, the user can press button A/X/menu to create a new gesture
                recording_gesture = gr.createGesture("Your gesture #" + (gr.numberOfGestures() - 4));
                // from now on: recording a new gesture
                HUDText.text = "Learning a new gesture (" + (gr.getGestureName(recording_gesture)) + "):\nPlease perform the gesture for a while.\n(0 samples recorded)";
                gr.contdIdentificationPeriod = (int)(this.GesturePeriod * 1000.0f); // to milliseconds
            } else
            {
                HUDText.text = "Learning gestures - please wait...\n(press A/X/menu button to stop the learning process)";
                // Set up the call-backs to receive information about the learning process.
                gr.setTrainingUpdateCallback(trainingUpdateCallback);
                gr.setTrainingUpdateCallbackMetadata((IntPtr)me);
                gr.setTrainingFinishCallback(trainingFinishCallback);
                gr.setTrainingFinishCallbackMetadata((IntPtr)me);
                gr.setMaxTrainingTime(30);
                // Set recording_gesture to -2 to indicate that we're currently in learning mode.
                recording_gesture = -2;
                if (gr.startTraining() == false)
                {
                    Debug.Log("COULD NOT START TRAINING");
                }
            }
            return;
        }

        // If the user is not yet dragging (pressing the trigger) on either controller, he hasn't started a gesture yet.
        if (active_controller == null) {
            // If the user presses either controller's trigger, we start a new gesture.
            if (trigger_right > 0.9) {
                // Right controller trigger pressed.
                active_controller = GameObject.Find("Right Hand");
            } else if (trigger_left > 0.9) {
                // Left controller trigger pressed.
                active_controller = GameObject.Find("Left Hand");
            } else {
                // If we arrive here, the user is pressing neither controller's trigger:
                // nothing to do.
                return;
            }
            // If we arrive here: either trigger was pressed, so we start the gesture.
            gr.contdIdentificationPeriod = (int)(this.GesturePeriod * 1000.0f); // to milliseconds
            GameObject hmd = GameObject.Find("Main Camera"); // alternative: Camera.main.gameObject
            Vector3 hmd_p = hmd.transform.localPosition;
            Quaternion hmd_q = hmd.transform.localRotation;
            gr.startStroke(hmd_p, hmd_q, recording_gesture);
            this.stroke_start_time = Time.time;
        }

        double similarity = -1.0; // This will receive a value of how similar the performed gesture was to previous recordings.
        int gesture_id = -1; // This will receive the ID of the gesture.

        // If we arrive here, the user is currently dragging with one of the controllers.
        // Check if the user is still dragging or if he let go of the trigger button.
        if (trigger_left > 0.8 || trigger_right > 0.8)
        {
            Vector3 p = active_controller.transform.position;
            Quaternion q = active_controller.transform.rotation;
            gr.contdStrokeQ(p, q);
            // The user is still dragging with the controller: continue the gesture.
            if (Time.time - this.stroke_start_time > GesturePeriod && Time.time - this.last_recognition_time > RecognitionInterval)
            {
                GameObject hmd = GameObject.Find("Main Camera"); // alternative: Camera.main.gameObject
                Vector3 hmd_p = hmd.transform.localPosition;
                Quaternion hmd_q = hmd.transform.localRotation;
                if (recording_gesture >= 0)
                {
                    gr.contdRecord(hmd_p, hmd_q);
                    int num_samples = gr.getGestureNumberOfSamples(recording_gesture);
                    HUDText.text = "Learning a new gesture (" + (gr.getGestureName(recording_gesture)) + ").\n\n(" + num_samples + " samples recorded)";
                }
                else
                {
                    gesture_id = gr.contdIdentify(hmd_p, hmd_q, ref similarity);
                    if (gesture_id >= 0)
                    {
                        string gesture_name = gr.getGestureName(gesture_id);
                        HUDText.text = "Identifying gesture '" + gesture_name + "'.\n\n(similarity: " + similarity + "%)";
                    }
                }
                this.last_recognition_time = Time.time;
            }
            // Prune the stroke of all items that are too old to be used
            float cutoff_time = Time.time - GesturePeriod;
            while (stroke.Count > 0 && stroke[0].time < cutoff_time)
            {
                GameObject star_object = GameObject.Find(stroke[0].name);
                if (star_object != null)
                {
                    Destroy(star_object);
                }
                stroke.RemoveAt(0);
            }
            // Extend the stroke by instatiating new objects
            stroke.Add(new StrokePoint(p));
            return;
        }
        // else: if we arrive here, the user let go of the trigger, ending a gesture.
        active_controller = null;
        gr.cancelStroke();
        // Delete the objects that we used to display the gesture.
        foreach (StrokePoint star in stroke)
        {
            GameObject star_object = GameObject.Find(star.name);
            if (star_object != null)
            {
                Destroy(star_object);
            }
        }
        stroke.Clear();

        // If we are currently recording samples for a custom gesture, check if we have recorded enough samples yet.
        if (recording_gesture >= 0) {
            // Currently recording samples for a custom gesture - check how many we have recorded so far.
            int num_samples = gr.getGestureNumberOfSamples(recording_gesture);
            HUDText.text = "Learning a new gesture (" + (gr.getGestureName(recording_gesture)) + "):\n"
                         + "Please perform the gesture for a while.\n(" + num_samples + " samples recorded)\n"
                         + "\nor: press 'A'/'X'/Menu button\nto finish recording and start training.";
            return;
        }
        // else: if we arrive here, we're not recording new sampled for custom gestures,
        // but instead have identified a new gesture.
        // Perform the action associated with that gesture.


        HUDText = GameObject.Find("HUDText").GetComponent<Text>();
        HUDText.text = "Hold the trigger to draw gestures.\nAvailable gestures:";
        for (int i = 0; i < gr.numberOfGestures(); i++)
        {
            HUDText.text += "\n" + (i + 1) + " : " + gr.getGestureName(i);
        }
        HUDText.text += "\nor: press 'A'/'X'/Menu button\nto create new gesture.";
    }

    // Callback function to be called by the gesture recognition plug-in during the learning process.
    [MonoPInvokeCallback(typeof(GestureRecognition.TrainingCallbackFunction))]
    public static void trainingUpdateCallback(double performance, IntPtr ptr)
    {
        // Get the script/scene object back from metadata.
        GCHandle obj = (GCHandle)ptr;
        Sample_Continuous me = (obj.Target as Sample_Continuous);
        // Update the performance indicator with the latest estimate.
        me.last_performance_report = performance;
    }
    

    // Callback function to be called by the gesture recognition plug-in when the learning process was finished.
    [MonoPInvokeCallback(typeof(GestureRecognition.TrainingCallbackFunction))]
    public static void trainingFinishCallback(double performance, IntPtr ptr)
    {
        // Get the script/scene object back from metadata.
        GCHandle obj = (GCHandle)ptr;
        Sample_Continuous me = (obj.Target as Sample_Continuous);
        // Update the performance indicator with the latest estimate.
        me.last_performance_report = performance;
        // Signal that training was finished.
        me.recording_gesture = -3;

        // Save the data to file.
#if UNITY_EDITOR
        string GesturesFilePath = "Assets/GestureRecognition";
#elif UNITY_ANDROID
        string GesturesFilePath = Application.persistentDataPath;
#else
        string GesturesFilePath = Application.streamingAssetsPath;
#endif
        if (me.SaveGesturesFile == null)
        {
            me.SaveGesturesFile = "Sample_Continuous_MyRecordedGestures.dat";
        }
        me.gr.saveToFile(GesturesFilePath + "/" + me.SaveGesturesFile);
    }
}
