using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{
	
	[ RequireComponent (typeof(UnityEngine.AI.NavMeshAgent))]
	public class AI_CS : MonoBehaviour
	{
		
		// Set by "Tank_ID_Control_CS".
		GameObject wayPointPack;
		int patrolType; // 0=Order, 1=Random.
		Transform followTarget;
		public bool No_Attack; // Referred to from "Game_Controller".
		bool breakthrough;
		public float Visibility_Radius; // Referred to from "Cannon_Fire"
		float approachDistance;
		public float OpenFire_Distance; // Referred to from "Cannon_Fire"
		float lostCount;
		bool faceEnemy = false;
		float faceOffestAngle = 0.0f;
		Text aiStateText;
		public string Tank_Name; // Referred to from "UI_AIState_Control_CS"

		// Set in "AI_CS".
		public float WayPoint_Radius = 5.0f;
		public float BrakeTurn_Min_Angle = 20.0f;
		public float BrakeTurn_Max_Angle = 180.0f;
		public float Pivot_Turn_Angle = 45.0f;
		public float Min_Target_Angle = 1.0f;
		public float Min_Turn_Rate = 0.1f;
		public float Max_Turn_Rate = 1.0f;
		public float Min_Speed_Rate = 0.4f;
		public float Max_Speed_Rate = 1.0f;
		public float SlowDown_Range = 20.0f;
		public float Stuck_Count = 3.0f; // Referred to from "AI_Hand"
		public float Max_Speed_Error = 0.0f;

		public bool Direct_Fire = true; // Referred to from "Cannon_Fire"
		public float Fire_Angle = 2.0f; // Referred to from "Cannon_Vertical"
		public float Fire_Count = 0.5f; // Referred to from "Cannon_Fire"

		// Referred to from "Drive_Control".
		public float Speed_Order;
		public float Turn_Order;
		// Referred to from "Cannon_Fire"
		public Transform Target_Root_Transform;
		public float Target_Distance;
		public bool Detect_Flag = false; // Also referred to from "Game_Controller".
		public bool Approach_Flag = false;
		public float AI_Upper_Offset;
		public float AI_Lower_Offset;
		// Controled by "Cannon_Fire".
		public bool Can_Aim = false;
		// Referred to from "UI_PosMarker_Control_CS" and "UI_AIState_Control_CS".
		public int Action_Type;

		Transform thisTransform;
		Rigidbody bodyRigidbody;
		Transform eyeTransform;
		UnityEngine.AI.NavMeshAgent thisAgent;
		Vector3 initialPos;
		Turret_Horizontal_CS [] turretScripts;
		Vector3[] wayPoints;
		Vector3 lookPos;
		Transform followTransform;
		float followDist = 15.0f;
		Transform targetTransform;
		public float Losing_Count; // Referred to from "UI_AIState_Control_CS"
		int nextWayPoint = -1;
		float searchingCount;
		float setDestCount;
		bool isStaying = false;
		bool obstacleFlag = false;
		int layerMask = ~((1 << 10) + (1 << 2)); // Layer 2 = Ignore Ray, Layer 10 = Ignore All.
		float maxSpeed;

		Game_Controller_CS controllerScript;
		UI_AIState_Control_CS stateScript;
		AI_Hand_CS handScript;

		void Awake ()
		{
			thisTransform = transform;
			// Find "Turret_Horizontal_CS".
			turretScripts = thisTransform.parent.GetComponentsInChildren <Turret_Horizontal_CS> ();
			// Find "AI_Eye"
			eyeTransform = thisTransform.Find ("AI_Eye"); // Do not rename "AI_Eye".
			if (eyeTransform == null) {
				Debug.LogError ("'AI_Eye' can not be found. "); 
				Destroy (this);
			}
			// Find AI_Hand_CS script.
			handScript = thisTransform.GetComponentInChildren < AI_Hand_CS > ();
			// NavMeshAgent settings.
			initialPos = thisTransform.localPosition; // to fix this object on its initial position.
			thisAgent = GetComponent <UnityEngine.AI.NavMeshAgent> ();
		}

		void Initial_Settings (Tank_ID_Control_CS topScript)
		{
			// Variables settings.
			wayPointPack = topScript.WayPoint_Pack;
			patrolType = topScript.Patrol_Type;
			followTarget = topScript.Follow_Target;
			No_Attack = topScript.No_Attack;
			breakthrough = topScript.Breakthrough;
			Visibility_Radius = topScript.Visibility_Radius;
			approachDistance = topScript.Approach_Distance;
			OpenFire_Distance = topScript.OpenFire_Distance;
			lostCount = topScript.Lost_Count;
			faceEnemy = topScript.Face_Enemy;
			faceOffestAngle = topScript.Face_Offest_Angle;
			aiStateText = topScript.AI_State_Text;
			Tank_Name = topScript.Tank_Name;
			if (string.IsNullOrEmpty (Tank_Name)) {
				Tank_Name = topScript.Root_Transform.name;
			}
			// Text settings.
			if (aiStateText) {
				stateScript = aiStateText.GetComponent <UI_AIState_Control_CS> () ;
				if (stateScript == null) {
					stateScript = aiStateText.gameObject.AddComponent <UI_AIState_Control_CS> () ;
				}
				stateScript.AI_Script = this;
			}
			// 'wayPointPack' settings.
			Set_WayPoint ();
			// 'followTarget' settings.
			if (followTarget) {
				Get_Follow_Transform ();
			} else {
				Update_Next_WayPoint (); // Set the first waypoint.
			}
		}

		void Set_WayPoint ()
		{
			if (wayPointPack) {
				int childCount = wayPointPack.transform.childCount;
				if (childCount > 1) { // has more than two points.
					wayPoints = new Vector3 [ childCount ];
					for (int i = 0; i < childCount; i++) {
						wayPoints [i] = wayPointPack.transform.GetChild (i).position;
					}
					return;
				} else if (childCount == 1) { // has only one point.
					wayPoints = new Vector3 [ 1 ];
					wayPoints [0] = wayPointPack.transform.GetChild (0).position;
					lookPos = wayPoints [0] + (thisTransform.forward * 100.0f); // Store the initial direction.
					return;
				}
			} // 'wayPointPack' is not assigined, or has no point.
			wayPoints = new Vector3 [ 1 ];
			wayPoints [0] = thisTransform.position; // Set this initial position.
			lookPos = wayPoints [0] + (thisTransform.forward * 100.0f); // Store the initial direction.
		}

		void Get_Follow_Transform ()
		{
			Rigidbody rigidbody = followTarget.GetComponentInChildren < Rigidbody > ();
			if (rigidbody) {
				followTransform = rigidbody.transform;
			} else {
				Debug.LogError ("'Follow Target' has no rigidbody. AI cannot follow the target tank. ");
			}
		}

		void Update_Next_WayPoint ()
		{
			switch (patrolType) {
			case 0:
				nextWayPoint += 1;
				if (nextWayPoint >= wayPoints.Length) {
					nextWayPoint = 0;
				}
				break;
			case 1:
				nextWayPoint = Random.Range (0, wayPoints.Length);
				break;
			}
			thisAgent.SetDestination (wayPoints [nextWayPoint]);
		}

		void Update ()
		{
			// Fix this position and rotation.
			thisTransform.localPosition = initialPos;
			thisTransform.localRotation = Quaternion.identity;
			// Search the target.
			if (targetTransform) {
				searchingCount -= Time.deltaTime;
				if (searchingCount < 0.0f) {
					searchingCount = 1.0f; // Sometimes AI fails to notice the target.
					Search ();
				}
			}
			// Action
			switch (Action_Type) {
			case 0: // Patrol mode.
				if (followTarget) {
					Follow_Mode ();
				} else {
					WayPoint_Mode ();
				}
				break;
			case 1: // Chase mode.
				if (targetTransform) {
					if (breakthrough) {
						WayPoint_Mode ();
					} else {
						Chase_Mode ();
					}
				} else {
					Lost_Target ();
				}
				break;
			}
			// "AI_Hand" touches an obstacle.
			if (handScript && handScript.Is_Working) {
				Speed_Order = 0.0f; // The tank can only turn.
			}
		}

		public void Set_Target (TankProp targetTankProp)
		{ // Called from "Game_Controller_CS" in the scene.
			if (targetTransform != targetTankProp.bodyTransform) {
				Lost_Target ();
				targetTransform = targetTankProp.bodyTransform;
				Target_Root_Transform = targetTankProp.topScript.Root_Transform;
				AI_Upper_Offset = targetTankProp.bodyScript.AI_Upper_Offset;
				AI_Lower_Offset = targetTankProp.bodyScript.AI_Lower_Offset;
			}
		}

		void Lost_Target ()
		{
			Action_Type = 0;
			setDestCount = 0.0f;
			searchingCount = 0.0f;
			targetTransform = null;
			Target_Root_Transform = null;
			Detect_Flag = false;
			Losing_Count = lostCount;
			Approach_Flag = false;
			isStaying = false;
			// Send Message to "Turret_Horizontal".
			foreach (Turret_Horizontal_CS turretScript in turretScripts) {
				if (turretScript) {
					turretScript.AI_Reset_Lock_On ();
				}
			}
			if (followTarget == null) { // WayPoint mode.
				thisAgent.SetDestination (wayPoints [nextWayPoint]);
			}
		}

		void Search ()
		{
			Vector3 targetPos = targetTransform.position + (targetTransform.up * AI_Upper_Offset);
			Target_Distance = Vector3.Distance (eyeTransform.position, targetPos);
			if (Target_Root_Transform.tag != "Finish") { // Target is living.
				if (Target_Distance < Visibility_Radius) {
					// Cast Ray from "AI_Eye" to the target.
					Ray ray = new Ray (eyeTransform.position, targetPos - eyeTransform.position);
					RaycastHit raycastHit;
					if (Physics.Raycast (ray, out raycastHit, Visibility_Radius, layerMask)) { // Ray hits anything.
						if (raycastHit.transform.root == Target_Root_Transform) { // Ray hits the target.
							Detect_Flag = true;
							switch (Action_Type) {
							case 0: // Patrol mode.
								Action_Type = 1;
								setDestCount = 0.0f;
								isStaying = false;
								// Send targetTransform to "Turret_Horizontal".
								foreach (Turret_Horizontal_CS turretScript in turretScripts) {
									if (turretScript) {
										turretScript.AI_Set_Lock_On (targetTransform);
									}
								}
								break;
							case 1: // Chase mode.
								Losing_Count = lostCount;
								break;
							}
							return;
						}
					}
				}
			} else { // Target is already dead.
				Lost_Target ();
				controllerScript.Call_Assign_Target (); // Call "Game_Controller" to receive new target.
				return;
			}
			// Target is out of range, or Ray does not hit anything, or Ray hits other object.
			Detect_Flag = false;
			if (Action_Type == 1) { // Chase mode.
				Losing_Count -= Time.deltaTime + 1.0f;
				if (Losing_Count < 0.0f) { // AI has lost the target.
					Lost_Target ();
				}
			}
		}
			
		void Follow_Mode ()
		{
			if (followTransform) {
				// Update the destination.
				setDestCount -= Time.deltaTime;
				if (setDestCount < 0.0f) {
					thisAgent.SetDestination (followTransform.position);
					setDestCount = 0.5f;
				}
				// Calculate "Speed_Order" and "Turn_Order".
				Auto_Drive ();
				// Stay near the target, and look to the same direction.
				float tempDist = Vector3.Distance (thisTransform.position, followTransform.position);
				if (isStaying == false) { // not staying.
					if (tempDist < followDist) { // near the target (within followDist )
						isStaying = true;
						lookPos = followTransform.position + (followTransform.forward * 250.0f);
					} else if (tempDist < followDist + 5.0f) { // almost near the target. (within followDist + 5.0m)
						// Slow down the speed.
						float tempSpeedOrder = Mathf.Lerp (0.0f, Max_Speed_Rate, (tempDist - (followDist - 1.0f)) / 10.0f);
						if (Speed_Order > tempSpeedOrder) {
							Speed_Order = tempSpeedOrder;
						}
					} 
				} else { // staying now.
					if (tempDist < followDist + 5.0f) { // almost near the target. (within followDist + 5.0m)
						isStaying = true; // Keep staying.
						lookPos = followTransform.position + (followTransform.forward * 250.0f); // Look to the same direction.
					} else { // away from the target.
						isStaying = false;
					}
				}
			} else { // "followTransform" has been lost by respawn.
				Get_Follow_Transform ();
			}
		}

		void WayPoint_Mode ()
		{
			// Check the Path Status.
			if (thisAgent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid) {
				thisAgent.ResetPath ();
				thisAgent.SetDestination (wayPoints [nextWayPoint]);
			}
			if (wayPoints.Length > 1) { // Normal.
				if (Vector3.Distance (thisTransform.position, wayPoints [nextWayPoint]) < WayPoint_Radius) { // Arrived at the WayPoint.
					Update_Next_WayPoint ();
				}
			} else { // Only one waypoint.
				float tempDist = Vector3.Distance (thisTransform.position, wayPoints [0]);
				if (isStaying == false) { // not staying.
					if (tempDist < WayPoint_Radius) { // Arrived at the WayPoint (initial position).
						isStaying = true;
					}
				} else { // staying now.
					if (tempDist > WayPoint_Radius + 5.0f) { // away from the WayPoint (initial position).
						isStaying = false;
					}
				}
			}
			// Calculate "Speed_Order" and "Turn_Order".
			Auto_Drive ();
		}

		void Chase_Mode ()
		{
			// Update the destination.
			setDestCount -= Time.deltaTime;
			if (setDestCount < 0.0f) {
				thisAgent.SetDestination (targetTransform.position);
				setDestCount = 5.0f;
			}
			// Set 'Approach_Flag'.
			if (Target_Distance < approachDistance) { // within 'approachDistance'.
				if (Can_Aim || approachDistance == Mathf.Infinity) { // The target is within eyesight, or 'approachDistance' is set to infinity.
					Approach_Flag = true;
				} else {
					Approach_Flag = false;
				}
			} else { // out of 'approachDistance'.
				Approach_Flag = false;
			}
			// Calculate "Speed_Order" and "Turn_Order".
			Auto_Drive ();
		}

		void Auto_Drive ()
		{
			// Get the next corner position.
			Vector3 nextCornerPos;
			if (Approach_Flag && faceEnemy) { // The cannon can aim the target, and 'faceEnemy' option is enabled.
				nextCornerPos = targetTransform.position; // Look to the target.
			} else if (isStaying) { // only in case of 'Follow_Mode' or having only one waypoint.
				nextCornerPos = lookPos; // Look to the same direction as 'followTarget', or the initial direction.
			} else if (thisAgent.path.corners.Length > 1) { // Usual condition.
				nextCornerPos = thisAgent.path.corners [1];
			} else { // Something wrong in the Agent.
				return;
			}
			// Calculate the angle to the next corner.
			Vector3 localPos = thisTransform.InverseTransformPoint (nextCornerPos);
			float targetAng = Vector2.Angle (Vector2.up, new Vector2 (localPos.x, localPos.z)) * Mathf.Sign (localPos.x);
			// for looking to the target.
			if (Approach_Flag && faceEnemy) {
				targetAng -= faceOffestAngle * Mathf.Sign (localPos.x);
			}
			// Calculate "Speed_Order" and "Turn_Order".
			float sign = Mathf.Sign (targetAng);
			targetAng = Mathf.Abs (targetAng);
			if (targetAng > Min_Target_Angle) { // Turn
				if (targetAng > Pivot_Turn_Angle) { // Pivot Turn
					Turn_Order = 1.0f * sign;
					Speed_Order = 0.0f;
					return;
				} // Brake Turn
				//float tempVelocity = bodyRigidbody.velocity.magnitude;
				//float tempBaseAngle =  Mathf.Lerp (BrakeTurn_Min_Angle, BrakeTurn_Max_Angle, Mathf.Pow (tempVelocity / maxSpeed, 2.0f));
				//float tempAngleRate = targetAng / tempBaseAngle;
				float tempAngleRate = targetAng / Mathf.Lerp (BrakeTurn_Min_Angle, BrakeTurn_Max_Angle, Mathf.Pow (bodyRigidbody.velocity.magnitude / maxSpeed, 2.0f));
				Turn_Order = Mathf.Lerp (Min_Turn_Rate, Max_Turn_Rate, tempAngleRate) * sign;
				if (Approach_Flag || isStaying) { // It has approached or is staying.
					Speed_Order = 0.0f;
					return;
				}
				float tempDist = Vector3.Distance (thisTransform.position, nextCornerPos);
				Speed_Order = Mathf.Lerp (Max_Speed_Rate, Min_Speed_Rate, Mathf.Sqrt (tempAngleRate)); // Slow down to turn.
				Speed_Order *= Mathf.Lerp (Min_Speed_Rate, Max_Speed_Rate, tempDist / SlowDown_Range); // Slow down near the target.
				Speed_Order = Mathf.Clamp (Speed_Order, Min_Speed_Rate, Max_Speed_Rate);
			} else { // No Turn
				Turn_Order = 0.0f;
				if (Approach_Flag || isStaying) { // It has approached or is staying.
					Speed_Order = 0.0f;
					return;
				} else {
					float tempDist = Vector3.Distance (thisTransform.position, nextCornerPos);
					Speed_Order = Mathf.Lerp (Min_Speed_Rate, Max_Speed_Rate, tempDist / SlowDown_Range); // Slow down the speed.
				}
			}
		}

		public void Escape_Stuck ()
		{ // Called from "AI_Hand" when stuck.
			if (followTarget == null) {
				if (obstacleFlag == false && Random.Range (0, 3) == 0) {
					obstacleFlag = true;
					GameObject obstacleObject = new GameObject ("Obstacle_Object");
					obstacleObject.transform.position = handScript.transform.position;
					obstacleObject.transform.rotation = thisTransform.rotation * Quaternion.Euler (0.0f, 45.0f, 0.0f);
					UnityEngine.AI.NavMeshObstacle navMeshObstacle = obstacleObject.AddComponent < UnityEngine.AI.NavMeshObstacle > ();
					navMeshObstacle.carving = true;
					obstacleObject.AddComponent < Delete_Timer_CS > ().Count = 20.0f;
					StartCoroutine ("Obstacle_Timer");
				} else if (Action_Type == 0) {
					Update_Next_WayPoint ();
				}
			}
		}

		IEnumerator Obstacle_Timer ()
		{
			yield return new WaitForSeconds (20.0f);
			obstacleFlag = false;
		}

		public bool RayCast_Check (TankProp targetTankProp)
		{ // Called from "Game_Controller_CS".
			thisTransform.localPosition = initialPos; // Fix this position.
			Transform tempTargetTransform = targetTankProp.bodyTransform;
			Vector3 tempTargetPos = tempTargetTransform.position + (tempTargetTransform.up * targetTankProp.bodyScript.AI_Upper_Offset);
			Ray ray = new Ray (eyeTransform.position, tempTargetPos - eyeTransform.position);
			RaycastHit raycastHit;
			if (Physics.Raycast (ray, out raycastHit, Visibility_Radius, layerMask)) { // Ray hits anything.
				if (raycastHit.transform.root == targetTankProp.topScript.Root_Transform) { // Ray hits the target.
					return true;
				} else { // Ray does not hit the target.
					return false;
				}
			} else { // Ray does not hit anything.
				return false;
			}
		}

		public void Change_AI_Settings (Event_Controller_CS eventScript)
		{ // Called from "Event_Controller".
			if (eventScript) {
				// Change variables.
				wayPointPack = eventScript.New_WayPoint_Pack;
				patrolType = eventScript.New_Patrol_Type;
				followTarget = eventScript.New_Follow_Target;
				No_Attack = eventScript.New_No_Attack;
				breakthrough = eventScript.New_Breakthrough;
				Visibility_Radius = eventScript.New_Visibility_Radius;
				approachDistance = eventScript.New_Approach_Distance;
				OpenFire_Distance = eventScript.New_OpenFire_Distance;
				lostCount = eventScript.New_Lost_Count;
				faceEnemy = eventScript.New_Face_Enemy;
				faceOffestAngle = eventScript.New_Face_Offest_Angle;
				// Reset settings.
				Set_WayPoint ();
				if (followTarget) {
					Get_Follow_Transform ();
				} else {
					nextWayPoint = -1;
					Update_Next_WayPoint ();
				}
				Action_Type = 0;
				Lost_Target ();
			}
		}

		void MainBodyBroken_Linkage ()
		{ // Called from "Damage_Control_CS" in MainBody.
			if (stateScript) {
				stateScript.SendMessage ("Dead", SendMessageOptions.DontRequireReceiver);
			}
			Destroy (this.gameObject);
		}

		void Get_Tank_ID_Control (Tank_ID_Control_CS topScript)
		{
			controllerScript = topScript.Controller_Script;
			Initial_Settings (topScript);
			bodyRigidbody = topScript.Stored_TankProp.bodyRigidbody;
		}

		void Get_Drive_Control (Drive_Control_CS tempScript)
		{ // Called from "Drive_Control_CS".
			if (-Max_Speed_Error >= tempScript.Max_Speed) {
				Debug.LogWarning ("'Max Speed Error' value of AI_CS is set to too large. The value is ignored in this scene.");
			} else {
				maxSpeed = tempScript.Max_Speed + Max_Speed_Error;
			}
		}

	}

}