using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Delete_Timer_CS : MonoBehaviour
	{ // used for destroying NavMeshObstacle made by AI_CS.

		public float Count;

		void Start ()
		{
			Destroy (this.gameObject, Count);
		}

	}

}