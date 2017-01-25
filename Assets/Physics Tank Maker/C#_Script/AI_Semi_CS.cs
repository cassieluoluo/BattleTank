using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{

	[ RequireComponent (typeof(UnityEngine.AI.NavMeshAgent))]
	public class AI_Semi_CS : MonoBehaviour
	{

		// Set in "AI_Semi_CS".
		public float BrakeTurn_Min_Angle = 20.0f;
		public float BrakeTurn_Max_Angle = 360.0f;
		public float Pivot_Turn_Angle = 90.0f;
		public float Min_Target_Angle = 1.0f;
		public float Min_Turn_Rate = 0.1f;
		public float Max_Turn_Rate = 1.0f;
		public float Min_Speed_Rate = 0.4f;
		public float Max_Speed_Rate = 1.0f;
		public float SlowDown_Range = 20.0f;
		public float Max_Speed_Error = 0.0f;
		public float Sky_Distance = 512.0f;
		public GameObject Marker_Prefab;

		// Referred to from "Drive_Control".
		public float Speed_Order;
		public float Turn_Order;

		Transform thisTransform;
		Transform rootTransform;
		Vector3 initialPos;
		UnityEngine.AI.NavMeshAgent thisAgent;
		Transform markerObjectTransform;
		Renderer markerRenderer;
		Camera gunCamera;
		Cannon_Fire_CS [] fireScripts;
		Rigidbody bodyRigidbody;
		float approachDist = 5.0f;
		bool isStaying = false;
		float currentDist;
		float directionFloat = 1.0f;
		float speedMultiplier = 1.0f;
		float altitude = -0.5f;
		int layerMask = ~((1 << 10) + (1 << 2)); // Layer 2 = Ignore Ray, Layer 10 = Ignore All.
		float maxSpeed;

		bool isCurrent;
		int myID;

		void Initial_Settings ()
		{ // Called after "Get_Tank_ID_Control ()" is called.
			thisTransform = transform;
			// Find "Cannon_Fire_CS".
			fireScripts = thisTransform.parent.GetComponentsInChildren <Cannon_Fire_CS> ();
			// NavMeshAgent settings.
			initialPos = thisTransform.localPosition; // to fix this object on its initial position.
			thisAgent = GetComponent <UnityEngine.AI.NavMeshAgent> ();
			// Create "Move Marker".
			Set_Marker_Object ();
			// Send this reference to "Drive_Control_CS", "Steer_Wheel_CS".
			thisTransform.parent.BroadcastMessage ("Get_AI_Semi", this, SendMessageOptions.DontRequireReceiver);
		}

		void Set_Marker_Object ()
		{
			if (Marker_Prefab) {
				GameObject markerObject = Instantiate (Marker_Prefab, thisTransform.position, Quaternion.identity) as GameObject;
				markerObjectTransform = markerObject.transform;
				markerObjectTransform.parent = thisTransform.parent.parent; // Under the top object.
				markerRenderer = markerObject.GetComponent <Renderer>();
				markerRenderer.enabled = false;
				return;
			}
		}

		void Update ()
		{
			// Fix this position and rotation.
			thisTransform.localPosition = initialPos;
			thisTransform.localRotation = Quaternion.identity;
			// Input Control.
			if (isCurrent) {
				Input_Control ();
			}
			// Movement Control.
			Move ();
		}

		void LateUpdate ()
		{
			if (isCurrent && markerRenderer) {
				Marker_Object_Control ();
			}
		}

		void Input_Control ()
		{
			// Set destination position.
			if (Input.GetMouseButton (0)) {
				Vector3 mousePos = Input.mousePosition;
				if (gunCamera.enabled && gunCamera.pixelRect.Contains (mousePos)) { // On the Gun_Camera.
					foreach (Cannon_Fire_CS fireScript in fireScripts) {
						fireScript.AI_Semi_Input (); // Fire!!
					}
					return;
				} else { // On the Main_Camera.
					Ray ray = Camera.main.ScreenPointToRay (mousePos);
					RaycastHit raycastHit;
					if (Physics.Raycast (ray, out raycastHit, 2048.0f, layerMask)) { // Ray hits something.
						Transform colliderTransform = raycastHit.collider.transform;
						if (colliderTransform.root != rootTransform) { // Hit object is not myself.
							UnityEngine.AI.NavMeshHit navMeshHit;
							if (UnityEngine.AI.NavMesh.SamplePosition (raycastHit.point, out navMeshHit, 16.0f, UnityEngine.AI.NavMesh.AllAreas)) { // Set the marker on NavMesh.
								markerObjectTransform.position = navMeshHit.position + Vector3.up * altitude;
							} else { // NavMesh cannot be found in the range.
								return;
							}
						} else { // Ray hits itself.
							foreach (Cannon_Fire_CS fireScript in fireScripts) {
								fireScript.AI_Semi_Input (); // Fire!!
							}
							return;
						}
					} else { // Ray hits nothing.
						mousePos.z = Sky_Distance;
						ray = new Ray (Camera.main.ScreenToWorldPoint (mousePos), Vector3.down); // Cast Ray below.
						if (Physics.Raycast (ray, out raycastHit, 1024.0f, layerMask)) { // Ray hits something.
							UnityEngine.AI.NavMeshHit navMeshHit;
							if (UnityEngine.AI.NavMesh.SamplePosition (raycastHit.point, out navMeshHit, 16.0f, UnityEngine.AI.NavMesh.AllAreas)) { // Set the marker on NavMesh.
								markerObjectTransform.position = navMeshHit.position + Vector3.up * altitude;
							} else { // NavMesh cannot be found in the range.
								return;
							}
						} else { // Ray hits nothing.
							return;
						}
					}
					if (Input.GetMouseButton (1)) {
						directionFloat = -1.0f; // Reverse
					} else {
						directionFloat = 1.0f; // Forward
					}
				}
			}
			if (Input.GetMouseButtonUp (0)) {
				thisAgent.SetDestination (markerObjectTransform.position);
			}
			// Set "Speed Multiplier".
			if (Input.GetMouseButton (1) == false) {
				if (Input.GetAxis ("Mouse ScrollWheel") > 0) {
					if (gunCamera.enabled && gunCamera.pixelRect.Contains (Input.mousePosition)) {
						return;
					}
					speedMultiplier += 0.25f;
					speedMultiplier = Mathf.Clamp (speedMultiplier, 0.0f, 1.0f);
				} else if (Input.GetAxis ("Mouse ScrollWheel") < 0) {
					if (gunCamera.enabled && gunCamera.pixelRect.Contains (Input.mousePosition)) {
						return;
					}
					speedMultiplier -= 0.25f;
					speedMultiplier = Mathf.Clamp (speedMultiplier, 0.0f, 1.0f);
				}
			}
		}

		void Marker_Object_Control ()
		{
			if (isStaying) {
				markerRenderer.enabled = false;
				return;
			} else {
				markerRenderer.enabled = true;
			}
		}
			
		void Move ()
		{
			Auto_Drive ();
			// Set "isStaying".
			currentDist = Vector3.Distance (thisTransform.position, markerObjectTransform.position);
			if (isStaying == false) { // not staying.
				if (currentDist < approachDist) { // near the destination
					isStaying = true;
					return;
				}
			} else { // staying now.
				if (currentDist < approachDist + 5.0f) { // almost near the destination.
					isStaying = true; // Keep staying.
				} else { // away from the destination.
					isStaying = false;
				}
			}
		}
			
		void Auto_Drive ()
		{
			// Get the next corner position.
			Vector3 nextCornerPos;
			float faceFloat;
			if (isStaying == false) {
				if (speedMultiplier == 0.0f) { // Face to the marker.
					faceFloat = directionFloat;
				} else {
					faceFloat = 1.0f;
				}
				if (thisAgent.path.corners.Length > 1) { // Usual condition.
					nextCornerPos = thisAgent.path.corners [1];
				} else { // Something wrong in the NavMeshAgent.
					return;
				}
			} else { // is staying.
				Turn_Order = 0.0f;
				Speed_Order = 0.0f;
				return;
			}
			// Calculate the angle to the next corner.
			Vector3 localPos = thisTransform.InverseTransformPoint (nextCornerPos);
			float targetAng = Vector2.Angle (new Vector2 (0.0f, directionFloat), new Vector2 (localPos.x, localPos.z)) * Mathf.Sign (localPos.x);
			// Calculate "Speed_Order" and "Turn_Order".
			float sign = Mathf.Sign (targetAng);
			targetAng = Mathf.Abs (targetAng);
			if (targetAng > Min_Target_Angle) { // Turn
				if (targetAng > Pivot_Turn_Angle) { // Pivot Turn
					Turn_Order = directionFloat * sign;
					Speed_Order = 0.0f;
					return;
				} // Brake Turn
				//float tempVelocity = bodyRigidbody.velocity.magnitude;
				//float tempBaseAngle =  Mathf.Lerp (BrakeTurn_Min_Angle, BrakeTurn_Max_Angle, Mathf.Pow (tempVelocity / maxSpeed, 2.0f));
				//float tempAngleRate = targetAng / tempBaseAngle;
				float tempAngleRate = targetAng / Mathf.Lerp (BrakeTurn_Min_Angle, BrakeTurn_Max_Angle, Mathf.Pow (bodyRigidbody.velocity.magnitude / maxSpeed, 2.0f));
				Turn_Order = Mathf.Lerp (Min_Turn_Rate, Max_Turn_Rate, tempAngleRate) * sign * faceFloat;
				float tempDist = Vector3.Distance (thisTransform.position, nextCornerPos);
				Speed_Order = Mathf.Lerp (Max_Speed_Rate, Min_Speed_Rate, Mathf.Sqrt (tempAngleRate)); // Slow down to turn.
				Speed_Order *= Mathf.Lerp (Min_Speed_Rate, Max_Speed_Rate, tempDist / SlowDown_Range); // Slow down near the target.
				Speed_Order = Mathf.Clamp (Speed_Order, Min_Speed_Rate, Max_Speed_Rate);
			} else { // No Turn
				Turn_Order = 0.0f;
				float tempDist = Vector3.Distance (thisTransform.position, nextCornerPos);
				Speed_Order = Mathf.Lerp (Min_Speed_Rate, Max_Speed_Rate, tempDist / SlowDown_Range); // Slow down the speed.
			}
			Speed_Order *= directionFloat * speedMultiplier;
			if (directionFloat < 0.0f) {
				Speed_Order *= 0.5f; // Slow down in Revese.
			}
		}

		void MainBodyBroken_Linkage ()
		{ // Called from "Damage_Control_CS" in MainBody.
			if (markerObjectTransform) {
				Destroy (markerObjectTransform.gameObject);
			}
			Destroy (this.gameObject);
		}

		void Get_Tank_ID_Control (Tank_ID_Control_CS topScript)
		{
			if (topScript.Input_Type != 6) { // Other input type.
				if (markerObjectTransform) {
					Destroy (markerObjectTransform.gameObject);
				}
				Destroy (this.gameObject);
			} else {
				myID = topScript.Tank_ID;
				rootTransform = topScript.Root_Transform;
				bodyRigidbody = topScript.Stored_TankProp.bodyRigidbody;
				Initial_Settings ();
			}
		}

		void Receive_Current_ID (int id)
		{
			if (id == myID) {
				isCurrent = true;
			} else if (isCurrent) {
				isCurrent = false;
				if (markerRenderer) {
					markerRenderer.enabled = false;
				}
			}
		}

		void Get_Drive_Control (Drive_Control_CS tempScript)
		{ // Called from "Drive_Control_CS".
			if (-Max_Speed_Error >= tempScript.Max_Speed) {
				Debug.LogWarning ("'Max Speed Error' value of AI_Semi_CS is set to too large. The value is ignored in this scene.");
			} else {
				maxSpeed = tempScript.Max_Speed + Max_Speed_Error;
			}
		}

		void Get_GunCamera (Camera tempCamera)
		{ // Called from "Gun_Camera_CS".
			gunCamera = tempCamera;
		}

	}

}