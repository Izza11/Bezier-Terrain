using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierP4 : MonoBehaviour {

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

		vertSize = (Bpatches*Bpatches*stacks + 1) * (Bpatches*Bpatches*stacks + 1);

		k = m = 0;
		CVs = new List<Vector3> ();

		newVertices = new List<Vector3> ();
		newUVs = new List<Vector2> ();
		newTriangles = new int[stacks * stacks * Bpatches*Bpatches * 6];

		InitArray(ref CVs, stacks, slices);        // newVertices get filled with data from CreateSurface

		k = m = 0;

		UpdateMeshData (newVertices);

	}

	void InitializeCVs(ref List<Vector3> CVs) {
		int sizex = 4; int sizez = 4;
		int runCount = -1;

		for (int ir = 0; ir < Bpatches; ir++) {
			for (int jr = 0; jr < Bpatches; jr++) {
				for (int i = 0; i < sizez; i++) {
					for (int j = 0; j < sizex; j++) {

						CVs.Add ( new Vector3(i+ (3*ir), 0, j+ (3*jr) ) * scaleF );
						int s = j + (4*jr);
						int t = i + (4 * ir);
						print ("x: " + t + ", y: " + 0 + ", z: " + s);
					}

				}

			}
		}



	}

	void JoinCVs(ref List<Vector3> CVs){
		int CVsize = CVs.Count;
		bool doubleJoin = false;
		int rows_passed = 0;

		for (int i = 0; i < CVsize-16; i += 16) {

			if (i % ((Bpatches - 1) * 16) == 0) {

				rows_passed++;

			} else {
				
				// forward joining in u direction	
				CVs [i + 3] = CVs [i + 16] = (CVs [i + 2] + CVs [i + 16 + 1]) / 2;
				CVs [i + 7] = CVs [i + 20] = (CVs [i + 6] + CVs [i + 20 + 1]) / 2;
				CVs [i + 11] = CVs [i + 24] = (CVs [i + 10] + CVs [i + 24 + 1]) / 2;
				CVs [i + 15] = CVs [i + 28] = (CVs [i + 14] + CVs [i + 28 + 1]) / 2;

			}

			if ((rows_passed + 1) < Bpatches) {
				// downwards joining in v direction

				CVs [i + 12] = CVs [i + 48] = (CVs [i + 8] + CVs [i + 48]) / 2;
				CVs [i + 13] = CVs [i + 49] = (CVs [i + 9] + CVs [i + 49]) / 2;
				CVs [i + 14] = CVs [i + 50] = (CVs [i + 10] + CVs [i + 50]) / 2;

			}

		}

	}

	void Update() {

		// NOTHING HERE YET

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


	void CreateSurfaceSmooth(ref List<Vector3> vert, int stacks, int slices, Vector3[,] cv, int id){
		Vector3 a = new Vector3 (0, 0, 0);

		float deltau = 1 / (float)stacks;
		float deltav = 1 / (float)slices;

		for (int i = 0; i <= stacks; i++) {
			float u = i * deltau;
			for (int j = 0; j <= slices; j++) {
				float v = j * deltav;

				a = Q(u, v, cv);
				vert.Add (a);

				int uvD = stacks* Bpatches*Bpatches;
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

	

	}



	void InitArray(ref List<Vector3> CVs, int stacks, int slices)
	{

		InitializeCVs (ref CVs);
		JoinCVs (ref CVs);

		int k = 0;

		for (int i = 0; i < Bpatches*Bpatches; i++) {
			Vector3[,] CVnew = new Vector3[4,4];

			for (int m = 0; m < 4; m++) {
				for (int n = 0; n < 4; n++) {
					CVnew [m,n] = CVs[k];
					k++;
				}

			}

			CreateSurfaceSmooth(ref newVertices, stacks, slices, CVnew, i);

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