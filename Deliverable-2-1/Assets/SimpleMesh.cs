using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMesh : MonoBehaviour {

	public Material mat;
	public Matrix4x4 m;
	void OnPostRender() {
		if (!mat) {
			Debug.LogError("Please Assign a material on the inspector");
			return;
		}
		GL.PushMatrix();
		mat.SetPass(0);
		GL.LoadOrtho();
		//GL.LoadIdentity();
		//GL.MultMatrix (m);
		GL.Begin(GL.TRIANGLES);
		GL.Color(new Color(0, 0, 1, 1));
		GL.Vertex3(0.5F, 0.25F, 0);
		GL.Vertex3(0.25F, 0.25F, 0);
		GL.Vertex3(0.375F, 0.5F, 0);
		GL.End();
		GL.PopMatrix ();
		/*GL.Begin(GL.QUADS);
		GL.Color(new Color(0.5F, 0.5F, 0.5F, 1));
		GL.Vertex3(0.5F, 0.5F, 0);
		GL.Vertex3(0.5F, 0.75F, 0);
		GL.Vertex3(0.75F, 0.75F, 0);
		GL.Vertex3(0.75F, 0.5F, 0);
		GL.End();
		GL.Begin(GL.LINES);
		GL.Color(new Color(0, 0, 0, 1));
		GL.Vertex3(0, 0, 0);
		GL.Vertex3(0.75F, 0.75F, 0);
		GL.End();
		GL.PopMatrix();*/
	}
}
