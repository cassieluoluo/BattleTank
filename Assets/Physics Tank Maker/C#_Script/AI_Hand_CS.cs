using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{
	
	public class AI_Hand_CS : MonoBehaviour
	{
		
		public bool Is_Working = false; // Referred to from AI_CS.
		bool isTouching = false;
		float touchCount;
		Collider touchCollider;
		AI_CS aiScript;

		void Awake ()
		{
			gameObject.layer = 2; // "Ignore Raycast" layer.
			// Make it invisible.
			Renderer renderer = GetComponent < Renderer > ();
			if (renderer) {
				renderer.enabled = false;
			}
			// Make it a trigger.
			Collider collider = GetComponent < Collider > ();
			if (collider) {
				collider.isTrigger = true;
			}
		}

		void Update ()
		{
			if (Is_Working) {
				if (isTouching) {
					if (touchCollider == null) { // The touched tank may be removed by respawn.
						isTouching = false;
						return;
					}
					touchCount += Time.deltaTime;
					if (touchCount > aiScript.Stuck_Count) {
						touchCount = 0.0f;
						aiScript.Escape_Stuck ();
					}
					return;
				} else {
					touchCount -= Time.deltaTime;
					if (touchCount < 0.0f) {
						touchCount = 0.0f;
						Is_Working = false;
					}
				}
			}
		}

		void OnTriggerStay (Collider collider)
		{
			if (isTouching == false && collider.attachedRigidbody) {
				if (collider.transform.root.tag != "Finish") {
					Is_Working = true;
					isTouching = true;
					touchCollider = collider;
				}
			}
		}

		void OnTriggerExit ()
		{
			isTouching = false;
		}

		void Get_Tank_ID_Control (Tank_ID_Control_CS topScript)
		{
			aiScript = topScript.Stored_TankProp.aiScript;
		}

	}

}