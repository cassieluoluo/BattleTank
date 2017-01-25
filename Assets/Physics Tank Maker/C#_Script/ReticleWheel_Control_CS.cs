using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{
	public class ReticleWheel_Control_CS : MonoBehaviour
	{
		public float Speed = 1000.0f;
		public float Max_Distance = 4000.0f;
		public float Multiplier = 2.0f;

		Image reticleWheelImage;
		Turret_Horizontal_CS turretScript;
		float currentDist;
		Transform thisTransform;
		Transform gunCamTransform;

		void Awake ()
		{
			thisTransform = transform;
			reticleWheelImage = GetComponent <Image> ();
			reticleWheelImage.enabled = false;
		}

		void Update ()
		{
			if (reticleWheelImage.enabled) {
				float targetDist = Vector3.Distance (gunCamTransform.position, turretScript.Target_Pos) * Multiplier;
				currentDist = Mathf.MoveTowards (currentDist, targetDist, Speed * Mathf.Lerp (0.0f, 1.0f, Mathf.Abs(targetDist - currentDist) / 500.0f) * Time.deltaTime);
				thisTransform.localEulerAngles = new Vector3 (0.0f, 0.0f, (currentDist / Max_Distance) * 180.0f);
			}
		}

		public void Change_GunCam_Mode (int mode, Gun_Camera_CS gunCamScript)
		{ // Called from "Reticle_Control_CS".
			switch (mode) {
			case 0: // Off
				reticleWheelImage.enabled = false;
				break;
			case 1: // Small window.
				reticleWheelImage.enabled = true;
				break;
			case 2: // Full screen.
				reticleWheelImage.enabled = true;
				break;
			}
			turretScript = gunCamScript.Turret_Script;
			gunCamTransform = gunCamScript.transform;
		}
	}
}
