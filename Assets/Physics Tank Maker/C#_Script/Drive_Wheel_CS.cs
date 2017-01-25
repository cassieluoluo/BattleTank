using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Drive_Wheel_CS : MonoBehaviour
	{
	
		public bool Drive_Flag = true;
		public float Radius = 0.3f;
		public bool Use_BrakeTurn = true;

		Rigidbody thisRigidbody;
		Transform thisTransform;
		bool isLeft; // Left = true.
		float maxAngVelocity;
		float brakeMultiplier = 1.0f;

		Quaternion storedRot;
		bool isFixed = false;

		Drive_Control_CS driveControlScript;

		void Awake ()
		{
			thisRigidbody = GetComponent < Rigidbody > ();
			thisTransform = transform;
			// Set isLeft
			if (thisTransform.localEulerAngles.z == 0.0f) {
				isLeft = true; // Left
			} else {
				isLeft = false; // Right
			}
			// Set brakeMultiplier.
			if (Use_BrakeTurn) {
				brakeMultiplier = 1.0f;
			} else {
				brakeMultiplier = 0.0f;
			}
		}

		void Update ()
		{ // only for wheels with Physics_Track.
			if (driveControlScript.Fix_Useless_Rotaion) {
				Fix_Rotaion ();
			}
		}

		void Fix_Rotaion ()
		{ // only for wheels with Physics_Track.
			if (driveControlScript.Parking_Brake) {
				if (isFixed == false) {
					isFixed = true;
					storedRot = thisTransform.localRotation;
				}
			} else {
				isFixed = false;
				return;
			}
			if (isFixed) {
				thisTransform.localRotation = storedRot;
			}
		}

		void FixedUpdate ()
		{
			if (driveControlScript.Acceleration_Flag) {
				Acceleration_Mode ();
			} else {
				Constant_Mode ();
			}
		}

		void Acceleration_Mode ()
		{
			if (isLeft) { // Left
				// Set Max Angular Velocity.
				thisRigidbody.maxAngularVelocity = maxAngVelocity * driveControlScript.L_Speed_Rate;
				// Set Angular Drag.
				thisRigidbody.angularDrag = driveControlScript.L_Brake_Drag * brakeMultiplier;
				// Add Torque.
				if (Drive_Flag) {
					if (driveControlScript.Is_Forward_L) { // Forward.
						thisRigidbody.AddRelativeTorque (0.0f, -driveControlScript.Torque, 0.0f);
					} else { // Backward.
						thisRigidbody.AddRelativeTorque (0.0f, driveControlScript.Torque, 0.0f);
					}
				}
			} else { // Right
				// Set Max Angular Velocity.
				thisRigidbody.maxAngularVelocity = maxAngVelocity * driveControlScript.R_Speed_Rate;
				// Set Angular Drag.
				thisRigidbody.angularDrag = driveControlScript.R_Brake_Drag * brakeMultiplier;
				// Add Torque.
				if (Drive_Flag) {
					if (driveControlScript.Is_Forward_R) { // Forward.
						thisRigidbody.AddRelativeTorque (0.0f, driveControlScript.Torque, 0.0f);
					} else { // Backward.
						thisRigidbody.AddRelativeTorque (0.0f, -driveControlScript.Torque, 0.0f);
					}
				}
			}
		}

		void Constant_Mode ()
		{
			if (isLeft) { // Left
				// Set Max Angular Velocity.
				thisRigidbody.maxAngularVelocity = Mathf.Abs (maxAngVelocity * driveControlScript.L_Input_Rate);
				// Set Angular Drag.
				thisRigidbody.angularDrag = driveControlScript.L_Brake_Drag * brakeMultiplier;
				// Add Torque.
				if (Drive_Flag) {
					thisRigidbody.AddRelativeTorque (0.0f, driveControlScript.Torque * Mathf.Sign (driveControlScript.L_Input_Rate), 0.0f);
				}
			} else { // Right
				// Set Max Angular Velocity.
				thisRigidbody.maxAngularVelocity = Mathf.Abs (maxAngVelocity * driveControlScript.R_Input_Rate);
				// Set Angular Drag.
				thisRigidbody.angularDrag = driveControlScript.R_Brake_Drag * brakeMultiplier;
				// Add Torque.
				if (Drive_Flag) {
					thisRigidbody.AddRelativeTorque (0.0f, driveControlScript.Torque * Mathf.Sign (driveControlScript.R_Input_Rate), 0.0f);
				}
			}
		}

		void TrackBroken_Linkage (int direction)
		{ // Called from "Damage_Control_CS" in Physics_Track, TrackCollider.
			if ((isLeft && direction == 0) || (isLeft == false && direction == 1)) {
				// Lock the wheels.
				thisRigidbody.angularDrag = Mathf.Infinity;
				// Resize SphereCollider.
				MeshFilter meshFilter = GetComponent < MeshFilter > ();
				if (meshFilter && meshFilter.mesh) {
					SphereCollider sphereCollider = GetComponent <SphereCollider> ();
					if (sphereCollider) {
						sphereCollider.radius = meshFilter.mesh.bounds.extents.x;
					}
				}
				// Remove this script.
				Destroy (this);
			}
		}

		void MainBodyBroken_Linkage ()
		{ // Called from "Damage_Control_CS" in MainBody.
			// Lock the wheels.
			thisRigidbody.angularDrag = Mathf.Infinity;
			// Disable this script. (Don't destroy this. The tracks might still be alive.)
			this.enabled = false ;
		}

		void Get_Drive_Control (Drive_Control_CS tempScript)
		{ // Called from "Drive_Control_CS".
			driveControlScript = tempScript;
			maxAngVelocity = Mathf.Deg2Rad * ((driveControlScript.Max_Speed / (2.0f * Radius * Mathf.PI)) * 360.0f);
			maxAngVelocity = Mathf.Clamp (maxAngVelocity, 0.0f, driveControlScript.MaxAngVelocity_Limit); // To solve physics issues in the default physics quality.
		}
	
	}

}