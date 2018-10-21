/*

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBMotion : MonoBehaviour {

	Mesh mesh;
	List<Vector3> oldVertices;
	List<Vector3> newVertices;
	public float scaleF;
	List<int> newTriangles;


	void Start () {
		oldVertices = new List<Vector3> ();
		newVertices = new List<Vector3> ();
		
	}
	

	void Update () {
		
	}
		

	void InitVertices(){
		
		oldVertices.Add (new Vector3(-1, 0, 1)) * scaleF;
		oldVertices.Add (new Vector3(1, 0, 1)) * scaleF;
		oldVertices.Add (new Vector3(-1, 0, -1)) * scaleF;
		oldVertices.Add (new Vector3(1, 0, -1)) * scaleF;
	
	}

	void FBM(){
		for (int x = 0; x < oldVertices.Count-1; x++) {
			for (int z = 0; z < oldVertices.Count-1; z++) {
				float midpt_x = (oldVertices [z].x + oldVertices [z+1].x) / 2;
				float midpt_z = (oldVertices [z].z + oldVertices [z+1].z) / 2;    // not sure if needed

				newVertices.Add (new Vector3 (oldVertices [z].x), Random.Range (0.0f, 20.0f), oldVertices [z].z);
				newVertices.Add (new Vector3 (midpt_x, Random.Range (0.0f, 20.0f), midpt_z));
				newVertices.Add (new Vector3 (oldVertices [z+1].x), Random.Range (0.0f, 20.0f), oldVertices [z+1].z);

			}
		}


		// calculate triangles
		for (int i = 0; i < newVertices.Count; i++) {
			newTriangles.Add (i);
			newTriangles.Add (i+1);
			newTriangles.Add (i+3);

			newTriangles.Add (i);
			newTriangles.Add (i+1);
			newTriangles.Add (i+4);

		}

		oldVertices = newVertices;
		newVertices.Clear ();
	}



	static float GaussianDistribution() {
		float v1, v2, s;
		do {
			v1 = 2.0f * Random.Range(0f,1f) - 1.0f;
			v2 = 2.0f * Random.Range(0f,1f) - 1.0f;
			s = v1 * v1 + v2 * v2;
		} while (s >= 1.0f || s == 0f);

		s = Mathf.Sqrt((-2.0f * Mathf.Log(s)) / s);

		return v1 * s;
	}

}





*/



// Gausian formula : https://www.alanzucconi.com/2015/09/16/how-to-sample-from-a-gaussian-distribution/