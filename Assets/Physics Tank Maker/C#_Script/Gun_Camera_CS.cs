using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{

	[ RequireComponent (typeof(Camera))]
	[ RequireComponent (typeof(AudioListener))]
	public class Gun_Camera_CS : MonoBehaviour
	{

		public string Reticle_Name = "Reticle";
		public float Small_Width = 0.4f;
		public float Small_Height = 0.4f;

		int mode = 0;
		Camera thisCamera;
		AudioListener thisListener;
		Reticle_Control_CS reticleScript;

		float angleX;
		float zoomAxis;
		float angleAxis;

		Camera_Distance_CS mainCamScript;
		RC_Camera_CS rcCameraScript;
		public Turret_Horizontal_CS Turret_Script; // Referred to from "ReticleWheel_Control_CS".
	
		bool isCurrent;
		int myID;
		int inputType = 4;

		void Awake ()
		{
			this.tag = "Untagged";
			thisCamera = GetComponent < Camera > ();
			thisCamera.enabled = false;
			thisCamera.cullingMask = -1;
			thisCamera.depth = 1;
			thisListener = GetComponent < AudioListener > ();
			if (thisListener == null) {
				thisListener = gameObject.AddComponent < AudioListener > ();
			}
			thisListener.enabled = false;
			AudioListener.volume = 1.0f;
			// Find the Reticle Image.
			if (string.IsNullOrEmpty (Reticle_Name) == false) {
				GameObject reticleObject = GameObject.Find (Reticle_Name);
				if (reticleObject) {
					reticleScript = reticleObject.GetComponent <Reticle_Control_CS> ();
				} else {
					Debug.LogWarning (Reticle_Name + " cannot be found in the scene.");
				}
			}
		}

		void Start ()
		{ // After changing the hierarchy.
			if (reticleScript && Turret_Script == null) {
				Turret_Script = GetComponentInParent <Turret_Horizontal_CS> ();
			}
		}

		void Update ()
		{
			if (isCurrent) {
				switch (inputType) {
				case 0:
					KeyBoard_Input ();
					break;
				case 1:
					GamePad_Input ();
					break;
				case 2:
					GamePad_Input ();
					break;
				case 3:
					GamePad_Input ();
					break;
				case 4:
					Mouse_Input ();
					break;
				case 5:
					Mouse_Input ();
					break;
				case 6:
					AI_Semi_Input ();
					break;
				case 10:
					Mouse_Input ();
					break;
				}
			}
		}

		void KeyBoard_Input ()
		{
			if (Input.GetKeyDown ("r")) {
				mode += 1;
				if (mode > 2) {
					mode = 0;
				}
				Change_Mode ();
				return;
			}
			if (mode != 0) {
				if (Input.GetKey ("f")) { 
					zoomAxis = Input.GetAxisRaw ("Horizontal");
					angleAxis = Input.GetAxisRaw ("Vertical") * 0.05f;
					Zoom ();
					Rotate ();
				}
			}
		}

		void GamePad_Input ()
		{
			if (Input.GetButtonDown ("Fire2")) {
				mode += 1;
				if (mode > 2) {
					mode = 0;
				}
				Change_Mode ();
				return;
			}
			if (mode != 0) {
				if (Input.GetButton ("Jump")) {
					zoomAxis = Input.GetAxis ("Horizontal");
					angleAxis = Input.GetAxis ("Vertical") * 0.05f;
					Zoom ();
					Rotate ();
				}
			}
		}

		bool isForced = false;
		void Mouse_Input ()
		{
			if (Input.GetKeyDown ("r")) { 
				mode += 1;
				if (mode > 2) {
					mode = 0;
					isForced = false;
				} else {
					isForced = true;
				}
				Change_Mode ();
				return;
			}

			if (isForced == false) {
				switch (mode) {
				case 0:
					if (Input.GetKeyDown (KeyCode.Space)) {
						mode = 2;
						Change_Mode ();
						return;
					}
					break;
				case 2:
					if (Input.GetKeyUp (KeyCode.Space)) {
						mode = 0;
						Change_Mode ();
						return;
					}
					break;
				}
			}

			if (mode != 0 && Input.GetMouseButton (1) == false) {
				float inputAxis = Input.GetAxis ("Mouse ScrollWheel");
				if (inputAxis != 0.0f) {
					if (Input.GetKey ("f")) {
						angleAxis = inputAxis;
						Rotate ();
					} else {
						zoomAxis = inputAxis;
						Zoom ();
					}
				}
			}
		}

		void AI_Semi_Input ()
		{
			if (Input.GetKeyDown ("r")) { 
				mode += 1;
				if (mode > 2) {
					mode = 0;
				}
				Change_Mode ();
				return;
			}
			if (Input.GetMouseButton (1) == false && thisCamera.pixelRect.Contains (Input.mousePosition)) {
				float inputAxis = Input.GetAxis ("Mouse ScrollWheel");
				if (inputAxis != 0.0f) {
					if (Input.GetKey ("f")) {
						angleAxis = inputAxis;
						Rotate ();
					} else {
						zoomAxis = inputAxis;
						Zoom ();
					}
				}
			}
		}

		void Start_Tracking (bool flag)
		{ // Called from "Turret_Horizontal".
			if (isCurrent && inputType == 6 && mode == 0) { // Off >>
				mode = 1; // >> Small window
				Change_Mode ();
			}
		}

		void Stop_Tracking ()
		{ // Called from "Turret_Horizontal".
			if (isCurrent && mode != 0) { // Small or Full screen >>
				isForced = false;
				mode = 0; // >> Off
				Change_Mode ();
			}
		}

		void Change_Mode ()
		{
			switch (mode) {
			case 0: // Off
				thisCamera.enabled = false;
				thisListener.enabled = false;
				this.tag = "Untagged";
				break;
			case 1: // Small window.
				thisCamera.rect = new Rect (0.0f, 0.0f, Small_Width, Small_Height);
				thisCamera.enabled = true;
				thisListener.enabled = false;
				this.tag = "Untagged";
				break;
			case 2: // Full screen.
				thisCamera.rect = new Rect (0.0f, 0.0f, 1.0f, 1.0f);
				thisCamera.enabled = true;
				thisListener.enabled = true;
				this.tag = "MainCamera";
				break;
			}
			// Send message to "Camera_Distance_CS" in Main_Camera.
			mainCamScript.Change_GunCam_Mode (mode);
			// Send message to "Reticle_Control_CS" in the Reticle.
			if (reticleScript) {
				reticleScript.Change_GunCam_Mode (mode, this);
			}
			// Send message to RC_Camera.
			if (rcCameraScript) {
				rcCameraScript.Change_GunCam_Mode (mode);
			}
		}

		void Zoom ()
		{
			if (zoomAxis > 0.0f) {
				thisCamera.fieldOfView *= 0.9f;
			} else if (zoomAxis < 0.0f) {
				thisCamera.fieldOfView *= 1.1f;
			}
			thisCamera.fieldOfView = Mathf.Clamp (thisCamera.fieldOfView, 0.1f, 50.0f);
		}

		void Rotate ()
		{
			angleX -= angleAxis;
			angleX = Mathf.Clamp (angleX, 0.0f, 90.0f);
			transform.localEulerAngles = new Vector3 (angleX, 0.0f, 0.0f);
		}

		void TurretBroken_Linkage ()
		{ // Called from "Damage_Control_CS" in Turret.
			if (isCurrent) {
				mode = 0; // Off
				Change_Mode ();
			}
			Destroy (this.gameObject);
		}

		void Get_Tank_ID_Control (Tank_ID_Control_CS topScript)
		{
			inputType = topScript.Input_Type;
			myID = topScript.Tank_ID;

			mainCamScript = topScript.Stored_TankProp.bodyTransform.GetComponentInChildren <Camera_Distance_CS> ();
			topScript.Stored_TankProp.bodyTransform.BroadcastMessage ("Get_GunCamera", thisCamera, SendMessageOptions.DontRequireReceiver);
		}

		void Receive_Current_ID (int id)
		{
			if (id == myID) {
				isCurrent = true;
			} else if (isCurrent) {
				isCurrent = false;
				isForced = false;
				mode = 0; // Off
				Change_Mode ();
			}
		}

		public int Get_RC_Camera_Script (RC_Camera_CS tempScript)
		{ // Called from RC_Camera.
			rcCameraScript = tempScript;
			return mode;
		}

		void Prepare_ReSpawn () {
			// Called form "Tank_ID_Control_CS".
			mode = 0; // Off
			Change_Mode ();
		}

	}

}