using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinTerrain : MonoBehaviour {

	// Use this for initialization
	Mesh mesh;

	Vector3[] newVertices;
	Vector2[] newUVs;
	int[] newTriangles;

	public float tHeight;
	public float tSize;
	public int tDivisions;
	float tSizeHalf;
	float divSize;
	float flying;

	int vertSize;
	float[] Terrain;

	void Start () {
		
		mesh = new Mesh ();
		GetComponent<MeshFilter> ().mesh = mesh;

		vertSize = (tDivisions + 1) * (tDivisions + 1);

		flying = 0.0f;
		newVertices = new Vector3[vertSize];
		newUVs = new Vector2[vertSize];
		newTriangles = new int[tDivisions*tDivisions*6];

		tSizeHalf = tSize * 0.5f;
		divSize = tSize / tDivisions;

		GenerateTerrain ();

	}
		
	void Update(){

		UpdateTerrain ();
	}

	void UpdateTerrain(){
		int k = 0;      // index for vertices array
		int tri = 0;    // triangle offset
		int vi = 0;     // triangle ids

		flying -= 0.01f;
		float xCoord = 0.0f;

		for (int i = 0; i <= tDivisions; i++) {
			float zCoord = flying;

			for (int j = 0; j <= tDivisions; j++) {

				zCoord += 1.0f / (float)tDivisions;

				float height = Mathf.PerlinNoise (xCoord*4f, zCoord*4f) + 0.5f * Mathf.PerlinNoise(xCoord*8f, zCoord*8f) + 0.25f * Mathf.PerlinNoise(xCoord*16f, zCoord*16f);

				height = Mathf.Pow (height, 2);
				height *= tHeight;

				newVertices [k] = new Vector3 (j*divSize, height, i*divSize);
				newUVs [k] = new Vector2 ((float)j/tDivisions, (float)i/tDivisions);

				if (i < tDivisions && j < tDivisions) {
					newTriangles [tri] = vi;
					newTriangles [tri+1] = vi + tDivisions + 1;
					newTriangles[tri + 2] = vi + 1;

					newTriangles [tri + 3] = newTriangles [tri + 2];
					newTriangles [tri + 4] = newTriangles [tri + 1];
					newTriangles [tri + 5] = vi + tDivisions + 2;
					tri += 6;
				}
				k++;
				vi++;

			}

			xCoord += 1.0f / (float)tDivisions;

		}


		// update mesh properties
		mesh.vertices = newVertices;
		mesh.uv = newUVs;
		mesh.triangles = newTriangles;

		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();
	
	}



	void GenerateTerrain(){
		int k = 0;      // index for vertices array
		int tri = 0;    // triangle offset
		int vi = 0;     // triangle ids

		for (int i = 0; i <= tDivisions; i++) {
			for (int j = 0; j <= tDivisions; j++) {
				float xCoord = (float)j / tDivisions;
				float zCoord = (float)i / tDivisions;
				float height = Mathf.PerlinNoise (xCoord*4f, zCoord*4f) + 0.5f * Mathf.PerlinNoise(xCoord*8f, zCoord*8f) + 0.25f * Mathf.PerlinNoise(xCoord*16f, zCoord*16f);

				height = Mathf.Pow (height, 2);
				height *= tHeight;

				newVertices [k] = new Vector3 (j*divSize, height, i*divSize);
				newUVs [k] = new Vector2 ((float)j/tDivisions, (float)i/tDivisions);

				if (i < tDivisions && j < tDivisions) {
					newTriangles [tri] = vi;
					newTriangles [tri+1] = vi + tDivisions + 1;
					newTriangles[tri + 2] = vi + 1;

					newTriangles [tri + 3] = newTriangles [tri + 2];
					newTriangles [tri + 4] = newTriangles [tri + 1];
					newTriangles [tri + 5] = vi + tDivisions + 2;
					tri += 6;
				}
				k++;
				vi++;

			}
		
		}


		// update mesh properties
		mesh.vertices = newVertices;
		mesh.uv = newUVs;
		mesh.triangles = newTriangles;

		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();
		

	}
		



}