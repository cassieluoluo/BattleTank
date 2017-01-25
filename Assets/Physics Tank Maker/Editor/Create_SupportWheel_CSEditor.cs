using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Create_SupportWheel_CS))]
	public class Create_SupportWheel_CSEditor : Editor
	{
	
		SerializedProperty Wheel_DistanceProp;
		SerializedProperty NumProp;
		SerializedProperty SpacingProp;
		SerializedProperty Wheel_MassProp;
		SerializedProperty Wheel_RadiusProp;
		SerializedProperty Collider_MaterialProp;
		SerializedProperty Wheel_MeshProp;
		SerializedProperty Wheel_MaterialProp;
		SerializedProperty Collider_MeshProp;
		SerializedProperty Collider_Mesh_SubProp;
		SerializedProperty Drive_WheelProp;
		SerializedProperty Wheel_ResizeProp;
		SerializedProperty ScaleDown_SizeProp;
		SerializedProperty Return_SpeedProp;
		SerializedProperty Wheel_DurabilityProp;
		SerializedProperty Static_FlagProp;
		SerializedProperty Radius_OffsetProp;

		SerializedProperty RealTime_FlagProp;
	
		Transform parentTransform;

		void OnEnable ()
		{
			Wheel_DistanceProp = serializedObject.FindProperty ("Wheel_Distance");
			NumProp = serializedObject.FindProperty ("Num");
			SpacingProp = serializedObject.FindProperty ("Spacing");
			Wheel_MassProp = serializedObject.FindProperty ("Wheel_Mass");
			Wheel_RadiusProp = serializedObject.FindProperty ("Wheel_Radius");
			Collider_MaterialProp = serializedObject.FindProperty ("Collider_Material");
			Wheel_MeshProp = serializedObject.FindProperty ("Wheel_Mesh");
			Wheel_MaterialProp = serializedObject.FindProperty ("Wheel_Material");
			Collider_MeshProp = serializedObject.FindProperty ("Collider_Mesh");
			Collider_Mesh_SubProp = serializedObject.FindProperty ("Collider_Mesh_Sub");
			Drive_WheelProp = serializedObject.FindProperty ("Drive_Wheel");
			Wheel_ResizeProp = serializedObject.FindProperty ("Wheel_Resize");
			ScaleDown_SizeProp = serializedObject.FindProperty ("ScaleDown_Size");
			Return_SpeedProp = serializedObject.FindProperty ("Return_Speed");
			Wheel_DurabilityProp = serializedObject.FindProperty ("Wheel_Durability");
			Static_FlagProp = serializedObject.FindProperty ("Static_Flag");
			Radius_OffsetProp = serializedObject.FindProperty ("Radius_Offset");

			RealTime_FlagProp = serializedObject.FindProperty ("RealTime_Flag");
		
			if (Selection.activeGameObject) {
				parentTransform = Selection.activeGameObject.transform;
			}
		}

		public override void OnInspectorGUI ()
		{
			bool isPrepared;
			if (Application.isPlaying || parentTransform.parent == null || parentTransform.parent.gameObject.GetComponent<Rigidbody> () == null) {
				isPrepared = false;
			} else {
				isPrepared = true;
			}

			if (isPrepared) {
				// Keep rotation.
				Vector3 ang = parentTransform.localEulerAngles;
				if (ang.z != 90.0f) {
					ang.z = 90.0f;
					parentTransform.localEulerAngles = ang;
				}
				// Set Inspector window.
				Set_Inspector ();
				// Update (Recreate) the parts.
				if (GUI.changed && RealTime_FlagProp.boolValue) {
					Create ();
				}
				if (Event.current.commandName == "UndoRedoPerformed") {
					Create ();
				}
			}
		}

		void Set_Inspector ()
		{
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			serializedObject.Update ();

			// for Static Wheel
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Wheel Type", MessageType.None, true);
			Static_FlagProp.boolValue = EditorGUILayout.Toggle ("Static Wheel", Static_FlagProp.boolValue);
			if (Static_FlagProp.boolValue) {
				EditorGUILayout.Slider (Radius_OffsetProp, -0.5f, 0.5f, "Radius Offset");
				EditorGUILayout.Space ();
				if (GUILayout.Button ("Update Values", GUILayout.Width (200))) {
					Create ();
				}
			}
			// Wheels settings
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Wheels settings", MessageType.None, true);
			EditorGUILayout.Slider (Wheel_DistanceProp, 0.1f, 10.0f, "Distance");
			EditorGUILayout.IntSlider (NumProp, 0, 30, "Number");
			EditorGUILayout.Slider (SpacingProp, 0.1f, 10.0f, "Spacing");
			if (!Static_FlagProp.boolValue) { // Physics Wheel.
				EditorGUILayout.Slider (Wheel_MassProp, 0.1f, 300.0f, "Mass");
			}
			EditorGUILayout.Space ();
			GUI.backgroundColor = new Color (1.0f, 0.5f, 0.5f, 1.0f);
			EditorGUILayout.Slider (Wheel_RadiusProp, 0.01f, 1.0f, "SphereCollider Radius");
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			Collider_MaterialProp.objectReferenceValue = EditorGUILayout.ObjectField ("Physic Material", Collider_MaterialProp.objectReferenceValue, typeof(PhysicMaterial), false);
			EditorGUILayout.Space ();
			Wheel_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh", Wheel_MeshProp.objectReferenceValue, typeof(Mesh), false);
			Wheel_MaterialProp.objectReferenceValue = EditorGUILayout.ObjectField ("Material", Wheel_MaterialProp.objectReferenceValue, typeof(Material), false);

			if (!Static_FlagProp.boolValue) {
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
				// Mesh Collider
				EditorGUILayout.HelpBox ("MeshCollider settings", MessageType.None, true);
				Collider_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh Collider", Collider_MeshProp.objectReferenceValue, typeof(Mesh), false);
				Collider_Mesh_SubProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh Sub Collider", Collider_Mesh_SubProp.objectReferenceValue, typeof(Mesh), false);
				// Scripts settings
				EditorGUILayout.Space ();
				EditorGUILayout.Space ();
				EditorGUILayout.HelpBox ("Scripts settings", MessageType.None, true);
				// Drive Wheel
				Drive_WheelProp.boolValue = EditorGUILayout.Toggle ("Drive Wheel", Drive_WheelProp.boolValue);
				EditorGUILayout.Space ();
				// Wheel Resize
				Wheel_ResizeProp.boolValue = EditorGUILayout.Toggle ("Wheel Resize Script", Wheel_ResizeProp.boolValue);
				if (Wheel_ResizeProp.boolValue) {
					EditorGUILayout.Slider (ScaleDown_SizeProp, 0.1f, 3.0f, "Scale Size");
					EditorGUILayout.Slider (Return_SpeedProp, 0.01f, 0.1f, "Return Speed");
				}
				EditorGUILayout.Space ();
				// Durability
				EditorGUILayout.Slider (Wheel_DurabilityProp, 1, 1000000, "Wheel Durability");
				if (Wheel_DurabilityProp.floatValue >= 1000000) {
					Wheel_DurabilityProp.floatValue = Mathf.Infinity;
				}
			}

			// Update Value
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			RealTime_FlagProp.boolValue = EditorGUILayout.Toggle ("Real Time Update", RealTime_FlagProp.boolValue);
			if (GUILayout.Button ("Update Values")) {
				if (RealTime_FlagProp.boolValue == false) {
					Create ();
				}
			}
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			//
			serializedObject.ApplyModifiedProperties ();
		}

	
		void Create ()
		{
			// Delete Objects
			int childCount = parentTransform.childCount;
			for (int i = 0; i < childCount; i++) {
				DestroyImmediate (parentTransform.GetChild (0).gameObject);
			}
			// Create Wheel	
			Vector3 pos;
			for (int i = 0; i < NumProp.intValue; i++) {
				pos.x = 0.0f;
				pos.y = Wheel_DistanceProp.floatValue / 2.0f;
				pos.z = -SpacingProp.floatValue * i;
				Create_Wheel ("L", pos, i + 1);
			}
			for (int i = 0; i < NumProp.intValue; i++) {
				pos.x = 0.0f;
				pos.y = -Wheel_DistanceProp.floatValue / 2.0f;
				pos.z = -SpacingProp.floatValue * i;
				Create_Wheel ("R", pos, i + 1);
			}
		}

		void Create_Wheel (string direction, Vector3 position, int number)
		{
			//Create_Gameobject
			GameObject gameObject = new GameObject ("SupportWheel_" + direction + "_" + number);
			gameObject.transform.parent = parentTransform;
			gameObject.transform.localPosition = position;
			if (direction == "L") {
				gameObject.transform.localRotation = Quaternion.Euler (Vector3.zero);
			} else {
				gameObject.transform.localRotation = Quaternion.Euler (0.0f, 0.0f, 180.0f);
			}
			gameObject.layer = 9; // Wheel
			// Mesh
			MeshFilter meshFilter = gameObject.AddComponent < MeshFilter > ();
			meshFilter.mesh = Wheel_MeshProp.objectReferenceValue as Mesh;
			MeshRenderer meshRenderer = gameObject.AddComponent < MeshRenderer > ();
			meshRenderer.material = Wheel_MaterialProp.objectReferenceValue as Material;
			// SphereCollider
			SphereCollider sphereCollider = gameObject.AddComponent < SphereCollider > ();
			sphereCollider.radius = Wheel_RadiusProp.floatValue;
			sphereCollider.material = Collider_MaterialProp.objectReferenceValue as PhysicMaterial;
			// for Physics Wheel.
			if (Static_FlagProp.boolValue == false) {
				// MeshCollider
				if (Collider_MeshProp.objectReferenceValue) {
					MeshCollider meshCollider = gameObject.AddComponent < MeshCollider > ();
					meshCollider.material = Collider_MaterialProp.objectReferenceValue as PhysicMaterial;
					meshCollider.sharedMesh = Collider_MeshProp.objectReferenceValue as Mesh;
					meshCollider.convex = true;
					meshCollider.enabled = false;
				}
				// Sub MeshCollider
				if (Collider_Mesh_SubProp.objectReferenceValue) {
					MeshCollider meshCollider;
					meshCollider = gameObject.AddComponent < MeshCollider > ();
					meshCollider.material = Collider_MaterialProp.objectReferenceValue as PhysicMaterial;
					meshCollider.sharedMesh = Collider_Mesh_SubProp.objectReferenceValue as Mesh;
					meshCollider.convex = true;
					meshCollider.enabled = false;
				}
				// Rigidbody
				Rigidbody rigidbody = gameObject.AddComponent < Rigidbody > ();
				rigidbody.mass = Wheel_MassProp.floatValue;
				// HingeJoint
				HingeJoint hingeJoint = gameObject.AddComponent < HingeJoint > ();
				hingeJoint.anchor = Vector3.zero;
				hingeJoint.axis = new Vector3 (0.0f, 1.0f, 0.0f);
				hingeJoint.connectedBody = parentTransform.parent.gameObject.GetComponent < Rigidbody > ();
				// Drive_Wheel_CS
				Drive_Wheel_CS driveScript = gameObject.AddComponent < Drive_Wheel_CS > ();
				driveScript.Radius = Wheel_RadiusProp.floatValue;
				driveScript.Drive_Flag = Drive_WheelProp.boolValue;
				driveScript.Use_BrakeTurn = true;
				// Wheel_Resize_CS
				if (Wheel_ResizeProp.boolValue) {
					Wheel_Resize_CS resizeScript = gameObject.AddComponent < Wheel_Resize_CS > ();
					resizeScript.ScaleDown_Size = ScaleDown_SizeProp.floatValue;
					resizeScript.Return_Speed = Return_SpeedProp.floatValue;
				}
				// Damage_Control_CS
				Damage_Control_CS damageScript = gameObject.AddComponent < Damage_Control_CS > ();
				damageScript.Type = 8; // 8=Wheel
				damageScript.Durability = Wheel_DurabilityProp.floatValue;
				if (direction == "L") {
					damageScript.Direction = 0;
				} else {
					damageScript.Direction = 1;
				}
				// Stabilizer_CS
				gameObject.AddComponent < Stabilizer_CS > ();
			} else { // for Static Wheel
				Static_Wheel_CS staticScript = gameObject.AddComponent < Static_Wheel_CS > ();
				staticScript.Radius_Offset = Radius_OffsetProp.floatValue;
			}

		}

	}

}