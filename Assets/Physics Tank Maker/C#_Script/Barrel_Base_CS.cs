using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Barrel_Base_CS : MonoBehaviour
	{
		public Mesh Part_Mesh;
		public Mesh Collider_Mesh;
		public Mesh Sub_Collider_Mesh;

		public int Materials_Num = 1;
		public Material[] Materials;
		public Material Part_Material;

		public float Offset_X = 0.0f;
		public float Offset_Y = 0.0f;
		public float Offset_Z = 0.0f;

		public float Durability = Mathf.Infinity;
		public float Sub_Durability = 100000.0f;
		public float Trouble_Time = 20.0f;
		public GameObject Trouble_Effect_Object;

		public int Barrel_Type = 0;
	
		void Awake ()
		{
			Destroy (this);
		}
	}

}