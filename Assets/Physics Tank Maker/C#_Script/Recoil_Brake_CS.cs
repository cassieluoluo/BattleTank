using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Recoil_Brake_CS : MonoBehaviour
	{
	
		public float Total_Time = 1.0f;
		public float Recoil_Length = 0.4f;
		public AnimationCurve Motion_Curve;

		public int Barrel_Type = 0; // Set by "Barrel_Base".

		bool isReady = true;
		Transform thisTransform;
		Vector3 initialPos;


		void Awake ()
		{
			thisTransform = transform;
			if (Recoil_Length !=0.0f && Motion_Curve.keys.Length < 3) { // Motion_Curve is not set yet.
				Create_Curve ();
			}
		}

		void Create_Curve ()
		{ // Create temporary Curve.
			Debug.LogWarning ("'Motion Curve' is not set correctly in 'Recoil_Brake_CS'."); 
			Keyframe key1 = new Keyframe (0.0f, 0.0f, 11.0f, 11.0f);
			Keyframe key2 = new Keyframe (0.2f, 1.0f, 0.01895372f, 0.01895372f);
			Keyframe key3 = new Keyframe (1.0f, 0.0f, -0.02f, -0.02f);
			Motion_Curve = new AnimationCurve (key1, key2, key3);
		}

		void Start ()
		{ // After changing the hierarchy.
			initialPos = thisTransform.localPosition;
		}

		void Fire_Linkage (int direction)
		{
			if (isReady && (Barrel_Type == 0 || Barrel_Type == direction)) {
				isReady = false;
				StartCoroutine ("Recoil_Brake");
			}
		}

		IEnumerator Recoil_Brake ()
		{
			float tempTime = 0.0f;
			while (tempTime < Total_Time) {
				float rate = Motion_Curve.Evaluate (tempTime / Total_Time);
				thisTransform.localPosition = new Vector3 (initialPos.x, initialPos.y, initialPos.z - (rate * Recoil_Length));
				tempTime += Time.deltaTime;
				yield return null;
			}
			thisTransform.localPosition = initialPos;
			isReady = true;
		}

		void Show_Keyframes ()
		{ // Use for debugging
			for (int i = 0; i < Motion_Curve.keys.Length; i++) {
				Debug.Log ("Number" + i);
				Debug.Log (Motion_Curve.keys [i].time);
				Debug.Log (Motion_Curve.keys [i].value);
				Debug.Log (Motion_Curve.keys [i].inTangent);
				Debug.Log (Motion_Curve.keys [i].outTangent);
			}
		}

		void TurretBroken_Linkage ()
		{ // Called from "Damage_Control_CS" in Turret.
			Destroy (this);
		}

	}

}