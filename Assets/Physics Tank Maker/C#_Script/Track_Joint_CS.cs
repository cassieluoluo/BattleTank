using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{
	public class Track_Joint_CS : MonoBehaviour
	{

		public Transform Base_Transform;
		public Transform Front_Transform;
		public float Joint_Offset;
		public float Broken_Offset;
		public int Direction; // 0=Left, 1=Right.

		MainBody_Setting_CS bodyScript;
		Transform thisTransform;

		void Awake ()
		{
			thisTransform = transform;
		}

		void Update ()
		{
			if (bodyScript.Visible_Flag) { // MainBody is visible by any camera.
				Vector3 basePos = Base_Transform.position + (Base_Transform.forward * Joint_Offset);
				if (Front_Transform) {
					Vector3 frontPos = Front_Transform.position - (Front_Transform.forward * Joint_Offset);
					thisTransform.position = Vector3.Slerp (basePos, frontPos, 0.5f);
					thisTransform.rotation = Quaternion.Slerp (Base_Transform.rotation, Front_Transform.rotation, 0.5f);
				} else { // Track must be broken.
					thisTransform.position = Base_Transform.position + (Base_Transform.forward * Joint_Offset * 0.5f);
					thisTransform.localEulerAngles = Vector3.zero;
					Destroy (this);
				}
			}
		}

		void TrackBroken_Linkage (int direction)
		{ // Called from "Damage_Control_CS".
			if (Direction == direction) {
				Front_Transform = null;
			}
		}

		public void Set_Value (Transform baseTransform, Transform frontTransform, float offset, string dir)
		{
			Base_Transform = baseTransform;
			Front_Transform = frontTransform;
			Joint_Offset = offset;
			if (dir == "L") {
				Direction = 0;
			} else {
				Direction = 1;
			}
		}

		void Get_Tank_ID_Control (Tank_ID_Control_CS topScript)
		{
			bodyScript = topScript.Stored_TankProp.bodyScript;
		}

	}

}