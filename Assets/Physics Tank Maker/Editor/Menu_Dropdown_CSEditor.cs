using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{
	
	[ CustomEditor (typeof(Menu_Dropdown_CS))]
	public class Menu_Dropdown_CSEditor : Editor
	{

		SerializedProperty NumProp;
		SerializedProperty Prefabs_ArrayProp;
		SerializedProperty Default_ValueProp;
		SerializedProperty Title_TextProp;
		SerializedProperty Symbol_TransformProp;
		SerializedProperty OffsetProp;

		void OnEnable ()
		{
			NumProp = serializedObject.FindProperty ("Num");
			Prefabs_ArrayProp = serializedObject.FindProperty ("Prefabs_Array");
			Default_ValueProp = serializedObject.FindProperty ("Default_Value");
			Title_TextProp = serializedObject.FindProperty ("Title_Text");
			Symbol_TransformProp = serializedObject.FindProperty ("Symbol_Transform");
			OffsetProp = serializedObject.FindProperty ("Offset");
		}

		public override void OnInspectorGUI ()
		{
			if (Application.isPlaying == false) {
				Set_Inspector ();
			}
		}

		void Set_Inspector ()
		{
			GUI.backgroundColor = new Color (1.0f, 1.0f, 0.5f, 1.0f);
			serializedObject.Update ();

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.HelpBox ("Dropdown Settings", MessageType.None, true);

			EditorGUILayout.Space ();
			EditorGUILayout.IntSlider (NumProp, 1, 64, "Number of Prefabs");
			Prefabs_ArrayProp.arraySize = NumProp.intValue;
			for (int i = 0; i < NumProp.intValue; i++) {
				Prefabs_ArrayProp.GetArrayElementAtIndex (i).objectReferenceValue = EditorGUILayout.ObjectField ("Prefab " + "[" + i + "]", Prefabs_ArrayProp.GetArrayElementAtIndex (i).objectReferenceValue, typeof(GameObject), false);
			}

			EditorGUILayout.Space ();
			EditorGUILayout.IntSlider (Default_ValueProp, 0, Prefabs_ArrayProp.arraySize - 1, "Default Prefab's Index");
			if (Default_ValueProp.intValue > Prefabs_ArrayProp.arraySize - 1) {
				Default_ValueProp.intValue = Prefabs_ArrayProp.arraySize - 1;
			}
			EditorGUILayout.LabelField (Prefabs_ArrayProp.GetArrayElementAtIndex (Default_ValueProp.intValue).objectReferenceValue.name);

			EditorGUILayout.Space ();
			Title_TextProp.objectReferenceValue = EditorGUILayout.ObjectField ("Title Text", Title_TextProp.objectReferenceValue, typeof(Text), true);

			EditorGUILayout.Space ();
			Symbol_TransformProp.objectReferenceValue = EditorGUILayout.ObjectField ("Symbol Object", Symbol_TransformProp.objectReferenceValue, typeof(Transform), true);

			EditorGUILayout.Space ();
			EditorGUILayout.Slider (OffsetProp, -1000.0f, 1000.0f, "Offset");

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			//
			serializedObject.ApplyModifiedProperties ();
		}



	}
}
