using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	[ RequireComponent (typeof(Camera))]
	[ RequireComponent (typeof(AudioListener))]

	public class RC_Camera_CS : MonoBehaviour
	{
		public int Input_Type = 4;
		public float FOV = 20.0f;
		public float Horizontal_Speed = 1.0f;
		public float Vertical_Speed = 1.0f;
		public float Zoom_Speed = 0.3f;
		public float Min_FOV = 1.0f;
		public float Max_FOV = 50.0f;
		public Transform Position_Pack;

		Transform thisTransform;
		bool isRcCamEnabled = true;
		Transform targetTransform;
		Camera thisCamera;
		AudioListener thisAudioListener;
		int gunCamMode = 0;
		Vector3[] camPositions;
		Camera_Distance_CS mainCamScript;

		float horizontal;
		float vertical;
		bool isTurning;
	
		Game_Controller_CS controllerScript;
		int currentID = 1;

		void Awake ()
		{
			this.tag = "Untagged";
			thisTransform = transform;
			thisCamera = GetComponent < Camera > ();
			thisCamera.enabled = false;
			thisCamera.cullingMask = -1;
			thisCamera.depth = 0;
			thisCamera.fieldOfView = FOV;
			thisAudioListener = GetComponent < AudioListener > ();
			thisAudioListener.enabled = false;
			AudioListener.volume = 1.0f;
			// Set and store the 'camPositions'.
			Set_Positions ();
		}

		void Set_Positions ()
		{
			if (Position_Pack) {
				if (Position_Pack.childCount != 0) {
					camPositions = new Vector3 [ Position_Pack.childCount ];
					for (int i = 0; i < Position_Pack.childCount; i++) {
						camPositions [i] = Position_Pack.GetChild (i).position;
					}
					thisTransform.position = camPositions [0];
				} else { // Position_Pack is empty.
					camPositions = new Vector3 [ 1 ];
					camPositions [0] = thisTransform.position; // Set initial position.
				}
			}
		}

		void Start ()
		{
			// Find "Game_Controller" and send this reference.
			GameObject controllerObject = GameObject.FindGameObjectWithTag ("GameController");
			if (controllerObject) {
				controllerScript = controllerObject.GetComponent < Game_Controller_CS > ();
			}
			if (controllerScript == null) {
				Debug.LogError ("There is no 'Game_Controller' in the scene.");
				Destroy (gameObject);
			}
			controllerScript.RC_Cam_Script = this ;
		}

		void Update ()
		{
			if (targetTransform) {
				// Switch the camera.
				if (Input.GetKeyDown (KeyCode.Tab)) {
					if (isRcCamEnabled) {
						isRcCamEnabled = false;
					} else {
						isRcCamEnabled = true;
					}
					// Send message to the Main_Camera (Camera_Distance_CS).
					mainCamScript.Switch_Camera (isRcCamEnabled);
					Control_Enabled ();
				}
				// Turn and zoom.
				if (thisCamera.enabled) {
					switch (Input_Type) {
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
					}
					if (isTurning == false) {
						thisTransform.LookAt (targetTransform);
					}
					// Control camera position.
					if (Position_Pack) {
						Control_Position ();
					}
				}
			} else { // 'targetTransform' is empty at the opening.
				Receive_Current_ID (currentID);
			}
		}

		void Control_Position ()
		{
			Vector3 tempPos = targetTransform.position;
			float shortestDist = Mathf.Infinity;
			int tempIndex = 0;
			for (int i = 0; i < camPositions.Length; i++) {
				float tempDist = Vector3.Distance (tempPos, camPositions [i]);
				if (tempDist < shortestDist) {
					shortestDist = tempDist;
					tempIndex = i;
				}
			}
			thisTransform.position = camPositions [tempIndex];
		}

		void KeyBoard_Input ()
		{
			if (Input.GetKey ("e")) {
				Zoom (-Zoom_Speed);
			} else if (Input.GetKey ("r")) {
				Zoom (Zoom_Speed);
			}
		}

		void Stick_Input ()
		{
			if (Input.GetButton ("Fire3")) {
				Zoom (-Zoom_Speed);
			} else if (Input.GetButton ("Fire1")) {
				Zoom (Zoom_Speed);
			}
			if (Input.GetButton ("L_Button")) {
				isTurning = true;
				horizontal = Input.GetAxis ("Horizontal2");
				vertical = Input.GetAxis ("Vertical2");
				Stick_Rotate ();
			} else {
				isTurning = false;
			}
		}

		void Trigger_Input ()
		{
			if (Input.GetButton ("Fire1") && Input.GetAxis ("Vertical") > 0) {
				Zoom (-Zoom_Speed);
			} else if (Input.GetButton ("Fire1") && Input.GetAxis ("Vertical") < 0) {
				Zoom (Zoom_Speed);
			}
			horizontal = Input.GetAxis ("Horizontal2");
			vertical = Input.GetAxis ("Vertical2");
			if (horizontal == 0.0f && vertical == 0.0f) {
				isTurning = false;
			} else {
				isTurning = true;
				Stick_Rotate ();
			}
		}

		Vector2 previousMousePos;
		void Mouse_Input ()
		{
			if (Input.GetMouseButtonDown (1)) {
				previousMousePos = Input.mousePosition;
			} else if (Input.GetMouseButton (1)) {
				// Zoom
				if (Input.GetAxis ("Mouse ScrollWheel") > 0) {
					Zoom (-Zoom_Speed * 3.0f);
				} else if (Input.GetAxis ("Mouse ScrollWheel") < 0) {
					Zoom (Zoom_Speed * 3.0f);
				}
				// Turn
				Vector2 currentMousePos = Input.mousePosition;
				if (currentMousePos != previousMousePos) {
					isTurning = true;
				}
				horizontal = (currentMousePos.x - previousMousePos.x) * 0.1f;
				vertical = (currentMousePos.y - previousMousePos.y) * 0.1f;
				Rotate ();
				previousMousePos = currentMousePos;
			} else if (Input.GetMouseButtonUp (1)) {
				isTurning = false;
			}

		}

		void Zoom (float Temp_Speed)
		{
			FOV += Temp_Speed;
			FOV = Mathf.Clamp (FOV, Min_FOV, Max_FOV);
			thisCamera.fieldOfView = FOV;
		}

		void Rotate ()
		{
			if (horizontal != 0.0f) {
				float Temp_X = thisTransform.localEulerAngles.x;
				float Temp_Y = thisTransform.localEulerAngles.y + horizontal * Horizontal_Speed;
				float Temp_Z = thisTransform.localEulerAngles.z;
				thisTransform.localEulerAngles = new Vector3 (Temp_X, Temp_Y, Temp_Z);
			} 
			if (vertical != 0.0f) {
				float Temp_X = thisTransform.localEulerAngles.x - vertical * Vertical_Speed;
				float Temp_Y = thisTransform.localEulerAngles.y;
				float Temp_Z = thisTransform.localEulerAngles.z;
				thisTransform.localEulerAngles = new Vector3 (Temp_X, Temp_Y, Temp_Z);
			}
		}

		void Stick_Rotate ()
		{
			thisTransform.LookAt (targetTransform);
			thisTransform.eulerAngles = new Vector3 (thisTransform.eulerAngles.x - vertical * 22.5f, thisTransform.eulerAngles.y + horizontal * 45.0f, 0.0f);
		}

		void Control_Enabled ()
		{
			if (gunCamMode == 2 || isRcCamEnabled == false) {
				thisCamera.enabled = false;
				thisAudioListener.enabled = false;
				this.tag = "Untagged";
			} else {
				thisCamera.enabled = true;
				thisAudioListener.enabled = true;
				this.tag = "MainCamera";
			}
		}

		public void Receive_Current_ID (int id)
		{ // Called from Game_Controller, also called from this script.
			currentID = id;
			// Operable_Tanks [id] may be null at the opening.
			if (controllerScript.Operable_Tanks [id] == null) {
				return;
			}
			// Find the MainBody.
			MainBody_Setting_CS bodyScript = controllerScript.Operable_Tanks [id].GetComponentInChildren < MainBody_Setting_CS > ();
			targetTransform = bodyScript.transform;
			// Send message to the Main_Camera (Camera_Distance_CS).
			mainCamScript = targetTransform.GetComponentInChildren <Camera_Distance_CS> ();
			mainCamScript.Switch_Camera (isRcCamEnabled);
			// Send message to the Gun_Camera (Gun_Camera_CS), and get the current gun-camera mode.
			Gun_Camera_CS gunCamScript = targetTransform.GetComponentInChildren <Gun_Camera_CS> ();
			if (gunCamScript) {
				gunCamMode = gunCamScript.Get_RC_Camera_Script (this);
			} else { // This tank has no Gun_Camera.
				gunCamMode = 0;
			}
			Control_Enabled ();
		}

		public void Change_GunCam_Mode (int tempMode)
		{ // Called from "Gun_Camera_CS". (Also called when the turret is broken.)
			gunCamMode = tempMode;
			Control_Enabled ();
		}
	
	}

}