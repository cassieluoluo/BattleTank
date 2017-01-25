using UnityEngine;
using System.Collections;

namespace ChobiAssets.PTM
{

	[ System.Serializable]
	public class IntArray
	{
		public int[] intArray;
		public IntArray (int[] newIntArray)
		{
			intArray = newIntArray;
		}
	}

	public class Track_Deform_CS : MonoBehaviour
	{
	
		public int Anchor_Num = 1;
		public Transform[] Anchor_Array;
		public string [] Anchor_Names;
		public string [] Anchor_Parent_Names;
		public float[] Width_Array;
		public float[] Height_Array;
		public float[] Offset_Array;

		public float[] Initial_Pos_Array;
		public Vector3[] Initial_Vertices;
		public IntArray[] Movable_Vertices_List;

		Mesh thisMesh;
		MainBody_Setting_CS bodyScript;

		void Awake ()
		{
			thisMesh = GetComponent < MeshFilter > ().mesh;
			// Check Anchor wheels.
			for (int i = 0; i < Anchor_Array.Length; i++) {
				if (Anchor_Array [i] == null) {
					// Find Anchor with reference to the name.
					if (string.IsNullOrEmpty (Anchor_Names [i]) == false && string.IsNullOrEmpty (Anchor_Parent_Names [i]) == false) {
						Anchor_Array [i] = transform.parent.Find (Anchor_Parent_Names [i] + "/" + Anchor_Names [i]);
					} else {
						Debug.LogError ("Anchor Wheel is not assigned in " + this.name);
						Destroy (this);
					}
				}
			}
		}

		void Update ()
		{
			if (bodyScript.Visible_Flag) { // MainBody is visible by any camera.
				Vector3[] tempVertices = new Vector3 [ Initial_Vertices.Length ];
				Initial_Vertices.CopyTo (tempVertices, 0);
				for (int i = 0; i < Anchor_Array.Length; i++) {
					float tempDist = Anchor_Array [i].localPosition.x - Initial_Pos_Array [i];
					for (int j = 0; j < Movable_Vertices_List [i].intArray.Length; j++) {
						tempVertices [Movable_Vertices_List [i].intArray [j]].y += tempDist;
					}
				}
				thisMesh.vertices = tempVertices;
			}
		}

		void OnDrawGizmosSelected ()
		{
			if (Anchor_Array.Length != 0 && Offset_Array.Length != 0) {
				Gizmos.color = Color.green;
				for (int i = 0; i < Anchor_Array.Length; i++) {
					if (Anchor_Array [i] != null) {
						Vector3 tempSize = new Vector3 (0.0f, Height_Array [i], Width_Array [i]);
						Vector3 tempCenter = Anchor_Array [i].position;
						tempCenter.y += Offset_Array [i];
						Gizmos.DrawWireCube (tempCenter, tempSize);
					}
				}
			}
		}

		void Get_Tank_ID_Control (Tank_ID_Control_CS topScript)
		{
			bodyScript = topScript.Stored_TankProp.bodyScript;
		}

	}

}