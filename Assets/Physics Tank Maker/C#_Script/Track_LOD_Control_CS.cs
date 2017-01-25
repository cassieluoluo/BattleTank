using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{
	public class Track_LOD_Control_CS : MonoBehaviour
	{

		public GameObject Static_Track;
		public GameObject Scroll_Track_L;
		public GameObject Scroll_Track_R;
		public float Threshold = 15.0f;

		Transform thisTransform;
		MainBody_Setting_CS bodyScript;

		void Awake ()
		{
			thisTransform = transform;
			if (Static_Track == null || Scroll_Track_L == null || Scroll_Track_R == null) {
				Debug.LogWarning ("Track LOD system cannot work, because the tracks for 'LOD_Control_CS' are not assigned.");
				Destroy (this);
			} else {
				// Set Active all the tracks for their initial settings.
				Static_Track.SetActive (true);
				Scroll_Track_L.SetActive (true);
				Scroll_Track_R.SetActive (true);
			}
		}

		void Update ()
		{
			if (bodyScript.Visible_Flag) {
				Camera[] currentCams = Camera.allCameras;
				Camera chosenCam = null;
				float value = Mathf.Infinity;
				foreach (Camera currentCam in currentCams) {
					float tempValue = 2.0f * Vector3.Distance (thisTransform.position, currentCam.transform.position) * Mathf.Tan (currentCam.fieldOfView * 0.5f * Mathf.Deg2Rad);
					if (tempValue < value) {
						value = tempValue;
						chosenCam = currentCam;
					}
				}
				value *= Screen.width / chosenCam.pixelWidth;
				//
				if (value < Threshold) {
					Static_Track.SetActive (true);
					Scroll_Track_L.SetActive (false);
					Scroll_Track_R.SetActive (false);
				} else {
					Static_Track.SetActive (false);
					Scroll_Track_L.SetActive (true);
					Scroll_Track_R.SetActive (true);
				}
			} else {
				//Static_Track.SetActive (false);
				Scroll_Track_L.SetActive (false);
				Scroll_Track_R.SetActive (false);
			}
		}

		void TrackBroken_Linkage (int direction)
		{ // Called from "Damage_Control_CS" in Track_Collider.
			if (Static_Track) {
				Static_Track.SetActive (true);
			}
			if (Scroll_Track_L) {
				Destroy (Scroll_Track_L);
			}
			if (Scroll_Track_R) {
				Destroy (Scroll_Track_R);
			}
			Destroy (this);
		}

		void Get_Tank_ID_Control (Tank_ID_Control_CS topScript)
		{
			bodyScript = topScript.Stored_TankProp.bodyScript;
		}

	}
}
