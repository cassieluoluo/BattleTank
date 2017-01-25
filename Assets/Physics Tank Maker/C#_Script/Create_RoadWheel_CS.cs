using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{
	
	[ System.Serializable]
	public class RoadWheelsProp
	{
		public string parentName;
		public float baseRadius;
		public float [] angles;
	}

	public class Create_RoadWheel_CS : MonoBehaviour
	{

		public bool Fit_ST_Flag = false;

		public float Sus_Distance = 2.06f;
		public int Num = 6;
		public float Spacing = 0.88f;
		public float Sus_Length = 0.5f;
		public bool Set_Individually = false;
		public float Sus_Angle = 0.0f;
		public float [] Sus_Angles;
		public float Sus_Anchor = 0.0f;
		public float Sus_Mass = 30.0f;
		public float Sus_Spring = 900.0f;
		public float Sus_Damper = 20.0f;
		public float Sus_Target = 30.0f;
		public float Sus_Forward_Limit = 30.0f;
		public float Sus_Backward_Limit = 30.0f;
		public Mesh Sus_L_Mesh;
		public Mesh Sus_R_Mesh;
		public Material Sus_L_Material;
		public Material Sus_R_Material;
		public float Reinforce_Radius = 0.5f;

		public float Wheel_Distance = 2.7f;
		public float Wheel_Mass = 30.0f;
		public float Wheel_Radius = 0.3f;
		public PhysicMaterial Collider_Material;
		public Mesh Wheel_Mesh;
		public Material Wheel_Material;
		public Mesh Collider_Mesh;
		public Mesh Collider_Mesh_Sub;

		public bool Drive_Wheel = true;
		public bool Use_BrakeTurn = true;
		public bool Wheel_Resize = false;
		public float ScaleDown_Size = 0.5f;
		public float Return_Speed = 0.05f;
		public float Wheel_Durability = 55000.0f;

		public bool RealTime_Flag = true;

		public RoadWheelsProp Get_Current_Angles ()
		{
			RoadWheelsProp currentProp = new RoadWheelsProp ();
			currentProp.parentName = transform.name;
			currentProp.baseRadius = Wheel_Radius;
			currentProp.angles = new float [Num];
			for (int i = 0; i < Num; i++) {
				Transform susTransform = transform.FindChild ("Suspension_L_" + (i + 1));
				if (susTransform) {
					float currentAngle = susTransform.localEulerAngles.y;
					if (currentAngle > 180.0f) {
						currentAngle = -(360.0f - currentAngle);
					} else if (currentAngle < -180.0f) {
						currentAngle = 360.0f - currentAngle;
					}
					currentProp.angles [i] = currentAngle;
				}
			}
			return currentProp;
		}

	}

}