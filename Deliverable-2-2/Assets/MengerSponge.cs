using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MengerSponge : MonoBehaviour {

	public GameObject prefab;
	public float size = 300f;
	List<GameObject> cubes = new List<GameObject>();
	// Use this for initialization
	void Start () {
		GameObject go = Instantiate (prefab, new Vector3 (0, 0, 0), Quaternion.identity);
		go.transform.localScale = new Vector3 (size, size, size);
		go.GetComponent<MengerBox>().size = size;
		cubes.Add (go);
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(0f, 30f * Time.deltaTime, 0f);
		if (Input.GetButtonUp ("Fire1")) {
			List<GameObject> newCubes = Split(cubes);

			foreach (var go in cubes) {
				Destroy (go);
			}
			cubes = newCubes;
		}	
	}

	List<GameObject> Split(List<GameObject> cubes) {
		List<GameObject> subCubes = new List<GameObject> ();

		foreach (var cube in cubes) {
			float size = cube.GetComponent<MengerBox>().size;

			for (int x = -1; x < 2; x++) {
				for (int y = -1; y < 2; y++) {
					for (int z = -1; z < 2; z++) {

						float _x = x * (size / 3f);
						float _y = y * (size / 3f);
						float _z = z * (size / 3f);

						Vector3 cubePos = new Vector3(_x,_y, _z) + cube.transform.position;

						int sum = Mathf.Abs(x) + Mathf.Abs(y) + Mathf.Abs(z);

						if (sum > 1) {

							GameObject copy = Instantiate (cube, cubePos, Quaternion.identity);
							copy.GetComponent<MengerBox> ().size = size / 3f;
							copy.transform.localScale = new Vector3 (size / 3f, size / 3f, size / 3f);
							subCubes.Add (copy);
						}
					}
				}
			}

		}


		return subCubes;

	}
}
