using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierP3 : MonoBehaviour {

	List<Vector3> CVs;
	List<Vector3> newVertices;
	List<Vector2> newUVs;
	int[] newTriangles;
	Mesh mesh;
	int k; // triangle array index value
	int m;  // triangle array value

	public int tHeight;
	public float pDist;

	int[][,] CV = new int[16][,];  // declaring a jagged array, containing 2D arrays

	Vector3[,] cv, cv2, cv3, cv4;
	public int stacks, slices;
	public int num_of_patches;
	public float scaleF;
	public int Bpatches;

	public float p0;
	public float p1;
	public float p2;
	public float p3;

	float Pdivisor = 50.0f;
	bool Pincrease;
	int vertSize;
	float flying;

	void Start() {

		mesh = GetComponent<MeshFilter> ().mesh;

		Pincrease = true;
		flying = 0.0f;

		cv = new Vector3[4,4];
		cv2 = new Vector3[4,4];
		cv3 = new Vector3[4,4];
		cv4 = new Vector3[4,4];

		vertSize = (num_of_patches*stacks + 1) * (num_of_patches*stacks + 1);

		k = m = 0;
		CVs = new List<Vector3> ();

		newVertices = new List<Vector3> ();
		newUVs = new List<Vector2> ();
		newTriangles = new int[stacks * stacks * num_of_patches * 6];


		InitArray(stacks, slices);        // newVertices get filled with data from CreateSurface
		k = m = 0;

		UpdateMeshData (newVertices);

	}



	void Update() {


		cv[1,0].y = 1*scaleF * Mathf.Sin(Time.time);
		cv[2,0].y = 1*scaleF * Mathf.Sin(Time.time);
		cv[2,2].y = 1*scaleF * Mathf.Sin(Time.time);
		cv[1,1].y = 1*scaleF * Mathf.Sin(Time.time);
		cv[3,3].y = 1*scaleF * Mathf.Sin(Time.time);

		UpdateSurface(ref newVertices, stacks, slices, cv, 1);

		Patch2 (stacks, slices, ref cv2);
		UpdateSurface(ref newVertices, stacks, slices, cv2, 2);

		Patch3 (stacks, slices, ref cv3);
		UpdateSurface(ref newVertices, stacks, slices, cv3, 3);

		Patch4 (stacks, slices, ref cv4);
		UpdateSurface(ref newVertices, stacks, slices, cv4, 4);

		k = 0;

		UpdateMeshData (newVertices);

	}

	Vector3 Q(float u, float v, Vector3[,] cv) {

		List<float> Bu = new List<float> ();
		List<float> Bv = new List<float> ();

		Bu.Add (Mathf.Pow (1 - u, 3));
		Bu.Add (3 * u * Mathf.Pow (1 - u, 2));
		Bu.Add (3 * Mathf.Pow (u, 2) * (1 - u));
		Bu.Add (Mathf.Pow (u, 3));

		Bv.Add (Mathf.Pow (1 - v, 3));
		Bv.Add (3 * v * Mathf.Pow (1 - v, 2));
		Bv.Add (3 * Mathf.Pow (v, 2) * (1 - v));
		Bv.Add (Mathf.Pow (v, 3));

		Vector3 tmp = new Vector3 (0, 0, 0);

		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < 4; j++) {
				tmp += cv[i,j] * Bu[i] * Bv[j];
			}
		}

		return tmp;

	}

	void GeneratePerlin(ref Vector3 v){

		float xCoord = (float)v.x / Pdivisor;
		float zCoord = (float)v.z / Pdivisor;
		float height = Mathf.PerlinNoise (xCoord*4f, zCoord*4f) + 0.5f * Mathf.PerlinNoise(xCoord*8f, zCoord*8f) + 0.25f * Mathf.PerlinNoise(xCoord*16f, zCoord*16f);

		height = Mathf.Pow (height, 3);
		height *= tHeight;
		v.y =  v.y + height;
	}


	void CreateSurface(ref List<Vector3> vert, int stacks, int slices, Vector3[,] cv, int id) {

		Vector3 a = new Vector3 (0, 0, 0);

		float deltau = 1 / (float)stacks;
		float deltav = 1 / (float)slices;

		for (int i = 0; i <= stacks; i++) {
			float u = i * deltau;
			for (int j = 0; j <= slices; j++) {
				float v = j * deltav;

				a = Q(u, v, cv);
				vert.Add (a);

				int uvD = stacks* num_of_patches;
				newUVs.Add(new Vector2 ((float)i*id/uvD, (float)j*id/uvD));

				if (i < stacks && j < slices) {
					newTriangles[k] = m;
					newTriangles[k + 1] = m + 1;
					newTriangles [k + 2] = m + ((int)stacks) + 1;

					newTriangles [k + 3] = newTriangles [k + 2];
					newTriangles [k + 4] = newTriangles [k + 1];
					newTriangles [k + 5] = m + ((int)stacks) + 2;

					k += 6;
				
				}

				m++;
			}

		}

	}
		
	void UpdateSurface(ref List<Vector3> vert, int stacks, int slices, Vector3[,] cv, int id) {  // k = starting index of triangle in tri List for each patch

		Vector3 a = new Vector3 (0, 0, 0);

		float deltau = 1 / (float)stacks;
		float deltav = 1 / (float)slices;

		for (int i = 0; i <= stacks; i++) {
			float u = i * deltau;

			for (int j = 0; j <= slices; j++) {
				float v = j * deltav;

				a = Q(u,          v, cv);

				//Add Perlin Noise

				float xCoord = (float)a.x*pDist/ ((float)stacks*(float)num_of_patches);
				float zCoord = (float)a.z*pDist/ ((float)slices*(float)num_of_patches);
				float height = p0 * Mathf.PerlinNoise (xCoord*4f, zCoord*4f) + p1*Mathf.PerlinNoise(xCoord*p1, zCoord*p1) + 
					p2*Mathf.PerlinNoise(xCoord*p2, zCoord*p2) + p3*Mathf.PerlinNoise(xCoord*32f, zCoord*32f);;

				height = Mathf.Pow (height, 3);
				height *= tHeight;

				a.y = a.y + height;
				vert [k] = a;
				k++;
			}

		}

	}
		


	void InitArray(int stacks, int slices)
	{

		for (int i = 0; i<4; i++)
			for (int j = 0; j<4; j++)
				cv[i,j] = new Vector3(i, 0, j) * scaleF;

		cv[1,0] = new Vector3(1, 3, 0)* scaleF;
		cv[2,0] = new Vector3(2, 3, 0)* scaleF;
		cv[2,2] = new Vector3(2, 3, 2)* scaleF;
		cv[1,1] = new Vector3(1, 3, 1)* scaleF; 
		cv[3,3] = new Vector3(3, 1, 3)* scaleF; 

		CreateSurface(ref newVertices, stacks, slices, cv, 1);

		Patch2 (stacks, slices, ref cv2);
		CreateSurface(ref newVertices, stacks, slices, cv2, 2);

		Patch3 (stacks, slices, ref cv3);
		CreateSurface(ref newVertices, stacks, slices, cv3, 3);

		Patch4 (stacks, slices, ref cv4);
		CreateSurface(ref newVertices, stacks, slices, cv4, 4);





	}


	void Patch2(int stacks, int slices, ref Vector3[,] cv2){
		//////////////////////////////  2nd surface

		for(int i = 0; i < 4; i++) {
			cv2[0,i] = new Vector3(cv[3,i].x, cv[3,i].y, cv[3,i].z);
			cv2[1,i] = new Vector3((2*cv[3,i] - cv[2,i]).x, (2*cv[3,i] - cv[2,i]).y, (2*cv[3,i] - cv[2,i]).z);
		}

		for (int i = 2; i < 4; i++) {
			for (int j = 0; j < 4; j++) {
				cv2[i,j] = new Vector3(i+3, 0, j)*scaleF;
			}
		}

	}

	void Patch3(int stacks, int slices, ref Vector3[,] cv3){
		///////////////////////////////3rd surf
		for(int i = 0; i < 4; i++) {
			cv3[i,0] = new Vector3(cv[i,3].x, cv[i,3].y, cv[i,3].z);
			cv3[i,1] = new Vector3((2*cv[i,3] - cv[i,2]).x, (2*cv[i,3] - cv[i,2]).y, (2*cv[i,3] - cv[i,2]).z);
		}

		for (int i = 0; i < 4; i++) {
			for (int j = 2; j < 4; j++) {
				cv3[i,j] = new Vector3(i, 0, j+3)*scaleF;
			}
		}

	}

	void Patch4(int stacks, int slices, ref Vector3[,] cv4){
		//////////////////////////////  4th surface

		for(int i = 0; i < 4; i++) {
			cv4[i,0] = new Vector3(cv2[i,3].x, cv2[i,3].y, cv2[i,3].z);
			cv4[i,1] = new Vector3((2*cv2[i,3] - cv2[i,2]).x, (2*cv2[i,3] - cv2[i,2]).y, (2*cv2[i,3] - cv2[i,2]).z);
		}

		for (int i = 0; i < 4; i++) {
			for (int j = 2; j < 4; j++) {
				cv4[i,j] = new Vector3(i+3, 0, j+3)*scaleF;
			}
		}
	}


	void UpdateMeshData(List<Vector3> v) {
		mesh.vertices = v.ToArray ();
		mesh.uv = newUVs.ToArray();
		mesh.triangles = newTriangles;

		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
	}


}