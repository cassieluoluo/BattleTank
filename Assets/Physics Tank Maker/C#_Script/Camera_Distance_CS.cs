using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	[ RequireComponent (typeof(Camera))]
	[ RequireComponent (typeof(AudioListener))]
	public class Camera_Distance_CS : MonoBehaviour
	{
		public float FPV_FOV = 50.0f;
		public float TPV_FOV = 30.0f;
		public float FPV_ClippingPlanesNear = 0.1f;
		public float TPV_ClippingPlanesNear = 1.0f;
		public float Min_Dist = 3.0f;
		public float Max_Dist = 30.0f;
		public float Zoom_Speed = 15.0f;

		Transform thisTransform;
		Transform parentTransform;
		float currentDistance;
		float targetDistance;

		Camera thisCamera;
		AudioListener thisAudioListener;
		bool isTPV = true;
		int gunCamMode = 0;
		bool isRcCamEnabled = false;

		bool isCurrent;
		int myID;
		int inputType = 4;

		void Awake ()
		{
			this.tag = "MainCamera";
			thisCamera = GetComponent < Camera > ();
			thisCamera.enabled = false;
			thisCamera.cullingMask = -1;
			thisCamera.depth = 0;
			thisCamera.fieldOfView = TPV_FOV;
			thisCamera.nearClipPlane = TPV_ClippingPlanesNear;
			thisAudioListener = GetComponent < AudioListener > ();
			thisAudioListener.enabled = false;
			thisTransform = transform;
			parentTransform = thisTransform.parent;
			thisTransform.LookAt (parentTransform);
			currentDistance = thisTransform.localPosition.x;
			targetDistance = currentDistance;
		}

		void Update ()
		{
			if (isCurrent && thisCamera.enabled) {
				switch (inputType) {
				case 0:
					KeyBoard_Input ();
					break;
				case 1:
					Stick_Input ();
					break;
				case 2:
					Trigger_Input ();
					break;
				case 3:
					Stick_Input ();
					break;
				case 4:
					Mouse_Input ();
					break;
				case 5:
					Mouse_Input ();
					break;
				case 6:
					Mouse_Input ();
					break;
				case 10:
					Mouse_Input ();
					break;
				}
			}
		}

		void KeyBoard_Input ()
		{
			if (Input.GetKey ("e")) {
				Move (-Zoom_Speed);
			} else if (Input.GetKey ("q")) {
				Move (Zoom_Speed);
			}
		}

		void Stick_Input ()
		{
			if (Input.GetButton ("Fire3")) {
				Move (-Zoom_Speed);
			} else if (Input.GetButton ("Fire1")) {
				Move (Zoom_Speed);
			}
		}

		void Trigger_Input ()
		{
			if (Input.GetButton ("Fire1") && Input.GetAxis ("Vertical") > 0) {
				Move (-Zoom_Speed);
			} else if (Input.GetButton ("Fire1") && Input.GetAxis ("Vertical") < 0) {
				Move (Zoom_Speed);
			}
		}

		void Move (float rate)
		{
			if (isTPV) { // Third Person View
				currentDistance += rate * Time.deltaTime;
				if (currentDistance < Min_Dist) {
					Switch_To_FPV ();
					return;
				}
				currentDistance = Mathf.Clamp (currentDistance, Min_Dist, Max_Dist);
				thisTransform.localPosition = new Vector3 (currentDistance, 0.0f, 0.0f);
			} else { // First Person View
				if (rate > 0.0f) {
					Switch_To_TPV ();
					return;
				}
			}
		}

		void Mouse_Input ()
		{
			if (Input.GetMouseButton (1)) {
				if (Input.GetAxis ("Mouse ScrollWheel") > 0) { // Forward
					if (isTPV) {
						targetDistance -= 2.0f;
					} else {
						return;
					}
				} else if (Input.GetAxis ("Mouse ScrollWheel") < 0) { //Backward
					if (isTPV) {
						targetDistance += 2.0f;
						targetDistance = Mathf.Clamp (targetDistance, Min_Dist, Max_Dist);
					} else {
						Switch_To_TPV ();
						return;
					}
				}
			}
			if (currentDistance != targetDistance) {
				Move_Smoothly ();
			}
		}

		void Move_Smoothly ()
		{
			currentDistance = Mathf.MoveTowards (currentDistance, targetDistance, Zoom_Speed * Time.fixedDeltaTime);
			if (currentDistance < Min_Dist) {
				Switch_To_FPV ();
			} else {
				thisTransform.localPosition = new Vector3 (currentDistance, 0.0f, 0.0f);
			}
		}

		void Switch_To_TPV ()
		{
			isTPV = true;
			parentTransform.SendMessage ("Switch_View", isTPV, SendMessageOptions.DontRequireReceiver); // Send Message to "Look_At_Point".
			targetDistance = Min_Dist * 2.0f;
			currentDistance = Min_Dist * 2.0f;
			thisTransform.localPosition = new Vector3 (currentDistance, 0.0f, 0.0f);
			thisCamera.fieldOfView = TPV_FOV;
			thisCamera.nearClipPlane = TPV_ClippingPlanesNear;
		}

		void Switch_To_FPV ()
		{
			isTPV = false;
			parentTransform.SendMessage ("Switch_View", isTPV, SendMessageOptions.DontRequireReceiver); // Send Message to "Look_At_Point".
			targetDistance = 0.0f;
			currentDistance = 0.0f;
			thisTransform.localPosition = Vector3.zero;
			thisCamera.fieldOfView = FPV_FOV;
			thisCamera.nearClipPlane = FPV_ClippingPlanesNear;
		}

		public void Change_GunCam_Mode (int tempMode)
		{ // Called from "Gun_Camera_CS". (Also called when the turret is broken.)
			gunCamMode = tempMode;
			if (isCurrent) {
				Control_Enabled ();
			}
		}

		public void Switch_Camera (bool flag)
		{ // Called from "RC_Camera".
			isRcCamEnabled = flag;
			Control_Enabled ();
		}

		void Control_Enabled ()
		{
			if (gunCamMode == 2 || isRcCamEnabled) { // Gun_Camera is full screen or RC_Camera is enabled now.
				thisCamera.enabled = false;
				thisAudioListener.enabled = false;
				this.tag = "Untagged";
			} else {
				thisCamera.enabled = true;
				thisAudioListener.enabled = true;
				this.tag = "MainCamera";
			}
		}

		void Get_Tank_ID_Control (Tank_ID_Control_CS topScript)
		{
			inputType = topScript.Input_Type;
			myID = topScript.Tank_ID;
		}

		void Receive_Current_ID (int id)
		{
			if (id == myID) {
				isCurrent = true;
				Control_Enabled ();
			} else if (isCurrent) {
				isCurrent = false;
				thisCamera.enabled = false;
				thisAudioListener.enabled = false;
			}
		}

	}

}