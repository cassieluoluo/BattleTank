using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{

	public class Tank_ID_Control_CS: MonoBehaviour
	{
		public int Tank_ID = 1;
		public int Relationship;
		public int ReSpawn_Times = 0;
		public float Attack_Multiplier = 1.0f;
		public int Input_Type = 4;
		public int Turn_Type;
		public bool Auto_Lead = false;
		public string Marker_Name = "Pos_Marker";

		// 'public' is needed for Editor script.
		public bool ReSpawn_Flag;
		public string Prefab_Path;
	
		// AI patrol settings.
		public GameObject WayPoint_Pack;
		public int Patrol_Type = 1; // 0 = Order, 1 = Random.
		public Transform Follow_Target;
		// AI combat settings.
		public bool No_Attack = false;
		public bool Breakthrough = false;
		public float Visibility_Radius = 512.0f;
		public float Approach_Distance = 256.0f;
		public float OpenFire_Distance = 512.0f;
		public float Lost_Count = 20.0f;
		public bool Face_Enemy = false;
		public float Face_Offest_Angle = 0.0f;
		// AI text settings.
		public Text AI_State_Text;
		public string Tank_Name;
		// AI Auto respawn settings.
		public float ReSpawn_Interval = 10.0f;
		public float Remove_Time = 30.0f;

		Transform thisTransform;
		public Transform Root_Transform; // Referred to from "AI_CS", "AI_Semi_CS", "Turret_Horizontal_CS".
		public Game_Controller_CS Controller_Script;  // Referred to from "AI_CS".
		public TankProp Stored_TankProp; // Stored by "Game_Controller_CS". (passed by reference)

		public bool Is_Current; // Referred to from "PosMarker".
		int currentID = 1;

		void Awake ()
		{
			thisTransform = transform;
		}

		void Start ()
		{
			// Set the Root object. (This must be placed in Start(). Because this object has no parent when the tank is instantiated by "Event_Controller_CS".
			Event_Controller_CS eventScript = GetComponentInParent < Event_Controller_CS > ();
			if (eventScript) { // This tank is spawned by "Event_Controller_CS".
				Root_Transform = eventScript.transform;
				Overwrite_Settings (eventScript);
			} else { // This object must be the Root object.
				Root_Transform = thisTransform;
				thisTransform.parent = null;
			}
			// Set the Root's tag.
			Set_Tag ();
			// Find "Game_Controller" in the scene.
			GameObject controllerObject = GameObject.FindGameObjectWithTag ("GameController");
			if (controllerObject) {
				Controller_Script = controllerObject.GetComponent < Game_Controller_CS > ();
			}
			if (Controller_Script) {
				Controller_Script.Receive_ID (this); // Send this reference, and get proper Tank_ID, and store components in "Stored_TankProp".
			} else {
				Debug.LogError ("There is no 'Game_Controller' in the scene.");
			}
			// Find "Pos_Marker" and send message.
			if (string.IsNullOrEmpty (Marker_Name) == false) {
				GameObject markerObject = GameObject.Find (Marker_Name) ;
				if (markerObject) {
					UI_PosMarker_Control_CS markerScript = markerObject.GetComponent < UI_PosMarker_Control_CS > ();
					if (markerScript == null) {
						markerScript = markerObject.AddComponent < UI_PosMarker_Control_CS > ();
					}
					markerScript.Create_PosMarker (Stored_TankProp); // Send this "Stored_TankProp".
				} else {
					Debug.LogWarning (Marker_Name + " cannot be found in the scene.");
				}
			}
			// Send settings to all the children.
			Send_Settings ();
		}

		void Overwrite_Settings (Event_Controller_CS eventScript)
		{
			Tank_ID = eventScript.Tank_ID;
			Relationship = eventScript.Relationship;
			ReSpawn_Times = eventScript.ReSpawn_Times;
			Attack_Multiplier = eventScript.Attack_Multiplier;
			Input_Type = eventScript.Input_Type;
			Turn_Type = eventScript.Turn_Type;
			Auto_Lead = eventScript.Auto_Lead;
			if (eventScript.OverWrite_Flag) { // Overwrite AI settings.
				WayPoint_Pack = eventScript.WayPoint_Pack;
				Patrol_Type = eventScript.Patrol_Type;
				Follow_Target = eventScript.Follow_Target;
				No_Attack = eventScript.No_Attack;
				Breakthrough = eventScript.Breakthrough;
				Visibility_Radius = eventScript.Visibility_Radius;
				Approach_Distance = eventScript.Approach_Distance;
				OpenFire_Distance = eventScript.OpenFire_Distance;
				Lost_Count = eventScript.Lost_Count;
				Face_Enemy = eventScript.Face_Enemy;
				Face_Offest_Angle = eventScript.Face_Offest_Angle;
				AI_State_Text = eventScript.AI_State_Text;
				Tank_Name = eventScript.Tank_Name;
				ReSpawn_Interval = eventScript.ReSpawn_Interval;
				Remove_Time = eventScript.Remove_Time;
			}
		}

		void Set_Tag ()
		{ // This function is called at the opening, and also called from "ReSpawn ()".
			if (Relationship == 0) { // Friendly.
				Root_Transform.tag = "Player";
			} else { // Hostile.
				Root_Transform.tag = "Untagged";
			}
		}
			
		void Send_Settings ()
		{ // This function is called at the opening, and also called from 'ReSpawn ()'.
			// Check the input type.
			if (Stored_TankProp.aiScript) { // This tank must be an AI tank.
				Input_Type = 10; // AI Input
			} else if (Input_Type == 10) { // This tank has no "AI_CS", but Input_Type is set to "AI".
				Input_Type = 4; // Mouse Input
				Debug.LogWarning ("This tank has no AI components.");
			}
			if (Input_Type == 6) {
				if (GetComponentInChildren < AI_Semi_CS > () == null) { // This tank has no "AI_Semi_CS", but Input_Type is set to "Semi Auto".
					Input_Type = 4; // Mouse Input
					Debug.LogWarning ("This tank has no Semi-Auto components.");
				}
			}
			// Broadcast this reference.
			BroadcastMessage ("Get_Tank_ID_Control", this, SendMessageOptions.DontRequireReceiver);
			// Broadcast Current_ID.
			BroadcastMessage ("Receive_Current_ID", currentID, SendMessageOptions.DontRequireReceiver);
		}

		void Update ()
		{
			if (Is_Current && Input.GetKeyDown (KeyCode.Return)) {
				if (ReSpawn_Flag && ReSpawn_Times > 0) {
					ReSpawn ();
				}
			}
		}

		void ReSpawn ()
		{
			// Make sure that the prefab exists.
			GameObject tempObject = Resources.Load (Prefab_Path) as GameObject;
			if (tempObject == null) {
				ReSpawn_Flag = false;
				return;
			}
			ReSpawn_Times -= 1;
			//
			BroadcastMessage ("Prepare_ReSpawn", SendMessageOptions.DontRequireReceiver);
			// This object is continuously used even when a new tank is spawned.
			// Destroy child parts.
			int childCount = thisTransform.childCount;
			for (int i = 0; i < childCount; i++) {
				DestroyImmediate (thisTransform.GetChild (0).gameObject);
			}
			if (thisTransform.childCount == 0) { // Destroying succeeded.
				// Reset the root's tag.
				Set_Tag ();
				// Instantiate the prefab.
				GameObject newObject = Instantiate (Resources.Load (Prefab_Path), thisTransform.position, thisTransform.rotation) as GameObject;
				// Change the hierarchy of the new tank.
				childCount = newObject.transform.childCount;
				for (int i = 0; i < childCount; i++) {
					newObject.transform.GetChild (0).parent = thisTransform; // New child objects are moved under this object as its children.
				}
				// Destroy the top object of the new tank.
				DestroyImmediate (newObject);
				// Reset the stored components in the "Game_Controller".
				Controller_Script.ReSpawn_ReSetting (Stored_TankProp);
				// Broadcast settings to the new children.
				Send_Settings ();
			}
		}

		void MainBodyBroken_Linkage ()
		{ // Called from "Damage_Control_CS" in MainBody.
			Root_Transform.tag = "Finish";
			if (Input_Type == 10) { // AI
				if (ReSpawn_Flag && ReSpawn_Times > 0) {
					StartCoroutine ("Auto_ReSpawn");
				} else {
					if (Remove_Time != Mathf.Infinity) {
						StartCoroutine ("Remove_Tank", Remove_Time);
					}
				}
			}
		}

		IEnumerator Auto_ReSpawn ()
		{
			yield return new WaitForSeconds (ReSpawn_Interval);
			ReSpawn ();
		}

		public IEnumerator Remove_Tank (float count)
		{ // Also called from Event_Controller.
			yield return new WaitForSeconds (count);
			// Remove this reference in the "Game_Controller", and check the current ID.
			if (Controller_Script.Remove_Tank (Stored_TankProp)) {
				Destroy (gameObject);
			}
		}

		void Receive_Current_ID (int id)
		{
			currentID = id;
			if (id == Tank_ID) {
				Is_Current = true;
			} else {
				Is_Current = false;
			}
		}

	}

}