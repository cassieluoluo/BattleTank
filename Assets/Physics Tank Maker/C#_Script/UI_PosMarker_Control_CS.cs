using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{

	public class UI_PosMarker_Control_CS : MonoBehaviour
	{

		public bool Show_All = false;
		public Color Friend_Color = Color.blue;
		public Color Hostile_Color = Color.red;
		public float Upper_Offset = 14.0f;
		public float Side_Offset = 28.0f;
	
		TankProp tankProp ;

		Transform thisTransform;
		Image thisImage;
		Quaternion leftRot;
		Quaternion rightRot;

		void Awake ()
		{
			thisTransform = GetComponent < Transform > ();
			thisImage = GetComponent < Image > ();
			leftRot = Quaternion.Euler (new Vector3 (0.0f, 0.0f, -90.0f));
			rightRot = Quaternion.Euler (new Vector3 (0.0f, 0.0f, 90.0f));
		}

		void LateUpdate ()
		{
			if (tankProp != null && tankProp.topScript) { // Tank is exist.
				if (Set_Enabled () == false) {
					return;
				}
				Set_Position ();
			} else { // Tank has been removed.
				Destroy (this.gameObject);
			}
		}

		bool Set_Enabled ()
		{
			if (tankProp.topScript.Is_Current || tankProp.bodyTransform.root.tag == "Finish") { // The tank is the current selected tank, or dead.
				thisImage.enabled = false;
				return false;
			}
			switch (tankProp.topScript.Relationship) {
			case 0: // Friendly.
				if (tankProp.aiScript) { // AI tank.
					thisImage.enabled = true;
					if (tankProp.aiScript.Action_Type == 0) { // Patrol mode.
						Friend_Color.a = 0.2f;
					} else { // Chase mode.
						Friend_Color.a = 1.0f;
					}
					thisImage.color = Friend_Color;
					return true;
				} else { // Player tank.
					thisImage.enabled = true;
					thisImage.color = Friend_Color;
					return true;
				}
			case 1: // Hostile.
				if (tankProp.aiScript) { // AI tank.
					if (tankProp.aiScript.Action_Type == 0) { // Patrol mode.
						if (Show_All) {
							thisImage.enabled = true;
							Hostile_Color.a = 0.2f;
							thisImage.color = Hostile_Color;
							return true;
						} else {
							thisImage.enabled = false;
							return false;
						}
					} else { // Chase mode.
						thisImage.enabled = true;
						if (tankProp.aiScript.Detect_Flag) {
							Hostile_Color.a = 1.0f;
						} else {
							Hostile_Color.a = 0.2f;
						}
						thisImage.color = Hostile_Color;
						return true;
					}
				} else { // Player tank.
					thisImage.enabled = true;
					thisImage.color = Hostile_Color;
					return true;
				}
			default :
				return false;
			}
		}

		void Set_Position ()
		{
			float dist = Vector3.Distance (Camera.main.transform.position, tankProp.bodyTransform.position);
			Vector3 currentPos = Camera.main.WorldToScreenPoint (tankProp.bodyTransform.position);
			if (currentPos.z > 0.0f) { // In front of the camera.
				currentPos.z = 100.0f;
				if (currentPos.x < 0.0f) { // Over the Left end.
					currentPos.x = Side_Offset;
					currentPos.y = Screen.height * Mathf.Lerp (0.2f, 0.9f, dist / 500.0f);
					thisTransform.localRotation = leftRot;
				} else if (currentPos.x > Screen.width) { // Over the Right end.
					currentPos.x = Screen.width - Side_Offset;
					currentPos.y = Screen.height * Mathf.Lerp (0.2f, 0.9f, dist / 500.0f);
					thisTransform.localRotation = rightRot;
				} else { // Within the screen.
					currentPos.y = Screen.height - Upper_Offset;
					thisTransform.localRotation = Quaternion.identity;
				}
			} else { // Behind of the camera.
				currentPos.z = -100.0f;
				if (currentPos.x > Screen.width * 0.5f) { // Left side.
					currentPos.x = Side_Offset;
					thisTransform.localRotation = leftRot;
				} else { // Right side.
					currentPos.x = Screen.width - Side_Offset;
					thisTransform.localRotation = rightRot;
				}
				currentPos.y = Screen.height * Mathf.Lerp (0.2f, 0.9f, dist / 500.0f);
			}
			thisTransform.position = currentPos;
		}

		public void Create_PosMarker (TankProp storedTankProp)
		{ // Called from "Tank_ID_Control_CS". (storedTankProp is passed by reference)
			if (tankProp != null) { // This marker is already used by other tank.
				// Duplicate this GameObject.
				GameObject newObject = Instantiate (gameObject, Vector3.zero, Quaternion.identity) as GameObject;
				newObject.name = this.name;
				newObject.transform.SetParent (thisTransform.parent);
				UI_PosMarker_Control_CS newMarkerScript = newObject.GetComponent < UI_PosMarker_Control_CS > ();
				newMarkerScript.tankProp = storedTankProp;
			} else { // Not used yet.
				tankProp = storedTankProp;
			}
		}
			
	}

}