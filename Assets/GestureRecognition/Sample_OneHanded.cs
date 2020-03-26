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

public class Sample_OneHanded : MonoBehaviour
{
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
    private GameObject           active_controller = null;

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

    // Temporary storage for objects to display the gesture stroke.
    List<string> stroke = new List<string>(); 

    // Temporary counter variable when creating objects for the stroke display:
    int stroke_index = 0; 

    // List of Objects created with gestures:
    List<GameObject> created_objects = new List<GameObject>();

    // Handle to this object/script instance, so that callbacks from the plug-in arrive at the correct instance.
    GCHandle me;

    // Initialization:
    void Start ()
    {
        // Set the welcome message.
        HUDText = GameObject.Find("HUDText").GetComponent<Text>();
        HUDText.text = "Welcome to MARUI Gesture Plug-in!\n"
                      + "Press the trigger to draw a gesture. Available gestures:\n"
                      + "1 - a circle/ring (creates a cylinder)\n"
                      + "2 - swipe left/right (rotate object)\n"
                      + "3 - shake (delete object)\n"
                      + "4 - draw a sword from your hip,\nhold it over your head (magic)\n"
                      + "or: press 'A'/'X'/Menu button\nto create new gesture.";

        me = GCHandle.Alloc(this);

        // Load the set of gestures.
        if (LoadGesturesFile == null)
        {
            LoadGesturesFile = "Sample_OneHanded_Gestures.dat";
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

        // Reset the skybox tint color
        RenderSettings.skybox.SetColor("_Tint", new Color(0.5f, 0.5f, 0.5f, 1.0f));

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
            HUDText.text = "...training...\n(Current recognition performance = " + (last_performance_report * 100.0) + "%)\nPress the 'A'/'X'/Menu button to cancel training.";
            // In this mode, the user may press the "A/X/menu" button to cancel the learning process.
            if (button_a_left || button_a_right) {
                // Button pressed: stop the learning process.
                gr.stopTraining();
                recording_gesture = -3;
            }
            return;
		}
        // Else: if we arrive here, we're not in training/learning mode,
        // so the user can draw gestures.

        // If recording_gesture is -1, we're currently not recording a new gesture.
        if (recording_gesture == -1) {
            // In this mode, the user can press button A/X/menu to create a new gesture
            if (button_a_left || button_a_right) {
                recording_gesture = gr.createGesture("custom gesture " + (gr.numberOfGestures() - 5));
                // from now on: recording a new gesture
                HUDText.text = "Learning a new gesture (custom gesture " + (recording_gesture-4) + "):\nPlease perform the gesture 25 times.\n(0 / 25)";
            }
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
            GameObject hmd = GameObject.Find("Main Camera"); // alternative: Camera.main.gameObject
            Vector3 hmd_p = hmd.transform.localPosition;
            Quaternion hmd_q = hmd.transform.localRotation;
            gr.startStroke(hmd_p, hmd_q, recording_gesture);
        }

        // If we arrive here, the user is currently dragging with one of the controllers.
        // Check if the user is still dragging or if he let go of the trigger button.
        if (trigger_left > 0.8 || trigger_right > 0.8) {
            // The user is still dragging with the controller: continue the gesture.
            Vector3 p = active_controller.transform.position;
            Quaternion q = active_controller.transform.rotation;
            gr.contdStrokeQ(p, q);
            // Show the stroke by instatiating new objects
            GameObject star_instance = Instantiate(GameObject.Find("star"));
            GameObject star = new GameObject("stroke_" + stroke_index++);
            star_instance.name = star.name + "_instance";
            star_instance.transform.SetParent(star.transform, false);
            System.Random random = new System.Random();
            star.transform.localPosition = new Vector3(p.x + (float)random.NextDouble() / 80, p.y + (float)random.NextDouble() / 80, p.z + (float)random.NextDouble() / 80);
            star.transform.localRotation = new Quaternion((float)random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f, (float)random.NextDouble() - 0.5f).normalized;
            //star.transform.localRotation.Normalize();
            float star_scale = (float)random.NextDouble() + 0.3f;
            star.transform.localScale    = new Vector3(star_scale, star_scale, star_scale);
            stroke.Add(star.name);
            return;
        }
        // else: if we arrive here, the user let go of the trigger, ending a gesture.
        active_controller = null;

        // Delete the objects that we used to display the gesture.
        foreach (string star in stroke) {
            GameObject star_object = GameObject.Find(star);
            if (star_object != null) {
                Destroy(star_object);
            }
        }
        stroke.Clear();
        stroke_index = 0;

        double similarity = 0; // This will receive a value of how similar the performed gesture was to previous recordings.
        Vector3 pos = Vector3.zero; // This will receive the position where the gesture was performed.
        double scale = 0; // This will receive the scale at which the gesture was performed.
        Vector3 dir0 = Vector3.zero; // This will receive the primary direction in which the gesture was performed (greatest expansion).
        Vector3 dir1 = Vector3.zero; // This will receive the secondary direction of the gesture.
        Vector3 dir2 = Vector3.zero; // This will receive the minor direction of the gesture (direction of smallest expansion).
        int gesture_id = gr.endStroke(ref similarity, ref pos, ref scale, ref dir0, ref dir1, ref dir2);

        // if (similarity < ???) {
        //     ...maybe this is not the gesture I was looking for...
        // }

        // If we are currently recording samples for a custom gesture, check if we have recorded enough samples yet.
        if (recording_gesture >= 0) {
            // Currently recording samples for a custom gesture - check how many we have recorded so far.
            int num_samples = gr.getGestureNumberOfSamples(recording_gesture);
            if (num_samples < 25) {
                // Not enough samples recorded yet.
                HUDText.text = "Learning a new gesture (custom gesture " + (recording_gesture - 4) + "):\nPlease perform the gesture 25 times.\n(" + num_samples + " / 25)";
            } else {
                // Enough samples recorded. Start the learning process.
                HUDText.text = "Learning gestures - please wait...\n(press A/X/menu button to stop the learning process)";
                // Set up the call-backs to receive information about the learning process.
                gr.setTrainingUpdateCallback(trainingUpdateCallback);
                gr.setTrainingUpdateCallbackMetadata((IntPtr)me);
                gr.setTrainingFinishCallback(trainingFinishCallback);
                gr.setTrainingFinishCallbackMetadata((IntPtr)me);
                gr.setMaxTrainingTime(10);
                // Set recording_gesture to -2 to indicate that we're currently in learning mode.
                recording_gesture = -2;
                if (gr.startTraining() == false) {
                    Debug.Log("COULD NOT START TRAINING");
                }
            }
            return;
        }
        // else: if we arrive here, we're not recording new sampled for custom gestures,
        // but instead have identified a new gesture.
        // Perform the action associated with that gesture.

        if (gesture_id < 0)
        {
            // Error trying to identify any gesture
            HUDText.text = "Failed to identify gesture.";
        }
        else if (gesture_id == 0)
        {
            // "loop"-gesture: create cylinder
            HUDText.text = "Identified a CIRCLE/LOOP gesture!";
            GameObject cylinder = Instantiate(GameObject.Find("controller_dummy"));
            cylinder.transform.localPosition = pos;
            cylinder.transform.localRotation = Quaternion.FromToRotation(new Vector3(0, 1, 0), dir2);
            cylinder.transform.localScale = new Vector3((float)scale * 2, (float)scale, (float)scale * 2);
            created_objects.Add(cylinder);
        }
        else if (gesture_id == 1)
        {
            // "swipe left"-gesture: rotate left
            HUDText.text = "Identified a SWIPE LEFT gesture!";
            GameObject closest_object = getClosestObject(pos);
            if (closest_object != null)
            {
                closest_object.transform.Rotate(new Vector3(0, 1, 0), (float)scale * 400, Space.World);
            }
        }
        else if (gesture_id == 2)
        {
            // "swipe right"-gesture: rotate right
            HUDText.text = "Identified a SWIPE RIGHT gesture!";
            GameObject closest_object = getClosestObject(pos);
            if (closest_object != null)
            {
                closest_object.transform.Rotate(new Vector3(0, 1, 0), -(float)scale * 400, Space.World);
            }
        }
        else if (gesture_id == 3)
        {
            // "shake" or "scrap" gesture: delete closest object
            HUDText.text = "Identified a SHAKE gesture!";
            GameObject closest_object = getClosestObject(pos);
            if (closest_object != null)
            {
                Destroy(closest_object);
                created_objects.Remove(closest_object);
            }
        }
        else if (gesture_id == 4)
        {
            // "draw sword" gesture
            HUDText.text = "MAGIC!";
            Color col = RenderSettings.skybox.GetColor("_Tint");
            if (col.r < 0.51)
            {
                RenderSettings.skybox.SetColor("_Tint", new Color(0.53f, 0.17f, 0.17f, 1.0f));
            }
            else // reset the tint
            {
                RenderSettings.skybox.SetColor("_Tint", new Color(0.5f, 0.5f, 0.5f, 1.0f));
            }
        }
        else
        {
            // Other ID: one of the user-registered gestures:
            HUDText.text = "Identified custom registered gesture " + (gesture_id - 4);
        }
    }

    // Callback function to be called by the gesture recognition plug-in during the learning process.
    [MonoPInvokeCallback(typeof(GestureRecognition.TrainingCallbackFunction))]
    public static void trainingUpdateCallback(double performance, IntPtr ptr)
    {
        // Get the script/scene object back from metadata.
        GCHandle obj = (GCHandle)ptr;
        Sample_OneHanded me = (obj.Target as Sample_OneHanded);
        // Update the performance indicator with the latest estimate.
        me.last_performance_report = performance;
    }
    

    // Callback function to be called by the gesture recognition plug-in when the learning process was finished.
    [MonoPInvokeCallback(typeof(GestureRecognition.TrainingCallbackFunction))]
    public static void trainingFinishCallback(double performance, IntPtr ptr)
    {
        // Get the script/scene object back from metadata.
        GCHandle obj = (GCHandle)ptr;
        Sample_OneHanded me = (obj.Target as Sample_OneHanded);
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
            me.SaveGesturesFile = "Sample_OneHanded_MyRecordedGestures.dat";
        }
        me.gr.saveToFile(GesturesFilePath + "/" + me.SaveGesturesFile);
    }

    // Helper function to find a GameObject in the world based on it's position.
    private GameObject getClosestObject(Vector3 pos)
    {
        GameObject closest_object = null;
        foreach (GameObject o in created_objects)
        {
            if (closest_object == null || (o.transform.localPosition - pos).magnitude < (closest_object.transform.localPosition - pos).magnitude)
            {
                closest_object = o;
            }
        }
        return closest_object;
    }
}
