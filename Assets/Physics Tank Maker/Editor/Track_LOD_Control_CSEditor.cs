using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Track_LOD_Control_CS))]
	public class Track_LOD_Control_CSEditor : Editor
	{

		SerializedProperty Static_TrackProp;
		SerializedProperty Scroll_Track_LProp;
		SerializedProperty Scroll_Track_RProp;
		SerializedProperty ThresholdProp;

		void OnEnable ()
		{
			Static_TrackProp = serializedObject.FindProperty ("Static_Track");
			Scroll_Track_LProp = serializedObject.FindProperty ("Scroll_Track_L");
			Scroll_Track_RProp = serializedObject.FindProperty ("Scroll_Track_R");
			ThresholdProp = serializedObject.FindProperty ("Threshold");
		}

		public override void OnInspectorGUI ()
		{
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			serializedObject.Update ();

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Track LOD Settings", MessageType.None, true);

			EditorGUILayout.Space ();
			Static_TrackProp.objectReferenceValue = EditorGUILayout.ObjectField ("Static_Track", Static_TrackProp.objectReferenceValue, typeof(GameObject), true);
			Scroll_Track_LProp.objectReferenceValue = EditorGUILayout.ObjectField ("Scroll_Track (Left)", Scroll_Track_LProp.objectReferenceValue, typeof(GameObject), true);
			Scroll_Track_RProp.objectReferenceValue = EditorGUILayout.ObjectField ("Scroll_Track (Right)", Scroll_Track_RProp.objectReferenceValue, typeof(GameObject), true);

			EditorGUILayout.Space ();
			EditorGUILayout.Slider (ThresholdProp, 1.0f, 64.0f, "Threshold");

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			//
			serializedObject.ApplyModifiedProperties ();
		}



	}
}
