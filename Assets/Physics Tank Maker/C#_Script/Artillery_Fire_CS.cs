using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Artillery_Fire_CS : MonoBehaviour
	{
		
		public float Interval_Min;
		public float Interval_Max;
		public float Radius;
		public float Height;
		public float Mass;
		public float Delete_Time;
		public float Explosion_Force;
		public float Explosion_Radius;
		public GameObject Explosion_Object;

		bool isFiring = false;

		IEnumerator Create_Shells (Vector3 targetPos, int totalNum)
		{
			int shellNum = 0;
			float timeCount = 0.0f;
			float currentInterval = Random.Range (Interval_Min, Interval_Max);
			while (shellNum <= totalNum) {
				timeCount += Time.deltaTime;
				if (timeCount > currentInterval) {
					for (int i = 0; i < Mathf.FloorToInt (timeCount / currentInterval); i++) {
						GameObject shellObject = new GameObject ("Artillery_Shell");
						// Set position.
						float randX = Random.Range (0.0f, Radius) * Mathf.Cos (Random.Range (0.0f, 2.0f * Mathf.PI));
						float randZ = Random.Range (0.0f, Radius) * Mathf.Sin (Random.Range (0.0f, 2.0f * Mathf.PI));
						shellObject.transform.position = new Vector3 (targetPos.x + randX, targetPos.y + Height, targetPos.z + randZ);
						// Add component.
						Rigidbody rigidbody = shellObject.AddComponent < Rigidbody > ();
						rigidbody.mass = Mass;
						shellObject.AddComponent < SphereCollider > ();
						// Add Scripts
						Bullet_Control_CS bulletScript = shellObject.AddComponent < Bullet_Control_CS > ();
						bulletScript.Type = 1;
						bulletScript.Delete_Time = Delete_Time;
						bulletScript.Explosion_Force = Explosion_Force;
						bulletScript.Explosion_Radius = Explosion_Radius;
						bulletScript.Explosion_Object = Explosion_Object;
						// Count the shells.
						shellNum += 1;
						if (shellNum >= totalNum) {
							break;
						}
					}
					currentInterval = Random.Range (Interval_Min, Interval_Max);
					timeCount = 0.0f;
				}
				yield return null;
			}
			isFiring = false;
		}

		public void Fire (Transform targetTransform, int num)
		{
			if (isFiring == false) {
				isFiring = true;
				// Set target's position.
				Vector3 targetPos;
				Rigidbody rigidbody = targetTransform.GetComponentInChildren < Rigidbody > ();
				if (rigidbody) {
					targetPos = rigidbody.position;
				} else {
					targetPos = targetTransform.position;
				}
				StartCoroutine (Create_Shells (targetPos, num));
			}
		}

	}

}