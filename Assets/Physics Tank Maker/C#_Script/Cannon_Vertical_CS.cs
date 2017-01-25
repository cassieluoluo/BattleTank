using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Cannon_Vertical_CS : MonoBehaviour
	{

		public float Max_Elevation = 13.0f;
		public float Max_Depression = 7.0f;
		public float Speed_Mag = 5.0f;
		public float Acceleration_Time = 0.1f;
		public float Deceleration_Time = 0.1f;
		public bool Auto_Angle_Flag = true;
		public bool Upper_Course = false;
		public float OpenFire_Angle = 180.0f;

		public float Current_Rate; // Referred to from "Sound_Control_CS".
		public bool OpenFire_Flag; // Referred to from "Cannon_Fire".
		public bool Is_Moving = false; // Referred to from "Sound_Control_CS".
		bool isTracking = false;
		bool isAutoAngle;
		float angX;
		float grabity;
		Transform thisTransform;
		Transform turretBaseTransform;
		Turret_Horizontal_CS turretScript;
		Bullet_Generator_CS generatorScript;

		bool isCurrent;
		int myID;
		int inputType = 4;

		void Awake ()
		{
			thisTransform = transform;
		}

		void Start ()
		{ // After changing the hierarchy.
			turretBaseTransform = transform.parent;
			turretScript = turretBaseTransform.GetComponent < Turret_Horizontal_CS > ();
			generatorScript = GetComponentInChildren < Bullet_Generator_CS > ();
			angX = thisTransform.localEulerAngles.x;
			Max_Elevation = angX - Max_Elevation;
			Max_Depression = angX + Max_Depression;
			grabity = Physics.gravity.y;
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

		void KeyBoard_Input ()
		{
			if (Input.GetKey ("z")) {
				Rotate (-Input.GetAxis ("Vertical"));
			}
		}

		void Stick_Input ()
		{
			if (Input.GetButton ("L_Button")) {
				Rotate (-Input.GetAxis ("Vertical2"));
			}
		}

		void Trigger_Input ()
		{
			if (Input.GetButton ("Fire1") == false && Input.GetButton ("Jump") == false && Input.GetAxis ("Vertical") != 0) {
				Rotate (-Input.GetAxis ("Vertical"));
			}
		}

		void Rotate (float rate)
		{
			angX += Speed_Mag * rate * Time.deltaTime;
			angX = Mathf.Clamp (angX, Max_Elevation, Max_Depression);
			thisTransform.localRotation = Quaternion.Euler (new Vector3 (angX, 0.0f, 0.0f));
		}

		void Start_Tracking (bool flag)
		{ // Called from "Turret_Horizontal".
			isTracking = true;
			Is_Moving = true;
			if (flag && Auto_Angle_Flag) {
				isAutoAngle = true;
			} else {
				isAutoAngle = false;
			}
		}

		void Stop_Tracking ()
		{ // Called from "Turret_Horizontal".
			isTracking = false;
		}

		float previousRate;
		void Auto_Turn ()
		{
			float targetAng;
			if (isTracking) {
				// Calculate Angle.
				if (isAutoAngle) {
					targetAng = Auto_Angle ();
				} else {
					targetAng = Manual_Angle ();
				}
				targetAng += Mathf.DeltaAngle (0.0f, angX) + turretScript.Adjust_Ang.y;
			} else { //Return to the initial position.
				targetAng = -Mathf.DeltaAngle (angX, 0.0f); 
				if (Mathf.Abs (targetAng) < 0.01f) {
					Is_Moving = false;
				}
			}
			float sign = Mathf.Sign (targetAng);
			targetAng = Mathf.Abs (targetAng);
			// Calculate Turn Rate.
			float currentSlowdownAng = Mathf.Abs (Speed_Mag * previousRate) * Deceleration_Time;
			float targetRate = -Mathf.Lerp (0.0f, 1.0f, targetAng / (Speed_Mag * Time.fixedDeltaTime + currentSlowdownAng)) * sign;
			if (targetAng > currentSlowdownAng) {
				Current_Rate = Mathf.MoveTowards (Current_Rate, targetRate, Time.fixedDeltaTime / Acceleration_Time);
			} else {
				Current_Rate = Mathf.MoveTowards (Current_Rate, targetRate, Time.fixedDeltaTime / Deceleration_Time);
			}
			angX += Speed_Mag * Current_Rate * Time.fixedDeltaTime;
			previousRate = Current_Rate;
			// Rotate
			//angX += Speed_Mag * Current_Rate * Time.fixedDeltaTime;
			angX = Mathf.Clamp (angX, Max_Elevation, Max_Depression);
			thisTransform.localRotation = Quaternion.Euler (new Vector3 (angX, 0.0f, 0.0f));
			// Set OpenFire_Flag.
			if (targetAng <= OpenFire_Angle) {
				OpenFire_Flag = true; // Referred to from "Cannon_Fire".
			} else {
				OpenFire_Flag = false; // Referred to from "Cannon_Fire".
			}
		}

		float Auto_Angle ()
		{ // Calculate the proper angle.
			float properAng;
			float distX = Vector2.Distance (new Vector2 (turretScript.Target_Pos.x, turretScript.Target_Pos.z), new Vector2 (thisTransform.position.x, thisTransform.position.z));
			float distY = turretScript.Target_Pos.y - thisTransform.position.y;
			float posBase = (grabity * Mathf.Pow (distX, 2.0f)) / (2.0f * Mathf.Pow (generatorScript.Bullet_Velocity, 2.0f));
			float posX = distX / posBase;
			float posY = (Mathf.Pow (posX, 2.0f) / 4.0f) - ((posBase - distY) / posBase);
			if (posY >= 0.0f) {
				if (Upper_Course) {
					properAng = Mathf.Rad2Deg * Mathf.Atan (-posX / 2.0f + Mathf.Pow (posY, 0.5f));
				} else {
					properAng = Mathf.Rad2Deg * Mathf.Atan (-posX / 2.0f - Mathf.Pow (posY, 0.5f));
				}
			} else {
				properAng = 45.0f;
			}
			Vector3 forwardPos = turretBaseTransform.forward;
			properAng -= Mathf.Rad2Deg * Mathf.Atan (forwardPos.y / Vector2.Distance (Vector2.zero, new Vector2 (forwardPos.x, forwardPos.z)));
			return properAng;
		}

		float Manual_Angle ()
		{ // Simply look to the target.
			float directAng;
			Vector3 localPos = turretBaseTransform.InverseTransformPoint (turretScript.Target_Pos);
			directAng = Mathf.Rad2Deg * (Mathf.Asin ((localPos.y - thisTransform.localPosition.y) / Vector3.Distance (thisTransform.localPosition, localPos)));
			return directAng;
		}

		void TurretBroken_Linkage ()
		{ // Called from "Damage_Control_CS" in Turret.
			thisTransform.localEulerAngles = new Vector3 (Max_Depression, 0.0f, 0.0f); // Depress the cannon.
			Destroy (this);
		}

		void Get_Tank_ID_Control (Tank_ID_Control_CS topScript)
		{
			inputType = topScript.Input_Type;
			myID = topScript.Tank_ID;

			if (topScript.Stored_TankProp.aiScript) { // AI tank.
				Auto_Angle_Flag = true;
				OpenFire_Angle = topScript.Stored_TankProp.aiScript.Fire_Angle;
			}
		}

		void Receive_Current_ID (int id)
		{
			if (id == myID) {
				isCurrent = true;
			} else {
				isCurrent = false;
			}
		}

	}

}