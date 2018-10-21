using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(MeshFilter))]
public class BezierPatch : MonoBehaviour {

	List<Triangle> v;   //all the triangles will be stored here
	Vector3[,] cv, cv2, cv3, cv4;
	uint stacks = 10, slices = 10;
	public float scaleF = 10;
	public Material lineMaterial;
	int startIndexTri;
	int ti; // index of mesh triangles



	// Use this for initialization
	void Start () {
		cv = new Vector3[4,4];
		cv2 = new Vector3[4,4];
		cv3 = new Vector3[4,4];
		cv4 = new Vector3[4,4];

		v = new List<Triangle>();
		InitArray(stacks, slices);
		DisplayPolygon (cv);
		startIndexTri = 0;
	}
		
	void Update() {


		cv[1,0].y = 3*scaleF * Mathf.Sin(Time.time);
		cv[2,0].y = 3*scaleF * Mathf.Sin(Time.time);
		cv[2,2].y = 3*scaleF * Mathf.Sin(Time.time);
		cv[1,1].y = 3*scaleF * Mathf.Sin(Time.time);
		cv[3,3].y = 1*scaleF * Mathf.Sin(Time.time);

		UpdateSurface(ref v, stacks, slices, cv, ref startIndexTri);

		Patch2 (stacks, slices, ref cv2);
		UpdateSurface(ref v, stacks, slices, cv2, ref startIndexTri);

		Patch2 (stacks, slices, ref cv3);
		UpdateSurface(ref v, stacks, slices, cv3, ref startIndexTri);

		Patch2 (stacks, slices, ref cv3);
		UpdateSurface(ref v, stacks, slices, cv4, ref startIndexTri);

		startIndexTri = 0;
	
	}

	void OnPostRender() {
		DrawSurface (ref v, new Vector3 (0, 0, 0));
	}
		

	void DrawSurface(ref List<Triangle> v, Vector3 color) {
		GL.PushMatrix();
		lineMaterial.SetPass(0);
		GL.Begin(GL.TRIANGLES);
		//GL.Color(Color.red);

		for (int i = 0; i < v.Count; i++) {
			GL.Vertex(v[i].c/5);
			GL.Vertex(v[i].b/5);
			GL.Vertex(v[i].a/5);

		}
		GL.End();
		GL.PopMatrix();

	}
		

	void DisplayPolygon(Vector3[,] cv) {
		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < 3; j++) {
				Debug.DrawLine(cv[i,j], cv[i,j+1], Color.red);
			}
		}

		for (int i = 0; i < 3; i++)
			for (int j = 0; j < 4; j++)
				Debug.DrawLine(cv[i,j], cv[i + 1,j], Color.red);

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


	void CreateSurface(ref List<Triangle> tri, uint stacks, uint slices, Vector3[,] cv) {
			
		Vector3 a = new Vector3 (0, 0, 0);
		Vector3 b = new Vector3 (0, 0, 0);
		Vector3 c = new Vector3 (0, 0, 0);

		float deltau = 1 / (float)stacks;
		float deltav = 1 / (float)slices;

		for (uint i = 0; i < stacks; i++) {
			float u = i * deltau;
			for (uint j = 0; j < slices; j++) {
				float v = j * deltav;
				//the first triangle
				a = Q(u,          v, cv);
				b = Q(u + deltau, v, cv);
				c = Q(u,          v + deltav, cv);
				tri.Add (new Triangle(a,b,c));

				//the second triangle
				a = Q(u + deltau, v + deltav, cv);
				tri.Add (new Triangle(c,b,a));
			}
		
		}
	
	}

	void UpdateSurface(ref List<Triangle> tri, uint stacks, uint slices, Vector3[,] cv, ref int k) {  // k = starting index of triangle in tri List for each patch

		Vector3 a = new Vector3 (0, 0, 0);
		Vector3 b = new Vector3 (0, 0, 0);
		Vector3 c = new Vector3 (0, 0, 0);

		float deltau = 1 / (float)stacks;
		float deltav = 1 / (float)slices;

		for (uint i = 0; i < stacks; i++) {
			float u = i * deltau;
			for (uint j = 0; j < slices; j++) {
				float v = j * deltav;
				//the first triangle
				a = Q(u,          v, cv);
				b = Q(u + deltau, v, cv);
				c = Q(u,          v + deltav, cv);
				tri [k].Set (c,b,a);

				//the second triangle
				a = Q(u + deltau, v + deltav, cv);
				tri [k+1].Set (a,b,c);
				k = k + 2;
			}

		}

	}
		


	void InitArray(uint stacks, uint slices)
	{
		
		for (int i = 0; i<4; i++)
			for (int j = 0; j<4; j++)
				cv[i,j] = new Vector3(i, 0, j) * scaleF;
		
		cv[1,0] = new Vector3(1, 3, 0)* scaleF;
		cv[2,0] = new Vector3(2, 3, 0)* scaleF;
		cv[2,2] = new Vector3(2, 3, 2)* scaleF;
		cv[1,1] = new Vector3(1, 3, 1)* scaleF; 
		cv[3,3] = new Vector3(3, 1, 3)* scaleF; 

		CreateSurface(ref v, stacks, slices, cv);


		Patch2 (stacks, slices, ref cv2);
		CreateSurface(ref v, stacks, slices, cv2);


		Patch3 (stacks, slices, ref cv3);
		CreateSurface(ref v, stacks, slices, cv3);


		Patch4 (stacks, slices, ref cv4);
		CreateSurface(ref v, stacks, slices, cv4);

	}


	void Patch2(uint stacks, uint slices, ref Vector3[,] cv2){
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

	void Patch3(uint stacks, uint slices, ref Vector3[,] cv3){
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

	void Patch4(uint stacks, uint slices, ref Vector3[,] cv4){
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
				
		
}


/*
 * bezir patch 4
 * bezier patch multiple
 * bezier patch color = 12pm
 * 
 * 12-2pm
 * 
 * 3-12am - fractals
 * 
 * sunday: 2 - 9pm midi
 * 
 *
 * 
 * 
 */