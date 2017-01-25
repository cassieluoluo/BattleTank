using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace ChobiAssets.PTM
{

	[ System.Serializable]
	public class TankProp
	{
		public Tank_ID_Control_CS topScript;
		public MainBody_Setting_CS bodyScript;
		public Transform bodyTransform;
		public Rigidbody bodyRigidbody;
		public AI_CS aiScript;
	}

	public class Game_Controller_CS : MonoBehaviour
	{

		public float Assign_Frequency = 3.0f;
		public float Time_Scale = 1.0f;
		public float Gravity = -9.81f;
		public float Fixed_TimeStep = 0.02f;

		public RC_Camera_CS RC_Cam_Script; // Set by "RC_Camera_CS".

		public Tank_ID_Control_CS[] Operable_Tanks; // Referred to from RC_Camera.
		List < TankProp > friendlyTanks = new List < TankProp > ();
		List < TankProp > hostileTanks = new List < TankProp > ();

		float assignCount;
		float currentTimeScale;
		int currentID = 1;

		void Awake ()
		{
			this.tag = "GameController"; // This tag is referred to by "Tank_ID_Control" and "RC_Camera_CS".
			Operable_Tanks = new Tank_ID_Control_CS [11];
			// Modify the Physics and Time manager.
			currentTimeScale = Time_Scale;
			Time.timeScale = currentTimeScale;
			Physics.gravity = new Vector3 (0.0f, Gravity, 0.0f);
			Physics.sleepThreshold = 0.5f;
			Time.fixedDeltaTime = Fixed_TimeStep;
		}
			
		public void Receive_ID (Tank_ID_Control_CS topScript)
		{ // Called from "Tank_ID_Control_CS" at the opening.
			// Store "Tank_ID_Control_CS" of operable tanks, and main components of all the tanks.
			if (topScript.Tank_ID != 0) { // Operable Tank.
				if (Operable_Tanks [topScript.Tank_ID] == null) { // Operable_Tanks[#] is empty.
					Operable_Tanks [topScript.Tank_ID] = topScript;
					Store_Components (topScript);
					return;
				} else { // Operable_Tanks[#] is not empty.
					for (int i = 1; i < Operable_Tanks.Length; i++) { // Search empty ID number.
						if (Operable_Tanks [i] == null) { // Operable_Tanks[#] is empty.
							Operable_Tanks [i] = topScript;
							topScript.Tank_ID = i; // Change the Tank_ID.
							Store_Components (topScript);
							return;
						}
					}
					// "Operable_Tanks" is full.
					topScript.Tank_ID = 0; // Change Tank_ID to "0". (Operable >> Not Operable)
				}
			} // Not operable tank.
			Store_Components (topScript);
		}

		void Store_Components (Tank_ID_Control_CS topScript)
		{ // Store "Tank_ID_Control_CS", "MainBody's Transform", "MainBody_Setting_CS", "AI_CS" in the tank.
			TankProp tankProp = new TankProp ();
			tankProp.topScript = topScript;
			tankProp.bodyScript = topScript.GetComponentInChildren < MainBody_Setting_CS > ();
			tankProp.bodyTransform = tankProp.bodyScript.transform;
			tankProp.bodyRigidbody = tankProp.bodyTransform.GetComponent <Rigidbody > ();
			tankProp.aiScript = tankProp.bodyTransform.GetComponentInChildren < AI_CS > ();
			topScript.Stored_TankProp = tankProp; //  // Store tankProp (pass by reference) in the "Tank_ID_Control_CS".
			if (topScript.Relationship == 0) { // Friend
				friendlyTanks.Add (tankProp);
			} else if (topScript.Relationship == 1) { // Hostile
				hostileTanks.Add (tankProp);
			}
		}

		void Update ()
		{
			if (assignCount > Assign_Frequency) {
				Call_Assign_Target ();
			} else {
				assignCount += Time.deltaTime;
			}
			if (Input.anyKeyDown) {
				Key_Check ();
			}
		}

		public void Call_Assign_Target () //Also called from "AI_CS".
		{
			assignCount = 0.0f;
			Assign_Target (friendlyTanks, hostileTanks);
			Assign_Target (hostileTanks, friendlyTanks);
		}

		void Assign_Target (List < TankProp > teamA, List < TankProp > teamB)
		{
			float shortestDist = 10000.0f;
			bool isFound = false;
			int targetIndex = 0;
			for (int i = 0; i < teamA.Count; i++) {
				if (teamA [i].aiScript && teamA [i].aiScript.No_Attack == false && teamA [i].aiScript.Detect_Flag == false) {
					for (int j = 0; j < teamB.Count; j++) {
						if (teamB [j].bodyTransform.root.tag != "Finish") {
							float tempDist = Vector3.Distance (teamA [i].bodyTransform.position, teamB [j].bodyTransform.position);
							if (tempDist < teamA [i].aiScript.Visibility_Radius && tempDist < shortestDist) {
								if (teamA [i].aiScript.RayCast_Check (teamB [j])) { // Make sure that the tank can aim the target by casting a Ray.
									shortestDist = tempDist;
									isFound = true;
									targetIndex = j;
								}
							}
						}
					}
					if (isFound) {
						teamA [i].aiScript.Set_Target (teamB [targetIndex]); // Send the new target to "AI_CS".
					}
				}
			}
		}

		KeyCode [] keypadCodes = new KeyCode [] {KeyCode.Keypad1, KeyCode.Keypad2, KeyCode.Keypad3, KeyCode.Keypad4, KeyCode.Keypad5, KeyCode.Keypad6, KeyCode.Keypad7, KeyCode.Keypad8, KeyCode.Keypad9, KeyCode.Keypad0};
		string [] numStrings = new string [] {"1", "2", "3", "4", "5", "6", "7", "8", "9", "0"};
		KeyCode [] optionKeyCodes = new KeyCode [] {KeyCode.KeypadEnter, KeyCode.KeypadPlus, KeyCode.KeypadMinus};
		void Key_Check ()
		{
			for (int i = 0; i < keypadCodes.Length; i++) {
				if (Input.GetKeyDown (keypadCodes [i])) {
					Cast_Current_ID (i + 1);
					return;
				}
			}
			for (int i = 0; i < numStrings.Length; i++) {
				if (Input.GetKeyDown (numStrings [i])) {
					Cast_Current_ID (i + 1);
					return;
				}
			}
			for (int i = 0; i < optionKeyCodes.Length; i++) {
				if (Input.GetKeyDown (optionKeyCodes [i])) {
					Set_TimeScale (i);
					return;
				}
			}

			if (Input.GetKeyDown (KeyCode.Backspace)) {
				SceneManager.LoadScene (SceneManager.GetActiveScene ().name); // Reload the scene.
			} else if (Input.GetKeyDown (KeyCode.Escape)) {
				Application.Quit (); // Quit
			}
		}

		void Cast_Current_ID (int tempID)
		{
			if (tempID < Operable_Tanks.Length) { // To avoid overflowing.
				if (Operable_Tanks [tempID]) {
					currentID = tempID;
					// Broadcast current ID to all the tanks.
					for (int i = 0; i < Operable_Tanks.Length; i++) {
						if (Operable_Tanks [i]) {
							Operable_Tanks [i].BroadcastMessage ("Receive_Current_ID", currentID, SendMessageOptions.DontRequireReceiver);
						}
					}
					// Send current ID to RC_Camera.
					if (RC_Cam_Script) {
						RC_Cam_Script.Receive_Current_ID (currentID);
					}
				}
			}
		}
			
		void Set_TimeScale (int value)
		{
			switch (value) {
			case 0: // Reset
				currentTimeScale = Time_Scale;
				break;
			case 1: // Increase
				if (currentTimeScale > 1.0f) {
					currentTimeScale += 1.0f;
				} else {
					currentTimeScale *= 2.0f;
				}
				break;
			case 2: // Decrease
				if (currentTimeScale > 1.0f) {
					currentTimeScale -= 1.0f;
				} else {
					currentTimeScale *= 0.5f;
				}
				break;
			}
			currentTimeScale = Mathf.Clamp (currentTimeScale, 0.125f, 3.0f);
			Time.timeScale = currentTimeScale;
		}

		public void ReSpawn_ReSetting (TankProp storedTankProp)
		{ // Called from "Tank_ID_Control" when ReSpawn.
			// Store the re-spawned components.
			TankProp tankProp = new TankProp ();
			if (storedTankProp.topScript.Relationship == 0) { // friend
				tankProp = friendlyTanks.Find (delegate ( TankProp tempTankProp) {
					return tempTankProp == storedTankProp;
				});
			} else { // hostile
				tankProp = hostileTanks.Find (delegate ( TankProp tempTankProp) {
					return tempTankProp == storedTankProp;
				});
			}
			tankProp.bodyScript = tankProp.topScript.GetComponentInChildren < MainBody_Setting_CS > ();
			tankProp.bodyTransform = tankProp.bodyScript.transform;
			tankProp.bodyRigidbody = tankProp.bodyTransform.GetComponent <Rigidbody > ();
			tankProp.aiScript = tankProp.bodyTransform.GetComponentInChildren < AI_CS > ();
			tankProp.topScript.Stored_TankProp = tankProp; // Store tankProp (pass by reference) in the "Tank_ID_Control_CS".
			// Reset assignCount.
			assignCount = Assign_Frequency;
		}

		public bool Remove_Tank (TankProp storedTankProp)
		{ // Called from "Tank_ID_Control".
			// Remove the reference from "Operable_Tanks" array.
			int tankID = storedTankProp.topScript.Tank_ID;
			if (tankID != 0) { // Operable Tanks.
				if (Operable_Tanks [tankID]) {
					Operable_Tanks [tankID] = null;
				}
			}
			// Remove components in the List ("friendlyTanks" or "hostileTanks").
			if (storedTankProp.topScript.Relationship == 0) { // friend
				friendlyTanks.Remove (storedTankProp);
			} else { // hostile
				hostileTanks.Remove (storedTankProp);
			}
			// Change the current ID when this tank is the current operable tank.
			if (tankID == currentID) { // This tank is the current operable tank.
				for (int i = 1; i < Operable_Tanks.Length; i++) {
					if (Operable_Tanks [i]) { // At least one of the operable tanks exists.
						Cast_Current_ID (i); // Change the current operable tank.
						return true; // Remove OK.
					}
				} // There is no operable tank.
				return false; // Remove NG.
			} // This tank is not the current operable tank.
			return true; // Remove OK.
		}

	}

}