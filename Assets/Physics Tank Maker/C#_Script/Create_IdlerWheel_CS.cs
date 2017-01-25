using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Create_IdlerWheel_CS : MonoBehaviour
	{
		public bool Static_Flag = false;
		public float Radius_Offset;
		public bool Invisible_Physics_Wheel = false;

		public bool Arm_Flag = false;
		public float Arm_Distance = 2.11f;
		public float Arm_Length = 0.25f;
		public float Arm_Angle = 110.0f;
		public Mesh Arm_L_Mesh;
		public Mesh Arm_R_Mesh;
		public Material Arm_L_Material;
		public Material Arm_R_Material;
	
		public float Wheel_Distance = 2.7f;
		public float Wheel_Mass = 30.0f;
		public float Wheel_Radius = 0.35f;
		public PhysicMaterial Collider_Material;
		public Mesh Wheel_Mesh;
		public Material Wheel_Material;
		public Mesh Collider_Mesh;
		public Mesh Collider_Mesh_Sub;
		public bool Drive_Wheel = true;
		public bool Wheel_Resize = true;
		public float ScaleDown_Size = 0.7f;
		public float Return_Speed = 0.05f;
		public float Wheel_Durability = 55000.0f;

	}

}