using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Damage_Control_CS : MonoBehaviour
	{
	
		public int Type = 1; // 1=Armor_Collider , 2=Turret, 3=Cannon, 4=Barrel, 5=MainBody, 6=Physics Track, 8=Physics_Wheel, 9=Track_Collider.
		public float Mass = 200.0f; // for Turret.
		public int Direction; // for Physics_Track piece, Physics_Wheel, TrackCollider. (0=Left, 1=Right)
		public float Durability = 130000.0f;
		public float Sub_Durability = 100000.0f;
		public float Trouble_Time = 20.0f;
		public bool Coming_Off = true; // for Turret
		public GameObject Damage_Effect_Object; // for Turret
		public GameObject Trouble_Effect_Object; // for Turret, Cannon and Barrel.
		public int Turret_Number = 1; // for MainBody.
		public Transform Linked_Transform; // for Track_Collider.

		bool isChildTurret = false; // for Turret.

		bool isLiving = true;

		void Awake ()
		{
			switch (Type) {
			case 1: // Armor_Collider
				// Make it invisible.
				GetComponent < MeshRenderer > ().enabled = false;
				break;
			case 9: // Track_Collider
				// Make it a trigger and invisible.
				GetComponent < Collider > ().isTrigger = true;
				GetComponent < MeshRenderer > ().enabled = false;
				// Set direction.
				if (transform.parent.GetComponent < Static_Track_CS > ()) { // Static_Track.
					if (transform.localPosition.y > 0.0f) { // Left
						Direction = 0;
					} else { // Right
						Direction = 1;
					}
				} else { // Scroll_Track.
					if (transform.localPosition.x < 0.0f) { // Left
						Direction = 0;
					} else { // Right
						Direction = 1;
					}
				}
				// Change hierarchy.
				transform.parent = transform.parent.parent;
				break;
			}
		}

		public bool Breaker (float hitEnergy)
		{ // Called from "Bullet_Control_CS".
			if (hitEnergy >= Durability) {
				Penetration ();
				return true;
			} else if (hitEnergy >= Sub_Durability) {
				Trouble ();
				return false;
			} else {
				return false;
			}
		}

		void  Penetration ()
		{
			switch (Type) {
			case 1: // Armor_Collider
				Armor_Collider_Broken ();
				break;
			case 2: // Turret
				Turret_Broken ();
				break;
			case 3: // Cannon
				// Send Message to "Turret" under the 'Turret_Base'. (Turret_Base > Cannon_Base > Cannon)
				transform.parent.parent.BroadcastMessage ("Turret_Broken", SendMessageOptions.DontRequireReceiver);
				Destroy (this);
				break;
			case 4: // Barrel
				// Send Message to "Turret" under the 'Turret_Base'. (Turret_Base > Cannon_Base > Barrel_Base > Barrel)
				transform.parent.parent.parent.BroadcastMessage ("Turret_Broken", SendMessageOptions.DontRequireReceiver);
				Destroy (this);
				break;
			case 5: // MainBody
				MainBody_Broken ();
				break;
			case 6: // Physics Track
				PhysicsTrack_Broken ();
				break;
			case 8: // Physics_Wheel
				Physics_Wheel_Broken ();
				break;
			case 9: // Track_Collider
				TrackCollider_Broken ();
				break;
			}
		}

		void Trouble ()
		{
			switch (Type) {
			case 2: // Turret
				Turret_Trouble ();
				break;
			case 3: // Cannon
				Cannon_Trouble ();
				break;
			case 4: // Barrel
				Barrel_Trouble ();
				break;
			}
		}

		void Turret_Trouble ()
		{
			Transform baseTransform = transform.parent ; // Turret_Base
			Turret_Horizontal_CS turretScript = baseTransform.GetComponent < Turret_Horizontal_CS > ();
			if (turretScript && turretScript.Trouble (Trouble_Time)) {
				Create_Trouble_Effect (baseTransform);
			}
		}

		void Cannon_Trouble ()
		{
			Transform baseTransform = transform.parent ; // Cannon_Base
			Cannon_Fire_CS fireScript = baseTransform.GetComponent < Cannon_Fire_CS > ();
			if (fireScript && fireScript.Trouble (Trouble_Time)) {
				Create_Trouble_Effect (baseTransform);
			}
		}

		void Barrel_Trouble ()
		{
			Transform baseTransform = transform.parent.parent ; // Cannon_Base
			Cannon_Fire_CS fireScript = baseTransform.GetComponent < Cannon_Fire_CS > ();
			if (fireScript && fireScript.Trouble (Trouble_Time)) {
				Create_Trouble_Effect (baseTransform);
			}
		}

		void Create_Trouble_Effect ( Transform baseTransform )
		{
			if (Trouble_Effect_Object) {
				GameObject effectObject = Instantiate (Trouble_Effect_Object, baseTransform.position, baseTransform.rotation) as GameObject;
				effectObject.transform.parent = baseTransform;
				// Send message to "Particle_Control_CS" in the effectObject.
				effectObject.SendMessage ("Set_Delete_Count", Trouble_Time);
			}
		}

		void Armor_Collider_Broken ()
		{
			if (isLiving) {
				isLiving = false;
				transform.parent.SendMessage ("Penetration", SendMessageOptions.DontRequireReceiver);
				Destroy (this.gameObject);
			}
		}

		void Turret_Broken ()
		{
			if (Type == 2 && isLiving) { // Turret
				Transform bodyTransform = GetComponentInParent <MainBody_Setting_CS> ().transform;
				isLiving = false;
				Transform baseTransform = transform.parent; // Turret_Base
				// Create Damage Effect.
				if (Damage_Effect_Object) {
					GameObject damageObject = Instantiate (Damage_Effect_Object, baseTransform.position, bodyTransform.rotation) as GameObject;
					damageObject.transform.parent = bodyTransform;
				}
				// Send Message to "Damage_Control_CS" in cannon, barrel and Armor_Collider, "Cannon_Fire_CS", "Cannon_Vertical_CS", "Gun_Camera_CS", "Look_At_Point_CS", "Recoil_Brake_CS", "Sound_Control_CS", "Turret_Horizontal_CS".
				baseTransform.BroadcastMessage ("TurretBroken_Linkage", SendMessageOptions.DontRequireReceiver);
				// Coming off.
				if (Coming_Off) {
					// Add rigidbody to the "Turret_Base".
					if (baseTransform.GetComponent < Rigidbody > () == null) {
						Rigidbody rigidbody = baseTransform.gameObject.AddComponent < Rigidbody > ();
						rigidbody.mass = Mass;
					}
					// Change the hierarchy.
					baseTransform.parent = bodyTransform.parent; // under the top object.
				}
				// Reduce the number of turrets.
				if (isChildTurret == false) {
					// Send Message to "Damage_Control_CS" in MainBody.
					bodyTransform.SendMessage ("Reduce_Turret_Number", SendMessageOptions.DontRequireReceiver);
				}
				// Remove this script.
				Destroy (this);
			}
		}

		void TurretBroken_Linkage ()
		{
			if (isLiving) {
				switch (Type) {
				case 1: // Armor_Collider
					// Remove useless Armor_Collider in the turret.
					Destroy (this.gameObject);
					break;
				case 3: // Cannon
					Destroy (this);
					break;
				case 4: // Barrel
					Destroy (this);
					break;
				}
			}
		}

		void Reduce_Turret_Number ()
		{ // Called from "Damage_Control_CS" in Turret.
			if (isLiving) { // MainBody is alive. Warning!! Do not make a infinite loop!!
				Turret_Number -= 1;
				if (Turret_Number <= 0) {
					MainBody_Broken ();
				}
			}
		}

		void MainBody_Broken ()
		{
			if (isLiving) {
				isLiving = false;
				// Send Message to "Tank_ID_Control_CS", "Damage_Control_CS" in Armor_Collider and Turret, "AI_CS", "Drive_Control_CS", "Drive_Wheel_CS", "Sound_Control_CS", "Stabilizer_CS", "Steer_Wheel_CS".
				transform.parent.BroadcastMessage ("MainBodyBroken_Linkage", SendMessageOptions.DontRequireReceiver);
				// Add NavMeshObstacle.
				if (GetComponent < UnityEngine.AI.NavMeshObstacle > () == null) {
					UnityEngine.AI.NavMeshObstacle navMeshObstacle = gameObject.AddComponent < UnityEngine.AI.NavMeshObstacle > ();
					navMeshObstacle.carvingMoveThreshold = 1.0f;
					navMeshObstacle.carving = true;
				}
				/// Release the parking brake, and Destroy this script.
				StartCoroutine ("Disable_Constraints");
			}
		}

		IEnumerator Disable_Constraints ()
		{
			// Disable constraints of MainBody's rigidbody.
			yield return new WaitForFixedUpdate (); // This wait is required for PhysX.
			Rigidbody rigidBody = GetComponent <Rigidbody> ();
			rigidBody.constraints = RigidbodyConstraints.None;
			Destroy (this);
		}

		void MainBodyBroken_Linkage ()
		{
			if (isLiving) {
				switch (Type) {
				case 1: // Armor_Collider
					// Remove useless Armor_Collider.
					Destroy (this.gameObject);
					break;
				case 2: // Turret
					// Break the turret.
					Turret_Broken () ;
					break;
				}
			}
		}

		void Physics_Wheel_Broken ()
		{
			if (isLiving) {
				isLiving = false;
				// Remove HingeJoint.
				HingeJoint hingeJoint = GetComponent <HingeJoint> ();
				if (hingeJoint) {
					Destroy (hingeJoint);
				}
				// Remove "Drive_Wheel_CS".
				Drive_Wheel_CS driveScrip = GetComponent <Drive_Wheel_CS>();
				if (driveScrip) {
					Destroy (driveScrip);
				}
				// Remove "Stabilizer_CS".
				Stabilizer_CS stabilizerScript = GetComponent <Stabilizer_CS>();
				if (stabilizerScript) {
					Destroy (stabilizerScript);
				}
				// Release the wheel lock.
				Rigidbody rigidbody = GetComponent <Rigidbody>();
				if (rigidbody) {
					rigidbody.angularDrag = 0.05f;
				}
				// Switch the collider.
				MeshCollider[] meshColliders = GetComponents <MeshCollider> ();
				if (meshColliders.Length != 0) {
					// Remove SphereCollider.
					SphereCollider sphereCollider = GetComponent <SphereCollider> ();
					if (sphereCollider) {
						Destroy (sphereCollider);
					}
					// Eneble MeshCollider(s).
					foreach (MeshCollider meshCollider in meshColliders) {
						meshCollider.enabled = true;
					}
				}
				gameObject.layer = 0;
				transform.parent = null;
				Destroy (this.gameObject, 20.0f);
			}
		}

		void PhysicsTrack_Broken ()
		{
			if (isLiving) {
				isLiving = false;
				// Remove HingeJoint.
				HingeJoint hingeJoint = GetComponent < HingeJoint > ();
				if (hingeJoint) {
					Destroy (hingeJoint);
				}
				// Send message to "Damage_Control_CS" and "Track_Joint_CS" in Physics_Track piece, "Drive_Wheel_CS", "Stabilizer_CS".
				transform.parent.parent.BroadcastMessage ("TrackBroken_Linkage", Direction, SendMessageOptions.DontRequireReceiver);
			}
		}

		void TrackCollider_Broken ()
		{
			if (isLiving) {
				isLiving = false;
				// Send message to "Damage_Control_CS" in other Track_Colliders, "Drive_Wheel_CS", "Stabilizer_CS", "Static_Wheel_CS", "Track_Scroll_CS", "Track_LOD_Control_CS".
				transform.parent.parent.BroadcastMessage ("TrackBroken_Linkage", Direction, SendMessageOptions.DontRequireReceiver);
				// Start breaking process in Static_Track.
				if (Linked_Transform) {
					Linked_Transform.SendMessage ("Start_Breaking", SendMessageOptions.DontRequireReceiver);
				}
				Destroy (this.gameObject);
			}
		}

		void TrackBroken_Linkage (int tempDirection)
		{
			if (tempDirection == Direction) {
				switch (Type) {
				case 6: // Physics Track.
					// Remove other pieces in the same side.
					isLiving = false;
					transform.parent = null;
					Destroy (this.gameObject, 20.0f);
					break;
				case 9: // Track_Collider.
					// Remove other Track_Collider in the same side.
					Destroy (this.gameObject);
					break;
				}
			}
		}

		void OnJointBreak ()
		{ // for Physics Track.
			if (Type == 6) { // Physics Track.
				PhysicsTrack_Broken ();
			}
		}

		void Set_ChildTurret ()
		{ // Called from "Turret_Finishing_CS".
			if (Type == 2) { // Turret
				isChildTurret = true;
			}
		}

	}

}