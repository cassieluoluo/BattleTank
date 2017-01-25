using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{
	
	public class UI_AIState_Control_CS : MonoBehaviour
	{

		public Color Color_Attack = Color.red;
		public Color Color_Lost = Color.magenta;
		public Color Color_Dead = Color.black;

		public string Search_Text = "Search";
		public string Attack_Text = "Attack";
		public string Lost_Text = "Lost";
		public string Dead_Text = "Dead";

		public AI_CS AI_Script; // Set by "AI_CS".
		Color defaultColor;
		Text thisText;

		void Awake ()
		{
			thisText = GetComponent < Text > ();
			defaultColor = thisText.color;
		}

		void LateUpdate ()
		{
			if (AI_Script) {
				AI_Process ();
			}
		}

		void AI_Process ()
		{
			if (AI_Script.Action_Type == 0) { // Patrol mode.
				thisText.text = AI_Script.Tank_Name + " = " + Search_Text;
				thisText.color = defaultColor;
			} else { // Chase mode.
				if (AI_Script.Detect_Flag) {
					thisText.text = AI_Script.Tank_Name + " = " + Attack_Text;
					thisText.color = Color_Attack;
				} else {
					thisText.text = AI_Script.Tank_Name + " = " + Lost_Text + Mathf.CeilToInt (AI_Script.Losing_Count);
					thisText.color = Color_Lost;
				}
			}
		}

		void Dead ()
		{ // Called from "AI_CS" when the tank is destroyed.
			thisText.text = AI_Script.Tank_Name + " = " + Dead_Text;
			thisText.color = Color_Dead;
		}

	}

}