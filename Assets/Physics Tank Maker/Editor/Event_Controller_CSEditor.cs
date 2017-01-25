using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Event_Controller_CS))]
	public class Event_Controller_CSEditor : Editor
	{

		SerializedProperty Trigger_TypeProp;
		SerializedProperty Trigger_TimeProp;
		SerializedProperty Trigger_Setting_TypeProp;
		SerializedProperty Trigger_NumProp;
		SerializedProperty Trigger_TanksProp;
		SerializedProperty Operator_TypeProp;
		SerializedProperty All_Trigger_FlagProp;
		SerializedProperty Necessary_NumProp;
		SerializedProperty Trigger_Collider_ScriptProp;
		SerializedProperty Useless_Event_NumProp;
		SerializedProperty Useless_EventsProp;
		SerializedProperty Disabled_Event_NumProp;
		SerializedProperty Disabled_EventsProp;
		SerializedProperty Event_TypeProp;

		SerializedProperty Event_TextProp;
		SerializedProperty Event_MessageProp;
		SerializedProperty Event_Message_ColorProp;
		SerializedProperty Event_Message_TimeProp;

		SerializedProperty Prefab_ObjectProp;
		SerializedProperty Key_NameProp;
		SerializedProperty Tank_IDProp;
		SerializedProperty RelationshipProp;
		SerializedProperty ReSpawn_TimesProp;
		SerializedProperty Attack_MultiplierProp;
		SerializedProperty Input_TypeProp;
		SerializedProperty Turn_TypeProp;
		SerializedProperty Auto_LeadProp;

		SerializedProperty OverWrite_FlagProp;
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

		SerializedProperty Trigger_Itself_FlagProp;
		SerializedProperty Target_NumProp;
		SerializedProperty Target_TanksProp;
		SerializedProperty New_WayPoint_PackProp;
		SerializedProperty New_Patrol_TypeProp;
		SerializedProperty New_Follow_TargetProp;
		SerializedProperty New_No_AttackProp;
		SerializedProperty New_BreakthroughProp;
		SerializedProperty New_Visibility_RadiusProp;
		SerializedProperty New_Approach_DistanceProp;
		SerializedProperty New_OpenFire_DistanceProp;
		SerializedProperty New_Lost_CountProp;
		SerializedProperty New_Face_EnemyProp;
		SerializedProperty New_Face_Offest_AngleProp;
		SerializedProperty Renew_ReSpawn_Times_FlagProp;
		SerializedProperty New_ReSpawn_TimesProp;

		SerializedProperty Artillery_ScriptProp;
		SerializedProperty Artillery_TargetProp;
		SerializedProperty Artillery_NumProp;

		string[] triggerNames = { "Timer", "Destroy", "Trigger Collider" };
		string[] triggerSettingNames = { "Set Manually", "Any Hostile tank", "Any Friendly tank" };
		string[] operatorNames = { "AND", "OR" };
		string[] eventNames = { "Spawn Tank", "Show Message", "Change AI Settings", "Remove Tank", "Artillery Fire", "", "", "", "", "","None" };
		string[] idNames = { "Not Operable", "[ 1 ]", "[ 2 ]", "[ 3 ]", "[ 4 ]", "[ 5 ]", "[ 6 ]", "[ 7 ]", "[ 8 ]", "[ 9 ]", "[10]" };
		string[] relationshipNames = { "Friendly", "Hostile" };
		string[] inputNames = { "Keyboard ( Keyboard only )", "GamePad ( Stick operation )", "GamePad ( Trigger operation )", "GamePad ( Stick+Trigger operation )", "Mouse + Keyboard ( Default )", "Mouse + Keyboard ( Easy )", "Semi Automatic", "", "", "", "AI" };
		string[] turnNames = { "Easy Turn (Pivot Turn)", "Classic Turn (only Brake-Turn)" };
		string[] patrolNames = { "Order", "Random" };

		void OnEnable ()
		{
			Trigger_TypeProp = serializedObject.FindProperty ("Trigger_Type");
			Trigger_TimeProp = serializedObject.FindProperty ("Trigger_Time");
			Trigger_Setting_TypeProp = serializedObject.FindProperty ("Trigger_Setting_Type");
			Trigger_NumProp = serializedObject.FindProperty ("Trigger_Num");
			Trigger_TanksProp = serializedObject.FindProperty ("Trigger_Tanks");
			Operator_TypeProp = serializedObject.FindProperty ("Operator_Type");
			All_Trigger_FlagProp = serializedObject.FindProperty ("All_Trigger_Flag");
			Necessary_NumProp = serializedObject.FindProperty ("Necessary_Num");
			Trigger_Collider_ScriptProp = serializedObject.FindProperty ("Trigger_Collider_Script");
			Useless_Event_NumProp = serializedObject.FindProperty ("Useless_Event_Num");
			Useless_EventsProp = serializedObject.FindProperty ("Useless_Events");
			Disabled_Event_NumProp = serializedObject.FindProperty ("Disabled_Event_Num");
			Disabled_EventsProp = serializedObject.FindProperty ("Disabled_Events");
			Event_TypeProp = serializedObject.FindProperty ("Event_Type");

			Event_TextProp = serializedObject.FindProperty ("Event_Text");
			Event_MessageProp = serializedObject.FindProperty ("Event_Message");
			Event_Message_ColorProp = serializedObject.FindProperty ("Event_Message_Color");
			Event_Message_TimeProp = serializedObject.FindProperty ("Event_Message_Time");

			Prefab_ObjectProp = serializedObject.FindProperty ("Prefab_Object");
			Key_NameProp = serializedObject.FindProperty ("Key_Name");
			Tank_IDProp = serializedObject.FindProperty ("Tank_ID");
			RelationshipProp = serializedObject.FindProperty ("Relationship");
			ReSpawn_TimesProp = serializedObject.FindProperty ("ReSpawn_Times");
			Attack_MultiplierProp = serializedObject.FindProperty ("Attack_Multiplier");
			Input_TypeProp = serializedObject.FindProperty ("Input_Type");
			Turn_TypeProp = serializedObject.FindProperty ("Turn_Type");
			Auto_LeadProp = serializedObject.FindProperty ("Auto_Lead");

			OverWrite_FlagProp = serializedObject.FindProperty ("OverWrite_Flag");
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

			Trigger_Itself_FlagProp = serializedObject.FindProperty ("Trigger_Itself_Flag");
			Target_NumProp = serializedObject.FindProperty ("Target_Num");
			Target_TanksProp = serializedObject.FindProperty ("Target_Tanks");
			New_WayPoint_PackProp = serializedObject.FindProperty ("New_WayPoint_Pack");
			New_Patrol_TypeProp = serializedObject.FindProperty ("New_Patrol_Type");
			New_Follow_TargetProp = serializedObject.FindProperty ("New_Follow_Target");
			New_No_AttackProp = serializedObject.FindProperty ("New_No_Attack");
			New_BreakthroughProp = serializedObject.FindProperty ("New_Breakthrough");
			New_Visibility_RadiusProp = serializedObject.FindProperty ("New_Visibility_Radius");
			New_Approach_DistanceProp = serializedObject.FindProperty ("New_Approach_Distance");
			New_OpenFire_DistanceProp = serializedObject.FindProperty ("New_OpenFire_Distance");
			New_Lost_CountProp = serializedObject.FindProperty ("New_Lost_Count");
			New_Face_EnemyProp = serializedObject.FindProperty ("New_Face_Enemy");
			New_Face_Offest_AngleProp = serializedObject.FindProperty ("New_Face_Offest_Angle");
			Renew_ReSpawn_Times_FlagProp = serializedObject.FindProperty ("Renew_ReSpawn_Times_Flag");
			New_ReSpawn_TimesProp = serializedObject.FindProperty ("New_ReSpawn_Times");

			Artillery_ScriptProp = serializedObject.FindProperty ("Artillery_Script");
			Artillery_TargetProp = serializedObject.FindProperty ("Artillery_Target");
			Artillery_NumProp = serializedObject.FindProperty ("Artillery_Num");
		}

		public override void OnInspectorGUI ()
		{
			if (EditorApplication.isPlaying == false) {
				GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
				serializedObject.Update ();

				// Trigger Settings.
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();	
				GUI.backgroundColor = new Color (0.5f, 0.5f, 1.0f, 1.0f);
				Trigger_TypeProp.intValue = EditorGUILayout.Popup ("Trigger Type", Trigger_TypeProp.intValue, triggerNames);
				GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
				EditorGUILayout.Space ();	
				switch (Trigger_TypeProp.intValue) {
				case 0: // Timer
					EditorGUILayout.Slider (Trigger_TimeProp, 0.0f, 6000.0f, "Trigger Time");
					break;
				case 1: // Destroy
					EditorGUILayout.IntSlider (Trigger_NumProp, 1, 64, "Number of Trigger Tanks");
					Trigger_TanksProp.arraySize = Trigger_NumProp.intValue;
					for (int i = 0; i < Trigger_TanksProp.arraySize; i++) {
						Trigger_TanksProp.GetArrayElementAtIndex (i).objectReferenceValue = EditorGUILayout.ObjectField ("Trigger Tank", Trigger_TanksProp.GetArrayElementAtIndex (i).objectReferenceValue, typeof(Transform), true);
					}
					Operator_TypeProp.intValue = EditorGUILayout.Popup ("Operator Type", Operator_TypeProp.intValue, operatorNames);
					if (Operator_TypeProp.intValue == 0) {
						All_Trigger_FlagProp.boolValue = EditorGUILayout.Toggle ("Require All the Triggers", All_Trigger_FlagProp.boolValue);
						if (All_Trigger_FlagProp.boolValue == false) {
							EditorGUILayout.IntSlider (Necessary_NumProp, 1, Trigger_NumProp.intValue, "Number of Necessary Triggers");
						}
					}
					break;
				case 2: // Trigger Collider
					Trigger_Collider_ScriptProp.objectReferenceValue = EditorGUILayout.ObjectField ("Trigger Collider", Trigger_Collider_ScriptProp.objectReferenceValue, typeof(Trigger_Collider_CS), true);
					EditorGUILayout.Space ();	
					Trigger_Setting_TypeProp.intValue = EditorGUILayout.Popup ("Trigger Setting Type", Trigger_Setting_TypeProp.intValue, triggerSettingNames);
					if (Trigger_Setting_TypeProp.intValue == 0) { // 'Set Manually'
						EditorGUILayout.IntSlider (Trigger_NumProp, 1, 64, "Number of Trigger Tanks");
						Trigger_TanksProp.arraySize = Trigger_NumProp.intValue;
						for (int i = 0; i < Trigger_TanksProp.arraySize; i++) {
							Trigger_TanksProp.GetArrayElementAtIndex (i).objectReferenceValue = EditorGUILayout.ObjectField ("Trigger Tank", Trigger_TanksProp.GetArrayElementAtIndex (i).objectReferenceValue, typeof(Transform), true);
						}
						Operator_TypeProp.intValue = EditorGUILayout.Popup ("Operator Type", Operator_TypeProp.intValue, operatorNames);
						if (Operator_TypeProp.intValue == 0) {
							All_Trigger_FlagProp.boolValue = EditorGUILayout.Toggle ("Require All the Triggers", All_Trigger_FlagProp.boolValue);
							if (All_Trigger_FlagProp.boolValue == false) {
								EditorGUILayout.IntSlider (Necessary_NumProp, 1, Trigger_NumProp.intValue, "Number of Necessary Triggers");
							}
						}
					} else { //'Any Hostile tank' or 'Any Friendly tank'.
						Operator_TypeProp.intValue = 1;
					}
					break;
				}

				// Event Settings.
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
				GUI.backgroundColor = new Color (0.5f, 0.5f, 1.0f, 1.0f);
				Event_TypeProp.intValue = EditorGUILayout.Popup ("Event Type", Event_TypeProp.intValue, eventNames);
				GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
				EditorGUILayout.Space ();	
				switch (Event_TypeProp.intValue) {
				case 0: // Spawn Tank
					EditorGUILayout.HelpBox ("Tank settings", MessageType.None, true);
					Prefab_ObjectProp.objectReferenceValue = EditorGUILayout.ObjectField ("Tank Prefab", Prefab_ObjectProp.objectReferenceValue, typeof(GameObject), false);
					Key_NameProp.stringValue = EditorGUILayout.TextField ("Key Name", Key_NameProp.stringValue);
					Tank_IDProp.intValue = EditorGUILayout.Popup ("Tank ID", Tank_IDProp.intValue, idNames);
					RelationshipProp.intValue = EditorGUILayout.Popup ("Relationship", RelationshipProp.intValue, relationshipNames);
					GUI.backgroundColor = new Color (1.0f, 0.5f, 0.5f, 1.0f);
					EditorGUILayout.IntSlider (ReSpawn_TimesProp, 0, 100, "ReSpawn Times");
					GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
					EditorGUILayout.Slider (Attack_MultiplierProp, 0.1f, 2.0f, "Attack Multiplier");
					EditorGUILayout.Space ();
					GameObject prefabObject = Prefab_ObjectProp.objectReferenceValue as GameObject;
					if (prefabObject && prefabObject.GetComponentInChildren <AI_CS> ()) {
						Input_TypeProp.intValue = 10;
					} else {
						if (Input_TypeProp.intValue == 10) {
							Input_TypeProp.intValue = 4;
						}
					}
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
					EditorGUILayout.Space ();	
				// for AI.
					if (Input_TypeProp.intValue == 10) {
						OverWrite_FlagProp.boolValue = EditorGUILayout.Toggle ("Overwrite AI settings", OverWrite_FlagProp.boolValue);
						if (OverWrite_FlagProp.boolValue) {
							EditorGUILayout.HelpBox ("AI Patrol Settings", MessageType.None, true);
							WayPoint_PackProp.objectReferenceValue = EditorGUILayout.ObjectField ("WayPoint Pack", WayPoint_PackProp.objectReferenceValue, typeof(GameObject), true);
							Patrol_TypeProp.intValue = EditorGUILayout.Popup ("Patrol Type", Patrol_TypeProp.intValue, patrolNames);
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
					}
					break;
				case 1: // Show Message
					EditorGUILayout.HelpBox ("Message Settings", MessageType.None, true);
					Event_TextProp.objectReferenceValue = EditorGUILayout.ObjectField ("Text", Event_TextProp.objectReferenceValue, typeof(Text), true);
					Event_MessageProp.stringValue = EditorGUILayout.TextArea (Event_MessageProp.stringValue, GUILayout.Height (60.0f));
					GUI.backgroundColor = new Color (1.0f, 1.0f, 1.0f, 1.0f);
					Event_Message_ColorProp.colorValue = EditorGUILayout.ColorField ("Color", Event_Message_ColorProp.colorValue);
					GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
					EditorGUILayout.Slider (Event_Message_TimeProp, 0.1f, 60.0f, "Displaying Time");
					break;
				case 2: // Change AI Settings
					if (Trigger_TypeProp.intValue == 2 && Operator_TypeProp.intValue == 1) { // Trigger Collider && OR
						Trigger_Itself_FlagProp.boolValue = EditorGUILayout.Toggle ("Target is trigger itself.", Trigger_Itself_FlagProp.boolValue);
					} else {
						Trigger_Itself_FlagProp.boolValue = false;
					}
					if (Trigger_Itself_FlagProp.boolValue) {
						Target_TanksProp.ClearArray ();
						Target_TanksProp.arraySize = 1;
						Target_NumProp.intValue = 1;
					} else {
						EditorGUILayout.IntSlider (Target_NumProp, 1, 64, "Number of Target Tanks");
						Target_TanksProp.arraySize = Target_NumProp.intValue;
						for (int i = 0; i < Target_TanksProp.arraySize; i++) {
							Target_TanksProp.GetArrayElementAtIndex (i).objectReferenceValue = EditorGUILayout.ObjectField ("Target Tank", Target_TanksProp.GetArrayElementAtIndex (i).objectReferenceValue, typeof(Transform), true);
						}
					}
					EditorGUILayout.Space ();
					EditorGUILayout.HelpBox ("New AI Patrol Settings", MessageType.None, true);
					New_WayPoint_PackProp.objectReferenceValue = EditorGUILayout.ObjectField ("WayPoint Pack", New_WayPoint_PackProp.objectReferenceValue, typeof(GameObject), true);
					New_Patrol_TypeProp.intValue = EditorGUILayout.Popup ("Patrol Type", New_Patrol_TypeProp.intValue, patrolNames);
					New_Follow_TargetProp.objectReferenceValue = EditorGUILayout.ObjectField ("Follow Target", New_Follow_TargetProp.objectReferenceValue, typeof(Transform), true);
					EditorGUILayout.Space ();
					EditorGUILayout.HelpBox ("New AI Combat Settings", MessageType.None, true);
					New_No_AttackProp.boolValue = EditorGUILayout.Toggle ("No Attack", New_No_AttackProp.boolValue);
					New_BreakthroughProp.boolValue = EditorGUILayout.Toggle ("Breakthrough", New_BreakthroughProp.boolValue);
					EditorGUILayout.Slider (New_Visibility_RadiusProp, 0.1f, 10000.0f, "Visibility Radius");
					EditorGUILayout.Slider (New_Approach_DistanceProp, 1.0f, 10000.0f, "Approach Distance");
					if (New_Approach_DistanceProp.floatValue == 10000.0f) {
						New_Approach_DistanceProp.floatValue = Mathf.Infinity;
					}
					EditorGUILayout.Slider (New_OpenFire_DistanceProp, 1.0f, 10000.0f, "Open Fire Distance");
					if (New_OpenFire_DistanceProp.floatValue == 10000.0f) {
						New_OpenFire_DistanceProp.floatValue = Mathf.Infinity;
					}
					EditorGUILayout.Slider (New_Lost_CountProp, 0.0f, 100.0f, "Lost Count");
					New_Face_EnemyProp.boolValue = EditorGUILayout.Toggle ("Face the Enemy", New_Face_EnemyProp.boolValue);
					if (New_Face_EnemyProp.boolValue) {
						EditorGUILayout.Slider (New_Face_Offest_AngleProp, 0.0f, 90.0f, "Face Offest Angle");
					}
					Renew_ReSpawn_Times_FlagProp.boolValue = EditorGUILayout.Toggle ("Renew ReSpawn Times", Renew_ReSpawn_Times_FlagProp.boolValue);
					if (Renew_ReSpawn_Times_FlagProp.boolValue) {
						EditorGUILayout.IntSlider (New_ReSpawn_TimesProp, 0, 100, "ReSpawn Times");
					}
					EditorGUILayout.Space ();
					break;
				case 3: // Remove Tank
					if (Trigger_TypeProp.intValue == 2 && Operator_TypeProp.intValue == 1) { // Trigger Collider && OR
						Trigger_Itself_FlagProp.boolValue = EditorGUILayout.Toggle ("Target is trigger itself.", Trigger_Itself_FlagProp.boolValue);
					} else {
						Trigger_Itself_FlagProp.boolValue = false;
					}
					if (Trigger_Itself_FlagProp.boolValue) {
						Target_TanksProp.ClearArray ();
						Target_TanksProp.arraySize = 1;
						Target_NumProp.intValue = 1;
					} else {
						EditorGUILayout.IntSlider (Target_NumProp, 1, 64, "Number of Target Tanks");
						Target_TanksProp.arraySize = Target_NumProp.intValue;
						for (int i = 0; i < Target_TanksProp.arraySize; i++) {
							Target_TanksProp.GetArrayElementAtIndex (i).objectReferenceValue = EditorGUILayout.ObjectField ("Target Tank", Target_TanksProp.GetArrayElementAtIndex (i).objectReferenceValue, typeof(Transform), true);
						}
					}
					EditorGUILayout.Space ();
					break;
				case 4: // Artillery Fire
					Artillery_ScriptProp.objectReferenceValue = EditorGUILayout.ObjectField ("Artillery Object", Artillery_ScriptProp.objectReferenceValue, typeof(Artillery_Fire_CS), true);
					Artillery_TargetProp.objectReferenceValue = EditorGUILayout.ObjectField ("Target", Artillery_TargetProp.objectReferenceValue, typeof(Transform), true);
					EditorGUILayout.IntSlider (Artillery_NumProp, 1, 128, "Number of Shells");

					EditorGUILayout.Space ();
					break;
				}

				// Other events settings.
				EditorGUILayout.Space ();
				EditorGUILayout.HelpBox ("Other Events Settings", MessageType.None, true);
				EditorGUILayout.IntSlider (Useless_Event_NumProp, 0, 64, "Number of Events to be Removed");
				Useless_EventsProp.arraySize = Useless_Event_NumProp.intValue;
				for (int i = 0; i < Useless_EventsProp.arraySize; i++) {
					Useless_EventsProp.GetArrayElementAtIndex (i).objectReferenceValue = EditorGUILayout.ObjectField ("Event to be Removed", Useless_EventsProp.GetArrayElementAtIndex (i).objectReferenceValue, typeof(Event_Controller_CS), true);
				}
				EditorGUILayout.Space ();
				EditorGUILayout.IntSlider (Disabled_Event_NumProp, 0, 64, "Number of Events to be Enabled");
				Disabled_EventsProp.arraySize = Disabled_Event_NumProp.intValue;
				for (int i = 0; i < Disabled_EventsProp.arraySize; i++) {
					Disabled_EventsProp.GetArrayElementAtIndex (i).objectReferenceValue = EditorGUILayout.ObjectField ("Event to be Enabled", Disabled_EventsProp.GetArrayElementAtIndex (i).objectReferenceValue, typeof(Event_Controller_CS), true);
				}

				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
			
				serializedObject.ApplyModifiedProperties ();
			}
		}
			
	}

}