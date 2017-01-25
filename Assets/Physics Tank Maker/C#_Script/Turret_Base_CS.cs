using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Turret_Base_CS : MonoBehaviour
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

		public bool Coming_Off = true;
		public float Turret_Mass = 200.0f;
		public float Durability = 130000.0f;
		public float Sub_Durability = 110000.0f;
		public float Trouble_Time = 20.0f;
		public GameObject Damage_Effect_Object;
		public GameObject Trouble_Effect_Object;

		void Awake ()
		{
			Destroy (this);
		}

	}

}