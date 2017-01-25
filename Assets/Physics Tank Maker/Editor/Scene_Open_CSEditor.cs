using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{

	[ CustomEditor (typeof(Scene_Open_CS))]
	public class Scene_Open_CSEditor : Editor
	{

		SerializedProperty Scene_NameProp;
		SerializedProperty Fade_ImageProp;

		void OnEnable ()
		{
			Scene_NameProp = serializedObject.FindProperty ("Scene_Name");
			Fade_ImageProp = serializedObject.FindProperty ("Fade_Image");
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
			EditorGUILayout.HelpBox ("Open Scene Settings", MessageType.None, true);
			EditorGUILayout.Space ();
			Scene_NameProp.stringValue = EditorGUILayout.TextField ("Scene Name", Scene_NameProp.stringValue);
			EditorGUILayout.Space ();
			Fade_ImageProp.objectReferenceValue = EditorGUILayout.ObjectField ("Fade Image", Fade_ImageProp.objectReferenceValue, typeof(Image), true);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			//
			serializedObject.ApplyModifiedProperties ();
		}



	}
}
