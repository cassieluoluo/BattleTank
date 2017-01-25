using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Static_Track_CS : MonoBehaviour
	{

		public int Type; // 0=Static, 1=Anchor, 2=Dynamic, 9=Parent.
		public Transform Front_Transform;
		public Transform Rear_Transform;
		public string Anchor_Name;
		public string Anchor_Parent_Name;
		public Transform Anchor_Transform;
		public Transform Reference_L; // Referred to from "Static_Wheel_CS", "Sound_Control_CS".
		public Transform Reference_R; // Referred to from "Static_Wheel_CS", "Sound_Control_CS".
		public string Reference_Name_L;
		public string Reference_Name_R;
		public string Reference_Parent_Name_L;
		public string Reference_Parent_Name_R;
		public float Length;
		public float Radius_Offset;
		public float Mass = 30.0f;
		public Mesh Track_L_Shadow_Mesh;
		public Mesh Track_R_Shadow_Mesh;
		public float SwingBall_Effective_Range = 0.15f;

		// Set by "Create_TrackBelt_CSEditor".
		public RoadWheelsProp [] RoadWheelsProp_Array;
		public float Stored_Body_Mass;
		public float Stored_Torque;
		public float Stored_Turn_Brake_Drag;

		// Referred to from "Static_Wheel_CS".
		public float Reference_Radius_L;
		public float Reference_Radius_R;
		public float Delta_Ang_L;
		public float Delta_Ang_R;

		Transform thisTransform;
		bool isLeft; // Left = true.
		float invertValue; // Lower piece = 0.0f, Upper pieces = 180.0f.
		public bool simpleFlag = false;
		public float Rate_L;
		public float Rate_R;
		Vector3 invisiblePos;
		float invisibleAngY;
		Static_Track_CS frontScript;
		Static_Track_CS rearScript;
		Static_Track_CS parentScript;
		float halfLength;
		MainBody_Setting_CS bodyScript;
		// only for Anchor.
		float initialPosX;
		float anchorInitialPosX;
		// only for Parent.
		float leftPreviousAng;
		float rightPreviousAng;
		float leftAngRate;
		float rightAngRate;
		int piecesCount;

		void Awake ()
		{
			thisTransform = transform;
			// Start initial settings.
			switch (Type) {
			case 0: // Static.
				Initial_Settings ();
				break;
			case 1: // Anchor.
				Find_Anchor ();
				Initial_Settings ();
				break;
			case 2: // Dynamic.
				Initial_Settings ();
				break;
			case 9: // Parent.
				Parent_Settings ();
				break;
			}
		}

		void Start ()
		{
			if (Type == 9) {
				// Send this reference to all the "Static_Track_CS" in children, "Static_Wheel_CS", "Sound_Control_CS"(Engine Sound).
				thisTransform.parent.BroadcastMessage ("Get_Static_Track_Parent", this, SendMessageOptions.DontRequireReceiver);
			}
		}

		void Parent_Settings ()
		{
			Transform bodyTransform = thisTransform.parent;
			// Find Reference Wheels.
			if (Reference_L == null) { // Left Reference Wheel is lost.
				if (string.IsNullOrEmpty (Reference_Name_L) == false && string.IsNullOrEmpty (Reference_Parent_Name_L) == false) {
					Reference_L = bodyTransform.Find (Reference_Parent_Name_L + "/" + Reference_Name_L);
				}
			}
			if (Reference_R == null) { // Right Reference Wheel is lost.
				if (string.IsNullOrEmpty (Reference_Name_R) == false && string.IsNullOrEmpty (Reference_Parent_Name_R) == false) {
					Reference_R = bodyTransform.Find (Reference_Parent_Name_R + "/" + Reference_Name_R);
				}
			}
			// Set "Reference_Radius" and "Rate_Ang".
			if (Reference_L && Reference_R) {
				Reference_Radius_L = Reference_L.GetComponent < MeshFilter > ().mesh.bounds.extents.x + Radius_Offset; // Axis X = hight.
				leftAngRate = 360.0f / ((2.0f * Mathf.PI * Reference_Radius_L) / Length);
				Reference_Radius_R = Reference_R.GetComponent < MeshFilter > ().mesh.bounds.extents.x + Radius_Offset; // Axis X = hight.
				rightAngRate = 360.0f / ((2.0f * Mathf.PI * Reference_Radius_R) / Length);
			} else {
				Debug.LogError ("'Reference Wheels' for Static_Track cannot be found. " + thisTransform.root.name);
				this.enabled = false;
			}
			// Set "halfLength" for children.
			halfLength = Length * 0.5f;
			// Set piecesCount.
			Static_Track_CS [] childScripts = thisTransform.GetComponentsInChildren < Static_Track_CS > ();
			piecesCount = (childScripts.Length - 1 ) / 2;
		}

		void Initial_Settings ()
		{
			// Set initial position and angle.
			invisiblePos = thisTransform.localPosition;
			invisibleAngY = thisTransform.localEulerAngles.y ;
			// Set direction.
			if (invisiblePos.y > 0.0f) {
				isLeft = true; // Left
			} else {
				isLeft = false; // Right
			}
			// Set invertValue.
			if (invisibleAngY > 90.0f && invisibleAngY < 270.0f) {  // Upper piece
				invertValue = 180.0f;
			} else { // Lower piece
				invertValue = 0.0f;
			}
			// Find front, rear and parent scripts.
			if (Front_Transform) {
				frontScript = Front_Transform.GetComponent < Static_Track_CS > ();
			}
			if (Rear_Transform) {
				rearScript = Rear_Transform.GetComponent < Static_Track_CS > ();
			}
			// Set simpleFlag.
			switch (Type) {
			case 1: // Anchor
				if (frontScript.Type == 1 && rearScript.Type == 1) {
					simpleFlag = false;
				} else if (frontScript.Type == 1 && (frontScript.Anchor_Parent_Name + frontScript.Anchor_Name) != (Anchor_Parent_Name + Anchor_Name)) {
					simpleFlag = false;
				} else if (rearScript.Type == 1 && (rearScript.Anchor_Parent_Name + rearScript.Anchor_Name) != (Anchor_Parent_Name + Anchor_Name)) {
					simpleFlag = false;
				} else {
					simpleFlag = true;
				}
				break;
			case 2: // Dynamic
				if (frontScript.Type == 2 && rearScript.Type == 2) {
					simpleFlag = true;
				}
				break;
			}
		}
			
		void Find_Anchor ()
		{
			if (Anchor_Transform == null) { // Anchor_Transform is lost.
				// Find Anchor with reference to the name.
				if (string.IsNullOrEmpty (Anchor_Name) == false && string.IsNullOrEmpty (Anchor_Parent_Name) == false) {
					Anchor_Transform = thisTransform.parent.parent.Find (Anchor_Parent_Name + "/" + Anchor_Name);
				}
			}
			// Set initial hight.
			if (Anchor_Transform) {
				initialPosX = thisTransform.localPosition.x; // Axis X = hight.
				anchorInitialPosX = Anchor_Transform.localPosition.x; // Axis X = hight.
			} else {
				Type = 2; // change the Type to 'Dynamic'.
			}
		}

		void Get_Static_Track_Parent (Static_Track_CS trackScript)
		{ // Called from the parent's "Static_Track_CS".
			if (Type != 9) { // Only children.
				parentScript = trackScript;
				halfLength = parentScript.halfLength;
			}
		}


		void Update ()
		{
			if (Type == 9 && bodyScript.Visible_Flag) { // Parent && MainBody is visible by any camera.
				Speed_Control ();
			}
		}

		void LateUpdate ()
		{
			if (bodyScript.Visible_Flag) { // MainBody is visible by any camera.
				switch (Type) {
				case 0: // Static.
					Slide_Control ();
					break;
				case 1: // Anchor.
					Anchor_Control ();
					Slide_Control ();
					break;
				case 2: // Dynamic.
					Dynamic_Control ();
					Slide_Control ();
					break;
				}
			}
		}

		void Anchor_Control ()
		{
			// Set position.
			invisiblePos.x = initialPosX + (Anchor_Transform.localPosition.x - anchorInitialPosX);  // Axis X = hight.
			// Set angle.
			if (simpleFlag == false) {
				// Calculate end positions.
				float tempRad = rearScript.invisibleAngY * Mathf.Deg2Rad;
				Vector3 rearEndPos = rearScript.invisiblePos + new Vector3 (halfLength * Mathf.Sin (tempRad), 0.0f, halfLength * Mathf.Cos (tempRad));
				tempRad = frontScript.invisibleAngY * Mathf.Deg2Rad;
				Vector3 frontEndPos = frontScript.invisiblePos - new Vector3 (halfLength * Mathf.Sin (tempRad), 0.0f, halfLength * Mathf.Cos (tempRad));
				// Set angle.
				invisibleAngY = Mathf.Rad2Deg * Mathf.Atan ((frontEndPos.x - rearEndPos.x) / (frontEndPos.z - rearEndPos.z)) + invertValue;
			}
		}

		void Dynamic_Control ()
		{
			if (simpleFlag) {
				// Set position.
				invisiblePos = Vector3.Lerp (rearScript.invisiblePos, frontScript.invisiblePos, 0.5f);
				// Set angle.
				invisibleAngY = Mathf.LerpAngle (rearScript.invisibleAngY, frontScript.invisibleAngY, 0.5f);
			} else {
				// Calculate end positions.
				float tempRad = rearScript.invisibleAngY * Mathf.Deg2Rad;
				Vector3 rearEndPos = rearScript.invisiblePos + new Vector3 (halfLength * Mathf.Sin (tempRad), 0.0f, halfLength * Mathf.Cos (tempRad));
				tempRad = frontScript.invisibleAngY * Mathf.Deg2Rad;
				Vector3 frontEndPos = frontScript.invisiblePos - new Vector3 (halfLength * Mathf.Sin (tempRad), 0.0f, halfLength * Mathf.Cos (tempRad));
				// Set position.
				invisiblePos = Vector3.Lerp (rearEndPos, frontEndPos, 0.5f);
				// Set angle.
				invisibleAngY = Mathf.Rad2Deg * Mathf.Atan ((frontEndPos.x - rearEndPos.x) / (frontEndPos.z - rearEndPos.z)) + invertValue;
			}
		}

		void Slide_Control ()
		{
			if (isLeft) { // Left
				thisTransform.localPosition = Vector3.Lerp (invisiblePos, rearScript.invisiblePos, parentScript.Rate_L);
				thisTransform.localRotation = Quaternion.Euler (new Vector3 (0.0f, Mathf.LerpAngle (invisibleAngY, rearScript.invisibleAngY, parentScript.Rate_L), 270.0f));
			} else { // Right
				thisTransform.localPosition = Vector3.Lerp (invisiblePos, rearScript.invisiblePos, parentScript.Rate_R);
				thisTransform.localRotation = Quaternion.Euler (new Vector3 (0.0f, Mathf.LerpAngle (invisibleAngY, rearScript.invisibleAngY, parentScript.Rate_R), 270.0f));
			}
		}

		public bool Switch_Mesh_L;
		public bool Switch_Mesh_R;
		void Speed_Control ()
		{
			// Left
			float currentAng = Reference_L.localEulerAngles.y;
			Delta_Ang_L = Mathf.DeltaAngle (currentAng, leftPreviousAng); // Also referred to from Static_Wheels.
			//Delta_Ang_L = Mathf.Clamp (Delta_Ang_L, -leftAngRate * 0.25f, leftAngRate * 0.25f); // Anti Stroboscopic Effect.
			Rate_L += Delta_Ang_L / leftAngRate;
			if (Rate_L > 1.0f) {
				Rate_L %= 1.0f;
				Switch_Mesh_L = !Switch_Mesh_L;
			} else if (Rate_L < 0.0f) {
				Rate_L = 1.0f + (Rate_L % 1.0f);
				Switch_Mesh_L = !Switch_Mesh_L;
			}
			leftPreviousAng = currentAng;
			// Right
			currentAng = Reference_R.localEulerAngles.y;
			Delta_Ang_R = Mathf.DeltaAngle (currentAng, rightPreviousAng); // Also referred to from Static_Wheels.
			//Delta_Ang_R = Mathf.Clamp (Delta_Ang_R, -rightAngRate * 0.25f, rightAngRate * 0.25f); // Anti Stroboscopic Effect.
			Rate_R += Delta_Ang_R / rightAngRate;
			if (Rate_R > 1.0f) {
				Rate_R %= 1.0f;
				Switch_Mesh_R = !Switch_Mesh_R;
			} else if (Rate_R < 0.0f) {
				Rate_R = 1.0f + (Rate_R % 1.0f);
				Switch_Mesh_R = !Switch_Mesh_R;
			}
			rightPreviousAng = currentAng;
		}
			
		void Start_Breaking ()
		{ // Called from "Damage_Control_CS" in TrackCollider.
			if (this.enabled) {
				// Reset parent script.
				if (parentScript) {
					parentScript.Rate_L = 0.0f;
					parentScript.Rate_R = 0.0f;
				}
				// Add Components into this piece.
				Add_Components (thisTransform);
				// Add Components into other pieces.
				Static_Track_CS tempScript = this;
				for (int i = 0; i < parentScript.piecesCount; i++) {
					Add_Components (tempScript.Front_Transform);
					tempScript = tempScript.frontScript;
				}
				// Add HingeJoint into front pieces.
				tempScript = this;
				for (int i = 0; i < parentScript.piecesCount - 1; i++) { // Add HingeJoint except for this piece.
					Add_HingeJoint (tempScript.Front_Transform, tempScript.frontScript.Front_Transform);
					tempScript = tempScript.frontScript;
				}
				// Disable and Destroy the pieces on the same side.
				thisTransform.parent.BroadcastMessage ("Disable_and_Destroy", isLeft, SendMessageOptions.DontRequireReceiver);
			}
		}

		void Add_Components (Transform tempTransform)
		{
			// Add RigidBody.
			if (tempTransform.GetComponent < Rigidbody > () == null) {
				Rigidbody rigidbody = tempTransform.gameObject.AddComponent < Rigidbody > ();
				rigidbody.mass = Mass;
			}
			// Enable BoxCollider.
			BoxCollider boxCollider = tempTransform.GetComponent < BoxCollider > ();
			boxCollider.enabled = true;
		}

		void Add_HingeJoint (Transform baseTransform, Transform connectedTransform)
		{
			HingeJoint hingeJoint = baseTransform.gameObject.AddComponent < HingeJoint > ();
			hingeJoint.connectedBody = connectedTransform.GetComponent < Rigidbody > ();
			float anchorZ = baseTransform.GetComponent < BoxCollider > ().size.z * 0.5f;
			hingeJoint.anchor = new Vector3 (0.0f, 0.0f, anchorZ);
			hingeJoint.axis = new Vector3 (1.0f, 0.0f, 0.0f);
			hingeJoint.breakForce = 30000.0f;
		}

		void Disable_and_Destroy (bool direction)
		{
			if (Type != 9 && direction == isLeft) {
				this.enabled = false;
				//yield return new WaitForSeconds (1.0f);
				thisTransform.parent = null;
				Destroy (this.gameObject, 20.0f);
			}
		}

		void Get_Tank_ID_Control (Tank_ID_Control_CS topScript)
		{
			bodyScript = topScript.Stored_TankProp.bodyScript;
		}
	}

}