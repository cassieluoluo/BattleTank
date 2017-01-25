using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ChobiAssets.PTM
{
	[ CustomEditor (typeof(RC_Camera_CS))]
	public class RC_Camera_CSEditor : Editor
	{
		SerializedProperty Input_TypeProp;
		SerializedProperty FOVProp;
		SerializedProperty Horizontal_SpeedProp;
		SerializedProperty Vertical_SpeedProp;
		SerializedProperty Zoom_SpeedProp;
		SerializedProperty Min_FOVProp;
		SerializedProperty Max_FOVProp;
		SerializedProperty Position_PackProp;

		string[] inputNames = { "Keyboard (Keyboard only)" ,"GamePad (Stick operation)" ,"GamePad (Trigger operation)" ,"GamePad (Stick+Trigger operation)" ,"Mouse + Keyboard (Default)" };
	
		void OnEnable ()
		{
			Input_TypeProp = serializedObject.FindProperty ("Input_Type");
			FOVProp = serializedObject.FindProperty ("FOV");
			Horizontal_SpeedProp = serializedObject.FindProperty ("Horizontal_Speed");
			Vertical_SpeedProp = serializedObject.FindProperty ("Vertical_Speed");
			Zoom_SpeedProp = serializedObject.FindProperty ("Zoom_Speed");
			Min_FOVProp = serializedObject.FindProperty ("Min_FOV");
			Max_FOVProp = serializedObject.FindProperty ("Max_FOV");
			Position_PackProp = serializedObject.FindProperty ("Position_Pack");
		}

		public override void OnInspectorGUI ()
		{
			if (EditorApplication.isPlaying == false) {
				GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
				serializedObject.Update ();

				EditorGUILayout.Space ();
				EditorGUILayout.Space ();	
				EditorGUILayout.HelpBox ("RC Camera settings.", MessageType.None, true);
				if (Input_TypeProp.intValue < 4) {
					EditorGUILayout.HelpBox ("Have you finished setting up 'Input Manager' ?", MessageType.Warning, true);
					EditorGUILayout.Space ();
				}
				Input_TypeProp.intValue = EditorGUILayout.Popup ("Input Device Type", Input_TypeProp.intValue, inputNames);

				EditorGUILayout.Space ();
				EditorGUILayout.Slider (Horizontal_SpeedProp, 0.1f, 10.0f, "Horizontal Speed");
				EditorGUILayout.Slider (Vertical_SpeedProp, 0.1f, 10.0f, "Vertical Speed");
				EditorGUILayout.Slider (Zoom_SpeedProp, 0.1f, 10.0f, "Zoom Speed");
				EditorGUILayout.Slider (FOVProp, 1.0f, 100.0f, "Initial FOV");
				EditorGUILayout.Slider (Min_FOVProp, 1.0f, 100.0f, "Minimum FOV");
				EditorGUILayout.Slider (Max_FOVProp, 1.0f, 100.0f, "Maximum FOV");
				Position_PackProp.objectReferenceValue = EditorGUILayout.ObjectField ("Camera Position Pack", Position_PackProp.objectReferenceValue, typeof(GameObject), true);

				EditorGUILayout.Space ();
				EditorGUILayout.Space ();

				serializedObject.ApplyModifiedProperties ();
			}
		}

	}

}