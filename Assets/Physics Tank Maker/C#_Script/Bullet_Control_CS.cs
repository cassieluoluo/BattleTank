using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Bullet_Control_CS : MonoBehaviour
	{

		public int Type; // 0=AP , 1=HE
		public float Delete_Time;
		public float Explosion_Force;
		public float Explosion_Radius;
		public GameObject Impact_Object;
		public GameObject Ricochet_Object;
		public GameObject Explosion_Object;
		public float Attack_Multiplier = 1.0f;
		public bool Debug_Flag = false;

		Transform thisTransform;
		Rigidbody thisRigidbody;
	
		bool isLiving = true;
		bool isDetecting = true;

		int layerMask = ~((1 << 10) + (1 << 2));
		Vector3 nextPos;
		GameObject rayHitObject;
		Vector3 rayHitNormal;

		void Awake ()
		{ // (Note.) Sometimes OnCollisionEnter() is called earlier than Start().
			thisTransform = transform;
			thisRigidbody = GetComponent < Rigidbody > ();
		}

		void Start ()
		{
			Destroy (this.gameObject, Delete_Time);
		}

		void FixedUpdate ()
		{
			if (isLiving) {
				thisTransform.LookAt (thisTransform.position + thisRigidbody.velocity);
				if (isDetecting) {
					Ray ray = new Ray (thisTransform.position, thisRigidbody.velocity);
					RaycastHit raycastHit;
					Physics.Raycast (ray, out raycastHit, thisRigidbody.velocity.magnitude * Time.fixedDeltaTime, layerMask);
					if (raycastHit.collider) {
						nextPos = raycastHit.point;
						rayHitObject = raycastHit.collider.gameObject;
						rayHitNormal = raycastHit.normal;
						isDetecting = false;
					}
				} else {
					thisTransform.position = nextPos;
					thisRigidbody.position = nextPos;
					Hit (rayHitObject, rayHitNormal);
				}
			}
		}

		void OnCollisionEnter (Collision collision)
		{
			if (isLiving) {
				Hit (collision.gameObject, collision.contacts [0].normal);
			}
		}

		void Hit (GameObject hitObject, Vector3 hitNormal)
		{
			isLiving = false;
			if (Type == 1) { // HE
				Destroy (GetComponent <Renderer> ());
				Destroy (GetComponent < Rigidbody > ());
				Destroy (GetComponent < Collider > ());
				// Create HE Explosion Particle.
				if (Explosion_Object) {
					Instantiate (Explosion_Object, thisTransform.position, Quaternion.identity);
				}
			}
			// Send Message
			if (hitObject) {
				Damage_Control_CS damageScript = hitObject.GetComponent < Damage_Control_CS > ();
				if (damageScript) {
					switch (Type) {
					case 0: // AP
						// Create AP Ricochet Particle.
						if (Ricochet_Object) {
							Instantiate (Ricochet_Object, thisTransform.position, Quaternion.identity);
						}
						// Calculate hitEnergy.
						float hitAngle = Mathf.Abs (90.0f - Vector3.Angle (thisRigidbody.velocity, hitNormal));
						float hitEnergy = 0.5f * thisRigidbody.mass * Mathf.Pow (thisRigidbody.velocity.magnitude, 2);
						hitEnergy *= Mathf.Lerp (0.0f, 1.0f, Mathf.Sqrt (hitAngle / 90.0f));
						hitEnergy *= Attack_Multiplier;
						// Output for debug.
						if (Debug_Flag) {
							Debug.Log ("AP Damage " + hitEnergy + " on " + hitObject.name);
						}
						// Send 'hitEnergy' to "Damage_Control" script.
						if (damageScript.Breaker (hitEnergy)) {
							Destroy (this.gameObject);
						}
						break;
					case 1: // HE
						Explosion_Force *= Attack_Multiplier;
						// Output for debug.
						if (Debug_Flag) {
							Debug.Log ("HE Damage " + Explosion_Force + " on " + hitObject.name);
						}
						Explosion_Process ();
						break;
					}
				} else { // Hit object does not have "Damage_Control" script.
					switch (Type) {
					case 0: // AP
						// Create AP Impact Particle.
						if (Impact_Object) {
							Instantiate (Impact_Object, thisTransform.position, Quaternion.identity);
						}
						break;
					case 1: // HE
						Explosion_Process ();
						break;
					}
				}
			}
		}

		void Explosion_Process ()
		{
			Collider[] colliders = Physics.OverlapSphere (thisTransform.position, Explosion_Radius);
			foreach (Collider collider in colliders) {
				Add_Explosion_Force (collider);
			}
			Destroy (this.gameObject, 0.01f * Explosion_Radius);
		}

		void Add_Explosion_Force (Collider collider)
		{
			if (collider) {
				Vector3 direction = (collider.transform.position - thisTransform.position).normalized;
				Ray ray = new Ray (thisTransform.position, direction);
				RaycastHit raycastHit;
				if (Physics.Raycast (ray, out raycastHit, Explosion_Radius, layerMask)) {
					if (raycastHit.collider == collider) {
						float loss = Mathf.Pow ((Explosion_Radius - raycastHit.distance) / Explosion_Radius, 2);
						// Add force.
						Rigidbody rigidbody = collider.GetComponent < Rigidbody > ();
						if (rigidbody) {
							rigidbody.AddForce (direction * Explosion_Force * loss);
						}
						// Add damage.
						Damage_Control_CS damageScript = collider.GetComponent < Damage_Control_CS > ();
						if (damageScript) {
							damageScript.Breaker (Explosion_Force * loss);
						}
					}
				}
			}
		}

	}

}