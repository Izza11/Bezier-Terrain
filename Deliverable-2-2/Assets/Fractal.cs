using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fractal : MonoBehaviour {

	public Mesh mesh;
	public Material material;
	public float childScale;
	public int maxDepth;
	int depth;

	private void Start () {
		gameObject.AddComponent<MeshFilter>().mesh = mesh;
		gameObject.AddComponent<MeshRenderer>().material = material;
		GetComponent<MeshRenderer>().material.color = Color.Lerp(Color.white, Color.yellow, (float)depth / maxDepth);
		if (depth < maxDepth) {
			StartCoroutine(CreateChildren());
		}
	}

	private void Update () {
		transform.Rotate(0f, 30f * Time.deltaTime, 0f);
	}

	private IEnumerator CreateChildren () {
		yield return new WaitForSeconds(0.5f);
		new GameObject("Fractal Child").AddComponent<Fractal>().Initialize(this, Vector3.up, Quaternion.identity);

		yield return new WaitForSeconds(0.5f);
		new GameObject("Fractal Child").AddComponent<Fractal>().Initialize(this, Vector3.right, Quaternion.Euler(0f, 0f, -90f));

		yield return new WaitForSeconds(0.5f);
		new GameObject("Fractal Child").AddComponent<Fractal>().Initialize(this, Vector3.left, Quaternion.Euler(0f, 0f, 90f));

		yield return new WaitForSeconds(0.5f);
		new GameObject("Fractal Child").AddComponent<Fractal>().Initialize(this, Vector3.down, Quaternion.Euler(0f, 0f, 180f));

		yield return new WaitForSeconds(0.5f);
		new GameObject("Fractal Child").AddComponent<Fractal>().Initialize(this, Vector3.forward, Quaternion.Euler(-90f, 0f, 0f));

		yield return new WaitForSeconds(0.5f);
		new GameObject("Fractal Child").AddComponent<Fractal>().Initialize(this, Vector3.back, Quaternion.Euler(90f, 0f, 0f));

	}




	void Initialize (Fractal parent, Vector3 direction, Quaternion orientation) {
		mesh = parent.mesh;
		material = parent.material;
		maxDepth = parent.maxDepth;
		depth = parent.depth + 1;
		childScale = parent.childScale;
		transform.parent = parent.transform;
		transform.localScale = Vector3.one * childScale;                       // or Vector3(1, 1, 1).
		transform.localPosition = direction * (0.5f + 0.5f * childScale);    // or Vector3(0, 1, 0).
		transform.localRotation = orientation;
	}




}
