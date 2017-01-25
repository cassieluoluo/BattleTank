using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{
	public class Menu_Dropdown_CS : MonoBehaviour
	{

		public int Num;
		public GameObject[] Prefabs_Array;
		public int Default_Value = 0;
		public Text Title_Text;
		public Transform Symbol_Transform;
		public float Offset;

		public GameObject Selected_Prefab; // Referred to from "Event_Controller_CS".
		Transform thisTransform;
		public Dropdown Dropdown; // Referred to from "Menu_Dictionary_CS".

		void Awake ()
		{
			thisTransform = transform;
			Dropdown = GetComponent <Dropdown> ();
			// Set Dropdown.
			Dropdown.ClearOptions ();
			foreach (GameObject prefab in Prefabs_Array) {
				if (prefab) {
					Dropdown.options.Add (new Dropdown.OptionData { text = prefab.name });
				} else {
					Dropdown.options.Add (new Dropdown.OptionData { text = "Empty" });
				}
			}
			Dropdown.RefreshShownValue();
			// Set initial selection.
			if (Prefabs_Array.Length > Default_Value) {
				Dropdown.value = Default_Value;
			}
			Selected_Prefab = Prefabs_Array [Dropdown.value];
			// Set title text.
			if (Title_Text) {
				Title_Text.text = this.name;
			}
		}
	
		public void On_Value_Changed ()
		{
			Selected_Prefab = Prefabs_Array [Dropdown.value];
		}

		void LateUpdate ()
		{
			if (Symbol_Transform) {
				Vector3 tempPos = Camera.main.WorldToScreenPoint (Symbol_Transform.position + Symbol_Transform.forward * Offset);
				thisTransform.position = tempPos;
			}
		}

	}
}
