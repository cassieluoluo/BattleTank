using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{
	public class Reticle_Control_CS : MonoBehaviour
	{
		public ReticleWheel_Control_CS ReticleWheel_Script;

		Image reticleImage;
		Vector2 refResolution;

		void Awake ()
		{
			reticleImage = GetComponent <Image> ();
			reticleImage.enabled = false;
			refResolution = transform.parent.GetComponent <CanvasScaler> ().referenceResolution;
		}
	
		public void Change_GunCam_Mode (int mode, Gun_Camera_CS gunCamScript)
		{ // Called from "Gun_Camera_CS".
			switch (mode) {
			case 0: // Off
				reticleImage.enabled = false;
				break;
			case 1: // Small window.
				reticleImage.enabled = true;
				reticleImage.transform.localScale = new Vector3 (gunCamScript.Small_Width, gunCamScript.Small_Height, 1.0f);
				reticleImage.transform.localPosition = new Vector3 ((-refResolution.x * 0.5f) + (refResolution.x * gunCamScript.Small_Width * 0.5f), (-refResolution.y * 0.5f) + (refResolution.y * gunCamScript.Small_Height * 0.5f), 0.0f);
				break;
			case 2: // Full screen.
				reticleImage.enabled = true;
				reticleImage.transform.localScale = Vector3.one;
				reticleImage.transform.localPosition = Vector3.zero;
				break;
			}
			// Send message to "ReticleWheel_Control_CS" in the Reticle-Wheel.
			if (ReticleWheel_Script) {
				ReticleWheel_Script.Change_GunCam_Mode (mode, gunCamScript);
			}
		}
	}
}

