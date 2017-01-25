using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Wall_Builder_Finishing_CS : MonoBehaviour
	{
	
		public float Mass = 10.0f;

		void Awake ()
		{
			if (GetComponent < Rigidbody > () == null) {
				Rigidbody rigidbody = gameObject.AddComponent < Rigidbody > ();
				rigidbody.mass = Mass;
				gameObject.name = "Block(Work)";
			}
			Destroy (this);
		}

		public void Set_Mass (float mass)
		{
			Mass = mass;
		}

	}

}