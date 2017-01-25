using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Bullet_Generator_CS : MonoBehaviour
	{

		public Mesh Bullet_Mesh;
		public Material Bullet_Material;
		public float Bullet_Mass = 5.0f;
		public float Bullet_Drag = 0.05f;
		public PhysicMaterial Bullet_PhysicMat;
		public Vector3 Bullet_Scale = new Vector3 (0.762f, 0.762f, 0.762f);
		public float Bullet_Force = 250.0f;
		public Vector3 BoxCollider_Scale = new Vector3 (1.0f, 1.0f, 1.0f);
		public float Delete_Time = 5.0f;
		public GameObject MuzzleFire_Object;
		public GameObject Impact_Object;
		public GameObject Ricochet_Object;
		public bool Trail_Flag = false;
		public Material Trail_Material;
		public float Trail_Start_Width = 0.01f;
		public float Trail_End_Width = 0.2f;
		public float Trail_Time = 0.1f;

		public Mesh Bullet_Mesh_HE;
		public Material Bullet_Material_HE;
		public float Bullet_Mass_HE = 5.0f;
		public float Bullet_Drag_HE = 0.05f;
		public Vector3 Bullet_Scale_HE = new Vector3 (0.762f, 0.762f, 0.762f);
		public float Bullet_Force_HE = 250.0f;
		public Vector3 BoxCollider_Scale_HE = new Vector3 (1.0f, 1.0f, 1.0f);
		public float Delete_Time_HE = 5.0f;
		public GameObject MuzzleFire_Object_HE;
		public float Explosion_Force = 60000.0f;
		public float Explosion_Radius = 20.0f;
		public GameObject Explosion_Object;
		public bool Trail_Flag_HE = false;
		public Material Trail_Material_HE;
		public float Trail_Start_Width_HE = 0.01f;
		public float Trail_End_Width_HE = 0.2f;
		public float Trail_Time_HE = 0.1f;

		public int Initial_Bullet_Type = 0;
		public float Offset = 0.5f;
		public bool Debug_Flag = false;

		public int Barrel_Type = 0; // Set by "Barrel_Base".
		int bulletType;
		public float Bullet_Velocity; // Referred to from "Turret_Horizontal" and "Cannon_Vertical".

		float attackMultiplier = 1.0f;

		Transform thisTransform;

		bool isCurrent;
		int myID;
		int inputType = 4;


		void Awake ()
		{
			thisTransform = transform;
			Change_Type (Initial_Bullet_Type);
		}

		void Update ()
		{
			if (isCurrent) {
				if (Input.GetKeyDown ("v")) {
					switch (bulletType) {
					case 0: // AP >>
						Change_Type (1); // >> HE.
						break;
					case 1: // HE >>
						Change_Type (0); // >> AP.
						break;
					}
				}
			}
		}

		void Change_Type (int type)
		{
			bulletType = type;
			switch (bulletType) {
			case 0:
				Bullet_Velocity = Bullet_Force;
				break;
			case 1:
				Bullet_Velocity = Bullet_Force_HE;
				break;
			}
		}

		void Fire_Linkage (int direction)
		{
			if (Barrel_Type == 0 || Barrel_Type == direction) {
				switch (bulletType) {
				case 0:
					Set_AP ();
					break;
				case 1:
					Set_HE ();
					break;
				}
			}
		}

		void Set_AP ()
		{
			// Create Particle ( Prefab )
			if (MuzzleFire_Object) {
				GameObject fireObject = Instantiate (MuzzleFire_Object, thisTransform.position, thisTransform.rotation) as GameObject;
				fireObject.transform.parent = thisTransform;
			}
			// Create GameObject & Set Transform
			GameObject bulletObject = new GameObject ("Bullet_AP");
			bulletObject.transform.position = thisTransform.position + (thisTransform.forward * Offset);
			bulletObject.transform.rotation = thisTransform.rotation;
			bulletObject.transform.localScale = Bullet_Scale;
			// Add Components
			MeshRenderer meshRenderer = bulletObject.AddComponent < MeshRenderer > ();
			meshRenderer.material = Bullet_Material;
			MeshFilter meshFilter = bulletObject.AddComponent < MeshFilter > ();
			meshFilter.mesh = Bullet_Mesh;
			Rigidbody rigidbody = bulletObject.AddComponent < Rigidbody > ();
			rigidbody.mass = Bullet_Mass;
			rigidbody.drag = Bullet_Drag;
			BoxCollider boxCollider = bulletObject.AddComponent < BoxCollider > ();
			boxCollider.size = Vector3.Scale (boxCollider.size, BoxCollider_Scale_HE);
			boxCollider.material = Bullet_PhysicMat;
			if (Trail_Flag) {
				TrailRenderer trailRenderer = bulletObject.AddComponent < TrailRenderer > ();
				trailRenderer.startWidth = Trail_Start_Width;
				trailRenderer.endWidth = Trail_End_Width;
				trailRenderer.time = Trail_Time;
				trailRenderer.material = Trail_Material;
			}
			// Add Scripts
			Bullet_Control_CS bulletScript;
			bulletScript = bulletObject.AddComponent < Bullet_Control_CS > ();
			bulletScript.Type = bulletType;
			bulletScript.Delete_Time = Delete_Time;
			bulletScript.Impact_Object = Impact_Object;
			bulletScript.Ricochet_Object = Ricochet_Object;
			bulletScript.Attack_Multiplier = attackMultiplier;
			bulletScript.Debug_Flag = Debug_Flag;
			// Shoot
			rigidbody.velocity = bulletObject.transform.forward * Bullet_Force;
		}

		void Set_HE ()
		{
			// Create Particle ( Prefab )
			if (MuzzleFire_Object_HE) {
				GameObject fireObject = Instantiate (MuzzleFire_Object_HE, thisTransform.position, thisTransform.rotation) as GameObject;
				fireObject.transform.parent = thisTransform;
			}
			// Create GameObject & Set Transform
			GameObject bulletObject = new GameObject ("Bullet_HE");
			bulletObject.transform.position = thisTransform.position + (thisTransform.forward * Offset);
			bulletObject.transform.rotation = thisTransform.rotation;
			bulletObject.transform.localScale = Bullet_Scale_HE;
			// Add Components
			MeshRenderer meshRenderer = bulletObject.AddComponent < MeshRenderer > ();
			meshRenderer.material = Bullet_Material_HE;
			MeshFilter meshFilter = bulletObject.AddComponent < MeshFilter > ();
			meshFilter.mesh = Bullet_Mesh_HE;
			Rigidbody rigidbody = bulletObject.AddComponent < Rigidbody > ();
			rigidbody.mass = Bullet_Mass_HE;
			rigidbody.drag = Bullet_Drag_HE;
			BoxCollider boxCollider;
			boxCollider = bulletObject.AddComponent < BoxCollider > ();
			boxCollider.size = Vector3.Scale (boxCollider.size, BoxCollider_Scale_HE);
			if (Trail_Flag_HE) {
				TrailRenderer trailRenderer = bulletObject.AddComponent < TrailRenderer > ();
				trailRenderer.startWidth = Trail_Start_Width_HE;
				trailRenderer.endWidth = Trail_End_Width_HE;
				trailRenderer.time = Trail_Time_HE;
				trailRenderer.material = Trail_Material_HE;
			}
			// Add Scripts
			Bullet_Control_CS bulletScript = bulletObject.AddComponent < Bullet_Control_CS > ();
			bulletScript.Type = bulletType;
			bulletScript.Delete_Time = Delete_Time_HE;
			bulletScript.Explosion_Force = Explosion_Force;
			bulletScript.Explosion_Radius = Explosion_Radius;
			bulletScript.Explosion_Object = Explosion_Object;
			bulletScript.Attack_Multiplier = attackMultiplier;
			bulletScript.Debug_Flag = Debug_Flag;
			// Shoot
			rigidbody.velocity = bulletObject.transform.forward * Bullet_Force_HE;
		}

		void Get_Tank_ID_Control (Tank_ID_Control_CS topScript)
		{
			inputType = topScript.Input_Type;
			myID = topScript.Tank_ID;
			attackMultiplier = topScript.Attack_Multiplier;
		}
			
		void Receive_Current_ID (int id)
		{
			if (id == myID) {
				if (inputType != 10) {
					isCurrent = true;
				}
			} else {
				isCurrent = false;
			}
		}

	}

}