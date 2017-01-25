using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{
	
	public class Static_Track_Setting_CS : MonoBehaviour
	{

		public bool Use_2ndPiece;

		Transform frontTransform;
		Transform rearTransform;

		int type;
		string anchorName;
		string anchorParentName;

		Transform thisTransform;
		Rigidbody bodyRigidbody;
		float count;

		void Start ()
		{
			thisTransform = transform;
			Transform parentTransform = thisTransform.parent;
			string baseName = this.name.Substring (0, 12); // e.g. "TrackBelt_L_"
			int thisNum = int.Parse (this.name.Substring (12)); // e.g. "1"
			// Find front piece.
			frontTransform = parentTransform.Find (baseName + (thisNum + 1)); // Find a piece having next number.
			if (frontTransform == null) { // It must be the last piece.
				frontTransform = parentTransform.Find (baseName + 1); // The 1st piece.
			}
			// Find rear piece.
			rearTransform = parentTransform.Find (baseName + (thisNum - 1)); // Find a piece having previous number.
			if (rearTransform == null) { // It must be the 1st piece.
				rearTransform = parentTransform.Find (baseName + (transform.parent.childCount / 2)); // The last piece.
			}
			// Find MainBody's Rigidbody.
			bodyRigidbody = parentTransform.parent.GetComponent <Rigidbody> ();
		}
			
		void Update ()
		{
			if (thisTransform.parent) {
				Set_Type ();
				if (bodyRigidbody.velocity.magnitude < 0.1f) {
					count += Time.deltaTime;
					if (count > 1.0f) {
						Time.timeScale = 0.0f;
					}
				}

			} else { // Tracks may be broken.
				Destroy (this);
			}
		}

		void Set_Type ()
		{
			// Detect RoadWheel.
			int layerMask = ~((1 << 10) + (1 << 2)); // Layer 2 = Ignore Ray, Layer 10 = Ignore All.
			Collider[] hitColliders = Physics.OverlapSphere (thisTransform.position, 0.1f, layerMask);
			foreach (Collider hitCollider in hitColliders) {
				Transform tempParent = hitCollider.transform.parent;
				if (tempParent) {
					if (tempParent.GetComponent <Create_RoadWheel_CS> () || tempParent.GetComponent <Create_RoadWheel_Type89_CS> ()) {
						type = 1; // Anchor type
						anchorName = hitCollider.transform.name;
						anchorParentName = hitCollider.transform.parent.name;
						return;
					} else if (tempParent.GetComponent <Create_SprocketWheel_CS> () || tempParent.GetComponent <Create_IdlerWheel_CS> () || tempParent.GetComponent <Create_SupportWheel_CS> ()) {
						type = 0; // Static type
						anchorName = null;
						anchorParentName = null;
						return;
					}
				}
			}
			// cannot detect any wheel.
			type = 2; // Dynamic type
			anchorName = null;
			anchorParentName = null;
		}

		void Set_Static_Track_Value ()
		{ // Called from "Create_TrackBelt_CSEditor".
			// Add "Static_Track_CS" and set values, and disable it.
			Static_Track_CS trackScript = gameObject.AddComponent < Static_Track_CS > ();
			trackScript.Front_Transform = frontTransform;
			trackScript.Rear_Transform = rearTransform;
			switch (type) {
			case 0: // Static
				if (frontTransform.GetComponent < Static_Track_Setting_CS > ().type == 1) { // The front piece is Anchor type.
					trackScript.Type = 2; // >> Dynamic
				} else if (rearTransform.GetComponent < Static_Track_Setting_CS > ().type == 1) { // The rear piece is Anchor type.
					trackScript.Type = 2; // >> Dynamic
				} else {
					trackScript.Type = 0; // Static
				}
				break;
			case 1: // Anchor
				trackScript.Type = 1; // Anchor
				trackScript.Anchor_Name = anchorName;
				trackScript.Anchor_Parent_Name = anchorParentName;
				break;
			case 2: // Dynamic
				trackScript.Type = 2; // Dynamic
				break;
			}
			trackScript.enabled = false;
			// Add "Static_Track_Switch_Mesh_CS", and disable it.
			if (Use_2ndPiece) {
				Static_Track_Switch_Mesh_CS switchScript = gameObject.AddComponent < Static_Track_Switch_Mesh_CS > ();
				switchScript.enabled = false;
			}
			// Remove needless components.
			HingeJoint hingeJoint = GetComponent < HingeJoint > ();
			if (hingeJoint) {
				Destroy (hingeJoint);
			}
			Rigidbody rigidbody = GetComponent < Rigidbody > ();
			if (rigidbody) {
				Destroy (rigidbody);
			}
			Damage_Control_CS damageScript = GetComponent < Damage_Control_CS > ();
			if (damageScript) {
				Destroy (damageScript);
			}
			Stabilizer_CS stabilizerScript = GetComponent < Stabilizer_CS > ();
			if (stabilizerScript) {
				Destroy (stabilizerScript);
			}
			// Disable BoxCollider.
			BoxCollider boxCollider = GetComponent < BoxCollider > ();
			if (boxCollider) {
				boxCollider.enabled = false;
			}
			// Remove child objects such as Reinforce piece.
			for (int i = 0; i < transform.childCount; i++) {
				GameObject childObject = transform.GetChild (i).gameObject;
				if (childObject.layer == 10) { // Reinforce.
					Destroy (childObject);
				}
			}
			// Destroy this script/
			Destroy (this);
		}

	}

}