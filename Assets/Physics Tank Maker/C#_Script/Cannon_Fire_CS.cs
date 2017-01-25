using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Cannon_Fire_CS : MonoBehaviour
	{

		public float Reload_Time = 2.0f;
		public float Recoil_Force = 5000.0f;
		public bool AI_Reference = true;
		public bool Karl_Flag = false;
	
		Rigidbody bodyRigidbody;
		Transform thisTransform;
		int direction = 1;
		bool isReloaded = true;
		bool isTrouble = false;
		Turret_Horizontal_CS turretScript;
		Cannon_Vertical_CS cannonScript;
		AI_CS aiScript;
		Transform generatorTransform;

		float castRayCount;
		int layerMask = ~((1 << 10) + (1 << 2)); // Layer 2 = Ignore Ray, Layer 10 = Ignore All.
		float aimingCount;
		float waitingCount;

		bool isCurrent;
		int myID;
		int inputType = 4;

		void Awake ()
		{
			thisTransform = transform;
			cannonScript = GetComponent < Cannon_Vertical_CS > ();
		}

		void Start ()
		{ // After changing the hierarchy.
			turretScript = thisTransform.parent.GetComponent < Turret_Horizontal_CS > ();
			generatorTransform = GetComponentInChildren < Bullet_Generator_CS > ().transform;
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
				case 4:
					Mouse_Input ();
					break;
				case 5:
					Mouse_Input ();
					break;
				case 10:
					AI_Input ();
					break;
				}
			}
		}

		void Stick_Input ()
		{
			if (isReloaded && isTrouble == false && Input.GetButton ("R_Button")) {
				Fire ();
			}
		}

		void Trigger_Input ()
		{
			if (isReloaded && isTrouble == false && Input.GetButton ("Fire3")) {
				Fire ();
			}
		}

		void  KeyBoard_Input ()
		{
			if (isReloaded && isTrouble == false && Input.GetKey ("x")) {
				Fire ();
			}
		}

		void Mouse_Input ()
		{
			if (isReloaded && isTrouble == false && turretScript.OpenFire_Flag && Input.GetMouseButton (0)) {
				Fire ();
			}
		}

		public void AI_Semi_Input ()
		{ // Called from "AI_Semi_CS".
			if (isReloaded && isTrouble == false && turretScript.OpenFire_Flag) {
				Fire ();
			}
		}

		void AI_Input ()
		{
			if (AI_Reference) { // AI refers to this Cannon as the main cannon to decide its action.
				AI_Set_Can_Aim ();
			}
			if (isReloaded && isTrouble == false) {
				AI_Fire_Process ();
			}
		}
			
		void AI_Set_Can_Aim ()
		{
			// Make sure that the target is within eyesight.
			if (aiScript.Direct_Fire) { // Direct Fire
				if (aiScript.Detect_Flag) { // AI detects the target.
					castRayCount += Time.fixedDeltaTime;
					if (castRayCount > 2.0f) {
						aiScript.Can_Aim = AI_Cast_Ray ();
						castRayCount = 0.0f;
					}
				} else { // AI does not detect the target.
					aiScript.Can_Aim = false;
					castRayCount = 0.0f;
				}
			} else { // InDirect Fire
				aiScript.Can_Aim = aiScript.Detect_Flag;
			}
		}

		bool AI_Cast_Ray ()
		{
			// Cast Ray from "Bullet_Generator" to the target.
			Ray ray = new Ray (generatorTransform.position, turretScript.Target_Pos - generatorTransform.position);
			RaycastHit raycastHit;
			if (Physics.Raycast (ray, out raycastHit, Vector3.Distance (generatorTransform.position, turretScript.Target_Pos), layerMask)) {
				if (raycastHit.transform.root == aiScript.Target_Root_Transform) { // Ray hits the target.
					return true;
				} else { // Ray hits something else.
					turretScript.AI_Random_Offset ();
					return false;
				}
			} else { // Ray does not hit anyhing.
				return true;
			}
		}

		void AI_Fire_Process ()
		{
			if (aiScript.Target_Distance < aiScript.OpenFire_Distance && aiScript.Can_Aim) { // Target is within OpenFire_Distance and eyesight.
				if (turretScript.OpenFire_Flag && cannonScript.OpenFire_Flag) { // Angles of Turret and Cannon are OK.
					aimingCount += Time.deltaTime;
					if (aimingCount > aiScript.Fire_Count) { // aimingCount is over.
						Fire ();
						aimingCount = 0.0f;
						waitingCount = 0.0f;
						turretScript.AI_Random_Offset ();
					}
				} else { // Angle of Turret or Cannon is not prepared.
					if (aiScript.Direct_Fire) { // Direct fire
						if (aiScript.Approach_Flag) { // The target is within Approach_Distance.
							waitingCount += Time.deltaTime;
							if (waitingCount > 5.0f) { // The target might be out of the cannon's angle range. e.g. The target is on the opposite side of a hill.
								aimingCount = 0.0f;
								waitingCount = 0.0f;
								turretScript.AI_Random_Offset ();
							}
						} else { // The target is out of Approach_Distance.
							aimingCount = 0.0f;
							waitingCount = 0.0f;
						}
					} else { // Indirect fire
						aimingCount = 0.0f;
					}
				}
			} else { // Target is out of OpenFire_Distance or eyesight.
				aimingCount = 0.0f;
				waitingCount = 0.0f;
			}
		}

		void Fire ()
		{
			if (Karl_Flag == false) { // Normal Tank
				BroadcastMessage ("Fire_Linkage", direction, SendMessageOptions.DontRequireReceiver);
			} else { // Moser Karl
				// BroadcastMessage from "Turret_Base". Karl has two 'Recoil_Brake' in the "Turret_Base" and "Barrel_Base". 
				thisTransform.parent.BroadcastMessage ("Fire_Linkage", direction, SendMessageOptions.DontRequireReceiver);
			}
			bodyRigidbody.AddForceAtPosition (-thisTransform.forward * Recoil_Force, thisTransform.position, ForceMode.Impulse);
			isReloaded = false;
			StartCoroutine ("Reload");
		}

		IEnumerator Reload ()
		{
			yield return new WaitForSeconds (Reload_Time);
			isReloaded = true;
			if (direction == 1) {
				direction = 2;
			} else {
				direction = 1;
			}
		}

		void TurretBroken_Linkage ()
		{ // Called from "Damage_Control_CS" in Turret.
			if (inputType == 10 && AI_Reference) { // AI and referred to from AI.
				aiScript.Can_Aim = false; // In case of multiple turrets tank still alive, AI can not stop near the target.
			}
			Destroy (this);
		}

		public bool Trouble (float count)
		{ // Called from "Damage_Control_CS" in Cannon or Barrel.
			if (!isTrouble) {
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

			bodyRigidbody = topScript.Stored_TankProp.bodyRigidbody;
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

	}

}