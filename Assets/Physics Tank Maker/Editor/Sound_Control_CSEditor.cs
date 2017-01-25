using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Sound_Control_CS))]
	public class Sound_Control_CSEditor : Editor
	{
	
		SerializedProperty TypeProp;

		SerializedProperty Track_TypeProp;
		SerializedProperty Left_Wheel_RigidbodyProp;
		SerializedProperty Right_Wheel_RigidbodyProp;

		SerializedProperty Min_Engine_PitchProp;
		SerializedProperty Max_Engine_PitchProp;
		SerializedProperty Min_Engine_VolumeProp;
		SerializedProperty Max_Engine_VolumeProp;
		SerializedProperty Max_VelocityProp;
		SerializedProperty Left_VelocityProp;
		SerializedProperty Right_VelocityProp;

		SerializedProperty Min_ImpactProp;
		SerializedProperty Max_ImpactProp;
		SerializedProperty Min_Impact_PitchProp;
		SerializedProperty Max_Impact_PitchProp;
		SerializedProperty Min_Impact_VolumeProp;
		SerializedProperty Max_Impact_VolumeProp;
	
		SerializedProperty Max_Motor_VolumeProp;

		string[] typeNames = { "Engine Sound", "Impact Sound", "Turret Sound", "Cannon Sound" };
		string[] trackTypes = { "'Physics Track'", "'Static_Track' or 'Scroll_Track'", "No Track (Wheeled Vehicle)" };

		void  OnEnable ()
		{
			TypeProp = serializedObject.FindProperty ("Type");

			Track_TypeProp = serializedObject.FindProperty ("Track_Type");
			Left_Wheel_RigidbodyProp = serializedObject.FindProperty ("Left_Wheel_Rigidbody");
			Right_Wheel_RigidbodyProp = serializedObject.FindProperty ("Right_Wheel_Rigidbody");

			Min_Engine_PitchProp = serializedObject.FindProperty ("Min_Engine_Pitch");
			Max_Engine_PitchProp = serializedObject.FindProperty ("Max_Engine_Pitch");
			Min_Engine_VolumeProp = serializedObject.FindProperty ("Min_Engine_Volume");
			Max_Engine_VolumeProp = serializedObject.FindProperty ("Max_Engine_Volume");
			Max_VelocityProp = serializedObject.FindProperty ("Max_Velocity");
			Left_VelocityProp = serializedObject.FindProperty ("Left_Velocity");
			Right_VelocityProp = serializedObject.FindProperty ("Right_Velocity");

			Min_ImpactProp = serializedObject.FindProperty ("Min_Impact");
			Max_ImpactProp = serializedObject.FindProperty ("Max_Impact");
			Min_Impact_PitchProp = serializedObject.FindProperty ("Min_Impact_Pitch");
			Max_Impact_PitchProp = serializedObject.FindProperty ("Max_Impact_Pitch");
			Min_Impact_VolumeProp = serializedObject.FindProperty ("Min_Impact_Volume");
			Max_Impact_VolumeProp = serializedObject.FindProperty ("Max_Impact_Volume");

			Max_Motor_VolumeProp = serializedObject.FindProperty ("Max_Motor_Volume");
		}

		public override void  OnInspectorGUI ()
		{
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			serializedObject.Update ();
		
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Select the type of sound.", MessageType.None, true);
			TypeProp.intValue = EditorGUILayout.Popup ("Type", TypeProp.intValue, typeNames);
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
		
			switch (TypeProp.intValue) {
			case 0:
				EditorGUILayout.HelpBox ("This script must be attached to the object under the 'MainBody'.", MessageType.None, true);
				Track_TypeProp.intValue = EditorGUILayout.Popup ("Track Type", Track_TypeProp.intValue, trackTypes);
				if (Track_TypeProp.intValue == 0 || Track_TypeProp.intValue == 2) { // Physics_Track or No Tracks (Wheeled Vehicle)
					Left_Wheel_RigidbodyProp.objectReferenceValue = EditorGUILayout.ObjectField ("Left Reference Wheel", Left_Wheel_RigidbodyProp.objectReferenceValue, typeof(Rigidbody), true);
					Right_Wheel_RigidbodyProp.objectReferenceValue = EditorGUILayout.ObjectField ("Right Reference Wheel", Right_Wheel_RigidbodyProp.objectReferenceValue, typeof(Rigidbody), true);
				}
				if (Track_TypeProp.intValue == 0) { // Physics_Track
					EditorGUILayout.Slider (Max_VelocityProp, 1.0f, 100.0f, "Max Speed");
				}
				EditorGUILayout.Space ();
				EditorGUILayout.Slider (Min_Engine_PitchProp, 0.1f, 10.0f, "Idling Pitch");
				EditorGUILayout.Slider (Max_Engine_PitchProp, 0.1f, 10.0f, "Max Pitch");
				EditorGUILayout.Slider (Min_Engine_VolumeProp, 0.0f, 1.0f, "Idling Volume");
				EditorGUILayout.Slider (Max_Engine_VolumeProp, 0.0f, 1.0f, "Max Volume");
				float currentVelocity = (Left_VelocityProp.floatValue + Right_VelocityProp.floatValue) / 2.0f;
				EditorGUILayout.HelpBox ("Current Velocity " + currentVelocity, MessageType.None, true);
				break;
			case 1:
				EditorGUILayout.HelpBox ("This script must be attached to 'MainBody'", MessageType.None, true);
				EditorGUILayout.Slider (Min_ImpactProp, 0.1f, 5.0f, "Min Impact");
				EditorGUILayout.Slider (Max_ImpactProp, 0.1f, 5.0f, "Max Impact");
				EditorGUILayout.Slider (Min_Impact_PitchProp, 0.1f, 10.0f, "Min Pitch");
				EditorGUILayout.Slider (Max_Impact_PitchProp, 0.1f, 10.0f, "Max Pitch");
				EditorGUILayout.Slider (Min_Impact_VolumeProp, 0.0f, 1.0f, "Min Volume");
				EditorGUILayout.Slider (Max_Impact_VolumeProp, 0.0f, 1.0f, "Max Volume");
				break;
			case 2:
				EditorGUILayout.HelpBox ("This script must be attached to 'Turret_Base'", MessageType.None, true);
				EditorGUILayout.Slider (Max_Motor_VolumeProp, 0.0f, 1.0f, "Max Volume");
				break;
			case 3:
				EditorGUILayout.HelpBox ("This script must be attached to 'Cannon_Base'", MessageType.None, true);
				EditorGUILayout.Slider (Max_Motor_VolumeProp, 0.0f, 1.0f, "Max Volume");
				break;
			}
		
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
		
			serializedObject.ApplyModifiedProperties ();
		}

	}

}