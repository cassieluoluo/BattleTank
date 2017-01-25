using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{
	
	public class Turret_Horizontal_CS : MonoBehaviour
	{

		public bool Limit_Flag;
		public float Max_Right = 170.0f;
		public float Max_Left = 170.0f;
		public float Speed_Mag = 10.0f;
		public float Acceleration_Time = 0.5f;
		public float Deceleration_Time = 0.5f;
		public float OpenFire_Angle = 180.0f;
		public float Adjusting_Rate = 0.01f;
		public string Aim_Marker_Name = "Aim_Marker";


		public Vector3 Target_Pos; // Referred to from "Cannon_Vertical_CS" and "Cannon_Fire_CS".
		public Vector3 Adjust_Ang; // Referred to from "Cannon_Vertical_CS".
		public float Current_Rate; // Referred to from "Sound_Control_CS".
		public bool OpenFire_Flag = true; // Referred to from "Cannon_Fire_CS".
		public bool Is_Moving = false; // Referred to from "Sound_Control_CS".
		bool isTracking = false;
		bool isTrouble = false;
		float angY;
		Transform targetTransform;
		Rigidbody targetRigidbody;
		Vector3 targetOffset;
		Transform thisTransform;
		Transform parentTransform;
		Transform rootTransform;
		Image markerImage;
		Transform markerTransform;
		Bullet_Generator_CS generatorScript;
		Transform generatorTransform;
		AI_CS aiScript;
		Camera gunCamera;
		int mode; // 0=Keep initial positon, 1=Free aiming, 2=Lock On.
		int layerMask = ~((1 << 10) + (1 << 2)); // Layer 2 = Ignore Ray, Layer 10 = Ignore All.
		bool autoLead;

		bool isCurrent;
		int myID;
		int inputType = 4;

		void Awake ()
		{
			thisTransform = transform;
			// Find Marker Image.
			if (string.IsNullOrEmpty (Aim_Marker_Name) == false) {
				GameObject markerObject = GameObject.Find (Aim_Marker_Name);
				if (markerObject) {
					markerImage = markerObject.GetComponent < Image > ();
				}
				if (markerImage) {
					markerTransform = markerImage.transform;
				} else {
					Debug.LogWarning (Aim_Marker_Name + " cannot be found in the scene.");
				}
			}
		}

		void Start ()
		{ // After changing the hierarchy.
			parentTransform = thisTransform.parent;
			angY = thisTransform.localEulerAngles.y;
			Max_Right = angY + Max_Right;
			Max_Left = angY - Max_Left;
			generatorScript = GetComponentInChildren < Bullet_Generator_CS > ();
			generatorTransform = generatorScript.transform;
		}

		void Update ()
		{
			if (isCurrent) {
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
				}
			}
		}

		void LateUpdate ()
		{
			if (isCurrent) {
				switch (inputType) {
				case 4:
					Mouse_Input ();
					Marker_Control ();
					break;
				case 5:
					Mouse_Input ();
					Marker_Control ();
					break;
				case 6:
					Mouse_Input ();
					Marker_Control ();
					break;
				case 10:
					Marker_Control ();
					break;
				}
			}
		}

		void FixedUpdate ()
		{
			if (Is_Moving) {
				switch (inputType) {
				case 4:
					Auto_Turn ();
					break;
				case 5:
					Auto_Turn ();
					break;
				case 6:
					Auto_Turn ();
					break;
				case 10:
					Auto_Turn ();
					break;
				}
			}
		}

		void Marker_Control ()
		{
			if (markerImage) {
				switch (mode) {
				case 0: // Keep initial positon
					markerImage.enabled = false;
					return;
				case 1: // Free aiming
					markerImage.enabled = true;
					markerImage.color = Color.white;
					break;
				case 2: // Lock On
					markerImage.enabled = true;
					markerImage.color = Color.red;
					break;
				}
				Vector3 tempPos = Camera.main.WorldToScreenPoint (Target_Pos);
				if (tempPos.z < 0.0f) { // Behind of the camera.
					markerImage.enabled = false;
				} else {
					tempPos.z = 100.0f;
				}
				markerTransform.position = tempPos;
			}
		}

		void KeyBoard_Input ()
		{
			if ((Input.GetButton ("Fire1") || Input.GetKey ("z"))) {
				Rotate (Input.GetAxis ("Horizontal"));
			} else {
				Rotate (0.0f);
			}
		}

		void Stick_Input ()
		{
			if (Input.GetButton ("L_Button")) {
				Rotate (Input.GetAxis ("Horizontal2"));
			} else {
				Rotate (0.0f);
			}
		}

		void Trigger_Input ()
		{
			if (Input.GetButton ("Fire1") == false && Input.GetButton ("Jump") == false && Input.GetAxis ("Horizontal") != 0) {
				Rotate (Input.GetAxis ("Horizontal"));
			} else {
				Rotate (0.0f);
			}
		}

		void Rotate (float targetRate)
		{
			if (isTrouble == false) {
				if (targetRate != 0.0f) {
					Current_Rate = Mathf.MoveTowards (Current_Rate, targetRate, Time.deltaTime / Acceleration_Time);
				} else {
					Current_Rate = Mathf.MoveTowards (Current_Rate, targetRate, Time.deltaTime / Deceleration_Time);
				}
				angY += Speed_Mag * Current_Rate * Time.deltaTime;
				if (Limit_Flag) {
					angY = Mathf.Clamp (angY, Max_Left, Max_Right);
				}
				thisTransform.localRotation = Quaternion.Euler (new Vector3 (0.0f, angY, 0.0f));
			}
		}

		Vector3 previousMousePos;
		void Mouse_Input ()
		{
			if (Input.GetKeyDown (KeyCode.LeftShift)) {
				switch (mode) {
				case 0: // Keep initial positon >>
					mode = 1; // >>Free aimiing.
					break;
				case 1: // Free aiming >>
					mode = 0; // >> Keep initial positon.
					break;
				case 2: // Lock On >>
					mode = 1; // >>Free aimiing.
					break;
				}
				Switch_Mode ();
				return;
			} else if (Input.GetMouseButtonDown (2)) {
				Cast_Ray_LockOn (Input.mousePosition);
				return;
			}
			switch (mode) {
			case 1: // Free aiming
				Cast_Ray_Free (Input.mousePosition);
				break;
			case 2: // Lock On
			// Adjust aiming.
				if (Input.GetKeyDown (KeyCode.Space)) {
					previousMousePos = Input.mousePosition;
				} else if (Input.GetKey (KeyCode.Space)) {
					if (Input.GetMouseButton (1) == false) {
						Adjust_Ang += (Input.mousePosition - previousMousePos) * Adjusting_Rate;
						previousMousePos = Input.mousePosition;
					}
				} else if (Input.GetKeyUp (KeyCode.Space)) {
					Adjust_Ang = Vector3.zero;
				}
				break;
			}
		}

		void Switch_Mode ()
		{
			switch (mode) {
			case 0:
				isTracking = false;
				BroadcastMessage ("Stop_Tracking", SendMessageOptions.DontRequireReceiver);// Send message to "Cannon_Vertical".
				break;
			case 1:
				targetTransform = null;
				isTracking = true;
				BroadcastMessage ("Start_Tracking", false, SendMessageOptions.DontRequireReceiver);// Send message to "Cannon_Vertical".
				Is_Moving = true;
				break;
			case 2:
				Adjust_Ang = Vector3.zero;
				isTracking = true;
				BroadcastMessage ("Start_Tracking", true, SendMessageOptions.DontRequireReceiver);// Send message to "Cannon_Vertical".
				Is_Moving = true;
				break;

			}
		}

		void Cast_Ray_LockOn (Vector3 mousePos)
		{
			// Detect the camera clicked.
			Camera currentCam;
			if (gunCamera && gunCamera.enabled && gunCamera.pixelRect.Contains (mousePos)) { // Cursor is within gun camera window.
				currentCam = gunCamera;
			} else { // Cursor is out of gun camera window.
				currentCam = Camera.main;
			}
			Ray ray = currentCam.ScreenPointToRay (mousePos);
			RaycastHit raycastHit;
			if (Physics.Raycast (ray, out raycastHit, 2048.0f, layerMask)) {
				Transform colliderTransform = raycastHit.collider.transform;
				if (colliderTransform.root != rootTransform) { // Hit object is not myself.
					Rigidbody tempRigidbody = raycastHit.transform.GetComponent < Rigidbody > (); //When 'raycastHit.collider.transform' is Turret, 'raycastHit.transform' is the MainBody.
					if (tempRigidbody) {
						targetRigidbody = tempRigidbody; // Store the Rigidbody for lead-shooting.
						targetTransform = colliderTransform;
						targetOffset = targetTransform.InverseTransformPoint (raycastHit.point); // Store offset values from the center of targetTransform.
						if (targetTransform.localScale != Vector3.one) { // for Armor_Collider.
							targetOffset.x *= targetTransform.localScale.x;
							targetOffset.y *= targetTransform.localScale.y;
							targetOffset.z *= targetTransform.localScale.z;
						}
					} else { // Hit object has no Rigidbody.(It is not a tank)
						targetTransform = null;
						Target_Pos = raycastHit.point;
					}
					mode = 2; // >> Lock On
				} else { // Ray hits itself.
					mode = 0; // >> Keep initial positon.
				}
			} else { // Ray does not hit anythig.
				targetTransform = null;
				mousePos.z = 1024.0f;
				Target_Pos = currentCam.ScreenToWorldPoint (mousePos);
				mode = 2; // >> >> Lock On
			}
			Switch_Mode ();
		}

		void Cast_Ray_Free (Vector3 mousePos)
		{
			// Detect the camera that the cursor is on.
			Camera currentCam;
			if (gunCamera && gunCamera.enabled && gunCamera.pixelRect.Contains (mousePos)) { // Cursor is within gun camera window.
				currentCam = gunCamera;
			} else { // Cursor is out of gun camera window.
				currentCam = Camera.main;
			}
			mousePos.z = 1500.0f;
			Target_Pos = currentCam.ScreenToWorldPoint (mousePos);
		}

		public void AI_Set_Lock_On (Transform tempTransform)
		{ // Called from AI.
			targetTransform = tempTransform;
			targetRigidbody = targetTransform.GetComponent < Rigidbody > ();
			mode = 2;
			Switch_Mode ();
		}

		public void AI_Reset_Lock_On ()
		{ // Called from AI.
			targetTransform = null;
			mode = 0;
			Switch_Mode ();
		}

		float previousRate;
		void Auto_Turn ()
		{
			float targetAng;
			if (isTracking) {
				// Update Target position.
				if (targetTransform) {
					Target_Pos = targetTransform.position + (targetTransform.forward * targetOffset.z) + (targetTransform.right * targetOffset.x) + (targetTransform.up * targetOffset.y);
					if (targetRigidbody && autoLead) {
						Calculate_Lead ();
					}
				}
				// Calculate Angle.
				Vector3 generatorOffset = parentTransform.InverseTransformPoint (generatorTransform.position);
				Vector3 localPos = parentTransform.InverseTransformPoint (Target_Pos) - generatorOffset;
				targetAng = Vector2.Angle (Vector2.up, new Vector2 (localPos.x, localPos.z)) * Mathf.Sign (localPos.x);
				if (Limit_Flag) {
					targetAng -= angY;
				} else {
					targetAng = Mathf.DeltaAngle (angY, targetAng);
				}
				targetAng += Adjust_Ang.x;
			} else { // Return to the initial position.
				targetAng = Mathf.DeltaAngle (angY, 0.0f);
				if (Mathf.Abs (targetAng) < 0.01f) {
					Is_Moving = false;
				}
			}
			// Calculate Turn Rate.
			float sign = Mathf.Sign (targetAng);
			targetAng = Mathf.Abs (targetAng);
			float currentSlowdownAng = Mathf.Abs (Speed_Mag * previousRate) * Deceleration_Time;
			float targetRate = Mathf.Lerp (0.0f, 1.0f, targetAng / (Speed_Mag * Time.fixedDeltaTime + currentSlowdownAng)) * sign;
			if (targetAng > currentSlowdownAng) {
				Current_Rate = Mathf.MoveTowards (Current_Rate, targetRate, Time.fixedDeltaTime / Acceleration_Time);
			} else {
				Current_Rate = Mathf.MoveTowards (Current_Rate, targetRate, Time.fixedDeltaTime / Deceleration_Time);
			}
			previousRate = Current_Rate;
			// Rotate
			if (isTrouble == false) {
				angY += Speed_Mag * Current_Rate * Time.fixedDeltaTime;
				if (Limit_Flag) {
					angY = Mathf.Clamp (angY, Max_Left, Max_Right);
					if (angY <= Max_Left || angY >= Max_Right) {
						Current_Rate = 0.0f;
					}
				}
				thisTransform.localRotation = Quaternion.Euler (new Vector3 (0.0f, angY, 0.0f));
			}
			// Set OpenFire_Flag.
			if (targetAng <= OpenFire_Angle) {
				OpenFire_Flag = true; // Referred to from "Cannon_Vertical".
			} else {
				OpenFire_Flag = false; // Referred to from "Cannon_Vertical".
			}
		}

		void Calculate_Lead ()
		{
			if (inputType == 10) { // AI
				Target_Pos += targetRigidbody.velocity * (aiScript.Target_Distance / generatorScript.Bullet_Velocity);
				return;
			}
			float distance = Vector3.Distance (thisTransform.position, targetTransform.position);
			Target_Pos += targetRigidbody.velocity * (distance / generatorScript.Bullet_Velocity);
		}

		public void AI_Random_Offset ()
		{ // Called from "Cannon_Fire".
			Vector3 newOffset;
			newOffset.x = Random.Range (-1.0f, 1.0f);
			newOffset.y = Random.Range (-aiScript.AI_Lower_Offset, aiScript.AI_Upper_Offset);
			newOffset.z = Random.Range (-1.0f, 1.0f);
			targetOffset = newOffset;
		}
			
		void TurretBroken_Linkage ()
		{ // Called from "Damage_Control_CS" in Turret.
			if (isCurrent && markerImage) {
				markerImage.enabled = false;
			}
			Destroy (this);
		}

		public bool Trouble (float count)
		{ // Called from "Damage_Control_CS" in Turret.
			if (isTrouble == false) {
				isTrouble = true;
				StartCoroutine ("Trouble_Count", count);
				return true;
			} else {
				return false;
			}
		}

		IEnumerator Trouble_Count (float count)
		{
			yield return new WaitForSeconds (count);
			isTrouble = false;
		}

		void Get_Tank_ID_Control (Tank_ID_Control_CS topScript)
		{
			inputType = topScript.Input_Type;
			myID = topScript.Tank_ID;

			rootTransform = topScript.Root_Transform;
			if (inputType == 10) {
				aiScript = topScript.Stored_TankProp.aiScript;
				OpenFire_Angle = aiScript.Fire_Angle;
				autoLead = true;
			} else {
				autoLead = topScript.Auto_Lead;
			}
		}

		void Receive_Current_ID (int id)
		{
			if (id == myID) {
				isCurrent = true;
			} else if (isCurrent) {
				isCurrent = false;
				if (markerImage) {
					markerImage.enabled = false;
				}
			}
		}

		void Get_GunCamera (Camera tempCamera)
		{ // Called from "Gun_Camera_CS".
			gunCamera = tempCamera;
		}
			
	}

}