using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Steer_Wheel_CS : MonoBehaviour
	{

		public float Reverse = 1.0f;
		public float Max_Angle = 35.0f;
		public float Rotation_Speed = 45.0f;
	
		float horizontal;
		float currentAng;
	
		HingeJoint thisHingeJoint;
		JointSpring jointSpring;

		bool isCurrent;
		int myID;
		int inputType = 4;

		AI_CS aiScript;
		AI_Semi_CS aiSemiScript;
		Drive_Control_CS controlScript;

		void Awake ()
		{
			thisHingeJoint = GetComponent < HingeJoint > ();
			jointSpring = thisHingeJoint.spring;
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
					Mouse_Input ();
					break;
				case 6:
					AI_Semi_Input ();
					break;
				case 10:
					AI_Input ();
					break;
				}
			}
		}

		void KeyBoard_Input ()
		{
			if (Input.GetKey ("z") == false && Input.GetKey ("c") == false) {
				Basic_Input ();
			}
		}

		void Stick_Input ()
		{
			if (Input.GetButton ("Jump") == false) {
				Basic_Input ();
			}
		}

		void Trigger_Input ()
		{
			float leftAxis = Input.GetAxis ("L_Trigger") - Input.GetAxis ("L_Button");
			float rightAxis = Input.GetAxis ("R_Trigger") - Input.GetAxis ("R_Button");
			if (leftAxis > 0.0f || rightAxis > 0.0f) {
				horizontal = leftAxis - rightAxis;
				if (horizontal > 1.0f) {
					horizontal = 1.0f;
				} else if (horizontal < -1.0f) {
					horizontal = -1.0f;
				}
				Steer ();
			} else if (leftAxis < 0.0f || rightAxis < 0.0f) {
				horizontal = rightAxis - leftAxis;
				if (horizontal > 1.0f) {
					horizontal = 1.0f;
				} else if (horizontal < -1.0f) {
					horizontal = -1.0f;
				}
				Steer ();
			} 
		}

		void Stick_Trigger_Input ()
		{
			if (Input.GetButton ("Jump") == false) {
				horizontal = Input.GetAxis ("Horizontal");
				if (Input.GetAxis ("R_Trigger") != 0.0f || Input.GetAxis ("L_Trigger") != 0.0f || horizontal != 0.0f) {
					Steer ();
				}
			}
		}

		void Mouse_Input ()
		{
			if (Input.GetKey ("e")) {
				horizontal = 0.3f;
			} else if (Input.GetKey ("d")) {
				horizontal = 1.0f;
			} else if (Input.GetKey ("q")) {
				horizontal = -0.3f;
			} else if (Input.GetKey ("a")) {
				horizontal = -1.0f;
			} else {
				horizontal = 0.0f;
			}
			Steer ();
		}

		void Basic_Input ()
		{
			horizontal = Input.GetAxis ("Horizontal");
			if (horizontal != 0.0f) {
				Steer ();
			}
		}

		void AI_Input ()
		{
			horizontal = aiScript.Turn_Order;
			Steer ();
		}

		void AI_Semi_Input ()
		{ // Called from "AI_Semi_CS".
			horizontal = aiSemiScript.Turn_Order;
			Steer ();
		}

		void Steer ()
		{
			if (controlScript.Stop_Flag) {
				return; // No steer
			}
			// Steer
			float targetAng = Max_Angle * horizontal;
			currentAng = Mathf.MoveTowardsAngle (currentAng, targetAng, Rotation_Speed * Time.deltaTime);
			#if UNITY_5_0 || UNITY_5_1
			jointSpring.targetPosition = -currentAng * Reverse ;
			#else
			jointSpring.targetPosition = currentAng * Reverse;
			#endif
			thisHingeJoint.spring = jointSpring;
		}

		void MainBodyBroken_Linkage ()
		{ // Called from "Damage_Control_CS" in MainBody.
			Destroy (this);
		}

		void Get_Tank_ID_Control (Tank_ID_Control_CS topScript)
		{
			inputType = topScript.Input_Type;
			myID = topScript.Tank_ID;

			aiScript = topScript.Stored_TankProp.aiScript;
		}

		void Receive_Current_ID (int id)
		{
			if (id == myID) {
				isCurrent = true;
			} else {
				if (inputType == 10) { // AI
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

		void Get_Drive_Control (Drive_Control_CS tempScript)
		{ // Called from "Drive_Control_CS".
			controlScript = tempScript;
		}
	
	}

}