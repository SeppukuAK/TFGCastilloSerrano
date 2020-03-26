#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEditor;

[CustomEditor(typeof(GestureManager))]
public class GestureManagerEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        // DrawDefaultInspector();

        serializedObject.Update();

        GestureManager gm = (GestureManager)target;

        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("NUMBER OF GESTURE PARTS:");
        gm.numberOfParts = EditorGUILayout.IntField("Number of parts", gm.numberOfParts);
        EditorGUILayout.LabelField(" ", "1 for one-handed,");
        EditorGUILayout.LabelField(" ", "2 for two-handed or two sequential gestures,");
        EditorGUILayout.LabelField(" ", "3 for three sequential gestures, ...");
        if (GUILayout.Button("Update"))
        {
            if (gm.numberOfParts <= 0)
            {
                return;
            }
            if (gm.numberOfParts == 1)
            {
                gm.gc = null;
                if (gm.gr == null)
                {
                    gm.gr = new GestureRecognition();
                }
                gm.gr.setTrainingUpdateCallback(GestureManager.trainingUpdateCallback);
                gm.gr.setTrainingUpdateCallbackMetadata((IntPtr)gm.me);
                gm.gr.setTrainingFinishCallback(GestureManager.trainingFinishCallback);
                gm.gr.setTrainingFinishCallbackMetadata((IntPtr)gm.me);
            }
            else
            {
                gm.gr = null;
                if (gm.gc == null || gm.gc.numberOfParts() != gm.numberOfParts)
                {
                    gm.gc = new GestureCombinations(gm.numberOfParts);
                }
                gm.gc.setTrainingUpdateCallback(GestureManager.trainingUpdateCallback);
                gm.gc.setTrainingUpdateCallbackMetadata((IntPtr)gm.me);
                gm.gc.setTrainingFinishCallback(GestureManager.trainingFinishCallback);
                gm.gc.setTrainingFinishCallbackMetadata((IntPtr)gm.me);
            }
        }
        EditorGUILayout.EndVertical();

        if (gm.gc != null)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("HAND/PART MAPPING:", "(which hand performs which part)");
            int num_parts = gm.gc.numberOfParts();
            string[] part_names = new string[num_parts];
            for (int i = 0; i < num_parts; i++)
            {
                part_names[i] = "part (or side) " + i.ToString();
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Left hand:");
            gm.lefthand_combination_part = EditorGUILayout.Popup(gm.lefthand_combination_part, part_names);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Right hand:");
            gm.righthand_combination_part = EditorGUILayout.Popup(gm.righthand_combination_part, part_names);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        
        // Gesture file management
        if (gm.gr == null && gm.gc == null)
        {
            serializedObject.ApplyModifiedProperties();
            return;
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("GESTURE FILES:");
        if (gm.gr != null)
        {
            gm.file_load_gestures = EditorGUILayout.TextField("Load gestures file:", gm.file_load_gestures);
            if (GUILayout.Button("Load Gestures from File"))
            {
                bool success = gm.gr.loadFromFile(gm.file_load_gestures);
                Debug.Log((success ? "Gesture file loaded successfully" : "[ERROR] Failed to load gesture file."));
            }
            gm.file_save_gestures = EditorGUILayout.TextField("Save gestures file:", gm.file_save_gestures);
            if (GUILayout.Button("Save Gestures to File"))
            {
                bool success = gm.gr.saveToFile(gm.file_save_gestures);
                Debug.Log((success ? "Gesture file saved successfully" : "[ERROR] Failed to save gesture file."));
            }
        } else if (gm.gc != null)
        {
            gm.file_load_combinations = EditorGUILayout.TextField("Load GestureCombinations File: ", gm.file_load_combinations);
            if (GUILayout.Button("Load GestureCombinations File"))
            {
                bool success = gm.gc.loadFromFile(gm.file_load_combinations);
                Debug.Log((success ? "Gesture file loaded successfully" : "[ERROR] Failed to load gesture file."));
            }
            gm.file_save_combinations = EditorGUILayout.TextField("Save GestureCombinations File: ", gm.file_save_combinations);
            if (GUILayout.Button("Save GestureCombinations File"))
            {
                bool success = gm.gc.saveToFile(gm.file_save_combinations);
                Debug.Log((success ? "Gesture file saved successfully" : "[ERROR] Failed to save gesture file."));
            }
            EditorGUILayout.LabelField("(optional) Import single-handed gesture file:");
            gm.file_load_subgestures = EditorGUILayout.TextField("Import SubGestures File:", gm.file_load_subgestures);
            gm.file_load_subgestures_i = EditorGUILayout.IntField("^ ... for subgesture #", gm.file_load_subgestures_i);
            if (GUILayout.Button("Import SubGesture File"))
            {
                bool success = gm.gc.loadGestureFromFile(gm.file_load_subgestures_i, gm.file_load_subgestures);
                Debug.Log((success ? "Gesture file imported successfully" : "[ERROR] Failed to import gesture file."));
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("IGNORE HEAD ROTATION:");
        if (gm.gr != null)
        {
            gm.gr.ignoreHeadRotationLeftRight = EditorGUILayout.Toggle("Ignore left/right", gm.gr.ignoreHeadRotationLeftRight);
            gm.gr.ignoreHeadRotationUpDown = EditorGUILayout.Toggle("Ignore up/down", gm.gr.ignoreHeadRotationUpDown);
            gm.gr.ignoreHeadRotationTilt = EditorGUILayout.Toggle("Ignore tilting", gm.gr.ignoreHeadRotationTilt);
        } else if (gm.gc != null)
        {
            gm.gc.ignoreHeadRotationLeftRight = EditorGUILayout.Toggle("Ignore left/right", gm.gc.ignoreHeadRotationLeftRight);
            gm.gc.ignoreHeadRotationUpDown = EditorGUILayout.Toggle("Ignore up/down", gm.gc.ignoreHeadRotationUpDown);
            gm.gc.ignoreHeadRotationTilt = EditorGUILayout.Toggle("Ignore tilting", gm.gc.ignoreHeadRotationTilt);
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("GESTURES:");
        if (gm.gr != null)
        {
            int num_gestures = gm.gr.numberOfGestures();
            for (int i = 0; i < num_gestures; i++)
            {
                string gesture_name = gm.gr.getGestureName(i);
                int gesture_samples = gm.gr.getGestureNumberOfSamples(i);
                GUILayout.BeginHorizontal();
                string new_gesture_name = EditorGUILayout.TextField(gesture_name);
                if (gesture_name != new_gesture_name)
                {
                    bool success = gm.gr.setGestureName(i, new_gesture_name);
                    if (!success)
                    {
                        Debug.Log("[ERROR] Failed to rename gesture.");
                    }
                }
                EditorGUILayout.LabelField(gesture_samples.ToString() + " samples", GUILayout.Width(70));
                if (GUILayout.Button("Delete Last Sample"))
                {
                    bool success = gm.gr.deleteGestureSample(i, gesture_samples-1);
                    if (!success)
                    {
                        Debug.Log("[ERROR] Failed to delete gesture sample.");
                    }

                }
                if (GUILayout.Button("Delete All Samples"))
                {
                    bool success = gm.gr.deleteAllGestureSamples(i);
                    if (!success)
                    {
                        Debug.Log("[ERROR] Failed to delete gesture samples.");
                    }

                }
                if (GUILayout.Button("Delete Gesture"))
                {
                    bool success = gm.gr.deleteGesture(i);
                    if (!success)
                    {
                        Debug.Log("[ERROR] Failed to delete gesture.");
                    }

                }
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            gm.create_gesture_name = EditorGUILayout.TextField(gm.create_gesture_name);
            if (GUILayout.Button("Create new gesture"))
            {
                int gesture_id = gm.gr.createGesture(gm.create_gesture_name);
                if (gesture_id < 0)
                {
                    Debug.Log("[ERROR] Failed to create gesture.");
                }
                gm.create_gesture_name = "(new gesture name)";

            }
            GUILayout.EndHorizontal();
        }
        else if (gm.gc != null)
        {
            int num_combinations = gm.gc.numberOfGestureCombinations();
            int num_parts = gm.gc.numberOfParts();
            for (int combination_id = 0; combination_id < num_combinations; combination_id++)
            {
                string combination_name = gm.gc.getGestureCombinationName(combination_id);
                GUILayout.BeginHorizontal();
                string new_combination_name = EditorGUILayout.TextField(combination_name);
                if (combination_name != new_combination_name)
                {
                    bool success = gm.gc.setGestureCombinationName(combination_id, new_combination_name);
                    if (!success)
                    {
                        Debug.Log("[ERROR] Failed to rename GestureCombination.");
                    }
                }
                if (GUILayout.Button("Delete"))
                {
                    bool success = gm.gc.deleteGestureCombination(combination_id);
                    if (!success)
                    {
                        Debug.Log("[ERROR] Failed to delete gesture.");
                    }

                }
                GUILayout.EndHorizontal();
                for (int i = 0; i < num_parts; i++)
                {
                    int num_gestures = gm.gc.numberOfGestures(i);
                    string[] gesture_names = new string[num_gestures+1];
                    gesture_names[0] = "[NONE]";
                    for (int k = 0; k < num_gestures; k++)
                    {
                        gesture_names[k + 1] = gm.gc.getGestureName(i, k);
                    }
                    int connected_gesture_id = gm.gc.getCombinationPartGesture(combination_id, i);
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Subgesture for part/side " + i);
                    int new_connected_gesture_id = EditorGUILayout.Popup(connected_gesture_id + 1, gesture_names) - 1;
                    GUILayout.EndHorizontal();
                    if (new_connected_gesture_id != connected_gesture_id)
                    {
                        bool success = gm.gc.setCombinationPartGesture(combination_id, i, new_connected_gesture_id);
                        if (!success)
                        {
                            Debug.Log("[ERROR] Failed to change GestureCombination.");
                        }
                    }
                }
            }
            GUILayout.BeginHorizontal();
            gm.create_combination_name = EditorGUILayout.TextField(gm.create_combination_name);
            if (GUILayout.Button("Create new Gesture Combination"))
            {
                int gesture_id = gm.gc.createGestureCombination(gm.create_combination_name);
                if (gesture_id < 0)
                {
                    Debug.Log("[ERROR] Failed to create Gesture Combination.");
                }
                gm.create_combination_name = "(new Gesture Combination name)";
            }
            GUILayout.EndHorizontal();
            
            for (int part = 0; part < num_parts; part++)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Combination Part/Side:", part.ToString());
                int num_gestures = gm.gc.numberOfGestures(part);
                for (int i = 0; i < num_gestures; i++)
                {
                    string gesture_name = gm.gc.getGestureName(part, i);
                    int gesture_samples = gm.gc.getGestureNumberOfSamples(part, i);
                    GUILayout.BeginHorizontal();
                    string new_gesture_name = EditorGUILayout.TextField(gesture_name);
                    if (gesture_name != new_gesture_name)
                    {
                        bool success = gm.gc.setGestureName(part, i, new_gesture_name);
                        if (!success)
                        {
                            Debug.Log("[ERROR] Failed to rename gesture.");
                        }
                    }
                    EditorGUILayout.LabelField(gesture_samples.ToString() + " samples", GUILayout.Width(70));
                    if (GUILayout.Button("Delete Last Sample"))
                    {
                        bool success = gm.gc.deleteGestureSample(part, i, gesture_samples-1);
                        if (!success)
                        {
                            Debug.Log("[ERROR] Failed to delete gesture sample.");
                        }
                    }
                    if (GUILayout.Button("Delete All Samples"))
                    {
                        bool success = gm.gc.deleteAllGestureSamples(part, i);
                        if (!success)
                        {
                            Debug.Log("[ERROR] Failed to delete gesture samples.");
                        }
                    }
                    if (GUILayout.Button("Delete"))
                    {
                        bool success = gm.gc.deleteGesture(part, i);
                        if (!success)
                        {
                            Debug.Log("[ERROR] Failed to delete gesture.");
                        }
                    }
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                if (gm.create_gesture_names == null || gm.create_gesture_names.Length < num_parts)
                {
                    gm.create_gesture_names = new string[num_parts];
                    for (int i=0; i<num_parts; i++)
                    {
                        gm.create_gesture_names[i] = "(new gesture name)";
                    }
                }
                gm.create_gesture_names[part] = EditorGUILayout.TextField(gm.create_gesture_names[part]);
                if (GUILayout.Button("Add new gesture"))
                {
                    int gesture_id = gm.gc.createGesture(part, gm.create_gesture_names[part]);
                    if (gesture_id < 0)
                    {
                        Debug.Log("[ERROR] Failed to create gesture.");
                    }
                    gm.create_gesture_names[part] = "(new gesture name)";

                }
                GUILayout.EndHorizontal();
            }
            // copy gestures
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Copy gestures:");
            string[] part_names = new string[num_parts];
            for (int i = 0; i < num_parts; i++)
            {
                part_names[i] = i.ToString();
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("From (part/side):");
            gm.copy_gesture_part_from = EditorGUILayout.Popup(gm.copy_gesture_part_from, part_names);
            EditorGUILayout.EndHorizontal();

            int num_copy_gestures = gm.gc.numberOfGestures(gm.copy_gesture_part_from);
            string[] copy_gesture_names = new string[num_copy_gestures];
            for (int i = 0; i < num_copy_gestures; i++)
            {
                copy_gesture_names[i] = gm.gc.getGestureName(gm.copy_gesture_part_from, i);
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Gesture to copy:");
            gm.copy_gesture_id = EditorGUILayout.Popup(gm.copy_gesture_id, copy_gesture_names);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("To (part/side):");
            gm.copy_gesture_part_to = EditorGUILayout.Popup(gm.copy_gesture_part_to, part_names);
            EditorGUILayout.EndHorizontal();

            gm.copy_gesture_mirror = EditorGUILayout.Toggle("Mirror left/right", gm.copy_gesture_mirror);

            if (GUILayout.Button("Copy"))
            {
                int new_gesture = gm.gc.copyGesture(gm.copy_gesture_part_from, gm.copy_gesture_id, gm.copy_gesture_part_to, gm.copy_gesture_mirror, false, false);
                if (new_gesture < 0)
                {
                    Debug.Log("[ERROR] Failed to copy gesture.");
                }
            }

            //EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();


        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("RECORD GESTURE SAMPLES:");
        if (gm.gr != null)
        {
            int num_gestures = gm.gr.numberOfGestures();
            string[] gesture_names = new string[num_gestures+1];
            gesture_names[0] = "[Testing, not recording]";
            for (int i=0; i<num_gestures; i++)
            {
                gesture_names[i+1] = gm.gr.getGestureName(i);
            }
            gm.record_gesture_id = EditorGUILayout.Popup(gm.record_gesture_id +1, gesture_names)-1;
        } else if (gm.gc != null)
        {
            int num_combinations = gm.gc.numberOfGestureCombinations();
            string[] combination_names = new string[num_combinations + 1];
            combination_names[0] = "[Testing, not recording]";
            for (int i = 0; i < num_combinations; i++)
            {
                combination_names[i+1] = gm.gc.getGestureCombinationName(i);
            }
            gm.record_combination_id = EditorGUILayout.Popup(gm.record_combination_id + 1, combination_names) - 1;
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("START/STOP TRAINING:");
        if (gm.gr != null)
        {
            EditorGUILayout.LabelField("Performance:", (gm.gr.recognitionScore() * 100.0).ToString()+"%");
            EditorGUILayout.LabelField("Currently training:", (gm.gr.isTraining() ? "yes" : "no"));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Start training"))
            {
                bool success = gm.gr.startTraining();
                Debug.Log((success ? "Training successfully started" : "[ERROR] Failed to start training."));

            }
            if (GUILayout.Button("Stop training"))
            {
                gm.gr.stopTraining();
            }
            GUILayout.EndHorizontal();
        } else if (gm.gc != null)
        {
            EditorGUILayout.LabelField("Performance:", (gm.gc.recognitionScore() * 100.0).ToString() + "%");
            EditorGUILayout.LabelField("Currently training:", (gm.gc.isTraining() ? "yes" : "no"));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Start training"))
            {
                bool success = gm.gc.startTraining();
                Debug.Log((success ? "Training successfully started" : "[ERROR] Failed to start training."));

            }
            if (GUILayout.Button("Stop training"))
            {
                gm.gc.stopTraining();
            }
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
