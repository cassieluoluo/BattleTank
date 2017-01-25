using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{
	
	public class Static_Wheel_CS : MonoBehaviour
	{

		public float Radius_Offset;

		Transform thisTransform;
		bool isLeft;
		float staticTrackRate;
		float scrollTrackRate;
		MainBody_Setting_CS bodyScript;
		Static_Track_CS staticTrackScript;
		Track_Scroll_CS scrollTrackScript;

		void Awake ()
		{
			thisTransform = transform;
			if (thisTransform.localEulerAngles.z == 0.0f) {
				isLeft = true; // Left
			} else {
				isLeft = false; // Right
			}
		}

		void Get_Static_Track_Parent (Static_Track_CS script)
		{ // Called from "Static_Track_CS" (parent).
			staticTrackScript = script;
			if (staticTrackScript.Reference_L && staticTrackScript.Reference_R) {
				// Set rate.
				float radius = GetComponent < MeshFilter > ().mesh.bounds.extents.x + Radius_Offset;
				if (isLeft) { // Left
					staticTrackRate = staticTrackScript.Reference_Radius_L / radius;
				} else { // Right
					staticTrackRate = staticTrackScript.Reference_Radius_R / radius;
				}
			} else {
				Debug.LogWarning ("Static_Wheel can not find the reference wheel in the Static_Tracks.");
				Destroy (this);
			}
		}

		void Get_Track_Scroll (Track_Scroll_CS script)
		{
			// Set rate.
			if (script.Reference_Wheel) {
				if ((isLeft && script.Direction == 0) || (isLeft == false && script.Direction == 1)) {
					scrollTrackScript = script;
					float radius = GetComponent < MeshFilter > ().mesh.bounds.extents.x + Radius_Offset;
					float referenceRadius = scrollTrackScript.Reference_Wheel.GetComponent < MeshFilter > ().mesh.bounds.extents.x + Radius_Offset; // Axis X = hight.
					scrollTrackRate = referenceRadius / radius;
					return;
				}
			} else {
				Debug.LogWarning ("Static_Wheel can not find the reference wheel in the Scroll_Tracks.");
				Destroy (this);
			}
		}
		
		void Update ()
		{
			if (bodyScript.Visible_Flag) { // MainBody is visible by any camera.
				if (staticTrackScript && staticTrackScript.isActiveAndEnabled) {
					Work_with_Static_Track ();
				} else if (scrollTrackScript) {
					Work_with_Scroll_Track ();
				}
			}
		}

		void Work_with_Static_Track ()
		{
			Vector3 currentAng = thisTransform.localEulerAngles;
			if (isLeft) {
				currentAng.y -= staticTrackScript.Delta_Ang_L * staticTrackRate;
			} else {
				currentAng.y -= staticTrackScript.Delta_Ang_R * staticTrackRate;
			}
			thisTransform.localEulerAngles = currentAng;
		}

		void Work_with_Scroll_Track ()
		{
			Vector3 currentAng = thisTransform.localEulerAngles;
			currentAng.y -= scrollTrackScript.Delta_Ang* scrollTrackRate;
			thisTransform.localEulerAngles = currentAng;
		}

		void TrackBroken_Linkage (int direction)
		{ // Called from "Damage_Control_CS" in TrackCollider.
			if ((isLeft && direction == 0) || (isLeft == false && direction == 1)) {
				// Resize SphereCollider.
				SphereCollider sphereCollider = GetComponent <SphereCollider> ();
				if (sphereCollider) {
					MeshFilter meshFilter = GetComponent < MeshFilter > ();
					if (meshFilter && meshFilter.mesh) {
						sphereCollider.radius = meshFilter.mesh.bounds.extents.x;
					}
				}
				// Remove this script.
				Destroy (this);
			}
		}

		void Get_Tank_ID_Control (Tank_ID_Control_CS topScript)
		{
			bodyScript = topScript.Stored_TankProp.bodyScript;
		}

	}
		
}