using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Tank_ID_Control_CS))]
	public class Tank_ID_Control_CSEditor : Editor
	{
	
		SerializedProperty Tank_IDProp;
		SerializedProperty RelationshipProp;
		SerializedProperty ReSpawn_TimesProp;
		SerializedProperty Attack_MultiplierProp;
		SerializedProperty Input_TypeProp;
		SerializedProperty Turn_TypeProp;
		SerializedProperty Auto_LeadProp;
		SerializedProperty Marker_NameProp;

		SerializedProperty WayPoint_PackProp;
		SerializedProperty Patrol_TypeProp;
		SerializedProperty Follow_TargetProp;

		SerializedProperty No_AttackProp;
		SerializedProperty BreakthroughProp;
		SerializedProperty Visibility_RadiusProp;
		SerializedProperty Approach_DistanceProp;
		SerializedProperty OpenFire_DistanceProp;
		SerializedProperty Lost_CountProp;
		SerializedProperty Face_EnemyProp;
		SerializedProperty Face_Offest_AngleProp;

		SerializedProperty AI_State_TextProp;
		SerializedProperty Tank_NameProp;

		SerializedProperty ReSpawn_IntervalProp;
		SerializedProperty Remove_TimeProp;

		SerializedProperty ReSpawn_FlagProp;
		SerializedProperty Prefab_PathProp;

		string[] idNames = { "Not Operable" ,"[ 1 ]" ,"[ 2 ]" ,"[ 3 ]" ,"[ 4 ]" ,"[ 5 ]" ,"[ 6 ]" ,"[ 7 ]" ,"[ 8 ]" ,"[ 9 ]" ,"[10]" };
		string[] relationshipNames = { "Friendly", "Hostile" };
		string[] inputNames = { "Keyboard (Keyboard only)" ,"GamePad (Stick operation)" ,"GamePad (Trigger operation)" ,"GamePad (Stick+Trigger operation)" ,"Mouse + Keyboard (Default)" ,"Mouse + Keyboard (Easy)" ,"Semi Automatic", "", "", "", "AI" };
		string[] turnNames = { "Easy Turn (Pivot Turn)", "Classic Turn (only Brake-Turn)" };

		string[] patrolTypeNames = { "Order", "Random" };

		void OnEnable ()
		{
			Tank_IDProp = serializedObject.FindProperty ("Tank_ID");
			RelationshipProp = serializedObject.FindProperty ("Relationship");
			ReSpawn_TimesProp = serializedObject.FindProperty ("ReSpawn_Times");
			Attack_MultiplierProp = serializedObject.FindProperty ("Attack_Multiplier");
			Input_TypeProp = serializedObject.FindProperty ("Input_Type");
			Turn_TypeProp = serializedObject.FindProperty ("Turn_Type");
			Auto_LeadProp = serializedObject.FindProperty ("Auto_Lead");
			Marker_NameProp = serializedObject.FindProperty ("Marker_Name");

			WayPoint_PackProp = serializedObject.FindProperty ("WayPoint_Pack");
			Patrol_TypeProp = serializedObject.FindProperty ("Patrol_Type");
			Follow_TargetProp = serializedObject.FindProperty ("Follow_Target");

			No_AttackProp = serializedObject.FindProperty ("No_Attack");
			BreakthroughProp = serializedObject.FindProperty ("Breakthrough");
			Visibility_RadiusProp = serializedObject.FindProperty ("Visibility_Radius");
			Approach_DistanceProp = serializedObject.FindProperty ("Approach_Distance");
			OpenFire_DistanceProp = serializedObject.FindProperty ("OpenFire_Distance");
			Lost_CountProp = serializedObject.FindProperty ("Lost_Count");
			Face_EnemyProp = serializedObject.FindProperty ("Face_Enemy");
			Face_Offest_AngleProp = serializedObject.FindProperty ("Face_Offest_Angle");

			AI_State_TextProp = serializedObject.FindProperty ("AI_State_Text");
			Tank_NameProp = serializedObject.FindProperty ("Tank_Name");

			ReSpawn_IntervalProp = serializedObject.FindProperty ("ReSpawn_Interval");
			Remove_TimeProp = serializedObject.FindProperty ("Remove_Time");

			ReSpawn_FlagProp = serializedObject.FindProperty ("ReSpawn_Flag");
			Prefab_PathProp = serializedObject.FindProperty ("Prefab_Path");
		}

		public override void OnInspectorGUI ()
		{
			if (EditorApplication.isPlaying == false) {
				GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
				serializedObject.Update ();

				// Set Prefab_Path and ReSpawn_Flag.
				Get_Prefab_Path ();

				EditorGUILayout.Space ();
				EditorGUILayout.Space ();	
				EditorGUILayout.HelpBox ("Basic settings.", MessageType.None, true);
				Tank_IDProp.intValue = EditorGUILayout.Popup ("Tank ID", Tank_IDProp.intValue, idNames);
				EditorGUILayout.Space ();
				RelationshipProp.intValue = EditorGUILayout.Popup ("Relationship", RelationshipProp.intValue, relationshipNames);

				EditorGUILayout.Space ();
				GUI.backgroundColor = new Color (1.0f, 0.5f, 0.5f, 1.0f);
				EditorGUILayout.IntSlider (ReSpawn_TimesProp, 0, 100, "ReSpawn Times");
				GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
				EditorGUILayout.Slider (Attack_MultiplierProp, 0.1f, 2.0f, "Attack Multiplier");

				EditorGUILayout.Space ();
				EditorGUILayout.HelpBox ("'Input Device Type' is ignored when this tank has AI.", MessageType.Warning, true);
				if (Input_TypeProp.intValue < 4) {
					EditorGUILayout.HelpBox ("Have you finished setting up 'Input Manager' ?", MessageType.Warning, true);
					EditorGUILayout.Space ();
				}
				Input_TypeProp.intValue = EditorGUILayout.Popup ("Input Device Type", Input_TypeProp.intValue, inputNames);
			
				if (Input_TypeProp.intValue == 0 || Input_TypeProp.intValue == 1 || Input_TypeProp.intValue == 5) {
					Turn_TypeProp.intValue = EditorGUILayout.Popup ("Turn Type", Turn_TypeProp.intValue, turnNames);
				}

				if (Input_TypeProp.intValue == 4 || Input_TypeProp.intValue == 5 || Input_TypeProp.intValue == 6) {
					Auto_LeadProp.boolValue = EditorGUILayout.Toggle ("Auto Lead", Auto_LeadProp.boolValue);
				}

				EditorGUILayout.Space ();
				Marker_NameProp.stringValue = EditorGUILayout.TextField ("Position Marker Name", Marker_NameProp.stringValue);

				EditorGUILayout.Space ();
				EditorGUILayout.Space ();

				// for AI.
				if (Input_TypeProp.intValue == 10) {
					EditorGUILayout.HelpBox ("AI Patrol Settings", MessageType.None, true);
					WayPoint_PackProp.objectReferenceValue = EditorGUILayout.ObjectField ("WayPoint Pack", WayPoint_PackProp.objectReferenceValue, typeof(GameObject), true);
					Patrol_TypeProp.intValue = EditorGUILayout.Popup ("Patrol Type", Patrol_TypeProp.intValue, patrolTypeNames);
					Follow_TargetProp.objectReferenceValue = EditorGUILayout.ObjectField ("Follow Target", Follow_TargetProp.objectReferenceValue, typeof(Transform), true);
					EditorGUILayout.Space ();
					EditorGUILayout.HelpBox ("AI Combat Settings", MessageType.None, true);
					No_AttackProp.boolValue = EditorGUILayout.Toggle ("No Attack", No_AttackProp.boolValue);
					BreakthroughProp.boolValue = EditorGUILayout.Toggle ("Breakthrough", BreakthroughProp.boolValue);
					EditorGUILayout.Slider (Visibility_RadiusProp, 0.1f, 10000.0f, "Visibility Radius");
					EditorGUILayout.Slider (Approach_DistanceProp, 1.0f, 10000.0f, "Approach Distance");
					if (Approach_DistanceProp.floatValue == 10000.0f) {
						Approach_DistanceProp.floatValue = Mathf.Infinity;
					}
					EditorGUILayout.Slider (OpenFire_DistanceProp, 1.0f, 10000.0f, "Open Fire Distance");
					if (OpenFire_DistanceProp.floatValue == 10000.0f) {
						OpenFire_DistanceProp.floatValue = Mathf.Infinity;
					}
					EditorGUILayout.Slider (Lost_CountProp, 0.0f, 100.0f, "Lost Count");
					Face_EnemyProp.boolValue = EditorGUILayout.Toggle ("Face the Enemy", Face_EnemyProp.boolValue);
					if (Face_EnemyProp.boolValue) {
						EditorGUILayout.Slider (Face_Offest_AngleProp, 0.0f, 90.0f, "Face Offest Angle");
					}
					EditorGUILayout.Space ();
					EditorGUILayout.HelpBox ("AI State Text Settings", MessageType.None, true);
					AI_State_TextProp.objectReferenceValue = EditorGUILayout.ObjectField ("Text", AI_State_TextProp.objectReferenceValue, typeof(Text), true);
					Tank_NameProp.stringValue = EditorGUILayout.TextField ("Tank Name", Tank_NameProp.stringValue);
					EditorGUILayout.Space ();
					EditorGUILayout.HelpBox ("Auto ReSpawn Settings", MessageType.None, true);
					EditorGUILayout.Slider (ReSpawn_IntervalProp, 1.0f, 100.0f, "Interval Time");
					EditorGUILayout.Slider (Remove_TimeProp, 10.0f, 120.0f, "Remove Time");
					if (Remove_TimeProp.floatValue >= 120.0f) {
						Remove_TimeProp.floatValue = Mathf.Infinity;
					}
				}

				EditorGUILayout.Space ();
				EditorGUILayout.Space ();	
				if (ReSpawn_FlagProp.boolValue) {
					EditorGUILayout.HelpBox ("Path of the prefab in 'Resources' folder.", MessageType.None, true);
					EditorGUILayout.HelpBox (Prefab_PathProp.stringValue, MessageType.None, true);
				} else {
					EditorGUILayout.HelpBox ("Tank prefab must be placed under 'Resources' folder.", MessageType.Error, true);
				}

				EditorGUILayout.Space ();
				EditorGUILayout.Space ();

				serializedObject.ApplyModifiedProperties ();
			}
		}

		void Get_Prefab_Path ()
		{
			Object tempObject = PrefabUtility.GetPrefabParent (Selection.activeGameObject);
			if (tempObject) {
				string path = AssetDatabase.GetAssetPath (tempObject);
				if (string.IsNullOrEmpty (path) == false) {
					int index = path.IndexOf ("Resources/");
					if (index < 0) { // This prefab is not placed in 'Resources' folder.
						ReSpawn_FlagProp.boolValue = false;
						Prefab_PathProp.stringValue = null;
					} else {
						path = path.Substring (index + 10);
						index = path.IndexOf (".prefab");
						if (index < 0) {
							ReSpawn_FlagProp.boolValue = false;
							Prefab_PathProp.stringValue = null;
						} else {
							ReSpawn_FlagProp.boolValue = true;
							path = path.Substring (0, path.Length - 7);
							Prefab_PathProp.stringValue = path;
						}
					}
				}
			}
		}

	}

}