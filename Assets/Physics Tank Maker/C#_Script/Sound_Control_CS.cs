using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Sound_Control_CS : MonoBehaviour
	{
		public int Type = 0;

		// Engine Sound.
		public int Track_Type = 1;
		//public bool For_PhysicsTrack = false;
		public float Min_Engine_Pitch = 1.0f;
		public float Max_Engine_Pitch = 2.0f;
		public float Min_Engine_Volume = 0.1f;
		public float Max_Engine_Volume = 0.3f;
		public float Max_Velocity = 7.0f;
		public Rigidbody Left_Wheel_Rigidbody;
		public Rigidbody Right_Wheel_Rigidbody;

		float leftRadius;
		float rightRadius;
		float currentRate;
		// Used by Editor script to show the current velocity.
		public float Left_Velocity; 
		public float Right_Velocity;


		// Impact Sound from MainBody.
		public float Min_Impact = 0.25f;
		public float Max_Impact = 0.5f;
		public float Min_Impact_Pitch = 0.3f;
		public float Max_Impact_Pitch = 1.0f;
		public float Min_Impact_Volume = 0.1f;
		public float Max_Impact_Volume = 0.5f;

		float previousVelocity;
		bool isPrepared = true;
		Rigidbody bodyRigidbody;
		float clipLength;


		// Turret Motor Sound.
		public float Max_Motor_Volume = 0.5f;
		Turret_Horizontal_CS turretScript;
		Cannon_Vertical_CS cannonScript;


		// for all types.
		AudioSource thisAudioSource;

		bool isCurrent;
		int myID;

		void Awake () {
			thisAudioSource = GetComponent < AudioSource > ();
			if (thisAudioSource == null) {
				Debug.LogError ("AudioSource cannot be found in " + transform.name);
				Destroy (this);
			}
			thisAudioSource.playOnAwake = false;
			Initial_Settings ();
		}

		void Initial_Settings ()
		{
			// Initial settings.
			switch (Type) {
			case 0: // Engine Sound.
				thisAudioSource.loop = true;
				thisAudioSource.volume = 0.0f;
				thisAudioSource.Play ();
				// Set reference wheels for Physics_Track.
				switch (Track_Type) {
				case 0: // Physics_Track
					Find_Sprocket ();
					break;
				case 1: // Static_Track or Scroll_Track
					break;
				case 2: // No Track (Wheeled Vehicle)
					Set_Closest_DrivingWheel ();
					break;
				}
				break;
			case 1: // Impact Sound.
				thisAudioSource.loop = false;
				clipLength = thisAudioSource.clip.length;
				break;
			case 2: // Turret Motor Sound.
				thisAudioSource.loop = true;
				thisAudioSource.volume = 0.0f;
				thisAudioSource.Play ();
				turretScript = GetComponent < Turret_Horizontal_CS > ();
				break;
			case 3: // Cannon Motor Sound.
				thisAudioSource.loop = true;
				thisAudioSource.volume = 0.0f;
				thisAudioSource.Play ();
				cannonScript = GetComponent < Cannon_Vertical_CS > ();
				break;
			}
		}

		// In the case of "Physics_Track".
		void Find_Sprocket ()
		{
			// Find SprocketWheels.
			if (Left_Wheel_Rigidbody == null || Right_Wheel_Rigidbody == null) { // Reference is not assigned.
				Drive_Wheel_CS[] driveScripts = transform.parent.GetComponentsInChildren < Drive_Wheel_CS > ();
				foreach (Drive_Wheel_CS driveScript in driveScripts) {
					if (driveScript.name == "SprocketWheel_L") {
						Left_Wheel_Rigidbody = driveScript.GetComponent < Rigidbody > ();
					} else if (driveScript.name == "SprocketWheel_R") {
						Right_Wheel_Rigidbody = driveScript.GetComponent < Rigidbody > ();
					}
				}
			}
			if (Left_Wheel_Rigidbody && Right_Wheel_Rigidbody) {
				leftRadius = Left_Wheel_Rigidbody.GetComponent < SphereCollider > ().radius;
				rightRadius = Right_Wheel_Rigidbody.GetComponent < SphereCollider > ().radius;
			} else {
				Debug.LogError ("Reference Wheels for the engine sound can not be found.");
				Destroy (this);
			}
		}
	
		// In the case of "Static_Track".
		void Get_Static_Track_Parent (Static_Track_CS trackScript)
		{ // Called from "Static_Track_CS" (parent).
			if (Type == 0) { // Engine Sound.
				if (trackScript.Reference_L && trackScript.Reference_R) {
					Left_Wheel_Rigidbody = trackScript.Reference_L.GetComponent < Rigidbody > ();
					Right_Wheel_Rigidbody = trackScript.Reference_R.GetComponent < Rigidbody > ();
					leftRadius = trackScript.Reference_L.GetComponent < SphereCollider > ().radius;
					rightRadius = trackScript.Reference_R.GetComponent < SphereCollider > ().radius;
				} else {
					Debug.LogWarning ("Reference Wheels for the engine sound can not be found.");
					Destroy (this);
				}
			}
		}

		// In the case of "Scroll_Track".
		void Get_Track_Scroll (Track_Scroll_CS scrollScript)
		{ // Called from "Track_Scroll_CS".
			if (Type == 0 && (Left_Wheel_Rigidbody == null || Right_Wheel_Rigidbody == null)) { // Engine Sound && reference wheels are not assigned yet.
				if (scrollScript.Reference_Wheel) {
					if (scrollScript.Reference_Wheel.localEulerAngles.z == 0.0f) { // Left
						Left_Wheel_Rigidbody = scrollScript.Reference_Wheel.GetComponent < Rigidbody > ();
						leftRadius = scrollScript.Reference_Wheel.GetComponent < SphereCollider > ().radius;
					} else { // Right
						Right_Wheel_Rigidbody = scrollScript.Reference_Wheel.GetComponent < Rigidbody > ();
						rightRadius = scrollScript.Reference_Wheel.GetComponent < SphereCollider > ().radius;
					}
				} else {
					Debug.LogWarning ("Reference Wheels for the engine sound can not be found.");
					Destroy (this);
				}
			}
		}

		// In the case of No Track (Wheeled Vehicle)
		void Set_Closest_DrivingWheel ()
		{
			if (Left_Wheel_Rigidbody == null || Right_Wheel_Rigidbody == null) { // Reference is not assigned.
				Transform	bodyTransform = transform.parent;
				Drive_Wheel_CS[] driveScripts = bodyTransform.GetComponentsInChildren <Drive_Wheel_CS> ();
				float minDistL = Mathf.Infinity;
				float minDistR = Mathf.Infinity;
				Transform closestWheelL = null;
				Transform closestWheelR = null;
				for (int i = 0; i < driveScripts.Length; i++) {
					Transform connectedTransform = driveScripts [i].GetComponent <HingeJoint> ().connectedBody.transform;
					MeshFilter meshFilter = driveScripts [i].GetComponent <MeshFilter> ();
					if (connectedTransform != bodyTransform && meshFilter && meshFilter.sharedMesh) { // connected to Suspension && not invisible.
						float tempDist = Vector3.Distance (bodyTransform.position, driveScripts [i].transform.position); // Distance to the MainBody.
						if (driveScripts [i].transform.localEulerAngles.z == 0.0f) { // Left
							if (tempDist < minDistL) {
								closestWheelL = driveScripts [i].transform;
								minDistL = tempDist;
							}
						} else { // Right
							if (tempDist < minDistR) {
								closestWheelR = driveScripts [i].transform;
								minDistR = tempDist;
							}
						}
					}
				}
				if (closestWheelL && closestWheelR) {
					Left_Wheel_Rigidbody = closestWheelL.GetComponent <Rigidbody> ();
					Right_Wheel_Rigidbody = closestWheelR.GetComponent <Rigidbody> ();
				}
			}
			if (Left_Wheel_Rigidbody && Right_Wheel_Rigidbody) {
				leftRadius = Left_Wheel_Rigidbody.GetComponent < SphereCollider > ().radius;
				rightRadius = Right_Wheel_Rigidbody.GetComponent < SphereCollider > ().radius;
			} else {
				Debug.LogError ("Reference Wheels for the engine sound can not be found.");
				Destroy (this);
			}
		}

		void FixedUpdate ()
		{
			switch (Type) {
			case 0:
				Engine_Sound ();
				break;
			case 1:
				if (isCurrent && isPrepared) {
					StartCoroutine ("Impact_Sound");
				}
				break;
			case 2:
				if (isCurrent) {
					Turret_Motor_Sound ();
				}
				break;
			case 3:
				if (isCurrent) {
					Cannon_Motor_Sound ();
				}
				break;
			}
		}

		void Engine_Sound ()
		{
			if (Left_Wheel_Rigidbody) {
				Left_Velocity = Left_Wheel_Rigidbody.angularVelocity.magnitude * leftRadius;
			} else {
				Left_Velocity = 0.0f;
			}
			if (Right_Wheel_Rigidbody) {
				Right_Velocity = Right_Wheel_Rigidbody.angularVelocity.magnitude * rightRadius;
			} else {
				Right_Velocity = 0.0f;
			}
			float targetRate = (Left_Velocity + Right_Velocity) / 2.0f / Max_Velocity;
			currentRate = Mathf.MoveTowards (currentRate, targetRate, 0.75f * Time.fixedDeltaTime);
			thisAudioSource.pitch = Mathf.Lerp (Min_Engine_Pitch, Max_Engine_Pitch, currentRate);
			thisAudioSource.volume = Mathf.Lerp (Min_Engine_Volume, Max_Engine_Volume, currentRate);
		}

		IEnumerator Impact_Sound ()
		{
			float currentVelocity = bodyRigidbody.velocity.y;
			float impact = Mathf.Abs (previousVelocity - currentVelocity);
			if (impact > Min_Impact) {
				isPrepared = false;
				float rate = impact / Max_Impact;
				thisAudioSource.pitch = Mathf.Lerp (Min_Impact_Pitch, Max_Impact_Pitch, rate);
				thisAudioSource.volume = Mathf.Lerp (Min_Impact_Volume, Max_Impact_Volume, rate);
				thisAudioSource.Play ();
				yield return new WaitForSeconds (clipLength);
				isPrepared = true;
			}
			previousVelocity = currentVelocity;
		}

		void Turret_Motor_Sound ()
		{
			if (turretScript.Is_Moving) {
				float targetVolume = Mathf.Lerp (0.0f, Max_Motor_Volume, Mathf.Abs (turretScript.Current_Rate));
				thisAudioSource.volume = Mathf.MoveTowards (thisAudioSource.volume, targetVolume, 0.8f * Time.fixedDeltaTime);
			} else {
				thisAudioSource.volume = 0.0f;
			}
		}

		void Cannon_Motor_Sound ()
		{
			if (cannonScript.Is_Moving) {
				float targetVolume = Mathf.Lerp (0.0f, Max_Motor_Volume, Mathf.Abs (cannonScript.Current_Rate));
				thisAudioSource.volume = Mathf.MoveTowards (thisAudioSource.volume, targetVolume, 0.8f * Time.fixedDeltaTime);
			} else {
				thisAudioSource.volume = 0.0f;
			}
		}

		void TurretBroken_Linkage ()
		{ // Called from "Damage_Control_CS" in Turret.
			thisAudioSource.Stop ();
			Destroy (this);
		}

		void MainBodyBroken_Linkage ()
		{ // Called from "Damage_Control_CS" in MainBody.
			if (Type != 1) { // except for Impact sound.
				thisAudioSource.Stop ();
				Destroy (this);
			}
		}

		void Get_Tank_ID_Control (Tank_ID_Control_CS topScript)
		{
			myID = topScript.Tank_ID;

			bodyRigidbody = topScript.Stored_TankProp.bodyRigidbody;
		}

		void Receive_Current_ID (int id)
		{
			if (id == myID) {
				isCurrent = true;
			} else {
				isCurrent = false;
			}
		}

		void Get_Drive_Control (Drive_Control_CS tempScript)
		{ // Called from "Drive_Control_CS".
			if (Track_Type != 0) { // Except for Physics_Track.
				Max_Velocity = tempScript.Max_Speed;
			}
		}

	}

}