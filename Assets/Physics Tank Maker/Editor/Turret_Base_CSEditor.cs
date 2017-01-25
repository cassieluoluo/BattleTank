using UnityEngine;
using System.Collections;
using UnityEditor;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Turret_Base_CS))]
	public class Turret_Base_CSEditor : Editor
	{

		SerializedProperty Part_MeshProp;
		SerializedProperty Collider_MeshProp;
		SerializedProperty Sub_Collider_MeshProp;

		SerializedProperty Materials_NumProp;
		SerializedProperty MaterialsProp;
		SerializedProperty Part_MaterialProp;

		SerializedProperty Offset_XProp;
		SerializedProperty Offset_YProp;
		SerializedProperty Offset_ZProp;

		SerializedProperty Coming_OffProp;
		SerializedProperty Turret_MassProp;
		SerializedProperty DurabilityProp;
		SerializedProperty Sub_DurabilityProp;
		SerializedProperty Trouble_TimeProp;
		SerializedProperty Damage_Effect_ObjectProp;
		SerializedProperty Trouble_Effect_ObjectProp;

		Transform parentTransform;

		void  OnEnable ()
		{
			Part_MeshProp = serializedObject.FindProperty ("Part_Mesh");
			Collider_MeshProp = serializedObject.FindProperty ("Collider_Mesh");
			Sub_Collider_MeshProp = serializedObject.FindProperty ("Sub_Collider_Mesh");

			Materials_NumProp = serializedObject.FindProperty ("Materials_Num");
			MaterialsProp = serializedObject.FindProperty ("Materials");
			Part_MaterialProp = serializedObject.FindProperty ("Part_Material");

			Offset_XProp = serializedObject.FindProperty ("Offset_X");
			Offset_YProp = serializedObject.FindProperty ("Offset_Y");
			Offset_ZProp = serializedObject.FindProperty ("Offset_Z");

			Coming_OffProp = serializedObject.FindProperty ("Coming_Off");
			Turret_MassProp = serializedObject.FindProperty ("Turret_Mass");
			DurabilityProp = serializedObject.FindProperty ("Durability");
			Sub_DurabilityProp = serializedObject.FindProperty ("Sub_Durability");
			Trouble_TimeProp = serializedObject.FindProperty ("Trouble_Time");
			Damage_Effect_ObjectProp = serializedObject.FindProperty ("Damage_Effect_Object");
			Trouble_Effect_ObjectProp = serializedObject.FindProperty ("Trouble_Effect_Object");
		
			if (Selection.activeGameObject) {
				parentTransform = Selection.activeGameObject.transform;
			}
		}

		public override void OnInspectorGUI ()
		{
			Set_Inspector ();
			if (GUI.changed) {
				Create ();
			}
			if (Event.current.commandName == "UndoRedoPerformed") {
				Create ();
			}
			// Call Create() while the object is moving.
			if (parentTransform.hasChanged) {
				Create ();
				parentTransform.hasChanged = false;
			}
		}

		void Set_Inspector ()
		{
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			serializedObject.Update ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Fold out above 'Transform' window when you move this object.", MessageType.Warning, true);

			// Mesh settings
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Mesh settings", MessageType.None, true);
			Part_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Mesh", Part_MeshProp.objectReferenceValue, typeof(Mesh), false);
			EditorGUILayout.Space ();
			EditorGUILayout.IntSlider (Materials_NumProp, 1, 10, "Number of Materials");
			MaterialsProp.arraySize = Materials_NumProp.intValue;
			if (Materials_NumProp.intValue == 1 && Part_MaterialProp.objectReferenceValue != null) {
				if (MaterialsProp.GetArrayElementAtIndex (0).objectReferenceValue == null) {
					MaterialsProp.GetArrayElementAtIndex (0).objectReferenceValue = Part_MaterialProp.objectReferenceValue;
				}
				Part_MaterialProp.objectReferenceValue = null;
			}
			for (int i = 0; i < Materials_NumProp.intValue; i++) {
				MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue = EditorGUILayout.ObjectField ("Material", MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue, typeof(Material), false);
			}

			// Position settings
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Position settings", MessageType.None, true);
			EditorGUILayout.Slider (Offset_XProp, -5.0f, 5.0f, "Offset X");
			EditorGUILayout.Slider (Offset_YProp, -5.0f, 5.0f, "Offset Y");
			EditorGUILayout.Slider (Offset_ZProp, -10.0f, 10.0f, "Offset Z");

			// Collider settings
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Collider settings", MessageType.None, true);
			Collider_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("MeshCollider", Collider_MeshProp.objectReferenceValue, typeof(Mesh), false);
			Sub_Collider_MeshProp.objectReferenceValue = EditorGUILayout.ObjectField ("Sub MeshCollider", Sub_Collider_MeshProp.objectReferenceValue, typeof(Mesh), false);

			// Damage settings
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Damage settings", MessageType.None, true);
			//  Coming off
			Coming_OffProp.boolValue = EditorGUILayout.Toggle ("Coming off", Coming_OffProp.boolValue);
			// Mass
			if (Coming_OffProp.boolValue) {
				EditorGUILayout.Slider (Turret_MassProp, 1.0f, 10000.0f, "Turret Mass");
			}
			EditorGUILayout.Space ();
			// Durability
			EditorGUILayout.Slider (DurabilityProp, 1.0f, 1000000.0f, "Durability");
			if (DurabilityProp.floatValue >= 1000000) {
				DurabilityProp.floatValue = Mathf.Infinity;
			}
			EditorGUILayout.Slider (Sub_DurabilityProp, 1.0f, 1000000.0f, "Sub Durability");
			if (Sub_DurabilityProp.floatValue >= 1000000) {
				Sub_DurabilityProp.floatValue = Mathf.Infinity;
			}
			// Effect
			EditorGUILayout.Slider (Trouble_TimeProp, 0.0f, 120.0f, "Trouble Time");
			Trouble_Effect_ObjectProp.objectReferenceValue = EditorGUILayout.ObjectField ("Trouble Effect Prefab", Trouble_Effect_ObjectProp.objectReferenceValue, typeof(GameObject), true);
			Damage_Effect_ObjectProp.objectReferenceValue = EditorGUILayout.ObjectField ("Damage Effect Prefab", Damage_Effect_ObjectProp.objectReferenceValue, typeof(GameObject), true);

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
			Transform oldTransform = parentTransform.Find ("Turret"); // Find the old object.
			int childCount;
			Transform[] childTransforms;
			if (oldTransform) {
				childCount = oldTransform.transform.childCount;
				childTransforms = new Transform [ childCount ];
				for (int i = 0; i < childCount; i++) {
					childTransforms [i] = oldTransform.GetChild (0); // Get the child object such as "Armor_Collider".
					childTransforms [i].parent = parentTransform; // Change the parent of the child object.
				}
				DestroyImmediate (oldTransform.gameObject); // Delete old object.
			} else {
				childCount = 0;
				childTransforms = null;
			}
			// Create new Gameobject & Set Transform.
			GameObject newObject = new GameObject ("Turret");
			newObject.transform.parent = parentTransform;
			newObject.transform.localPosition = -parentTransform.localPosition + new Vector3 (Offset_XProp.floatValue, Offset_YProp.floatValue, Offset_ZProp.floatValue);
			newObject.transform.localRotation = Quaternion.identity;
			// Add components
			MeshRenderer meshRenderer = newObject.AddComponent < MeshRenderer > ();
			Material[] materials = new Material [ Materials_NumProp.intValue ];
			for (int i = 0; i < materials.Length; i++) {
				materials [i] = MaterialsProp.GetArrayElementAtIndex (i).objectReferenceValue as Material;
			}
			meshRenderer.materials = materials;
			MeshFilter meshFilter = newObject.AddComponent < MeshFilter > ();
			meshFilter.mesh = Part_MeshProp.objectReferenceValue as Mesh;
			if (Collider_MeshProp.objectReferenceValue) {
				MeshCollider meshCollider = newObject.AddComponent < MeshCollider > ();
				meshCollider.sharedMesh = Collider_MeshProp.objectReferenceValue as Mesh;
				meshCollider.convex = true;
			}
			if (Sub_Collider_MeshProp.objectReferenceValue) {
				MeshCollider meshCollider = newObject.AddComponent < MeshCollider > ();
				meshCollider.sharedMesh = Sub_Collider_MeshProp.objectReferenceValue as Mesh;
				meshCollider.convex = true;
			}
			// Add script
			Damage_Control_CS damageScript = newObject.AddComponent < Damage_Control_CS > ();
			damageScript.Type = 2;  // 2 = Turret
			damageScript.Durability = DurabilityProp.floatValue;
			damageScript.Sub_Durability = Sub_DurabilityProp.floatValue;
			damageScript.Trouble_Time = Trouble_TimeProp.floatValue;
			damageScript.Trouble_Effect_Object = Trouble_Effect_ObjectProp.objectReferenceValue as GameObject;
			damageScript.Damage_Effect_Object = Damage_Effect_ObjectProp.objectReferenceValue as GameObject;
			damageScript.Coming_Off = Coming_OffProp.boolValue;
			// Set Layer
			newObject.layer = 0;
			// Return the child objects.
			if (childCount > 0) {
				for (int i = 0; i < childCount; i++) {
					childTransforms [i].transform.parent = newObject.transform;
				}
			}
		}

	}

}