using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Drive_Control_CS : MonoBehaviour
	{
	
		public float Torque = 2000.0f;
		public float Max_Speed = 8.0f;
		public float Turn_Brake_Drag = 150.0f;
		public float MaxAngVelocity_Limit = 45.0f;
		public float Pivot_Turn_Rate = 0.3f;

		public bool Acceleration_Flag = false;
		public float Acceleration_Time = 4.0f;
		public float Deceleration_Time = 0.1f;
		public float Lost_Brake_Rate = 0.85f;
		public float Lost_Speed_Rate = 0.3f;

		public bool Torque_Limitter = false;
		public float Max_Slope_Angle = 45.0f;

		public float ParkingBrake_Velocity = 0.5f;
		public float ParkingBrake_Lag = 0.5f;
		public bool Fix_Useless_Rotaion = false;

		// Referred to from Drive_Wheel.
		public bool Parking_Brake = false;
		public float L_Input_Rate;
		public float R_Input_Rate;
		public float L_Brake_Drag;
		public float R_Brake_Drag;
		public float L_Speed_Rate;
		public float R_Speed_Rate;
		public bool Is_Forward_L;
		public bool Is_Forward_R;

		// Referred to from Steer_Wheel.
		public bool Stop_Flag = true;
	
		int turnType = 0;
		float vertical;
		float horizontal;
		int speedStep;
		int turnStep;
		float defaultTorque;
		float acceleRate;
		float deceleRate;
		float rightForwardRate;
		float rightBackwardRate;
		float leftForwardRate;
		float leftBackwardRate;
		float lagCount;
		Rigidbody bodyRigidbody;
		Transform thisTransform;
	
		bool isCurrent;
		int myID;
		int inputType = 4;
	
		AI_CS aiScript;
		AI_Semi_CS aiSemiScript;

		void Awake ()
		{
			thisTransform = transform;
			defaultTorque = Torque;
			if (Acceleration_Flag) {
				acceleRate = 1.0f / Acceleration_Time;
				deceleRate = 1.0f / Deceleration_Time;
			}
			// Send this reference to "Drive_Wheel_CS", "Steer_Wheel_CS", "AI_CS", "AI_Semi_CS", "Sound_Control_CS".
			BroadcastMessage ("Get_Drive_Control", this, SendMessageOptions.DontRequireReceiver);
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
					Stick_Trigger_Input ();
					break;
				case 4:
					Mouse_Input ();
					break;
				case 5:
					Mouse_Input_Easy ();
					break;
				case 6:
					AI_Semi_Input ();
					break;
				case 10:
					AI_Input ();
					break;
				}
			}
			// Calculate Acceleration.
			if (Acceleration_Flag) {
				Acceleration ();
			}
			// Limit the Torque in slope.
			if (Torque_Limitter) {
				Limit_Torque ();
			}
		}

		void FixedUpdate ()
		{
			// Parking Brake Control.
			if (Stop_Flag) {
				float currentVelocity = bodyRigidbody.velocity.magnitude;
				float currentAngularVelocity = bodyRigidbody.angularVelocity.magnitude;
				if (Parking_Brake == false) {
					if (currentVelocity < ParkingBrake_Velocity && currentAngularVelocity < ParkingBrake_Velocity) {
						lagCount += Time.fixedDeltaTime;
						if (lagCount > ParkingBrake_Lag) {
							Parking_Brake = true;
							bodyRigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationY;
						}
					}
				} else {
					if (currentVelocity > ParkingBrake_Velocity || currentAngularVelocity > ParkingBrake_Velocity) {
						Parking_Brake = false;
						bodyRigidbody.constraints = RigidbodyConstraints.None;
						lagCount = 0.0f;
					}
				}
			} else {
				Parking_Brake = false;
				bodyRigidbody.constraints = RigidbodyConstraints.None;
				lagCount = 0.0f;
			}
		}

		void Acceleration ()
		{
			// Switch Rate.
			if (Is_Forward_L) {
				L_Speed_Rate = leftForwardRate;
			} else {
				L_Speed_Rate = leftBackwardRate;
			}
			if (Is_Forward_R) {
				R_Speed_Rate = rightForwardRate;
			} else {
				R_Speed_Rate = rightBackwardRate;
			}
			// Left
			if (L_Input_Rate < 0.0f) { // Forward
				if (leftBackwardRate == 0.0f) {
					leftForwardRate = Calculate_Speed_Rate (leftForwardRate, L_Input_Rate);
					Is_Forward_L = true;
				} else {
					leftBackwardRate = Calculate_Speed_Rate (leftBackwardRate, 0.0f);
					Is_Forward_L = false;
				}
			} else if (L_Input_Rate > 0.0f) { // Backward
				if (leftForwardRate == 0.0f) {
					leftBackwardRate = Calculate_Speed_Rate (leftBackwardRate, L_Input_Rate);
					Is_Forward_L = false;
				} else {
					leftForwardRate = Calculate_Speed_Rate (leftForwardRate, 0.0f);
					Is_Forward_L = true;
				}
			} else { // To stop ( L_Input_Rate == 0 ).
				if (leftBackwardRate != 0.0f) {
					leftBackwardRate = Calculate_Speed_Rate (leftBackwardRate, 0.0f);
				}
				if (leftForwardRate != 0.0f) {
					leftForwardRate = Calculate_Speed_Rate (leftForwardRate, 0.0f);
				}
			}
			// Right
			if (R_Input_Rate > 0.0f) { // Forward
				if (rightBackwardRate == 0.0f) {
					rightForwardRate = Calculate_Speed_Rate (rightForwardRate, R_Input_Rate);
					Is_Forward_R = true;
				} else {
					rightBackwardRate = Calculate_Speed_Rate (rightBackwardRate, 0.0f);
					Is_Forward_R = false;
				}
			} else if (R_Input_Rate < 0.0f) { // Backward
				if (rightForwardRate == 0.0f) {
					rightBackwardRate = Calculate_Speed_Rate (rightBackwardRate, R_Input_Rate);
					Is_Forward_R = false;
				} else {
					rightForwardRate = Calculate_Speed_Rate (rightForwardRate, 0.0f);
					Is_Forward_R = true;
				}
			} else { // To stop ( R_Input_Rate == 0 ).
				if (rightBackwardRate != 0.0f) {
					rightBackwardRate = Calculate_Speed_Rate (rightBackwardRate, 0.0f);
				}
				if (rightForwardRate != 0.0f) {
					rightForwardRate = Calculate_Speed_Rate (rightForwardRate, 0.0f);
				}
			}
			Stabilize ();
		}

		void Stabilize ()
		{
			if (Mathf.Abs (L_Input_Rate + R_Input_Rate) < 0.1f && L_Speed_Rate != R_Speed_Rate) { // Difference is almost zero, and Speed_Rates are different for left and right.
				float middleRate = (leftForwardRate + rightForwardRate) / 2.0f;
				leftForwardRate = middleRate;
				rightForwardRate = middleRate;
				middleRate = (leftBackwardRate + rightBackwardRate) / 2.0f;
				leftBackwardRate = middleRate;
				rightBackwardRate = middleRate;
			}
		}

		float Calculate_Speed_Rate (float currentRate, float inputRate)
		{
			float Target_Rate = Mathf.Abs (inputRate);
			if (currentRate < Target_Rate) {
				currentRate = Mathf.MoveTowards (currentRate, Target_Rate, acceleRate * Time.deltaTime);
			} else if (currentRate > Target_Rate) {
				currentRate = Mathf.MoveTowards (currentRate, Target_Rate, deceleRate * Time.deltaTime);
			}
			return currentRate;
		}

		void Limit_Torque ()
		{
			float torqueRate = Mathf.DeltaAngle (thisTransform.eulerAngles.x, 0.0f) / Max_Slope_Angle;
			if (Is_Forward_L && Is_Forward_R) {
				Torque = Mathf.Lerp (defaultTorque, 0.0f, torqueRate);
			} else { // backward
				Torque = Mathf.Lerp (defaultTorque, 0.0f, -torqueRate);
			}
			if (Torque == 0.0f) {
				L_Input_Rate = 0.0f;
				R_Input_Rate = 0.0f;
				leftForwardRate = 0.0f;
				rightForwardRate = 0.0f;
				leftBackwardRate = 0.0f;
				rightBackwardRate = 0.0f;
			}
		}

		void KeyBoard_Input ()
		{
			if (Input.GetKey ("z") == false && Input.GetKey ("f") == false) {
				vertical = Input.GetAxis ("Vertical");
				horizontal = Input.GetAxis ("Horizontal");
				Basic_Drive ();
			} else {
				Stop_Flag = true;
				L_Input_Rate = 0.0f;
				R_Input_Rate = 0.0f;
			}
		}

		void Stick_Input ()
		{
			if (Input.GetButton ("Jump") == false) {
				vertical = Input.GetAxis ("Vertical");
				horizontal = Input.GetAxis ("Horizontal");
				Basic_Drive ();
			} else {
				Stop_Flag = true;
				L_Input_Rate = 0.0f;
				R_Input_Rate = 0.0f;
			}
		}

		void Basic_Drive ()
		{
			if (vertical == 0.0f && horizontal == 0.0f) {
				Stop_Flag = true;
				L_Input_Rate = 0.0f;
				R_Input_Rate = 0.0f;
			} else {
				Stop_Flag = false;
				switch (turnType) {
				case 0:
					Easy_Turn ();
					break;
				case 1:
					Classic_Turn ();
					break;
				}
			}
		}

		void Easy_Turn ()
		{
			if (vertical == 0.0f) { // Pivot Turn
				L_Input_Rate = -horizontal * Pivot_Turn_Rate;
				R_Input_Rate = -horizontal * Pivot_Turn_Rate;
				L_Brake_Drag = 0.0f;
				R_Brake_Drag = 0.0f;
			} else { // Brake Turn
				L_Input_Rate = -vertical;
				R_Input_Rate = vertical;
				if (horizontal < 0.0f) {
					L_Brake_Drag = -horizontal * Turn_Brake_Drag;
					R_Brake_Drag = 0.0f;
				} else if (horizontal > 0.0f) {
					L_Brake_Drag = 0.0f;
					R_Brake_Drag = horizontal * Turn_Brake_Drag;
				} else { // No Turn
					L_Brake_Drag = 0.0f;
					R_Brake_Drag = 0.0f;
				}
			}
		}

		void Classic_Turn ()
		{
			L_Input_Rate = -vertical;
			R_Input_Rate = vertical;
			if (horizontal < 0.0f) {
				L_Brake_Drag = -horizontal * Turn_Brake_Drag;
				R_Brake_Drag = 0.0f;
			} else if (horizontal > 0.0f) {
				L_Brake_Drag = 0.0f;
				R_Brake_Drag = horizontal * Turn_Brake_Drag;
			} else {
				L_Brake_Drag = 0.0f;
				R_Brake_Drag = 0.0f;
			}
		}

		void Trigger_Input ()
		{
			float leftTrigger = -Input.GetAxis ("L_Trigger");
			float rightTrigger = Input.GetAxis ("R_Trigger");
			float leftButton = +Input.GetAxis ("L_Button");
			float rightButton = -Input.GetAxis ("R_Button");
			if (leftTrigger >= 0.0f && rightTrigger <= 0.0f && leftButton <= 0.0f && rightButton >= 0.0f) {
				Stop_Flag = true;
				L_Input_Rate = 0.0f;
				R_Input_Rate = 0.0f;
				L_Brake_Drag = 0.0f;
				R_Brake_Drag = 0.0f;
			} else {
				Stop_Flag = false;
				if (leftButton > 0.0f && leftTrigger >= 0.0f) {
					L_Input_Rate = leftButton;
					L_Brake_Drag = (1.0f - leftButton) * Turn_Brake_Drag;
				} else if (leftTrigger < 0.0f && leftButton == 0.0f) {
					L_Input_Rate = -1.0f;
					L_Brake_Drag = (1.0f + leftTrigger) * Turn_Brake_Drag;
				} else {
					if (R_Input_Rate != 0.0f) {
						L_Input_Rate = R_Input_Rate;
					} else {
						L_Input_Rate = 0.0f;
					}
					L_Brake_Drag = Turn_Brake_Drag;
				}
				if (rightButton < 0.0f && rightTrigger <= 0.0f) {
					R_Input_Rate = rightButton;
					R_Brake_Drag = (1.0f + rightButton) * Turn_Brake_Drag;
				} else if (rightTrigger > 0.0f && rightButton == 0.0f) {
					R_Input_Rate = 1.0f;
					R_Brake_Drag = (1.0f - rightTrigger) * Turn_Brake_Drag;
				} else {
					if (L_Input_Rate != 0.0f) {
						R_Input_Rate = L_Input_Rate;
					} else {
						R_Input_Rate = 0.0f;
					}
					R_Brake_Drag = Turn_Brake_Drag;
				}
			}
		}

		void Stick_Trigger_Input ()
		{
			if (Input.GetButton ("Jump") == false) {
				vertical = Input.GetAxis ("R_Trigger") - Input.GetAxis ("L_Trigger");
				horizontal = Input.GetAxis ("Horizontal");
				if (vertical == 0.0f) {
					if (horizontal == 0.0f) {
						Stop_Flag = true;
						L_Input_Rate = 0.0f;
						R_Input_Rate = 0.0f;
						L_Brake_Drag = 0.0f;
						R_Brake_Drag = 0.0f;
						return;
					} else {
						Stop_Flag = false;
						L_Input_Rate = -horizontal;
						R_Input_Rate = -horizontal;
						L_Brake_Drag = 0.0f;
						R_Brake_Drag = 0.0f;
						return;
					}
				} else {
					Stop_Flag = false;
					L_Input_Rate = -vertical;
					R_Input_Rate = vertical;
					if (horizontal < 0.0f) {
						L_Brake_Drag = -horizontal * Turn_Brake_Drag;
						R_Brake_Drag = 0.0f;
					} else if (horizontal > 0.0f) {
						L_Brake_Drag = 0.0f;
						R_Brake_Drag = horizontal * Turn_Brake_Drag;
					} else {
						L_Brake_Drag = 0.0f;
						R_Brake_Drag = 0.0f;
					}
				}
			} else {
				Stop_Flag = true;
				L_Input_Rate = 0.0f;
				R_Input_Rate = 0.0f;
				L_Brake_Drag = 0.0f;
				R_Brake_Drag = 0.0f;
			}
		}

		void Mouse_Input ()
		{
			if (Input.anyKey) {
				// Input Speed
				if (Input.GetKeyDown ("w")) {
					speedStep += 1;
				} else if (Input.GetKeyDown ("s")) {
					speedStep -= 1;
				} else if (Input.GetKey ("x")) {
					speedStep = 0;
				}
				speedStep = Mathf.Clamp (speedStep, -2, 4);
				// Input Turn
				if (Input.GetKey ("q")) { //Smooth Left Turn
					turnStep = 1;
				} else if (Input.GetKey ("e")) { //Smooth Right Turn
					turnStep = 2;
				} else if (Input.GetKey ("a")) { // Brake Left Turn
					if (speedStep != 0) {
						turnStep = 3;
					} else {
						turnStep = 5;
					}
				} else if (Input.GetKey ("d")) { //Brake Right Turn
					if (speedStep != 0) {
						turnStep = 4;
					} else {
						turnStep = 6;
					}
				} else { // No Turn
					turnStep = 0;
				}
			} else {
				turnStep = 0;
			}
			Rate_Control ();
		}

		void Rate_Control ()
		{
			if (speedStep == 0 && turnStep == 0) {
				Stop_Flag = true;
				L_Input_Rate = 0.0f;
				R_Input_Rate = 0.0f;
				L_Brake_Drag = Turn_Brake_Drag;
				R_Brake_Drag = Turn_Brake_Drag;
				leftForwardRate = 0.0f;
				rightForwardRate = 0.0f;
				leftBackwardRate = 0.0f;
				rightBackwardRate = 0.0f;
				return;
			} else {
				Stop_Flag = false;
				switch (turnStep) {
				case 1: // Smooth Left Turn
					L_Input_Rate = -speedStep / 4.0f;
					R_Input_Rate = speedStep / 4.0f;
					L_Brake_Drag = 0.15f * Turn_Brake_Drag;
					R_Brake_Drag = 0.0f;
					break;
				case 2: // Smooth Right Turn
					L_Input_Rate = -speedStep / 4.0f;
					R_Input_Rate = speedStep / 4.0f;
					L_Brake_Drag = 0.0f;
					R_Brake_Drag = 0.15f * Turn_Brake_Drag;
					break;
				case 3: // Brake Left Turn
					L_Input_Rate = -speedStep / 4.0f;
					R_Input_Rate = speedStep / 4.0f;
					L_Brake_Drag = (-Lost_Brake_Rate * L_Speed_Rate + 1.0f) * Turn_Brake_Drag;
					R_Brake_Drag = 0.0f;
					if (Is_Forward_L) {
						float lostRate = leftForwardRate - ((leftForwardRate * Lost_Speed_Rate) * Time.deltaTime);
						leftForwardRate = lostRate;
						rightForwardRate = lostRate;
					} else {
						float lostRate = leftBackwardRate - ((leftBackwardRate * Lost_Speed_Rate) * Time.deltaTime);
						leftBackwardRate = lostRate;
						rightBackwardRate = lostRate;
					}
					break;
				case 4: // Brake Right Turn
					L_Input_Rate = -speedStep / 4.0f;
					R_Input_Rate = speedStep / 4.0f;
					L_Brake_Drag = 0.0f;
					R_Brake_Drag = (-Lost_Brake_Rate * R_Speed_Rate + 1.0f) * Turn_Brake_Drag;
					if (Is_Forward_R) {
						float lostRate = rightForwardRate - ((rightForwardRate * Lost_Speed_Rate) * Time.deltaTime);
						leftForwardRate = lostRate;
						rightForwardRate = lostRate;
					} else {
						float lostRate = rightBackwardRate - ((rightBackwardRate * Lost_Speed_Rate) * Time.deltaTime);
						leftBackwardRate = lostRate;
						rightBackwardRate = lostRate;
					}
					break;
				case 5: // Pivot Left Turn
					L_Input_Rate = Pivot_Turn_Rate;
					R_Input_Rate = Pivot_Turn_Rate;
					L_Brake_Drag = 0.0f;
					R_Brake_Drag = 0.0f;
					break;
				case 6: // Pivot Right Turn
					L_Input_Rate = -Pivot_Turn_Rate;
					R_Input_Rate = -Pivot_Turn_Rate;
					L_Brake_Drag = 0.0f;
					R_Brake_Drag = 0.0f;
					break;
				default : // No Turn
					L_Input_Rate = -speedStep / 4.0f;
					R_Input_Rate = speedStep / 4.0f;
					L_Brake_Drag = 0.0f;
					R_Brake_Drag = 0.0f;
					break;
				}
			}
		}

		void Mouse_Input_Easy ()
		{
			if (Input.anyKey) {
				if (Input.GetKey ("w")) {
					vertical = 1.0f;
				} else if (Input.GetKey ("s")) {
					vertical = -1.0f;
				} else {
					vertical = 0.0f;
				}
				if (Input.GetKey ("x")) {
					L_Input_Rate = 0.0f;
					R_Input_Rate = 0.0f;
					L_Brake_Drag = Turn_Brake_Drag;
					R_Brake_Drag = Turn_Brake_Drag;
					leftForwardRate = 0.0f;
					rightForwardRate = 0.0f;
					leftBackwardRate = 0.0f;
					rightBackwardRate = 0.0f;
					return;
				} else if (Input.GetKey ("a")) {
					horizontal = -1.0f;
				} else if (Input.GetKey ("d")) {
					horizontal = 1.0f;
				} else {
					horizontal = 0.0f;
				}
			} else {
				vertical = 0.0f;
				horizontal = 0.0f;
			}
			Basic_Drive ();
		}

		void AI_Input ()
		{
			vertical = aiScript.Speed_Order;
			horizontal = aiScript.Turn_Order;
			if (vertical == 0.0f && horizontal == 0.0f) {
				Stop_Flag = true;
				L_Input_Rate = 0.0f;
				R_Input_Rate = 0.0f;
			} else {
				Stop_Flag = false;
				AI_Turn ();
			}
		}

		void AI_Semi_Input ()
		{
			vertical = aiSemiScript.Speed_Order;
			horizontal = aiSemiScript.Turn_Order;
			if (vertical == 0.0f && horizontal == 0.0f) {
				Stop_Flag = true;
				L_Input_Rate = 0.0f;
				R_Input_Rate = 0.0f;
			} else {
				Stop_Flag = false;
				AI_Turn ();
			}
		}

		void AI_Turn ()
		{
			if (vertical == 0.0f) { // Pivot Turn
				L_Input_Rate = -horizontal * Pivot_Turn_Rate;
				R_Input_Rate = -horizontal * Pivot_Turn_Rate;
				L_Brake_Drag = 0.0f;
				R_Brake_Drag = 0.0f;
			} else { // Brake Turn
				L_Input_Rate = -vertical;
				R_Input_Rate = vertical;
				if (horizontal < 0.0f) { // Left Turn.
					L_Brake_Drag = -horizontal * Turn_Brake_Drag;
					R_Brake_Drag = 0.0f;
				} else if (horizontal > 0.0f) { // Right Turn.
					L_Brake_Drag = 0.0f;
					R_Brake_Drag = horizontal * Turn_Brake_Drag;
				} else { // No Turn
					L_Brake_Drag = 0.0f;
					R_Brake_Drag = 0.0f;
				}
			}
		}

		void MainBodyBroken_Linkage ()
		{ // Called from "Damage_Control_CS" in MainBody.
			Destroy (this);
		}

		void Get_Tank_ID_Control (Tank_ID_Control_CS topScript)
		{
			inputType = topScript.Input_Type;
			myID = topScript.Tank_ID;
			turnType = topScript.Turn_Type;

			bodyRigidbody = topScript.Stored_TankProp.bodyRigidbody;
			aiScript = topScript.Stored_TankProp.aiScript;
		}

		void Receive_Current_ID (int id)
		{
			if (id == myID) {
				isCurrent = true;
			} else {
				if (inputType == 10 || inputType == 6) { // AI or AI_Semi
					isCurrent = true;
				} else {
					isCurrent = false;
				}
			}
		}

		void Get_AI_Semi (AI_Semi_CS tempScript)
		{
			aiSemiScript = tempScript;
		}
			
	}

}