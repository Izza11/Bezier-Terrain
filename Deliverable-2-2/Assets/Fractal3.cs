using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fractal3 : MonoBehaviour {

	public Mesh mesh;
	public Material material;
	public float childScale;
	public int maxDepth;
	int depth;

	private Material[,] materials;
	public Mesh[] meshes;

	private void InitializeMaterials () {
		materials = new Material[maxDepth + 1, 2];
		for (int i = 0; i <= maxDepth; i++) {
			float t = i / (maxDepth - 1f);
			t *= t;
			materials[i, 0] = new Material(material);
			materials[i, 0].color = Color.Lerp(Color.white, Color.yellow, t);
			materials[i, 1] = new Material(material);
			materials[i, 1].color = Color.Lerp(Color.white, Color.cyan, t);
		}
		materials[maxDepth, 0].color = Color.magenta;
		materials[maxDepth, 1].color = Color.red;
	}

	private void Start () {
		if (materials == null) {
			InitializeMaterials();
		}
		gameObject.AddComponent<MeshFilter>().mesh =
			meshes[Random.Range(0, meshes.Length)];
		gameObject.AddComponent<MeshRenderer>().material =
			materials[depth, Random.Range(0, 2)];
		if (depth < maxDepth) {
			StartCoroutine(CreateChildren());
		}
	}

	private void Update () {
		transform.Rotate(0f, 30f * Time.deltaTime, 0f);
	}

	private IEnumerator CreateChildren () {
		yield return new WaitForSeconds(0.5f);
		new GameObject("Fractal Child").AddComponent<Fractal3>().Initialize(this, Vector3.up, Quaternion.identity);

		yield return new WaitForSeconds(0.5f);
		new GameObject("Fractal Child").AddComponent<Fractal3>().Initialize(this, Vector3.right, Quaternion.Euler(0f, 0f, -90f));

		yield return new WaitForSeconds(0.5f);
		new GameObject("Fractal Child").AddComponent<Fractal3>().Initialize(this, Vector3.left, Quaternion.Euler(0f, 0f, 90f));

		yield return new WaitForSeconds(0.5f);
		new GameObject("Fractal Child").AddComponent<Fractal3>().Initialize(this, Vector3.down, Quaternion.Euler(0f, 0f, 180f));

		yield return new WaitForSeconds(0.5f);
		new GameObject("Fractal Child").AddComponent<Fractal3>().Initialize(this, Vector3.forward, Quaternion.Euler(-90f, 0f, 0f));

		yield return new WaitForSeconds(0.5f);
		new GameObject("Fractal Child").AddComponent<Fractal3>().Initialize(this, Vector3.back, Quaternion.Euler(90f, 0f, 0f));

	}




	void Initialize (Fractal3 parent, Vector3 direction, Quaternion orientation) {
		meshes = parent.meshes;
		materials = parent.materials;
		maxDepth = parent.maxDepth;
		depth = parent.depth + 1;
		childScale = parent.childScale;
		transform.parent = parent.transform;
		transform.localScale = Vector3.one * childScale;                       // or Vector3(1, 1, 1).
		transform.localPosition = direction * (0.5f + 0.5f * childScale);    // or Vector3(0, 1, 0).
		transform.localRotation = orientation;
	}




}
