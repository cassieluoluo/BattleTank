using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class UI_Canvas_Control_CS : MonoBehaviour
	{

		Canvas thisCanvas;
		bool isEnabled;

		void Awake ()
		{
			thisCanvas = GetComponent < Canvas > ();
			isEnabled = thisCanvas.enabled;
		}

		void Update ()
		{
			if (Input.GetKeyDown (KeyCode.Delete)) {
				isEnabled = !isEnabled;
				thisCanvas.enabled = isEnabled;
			}
		}
	}

}