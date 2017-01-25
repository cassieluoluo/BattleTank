using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ChobiAssets.PTM
{

	public class Trigger_Collider_CS : MonoBehaviour
	{

		public bool Invisible_Flag = true;
		public int Store_Count = 16;

		List < Event_Controller_CS > eventScripts = new List < Event_Controller_CS > ();
		List < GameObject > detectedObjects = new List < GameObject > ();

		void Awake ()
		{
			this.gameObject.layer = 2; // Ignore Raycast.
			// Set 'isTrigger' of all the colliders.
			Collider[] colliders = GetComponents < Collider > ();
			for (int i = 0; i < colliders.Length; i++) {
				colliders [i].isTrigger = true;
			}
			// Make the mesh invisible.
			if (Invisible_Flag) {
				MeshRenderer meshRenderer = GetComponent < MeshRenderer > ();
				if (meshRenderer) {
					meshRenderer.enabled = false;
				}
			}
		}

		void OnTriggerEnter (Collider collider)
		{
			GameObject detectedObject = collider.gameObject;
			if (detectedObject.layer == 11 && Check_DetectedObjects (detectedObject)) { // MainBody && This is the first time to be detected. 
				for (int i = 0; i < eventScripts.Count; i++) {
					if (eventScripts [i]) {
						// Send message to "Event_Controller_CS".
						eventScripts [i].Detect_Collider (collider.transform.root);
					}
				}
			}
		}

		bool Check_DetectedObjects (GameObject detectedObject)
		{
			GameObject newObject = detectedObjects.Find (delegate ( GameObject tempObject) {
				return tempObject == detectedObject;
			});
			if (newObject == null) {
				detectedObjects.Add (detectedObject);
				if (detectedObjects.Count > Store_Count) {
					detectedObjects.RemoveAt (0);
				}
				return true;
			}
			return false;
		}

		public void Get_Event_Controller (Event_Controller_CS eventScript)
		{ // Called from "Event_Controller_CS"..
			eventScripts.Add (eventScript);
		}

	}

}