using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(AI_CS))]
	public class AI_CSEditor : Editor
	{
		SerializedProperty WayPoint_RadiusProp;
		SerializedProperty BrakeTurn_Min_AngleProp;
		SerializedProperty BrakeTurn_Max_AngleProp;
		SerializedProperty Pivot_Turn_AngleProp;
		SerializedProperty Min_Target_AngleProp;
		SerializedProperty Min_Turn_RateProp;
		SerializedProperty Max_Turn_RateProp;
		SerializedProperty Min_Speed_RateProp;
		SerializedProperty Max_Speed_RateProp;
		SerializedProperty SlowDown_RangeProp;
		SerializedProperty Stuck_CountProp;
		SerializedProperty Max_Speed_ErrorProp;

		SerializedProperty Direct_FireProp;
		SerializedProperty Fire_AngleProp;
		SerializedProperty Fire_CountProp;
	
		void OnEnable ()
		{
			WayPoint_RadiusProp = serializedObject.FindProperty ("WayPoint_Radius");
			BrakeTurn_Min_AngleProp  = serializedObject.FindProperty ("BrakeTurn_Min_Angle");
			BrakeTurn_Max_AngleProp  = serializedObject.FindProperty ("BrakeTurn_Max_Angle");
			Pivot_Turn_AngleProp = serializedObject.FindProperty ("Pivot_Turn_Angle");
			Min_Target_AngleProp = serializedObject.FindProperty ("Min_Target_Angle");
			Min_Turn_RateProp = serializedObject.FindProperty ("Min_Turn_Rate");
			Max_Turn_RateProp = serializedObject.FindProperty ("Max_Turn_Rate");
			Min_Speed_RateProp = serializedObject.FindProperty ("Min_Speed_Rate");
			Max_Speed_RateProp = serializedObject.FindProperty ("Max_Speed_Rate");
			SlowDown_RangeProp = serializedObject.FindProperty ("SlowDown_Range");
			Stuck_CountProp = serializedObject.FindProperty ("Stuck_Count");
			Max_Speed_ErrorProp = serializedObject.FindProperty ("Max_Speed_Error");

			Direct_FireProp = serializedObject.FindProperty ("Direct_Fire");
			Fire_AngleProp = serializedObject.FindProperty ("Fire_Angle");
			Fire_CountProp = serializedObject.FindProperty ("Fire_Count");

		}

		public override void OnInspectorGUI ()
		{
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			serializedObject.Update ();

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Drive Settings", MessageType.None, true);
			EditorGUILayout.Slider (WayPoint_RadiusProp, 0.0f, 1000.0f, "WayPoint Radius");
			EditorGUILayout.Space ();
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
			EditorGUILayout.Space ();
			EditorGUILayout.Slider (Stuck_CountProp, 1.0f, 10.0f, "Stuck Count");
			EditorGUILayout.Slider (Max_Speed_ErrorProp, 0.0f, -10.0f, "Max Speed Error");

			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Combat Settings", MessageType.None, true);
			Direct_FireProp.boolValue = EditorGUILayout.Toggle ("Direct Fire", Direct_FireProp.boolValue);
			EditorGUILayout.Slider (Fire_AngleProp, 0.0f, 45.0f, "Fire Angle");
			EditorGUILayout.Slider (Fire_CountProp, 0.0f, 10.0f, "Fire Count");
			EditorGUILayout.Space ();

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			serializedObject.ApplyModifiedProperties ();
		}
	
	}

}