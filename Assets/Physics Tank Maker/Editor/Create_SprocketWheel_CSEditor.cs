using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Create_SprocketWheel_CS))]
	public class Create_SprocketWheel_CSEditor : Editor
	{
		SerializedProperty Static_FlagProp;
		SerializedProperty Radius_OffsetProp;

		SerializedProperty Arm_FlagProp;
		SerializedProperty Arm_DistanceProp;
		SerializedProperty Arm_LengthProp;
		SerializedProperty Arm_AngleProp;
		SerializedProperty Arm_L_MeshProp;
		SerializedProperty Arm_R_MeshProp;
		SerializedProperty Arm_L_MaterialProp;
		SerializedProperty Arm_R_MaterialProp;
	
		SerializedProperty Wheel_DistanceProp;
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

		Transform parentTransform;

		void OnEnable ()
		{
			Static_FlagProp = serializedObject.FindProperty ("Static_Flag");
			Radius_OffsetProp = serializedObject.FindProperty ("Radius_Offset");

			Arm_FlagProp = serializedObject.FindProperty ("Arm_Flag");
			Arm_DistanceProp = serializedObject.FindProperty ("Arm_Distance");
			Arm_LengthProp = serializedObject.FindProperty ("Arm_Length");
			Arm_AngleProp = serializedObject.FindProperty ("Arm_Angle");
			Arm_L_MeshProp = serializedObject.FindProperty ("Arm_L_Mesh");
			Arm_R_MeshProp = serializedObject.FindProperty ("Arm_R_Mesh");
			Arm_L_MaterialProp = serializedObject.FindProperty ("Arm_L_Material");
			Arm_R_MaterialProp = serializedObject.FindProperty ("Arm_R_Material");
		
			Wheel_DistanceProp = serializedObject.FindProperty ("Wheel_Distance");
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
				if (GUI.changed) {
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

			// Tensioner Arms settings
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Tensioner Arms settings", MessageType.None, true);
			Arm_FlagProp.boolValue = EditorGUILayout.Toggle ("Use Tensioner Arm", Arm_FlagProp.boolValue);
			if (Arm_FlagProp.boolValue) {
				EditorGUILayout.Slider (Arm_DistanceProp, 0.1f, 10.0f, "Distance");
				EditorGUILayout.Slider (Arm_LengthProp, -1.0f, 1.0f, "Length");
				EditorGUILayout.Slider (Arm_AngleProp, -180.0f, 180.0f, "Angle");
				Arm_L_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh of Left", Arm_L_MeshProp.objectReferenceValue, typeof(Mesh), false);
				Arm_R_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh of Right", Arm_R_MeshProp.objectReferenceValue, typeof(Mesh), false);
				Arm_L_MaterialProp.objectReferenceValue = EditorGUILayout.ObjectField ("Material of Left", Arm_L_MaterialProp.objectReferenceValue, typeof(Material), false);
				Arm_R_MaterialProp.objectReferenceValue = EditorGUILayout.ObjectField ("Material of Right", Arm_R_MaterialProp.objectReferenceValue, typeof(Material), false);
			}

			// Wheels settings
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Wheels settings", MessageType.None, true);
			EditorGUILayout.Slider (Wheel_DistanceProp, 0.1f, 10.0f, "Distance");
			EditorGUILayout.Slider (Wheel_MassProp, 0.1f, 300.0f, "Mass");
			EditorGUILayout.Space ();
			GUI.backgroundColor = new Color (1.0f, 0.5f, 0.5f, 1.0f);
			EditorGUILayout.Slider (Wheel_RadiusProp, 0.01f, 1.0f, "SphereCollider Radius");
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			Collider_MaterialProp.objectReferenceValue = EditorGUILayout.ObjectField ("Physic Material", Collider_MaterialProp.objectReferenceValue, typeof(PhysicMaterial), false);
			EditorGUILayout.Space ();
			Wheel_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh", Wheel_MeshProp.objectReferenceValue, typeof(Mesh), false);
			Wheel_MaterialProp.objectReferenceValue = EditorGUILayout.ObjectField ("Material", Wheel_MaterialProp.objectReferenceValue, typeof(Material), false);

			if (Static_FlagProp.boolValue == false) { // Physics Wheel
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
			if (GUILayout.Button ("Update Values")) {
				Create ();
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
			// Create Arm and Wheel
			Vector3 pos;
			if (Arm_FlagProp.boolValue) { //With Arm
				// Create Arms.
				Create_Arm ("L", new Vector3 (0.0f, Arm_DistanceProp.floatValue / 2.0f, 0.0f));
				Create_Arm ("R", new Vector3 (0.0f, -Arm_DistanceProp.floatValue / 2.0f, 0.0f));
				// Set Wheel Pos.
				pos.x = Mathf.Sin (Mathf.Deg2Rad * (180.0f + Arm_AngleProp.floatValue)) * Arm_LengthProp.floatValue;
				pos.y = Wheel_DistanceProp.floatValue / 2.0f;
				pos.z = Mathf.Cos (Mathf.Deg2Rad * (180.0f + Arm_AngleProp.floatValue)) * Arm_LengthProp.floatValue;
			} else { // No Arm
				// Set Wheel Pos.
				pos.x = 0.0f;
				pos.y = Wheel_DistanceProp.floatValue / 2.0f;
				pos.z = 0.0f;
			}
			// Create Wheels.
			if (Static_FlagProp.boolValue == false) { //Physics Wheel
				Create_Physics_Wheel ("L", new Vector3 (pos.x, pos.y, pos.z));
				Create_Physics_Wheel ("R", new Vector3 (pos.x, -pos.y, pos.z));
			} else { //Static Wheel
				Create_Static_Wheel ("L", new Vector3 (pos.x, pos.y, pos.z));
				Create_Static_Wheel ("R", new Vector3 (pos.x, -pos.y, pos.z));
				Create_Invisible_Wheel ("L", new Vector3 (pos.x, pos.y, pos.z));
				Create_Invisible_Wheel ("R", new Vector3 (pos.x, -pos.y, pos.z));
			}
		}

		void Create_Arm (string direction, Vector3 position)
		{
			//Create gameobject & Set transform
			GameObject gameObject = new GameObject ("TensionerArm_" + direction);
			gameObject.transform.parent = parentTransform;
			gameObject.transform.localPosition = position;
			gameObject.transform.localRotation = Quaternion.Euler (0.0f, Arm_AngleProp.floatValue, -90.0f);
			// Add Mesh
			if (direction == "L") { // Left
				if (Arm_L_MeshProp.objectReferenceValue) {
					MeshFilter meshFilter = gameObject.AddComponent < MeshFilter > ();
					meshFilter.mesh = Arm_L_MeshProp.objectReferenceValue as Mesh;
					MeshRenderer meshRenderer = gameObject.AddComponent < MeshRenderer > ();
					meshRenderer.material = Arm_L_MaterialProp.objectReferenceValue as Material;
				}
			} else { //Right
				if (Arm_R_MeshProp.objectReferenceValue) {
					MeshFilter meshFilter = gameObject.AddComponent < MeshFilter > ();
					meshFilter.mesh = Arm_R_MeshProp.objectReferenceValue as Mesh;
					MeshRenderer meshRenderer = gameObject.AddComponent < MeshRenderer > ();
					meshRenderer.material = Arm_R_MaterialProp.objectReferenceValue as Material;
				}
			}
		}

		void Create_Physics_Wheel (string direction, Vector3 position)
		{
			GameObject gameObject = Create_GameObject ("SprocketWheel", direction, position);
			Add_Mesh (gameObject);
			Add_SphereCollider (gameObject);
			Add_MeshColliders (gameObject);
			Add_DrivingComponents (gameObject);
			// Damage_Control_CS
			Damage_Control_CS damageScript;
			damageScript = gameObject.AddComponent < Damage_Control_CS > ();
			damageScript.Type = 8; // 8 = Wheel
			damageScript.Durability = Wheel_DurabilityProp.floatValue;
			if (direction == "L") {
				damageScript.Direction = 0;
			} else {
				damageScript.Direction = 1;
			}
			// Wheel_Resize_CS
			if (Wheel_ResizeProp.boolValue) {
				Wheel_Resize_CS resizeScript;
				resizeScript = gameObject.AddComponent < Wheel_Resize_CS > ();
				resizeScript.ScaleDown_Size = ScaleDown_SizeProp.floatValue;
				resizeScript.Return_Speed = Return_SpeedProp.floatValue;
			}
			// Stabilizer_CS
			gameObject.AddComponent < Stabilizer_CS > ();
		}

		void Create_Static_Wheel (string direction, Vector3 position)
		{
			GameObject gameObject = Create_GameObject ("SprocketWheel", direction, position);
			Add_Mesh (gameObject);
			// Static_Wheel_CS script
			Static_Wheel_CS staticScript = gameObject.AddComponent < Static_Wheel_CS > ();
			staticScript.Radius_Offset = Radius_OffsetProp.floatValue;
		}

		void Create_Invisible_Wheel (string direction, Vector3 position)
		{
			GameObject gameObject = Create_GameObject ("Invisible_SprocketWheel", direction, position);
			MeshFilter meshFilter = gameObject.AddComponent < MeshFilter > (); // Set only MeshFilter in order to get the mesh size.
			meshFilter.mesh = Wheel_MeshProp.objectReferenceValue as Mesh;
			Add_SphereCollider (gameObject);
			Add_DrivingComponents (gameObject);
		}

	
		GameObject Create_GameObject (string name, string direction, Vector3 position)
		{
			GameObject gameObject = new GameObject (name + "_" + direction);
			gameObject.transform.parent = parentTransform;
			gameObject.transform.localPosition = position;
			if (direction == "L") {
				gameObject.transform.localRotation = Quaternion.Euler (Vector3.zero);
			} else {
				gameObject.transform.localRotation = Quaternion.Euler (0.0f, 0.0f, 180.0f);
			}
			gameObject.layer = 9; // Wheel
			return gameObject;
		}

		void Add_Mesh (GameObject gameObject)
		{
			MeshFilter meshFilter = gameObject.AddComponent < MeshFilter > ();
			meshFilter.mesh = Wheel_MeshProp.objectReferenceValue as Mesh;
			MeshRenderer meshRenderer = gameObject.AddComponent < MeshRenderer > ();
			meshRenderer.material = Wheel_MaterialProp.objectReferenceValue as Material;
		}

		void Add_SphereCollider (GameObject gameObject)
		{
			SphereCollider sphereCollider = gameObject.AddComponent < SphereCollider > ();
			sphereCollider.radius = Wheel_RadiusProp.floatValue;
			sphereCollider.material = Collider_MaterialProp.objectReferenceValue as PhysicMaterial;
		
		}

		void Add_MeshColliders (GameObject gameObject)
		{
			// Main MeshCollider
			if (Collider_MeshProp.objectReferenceValue) {
				MeshCollider meshCollider = gameObject.AddComponent < MeshCollider > ();
				meshCollider.material = Collider_MaterialProp.objectReferenceValue as PhysicMaterial;
				meshCollider.sharedMesh = Collider_MeshProp.objectReferenceValue as Mesh;
				meshCollider.convex = true;
				meshCollider.enabled = false;
			}
			// Sub MeshCollider
			if (Collider_Mesh_SubProp.objectReferenceValue != null) {
				MeshCollider meshCollider = gameObject.AddComponent < MeshCollider > ();
				meshCollider.material = Collider_MaterialProp.objectReferenceValue as PhysicMaterial;
				meshCollider.sharedMesh = Collider_Mesh_SubProp.objectReferenceValue as Mesh;
				meshCollider.convex = true;
				meshCollider.enabled = false;
			}
		}

		void Add_DrivingComponents (GameObject gameObject)
		{
			// Rigidbody
			Rigidbody rigidbody = gameObject.AddComponent < Rigidbody > ();
			rigidbody.mass = Wheel_MassProp.floatValue;
			// HingeJoint
			HingeJoint hingeJoint;
			hingeJoint = gameObject.AddComponent < HingeJoint > ();
			hingeJoint.anchor = Vector3.zero;
			hingeJoint.axis = new Vector3 (0.0f, 1.0f, 0.0f);
			hingeJoint.connectedBody = parentTransform.parent.gameObject.GetComponent < Rigidbody > ();
			// Drive_Wheel_CS
			Drive_Wheel_CS driveScript = gameObject.AddComponent < Drive_Wheel_CS > ();
			driveScript.Radius = Wheel_RadiusProp.floatValue;
			driveScript.Drive_Flag = Drive_WheelProp.boolValue;
			driveScript.Use_BrakeTurn = true;
		}

	}

}