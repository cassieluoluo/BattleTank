using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Particle_Control_CS : MonoBehaviour
	{
	
		Transform thisTransform;
		ParticleSystem thisParticleSystem;
		AudioSource audioSource;
		Light thisLight;


		void Awake ()
		{
			thisTransform = transform;
			thisParticleSystem = GetComponent < ParticleSystem > ();
			audioSource = GetComponent < AudioSource > ();
			if (audioSource) {
				audioSource.playOnAwake = false;
			}
			thisLight = GetComponent < Light > ();
			if (thisLight) {
				StartCoroutine ("Flash");
			}
			if (audioSource && Camera.main) {
				float distance = Vector3.Distance (thisTransform.position, Camera.main.transform.position);
				audioSource.pitch = Mathf.Lerp (1.0f, 0.1f, distance / audioSource.maxDistance);
				audioSource.PlayDelayed (distance / 340.29f * Time.timeScale);
			}
		}

		void Update ()
		{
			if (thisParticleSystem.isStopped) {
				if (audioSource && audioSource.isPlaying) {
					return;
				}
				Destroy (this.gameObject);
			}
		}

		IEnumerator Flash ()
		{
			thisLight.enabled = true;
			yield return new WaitForSeconds (0.08f);
			thisLight.enabled = false;
		}

		void Set_Delete_Count (float count)
		{ // Called from "Damage_Control_CS" in the Turret, Cannon or Barrel.
			Destroy (this.gameObject, count);
		}

	}

}