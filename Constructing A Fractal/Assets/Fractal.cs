using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fractal : MonoBehaviour {

	public Mesh mesh;
	public Material material;
	public int maxDepth;

	private int depth;

	private Color[] colors = {Color.yellow, Color.red, Color.blue};
	private Material[,] materials;

	private void InitializeMaterials() {
		materials = new Material[maxDepth + 1, colors.Length];
		for (int i = 0; i <= maxDepth; i++) {
			float t = i / (float) (maxDepth - 1);
			t *= t;
			for (int j = 0; j < colors.Length; j++) {
				materials[i, j] = new Material(material);
				materials[i, j].color = Color.Lerp(Color.white, colors[j], t);
			}
		}
		materials[maxDepth, 0].color = Color.magenta;
		materials[maxDepth, 1].color = Color.green;
		materials[maxDepth, 2].color = Color.black;
	}

	public float maxRotationSpeed;

	private float rotationSpeed;

	private void Start() {
		if (materials == null) {
			InitializeMaterials();
		}

		rotationSpeed = Random.Range(-maxRotationSpeed, maxRotationSpeed);
		gameObject.AddComponent<MeshFilter>().mesh = mesh;
		gameObject.AddComponent<MeshRenderer>().material = material;
		GetComponent<MeshRenderer>().material = materials[depth, Random.RandomRange(0, colors.Length)];
		if (depth < maxDepth) {
			StartCoroutine(CreateChildren());
		}
	}

	private static Vector3[] childDirections = {
		Vector3.up,
		Vector3.right,
		Vector3.left,
		Vector3.forward,
		Vector3.back
	};

	private static Quaternion[] childOrientations = {
		Quaternion.identity,
		Quaternion.Euler(0f, 0f, -90f),
		Quaternion.Euler(0f, 0f, 90f),
		Quaternion.Euler(90f, 0f, 0f),
		Quaternion.Euler(-90f, 0f, 0f)
	};

	public float minYieldSecs = 0.1f;
	public float maxYieldSecs = 0.5f;

	public float spawnProbability;

	private IEnumerator CreateChildren() {
		for (int i = 0; i < childDirections.Length; i++) {
			if (Random.value < spawnProbability) {
				yield return new WaitForSeconds(Random.Range(minYieldSecs, maxYieldSecs));
				new GameObject("FractalChild").AddComponent<Fractal>().
					Initialize(this, i);
			}
		}
	}

	public float childScale;

	private void Initialize(Fractal parent, int childIndex) {
		mesh = parent.mesh;
		materials = parent.materials;
		maxDepth = parent.maxDepth;
		depth = parent.depth + 1;
		childScale = parent.childScale;
		spawnProbability = parent.spawnProbability;
		transform.parent = parent.transform;
		transform.localScale = Vector3.one * childScale;
		transform.localPosition =
			childDirections[childIndex] * (0.5f + 0.5f * childScale);
		transform.localRotation = childOrientations[childIndex];
		maxRotationSpeed = parent.maxRotationSpeed;
	}

	void Update () {
		transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
	}
}
