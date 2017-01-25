using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ChobiAssets.PTM
{
	public class Menu_Dictionary_CS : MonoBehaviour
	{

		public Dictionary <string, GameObject> Tank_Dictionary;

		void Awake ()
		{ // Awake() is not called when the dictionary is sent to other scene.
			// Copy old Dictionary, and delete it.
			Menu_Dictionary_CS[] dictionaryScripts = FindObjectsOfType <Menu_Dictionary_CS> ();
			foreach (Menu_Dictionary_CS dictionaryScript in dictionaryScripts) {
				if (dictionaryScript != this) {
					Tank_Dictionary = dictionaryScript.Tank_Dictionary;
					Destroy (dictionaryScript.gameObject);
				}
			}
			// Keep this object even if the scene has been changed.
			DontDestroyOnLoad (this.gameObject);
		}

		void Start ()
		{ // (Note.) Dropdown options are set in Awake() in Menu_Dropdown_CS.
			// Find "Menu_Dropdown_CS".
			Menu_Dropdown_CS[] dropdownScripts = GameObject.FindObjectsOfType <Menu_Dropdown_CS> ();
			if (dropdownScripts.Length == 0) {
				Destroy (this.gameObject); // No need.
			}
			// Set Dropdown by using this Dictionary that has old values.
			if (Tank_Dictionary != null) { // Dictionary is already created in Awake().
				foreach (Menu_Dropdown_CS dropdownScript in dropdownScripts) {
					if (Tank_Dictionary.ContainsKey (dropdownScript.name)) {
						for (int i = 0; i < dropdownScript.Prefabs_Array.Length; i++) {
							if (dropdownScript.Prefabs_Array [i] == Tank_Dictionary [dropdownScript.name]) {
								dropdownScript.Dropdown.value = i;
								break;
							}
						}
					}
				}
			}
		}

		public void Create_Dictionary ()
		{ // Called from "Scene_Open_CS".
			// Find "Menu_Dropdown_CS".
			Menu_Dropdown_CS[] dropdownScripts = GameObject.FindObjectsOfType <Menu_Dropdown_CS> ();
			if (dropdownScripts.Length == 0) {
				return;
			}
			// Create dictionary.
			Tank_Dictionary = new Dictionary <string, GameObject> ();
			foreach (Menu_Dropdown_CS dropdownScript in dropdownScripts) {
				Tank_Dictionary [dropdownScript.name] = dropdownScript.Selected_Prefab;
			}
		}
	}
}
