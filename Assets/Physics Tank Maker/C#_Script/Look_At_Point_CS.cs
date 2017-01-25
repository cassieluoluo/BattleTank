using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Look_At_Point_CS : MonoBehaviour
	{

		public float Offset_X = 0.0f;
		public float Offset_Y = -1.0f;
		public float Offset_Z = 0.75f;
		public float Horizontal_Speed = 3.0f;
		public float Vertical_Speed = 2.0f;
		public bool Invert_Flag = false;
	
		Transform thisTransform;
		Vector3 initialPos;
		float angY;
		float angZ;
		bool isTPV = true;
		float horizontal;
		float vertical;
		int invertNum = 1;
		Camera mainCamera;
		Transform bodyTransform;

		bool isCurrent;
		int myID;
		int inputType = 4;

		void Awake ()
		{
			thisTransform = transform;
			initialPos = thisTransform.localPosition;
			thisTransform.localPosition = initialPos + new Vector3 (Offset_X, Offset_Y, Offset_Z);
			angY = thisTransform.eulerAngles.y;
			angZ = thisTransform.eulerAngles.z;
			if (Invert_Flag) {
				invertNum = -1;
			} else {
				invertNum = 1;
			}
			mainCamera = GetComponentInChildren <Camera> ();
			if (mainCamera == null) {
				Debug.LogError ("'Main Camera' must be placed under the 'Look_At_Point' in the hierarchy.");
				Destroy (this);
			}
		}

		void Update ()
		{
			if (isCurrent && mainCamera.enabled) {
				switch (inputType) {
				case 0:
					KeyBoard_Input ();
					break;
				case 1:
					GamePad_Input ();
					break;
				case 2:
					KeyBoard_Input ();
					break;
				case 3:
					GamePad_Input ();
					break;
				case 4:
					Mouse_Input ();
					break;
				case 5:
					Mouse_Input ();
					break;
				case 6:
					Mouse_Input ();
					break;
				case 10:
					Mouse_Input ();
					break;
				case 11:
					Auto_Input ();
					break;
				}
				if (isTPV) {
					Rotate_TPV ();
				} else {
					Rotate_FPV ();
				}
			}
		}

		void KeyBoard_Input ()
		{
			horizontal = Input.GetAxis ("Horizontal2");
			vertical = Input.GetAxis ("Vertical2");
		}

		void GamePad_Input ()
		{
			if (Input.GetButton ("L_Button") == false) {
				horizontal = Input.GetAxis ("Horizontal2");
				vertical = Input.GetAxis ("Vertical2");
			} else {
				horizontal = 0.0f;
				vertical = 0.0f;
			}
		}
			
		Vector2 previousMousePos;
		void Mouse_Input ()
		{
			if (Input.GetMouseButtonDown (1)) {
				previousMousePos = Input.mousePosition;
			} else if (Input.GetMouseButton (1)) {
				horizontal = (Input.mousePosition.x - previousMousePos.x) * 0.1f;
				vertical = (Input.mousePosition.y - previousMousePos.y) * 0.1f;
				previousMousePos = Input.mousePosition;
			} else {
				horizontal = 0.0f;
				vertical = 0.0f;
			}
		}

		public bool Demo_Cam = false;
		float targetAng_Z;
		float minHight = 0.5f;
		float adjustingAng;
		void Auto_Input ()
		{
			horizontal = Mathf.Sin (Time.frameCount * 0.002f) * 0.1f;
			// calculate "vertical".
			if (isTPV) { // TPV v
				Check_Height ();
				if (adjustingAng == 0.0f) { // High enough.
					float deltaAng = Mathf.DeltaAngle (angZ, targetAng_Z);
					if (Mathf.Abs (deltaAng) < 0.1f) {
						targetAng_Z = Random.Range (0.0f, 10.0f); // Update target angle.
						return;
					}
					if (deltaAng > 0.0f) { // Lower case
						vertical = Mathf.Lerp (0.0f, -1.0f, deltaAng / 5.0f) * Time.deltaTime;
					} else { // Upper case
						vertical = Mathf.Lerp (0.0f, 1.0f, -deltaAng / 5.0f) * Time.deltaTime;
					}
				} else { // Too low.
					vertical = 0.0f;
					//targetAng_Z = Random.Range (5.0f, 10.0f); // Update target angle.
				}
			} else { // FPV
				vertical = 0.0f;
			}
		}

		void Check_Height ()
		{ // Check the camera height, and calculate the adjusting angle.
			Ray ray = new Ray (mainCamera.transform.position, Vector3.down); // Ray Down
			RaycastHit raycastHit;
			if (Physics.Raycast (ray, out raycastHit, 100.0f)) { // Ray hits anythig.
				if (raycastHit.distance < minHight) { // Lower case
					adjustingAng = Mathf.Lerp (0.0f, 30.0f, (minHight - raycastHit.distance) / 1.0f) * Time.deltaTime;
				} else { // Upper case
					adjustingAng = 0.0f;
				}
			} else {  // Ray does not hit anythig. Ray must be under the terrain.
				adjustingAng = 90.0f * Time.deltaTime;
			}
		}
			
		void Rotate_TPV ()
		{
			angY += horizontal * Horizontal_Speed;
			angZ -= vertical * Vertical_Speed * invertNum;
			angZ += adjustingAng;
			thisTransform.eulerAngles = new Vector3 (0.0f, angY, angZ);
		}
			
		void Rotate_FPV ()
		{
			Vector3 tempAngles = thisTransform.localEulerAngles;
			tempAngles.y += horizontal * Horizontal_Speed;
			tempAngles.z -= vertical * Vertical_Speed * invertNum;
			thisTransform.localEulerAngles = tempAngles;
		}

		void Switch_View (bool flag)
		{ // Called from Main_Camera (Camera_Distance_CS).
			isTPV = flag;
			if (isTPV) { // Third Person View
				angY = thisTransform.eulerAngles.y;
				angZ = thisTransform.eulerAngles.z;
				thisTransform.localPosition = initialPos + new Vector3 (Offset_X, Offset_Y, Offset_Z);
			} else { // First Person View
				thisTransform.localEulerAngles = new Vector3 (0.0f, 90.0f, 0.0f);
				thisTransform.localPosition = initialPos;
			}	
		}

		void TurretBroken_Linkage ()
		{ // Called from "Damage_Control_CS" in Turret.
			thisTransform.parent = bodyTransform; // Change the parent to MainBody.
		}

		void Get_Tank_ID_Control (Tank_ID_Control_CS topScript)
		{
			inputType = topScript.Input_Type;
			myID = topScript.Tank_ID;

			if (Demo_Cam) {
				inputType = 11;
			}
			bodyTransform = topScript.Stored_TankProp.bodyTransform;
		}

		void Receive_Current_ID (int id)
		{
			if (id == myID) {
				isCurrent = true;
			} else {
				isCurrent = false;
			}
		}

	}

}