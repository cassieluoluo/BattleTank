using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	public class Turret_Finishing_CS : MonoBehaviour
	{

		public bool Multiple_barrels_Flag = false;
		public bool Child_Flag = false;
		public Transform Parent_Transform;
	
		Transform turretBase;
		Transform cannonBase;
		Transform barrelBase;

		Transform thisTransform;

		void Awake ()
		{
			thisTransform = transform;
			if (Multiple_barrels_Flag) {
				Multiple_Barrels ();
			} else {
				Single_Barrel ();
			}
		}

		void Single_Barrel ()
		{
			turretBase = thisTransform.Find ("Turret_Base");
			cannonBase = thisTransform.Find ("Cannon_Base");
			barrelBase = thisTransform.Find ("Barrel_Base");
			if (turretBase && cannonBase && barrelBase) {
				// Change the hierarchy.
				barrelBase.parent = cannonBase;
				cannonBase.parent = turretBase;
				Finishing ();
			} else {
				Error_Message ();
			}
		}

		void Multiple_Barrels ()
		{
			turretBase = thisTransform.Find ("Turret_Base");
			if (turretBase) {
				for (int i = 0; i < thisTransform.childCount; i++) {
					cannonBase = thisTransform.Find ("Cannon_Base_" + (i + 1));
					if (cannonBase) {
						cannonBase.parent = turretBase;
						for (int j = 0; j < thisTransform.childCount; j++) {
							barrelBase = thisTransform.Find ("Barrel_Base_" + (i + 1));
							if (barrelBase) {
								barrelBase.parent = cannonBase;
							}
						}
					}
				}
			} else { // Turret_Base cannot be found.
				Error_Message ();
				return;
			}
			// Check the new hierarchy.
			if (thisTransform.childCount > 1) { // The number of children must be one.
				Error_Message ();
			}
			Finishing ();
		}

		void Finishing ()
		{
			// Send message to all the turret parts.
			BroadcastMessage ("Complete_Turret", SendMessageOptions.DontRequireReceiver);
			if (Child_Flag) { // Child turret.
				// Send message to "Damage_Control" in the Turret. 
				turretBase.BroadcastMessage ("Set_ChildTurret", SendMessageOptions.DontRequireReceiver);
			} else {
				Destroy (this);
			}
		}

		void Start ()
		{ // Only for Child Turret.
			if (Parent_Transform) {
				thisTransform.parent = Parent_Transform.Find ("Turret_Base"); // Change this parent.
			} else {
				Debug.LogError ("'Parent_Transform' for the Child Turret is not assigned.");
			}
			Destroy (this);
		}

		void Error_Message ()
		{
			Debug.LogError ("'Turret_Finishing_CS(Script)' could not change the hierarchy of the Turret.");
			Debug.LogWarning ("Make sure the names of 'Turret_Base', 'Cannon_Base' and 'Barrel_Base'.");
			Destroy (this);
		}
	
	}

}