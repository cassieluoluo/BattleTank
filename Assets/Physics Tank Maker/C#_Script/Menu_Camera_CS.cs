using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace ChobiAssets.PTM
{
	public class Menu_Camera_CS : MonoBehaviour
	{

		Camera thisCamera;
		Transform thisTransform;

		void Awake ()
		{
			thisCamera = GetComponent <Camera> ();
			thisTransform = transform;
			targetPos = thisTransform.position;
			currentPos = targetPos;
		}

		void Update ()
		{
			Rotaion ();
			Zoom ();
			Move ();
		}

		Vector2 previousMousePos;
		void Rotaion ()
		{
			float horizontal = 0.0f;
			float vertical = 0.0f;
			if (Input.GetMouseButtonDown (1)) {
				previousMousePos = Input.mousePosition;
			} else if (Input.GetMouseButton (1)) {
				horizontal = (Input.mousePosition.x - previousMousePos.x) * 0.2f;
				vertical = (Input.mousePosition.y - previousMousePos.y) * -0.2f;
				previousMousePos = Input.mousePosition;
			}
			Vector3 angles = thisTransform.eulerAngles;
			angles.y += horizontal;
			angles.x += vertical;
			angles.x = Mathf.Clamp (angles.x, 1.0f, 90.0f);
			thisTransform.eulerAngles = angles;
		}

		void Zoom ()
		{
			if (Input.GetMouseButton (1)) {
				float wheelInput = -Input.GetAxis ("Mouse ScrollWheel");
				if (wheelInput == 0.0f) {
					return;
				}
				float currentSize = thisCamera.orthographicSize;
				currentSize *= 1.0f + (Mathf.Sign (wheelInput) * 0.2f);
				currentSize = Mathf.Clamp (currentSize, 10.0f, 1280.0f);
				thisCamera.orthographicSize = currentSize;
			}
		}

		Vector3 currentPos;
		Vector3 targetPos;
		void Move ()
		{
			if (Input.GetMouseButtonDown (2) && EventSystem.current.IsPointerOverGameObject() == false) {
				Ray ray = thisCamera.ScreenPointToRay (Input.mousePosition);
				RaycastHit raycastHit;
				int layerMask = ~((1 << 10) + (1 << 2)); // Layer 2 = Ignore Ray, Layer 10 = Ignore All.
				if (Physics.Raycast (ray, out raycastHit, Mathf.Infinity, layerMask)) {
					targetPos = raycastHit.point;
				}
			}
			//
			currentPos = Vector3.MoveTowards (currentPos, targetPos, 5.0f * thisCamera.orthographicSize * Time.deltaTime);
			thisTransform.position = currentPos;
		}

	}
}
