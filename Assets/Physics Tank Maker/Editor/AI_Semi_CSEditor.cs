using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(AI_Semi_CS))]
	public class AI_Semi_CSEditor : Editor
	{

		SerializedProperty BrakeTurn_Min_AngleProp;
		SerializedProperty BrakeTurn_Max_AngleProp;
		SerializedProperty Pivot_Turn_AngleProp;
		SerializedProperty Min_Target_AngleProp;
		SerializedProperty Min_Turn_RateProp;
		SerializedProperty Max_Turn_RateProp;
		SerializedProperty Min_Speed_RateProp;
		SerializedProperty Max_Speed_RateProp;
		SerializedProperty SlowDown_RangeProp;
		SerializedProperty Max_Speed_ErrorProp;
		SerializedProperty Sky_DistanceProp;
		SerializedProperty Marker_PrefabProp;

		void OnEnable ()
		{
			BrakeTurn_Min_AngleProp  = serializedObject.FindProperty ("BrakeTurn_Min_Angle");
			BrakeTurn_Max_AngleProp  = serializedObject.FindProperty ("BrakeTurn_Max_Angle");
			Pivot_Turn_AngleProp = serializedObject.FindProperty ("Pivot_Turn_Angle");
			Min_Target_AngleProp = serializedObject.FindProperty ("Min_Target_Angle");
			Min_Turn_RateProp = serializedObject.FindProperty ("Min_Turn_Rate");
			Max_Turn_RateProp = serializedObject.FindProperty ("Max_Turn_Rate");
			Min_Speed_RateProp = serializedObject.FindProperty ("Min_Speed_Rate");
			Max_Speed_RateProp = serializedObject.FindProperty ("Max_Speed_Rate");
			SlowDown_RangeProp = serializedObject.FindProperty ("SlowDown_Range");
			Max_Speed_ErrorProp = serializedObject.FindProperty ("Max_Speed_Error");
			Sky_DistanceProp = serializedObject.FindProperty ("Sky_Distance");
			Marker_PrefabProp = serializedObject.FindProperty ("Marker_Prefab");
		}

		public override void OnInspectorGUI ()
		{
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			serializedObject.Update ();

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Drive Settings", MessageType.None, true);
			EditorGUILayout.Slider (BrakeTurn_Min_AngleProp, 1.0f, 60.0f, "BrakeTurn Min Angle");
			EditorGUILayout.Slider (BrakeTurn_Max_AngleProp, 90.0f, 720.0f, "BrakeTurn Max Angle");
			EditorGUILayout.Slider (Pivot_Turn_AngleProp, 0.0f, 360.0f, "Pivot Turn Angle");
			EditorGUILayout.Slider (Min_Target_AngleProp, 0.0f, 10.0f, "Min Target Angle");
			EditorGUILayout.Space ();
			EditorGUILayout.Slider (Min_Turn_RateProp, 0.0f, 1.0f, "Min Turn Rate");
			EditorGUILayout.Slider (Max_Turn_RateProp, 0.0f, 1.0f, "Max Turn Rate");
			EditorGUILayout.Slider (Min_Speed_RateProp, 0.0f, 1.0f, "Min Speed Rate");
			EditorGUILayout.Slider (Max_Speed_RateProp, 0.0f, 1.0f, "Max Speed Rate");
			EditorGUILayout.Space ();
			EditorGUILayout.Slider (SlowDown_RangeProp, 0.0f, 100.0f, "Slow Down Range");
			EditorGUILayout.Slider (Max_Speed_ErrorProp, 0.0f, -10.0f, "Max Speed Error");
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Operation Settings", MessageType.None, true);
			EditorGUILayout.Slider (Sky_DistanceProp, 128.0f, 2048.0f, "Sky Distance");
			Marker_PrefabProp.objectReferenceValue = EditorGUILayout.ObjectField ("Marker Prefab", Marker_PrefabProp.objectReferenceValue, typeof(GameObject), false);
			EditorGUILayout.Space ();

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			serializedObject.ApplyModifiedProperties ();
		}

	}
}
