using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Track_Scroll_CS : MonoBehaviour
	{

		public Transform Reference_Wheel; // Referred to from "Static_Wheel_CS".
		public string Reference_Name;
		public string Reference_Parent_Name;
		public float Scroll_Rate = 0.005f;
		public string Tex_Name = "_MainTex";

		Material thisMaterial;
		public int Direction; // Referred to from "Static_Wheel_CS"
		public float Delta_Ang; // Referred to from "Static_Wheel_CS".
		float previousAng;
		float offsetX;
		MainBody_Setting_CS bodyScript;

		void Awake ()
		{
			// Set Reference Wheel.
			if (Reference_Wheel == null) {
				if (string.IsNullOrEmpty (Reference_Name) == false && string.IsNullOrEmpty (Reference_Parent_Name) == false) {
					Reference_Wheel = transform.parent.Find (Reference_Parent_Name + "/" + Reference_Name);
				}
			}
			if (Reference_Wheel) {
				thisMaterial = GetComponent < Renderer > ().material;
				// Set Direction.
				if (Reference_Wheel.localEulerAngles.z == 0.0f) { // Left
					Direction = 0;
				} else { // Right
					Direction = 1;
				}
			} else {
				Debug.LogWarning ("Reference Wheel is not assigned in " + this.name);
				Destroy (this);
			}
		}

		void Start ()
		{
			// Send this reference to all the "Static_Wheel_CS", "Sound_Control_CS"(Engine Sound).
			transform.parent.BroadcastMessage ("Get_Track_Scroll", this, SendMessageOptions.DontRequireReceiver);
		}

		void Update ()
		{
			if (bodyScript.Visible_Flag) { // MainBody is visible by any camera.
				float currentAng = Reference_Wheel.localEulerAngles.y;
				Delta_Ang = Mathf.DeltaAngle (currentAng, previousAng);
				offsetX += Scroll_Rate * Delta_Ang;
				thisMaterial.SetTextureOffset (Tex_Name, new Vector2 (offsetX, 0.0f));
				previousAng = currentAng;
			}
		}

		void TrackBroken_Linkage (int tempDirection)
		{ // Called from "Damage_Control_CS" in Track_Collider.
			if (tempDirection == Direction) {
				Destroy (gameObject);
			}
		}

		void Get_Tank_ID_Control (Tank_ID_Control_CS topScript)
		{
			bodyScript = topScript.Stored_TankProp.bodyScript;
		}
	}

}