using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEditor.UnityLinker;

public class LSystemGenerator : MonoBehaviour
{
    public Camera camera;

    private bool initializing = true;

    [SerializeField]
    private GameObject branch;

    [SerializeField]
    private Material material;

    // Used for controlling the generation
    public bool autoUpdate;

    [Header("Generation")]
    [SerializeField]
    private LSystem lSystem;

    [SerializeField]
    [Range(1, 5)]
    private float length = 1f;

    [SerializeField]
    [Range(1, 5)]
    private float width = 1f;

    [SerializeField]
    [Range(-360, 360)]
    public float angle = 10f;

    [SerializeField]
    [Range(1, 5)]
    private int numberOfGenerations = 4;

    private Stack<LSystemTransform> stack = new Stack<LSystemTransform>();
    private Vector3 initialPosition = Vector3.zero;
    private int currentLine = 0;
    private List<GameObject> lines = new List<GameObject>();
    private List<GameObject> meshes = new List<GameObject>();

    private void Update()
    {
        if (initializing)
        {
            Generate(clean: true);
            initializing = false;
        }
    }

    private void CleanExistingSystem()
    {
        lSystem.RestoreToOriginalSentence();
        stack.Clear();
        foreach (GameObject line in lines)
        {
            DestroyImmediate(line);
        }
        foreach (GameObject mesh in meshes)
        {
            DestroyImmediate(mesh);
        }
        currentLine = 0;
    }

    public void Generate(bool clean = false)
    {
        lSystem.SaveOriginalSentence();

        if (clean) CleanExistingSystem();

        if (lSystem == null)
        {
            Debug.LogError("You must have an lSystem defined");
            enabled = false;
        }
        if (lSystem.RuleCount == 0)
        {
            Debug.LogError("You must have at least one rule defined");
            enabled = false;
        }

        length = Random.Range(1, 5);
        width = Random.Range(1, 5);
        angle = Random.Range(-360, 360);

        if (angle >= 0 && angle <= 90)
        {
            angle = 90;
        }

        if (angle <= 0 && angle >= -90)
        {
            angle = -90;
        }

        for (int i = 0; i < numberOfGenerations; i++)
        {
            lSystem.Generate();
            DrawLines(i + 1);
        }

        MeshHelper.CombineMeshes(gameObject);

        CleanExistingSystem();
    }

    private void Line(int generation)
    {
        initialPosition = transform.position;
        transform.Translate(Vector3.up * length);
        GameObject line = Instantiate(branch);
        line.transform.parent = transform;
        line.name = $"Line_{currentLine}";
        line.GetComponent<LineRenderer>().SetPosition(0, initialPosition);
        line.GetComponent<LineRenderer>().SetPosition(1, transform.position);
        line.GetComponent<LineRenderer>().startWidth = width / generation;
        line.GetComponent<LineRenderer>().endWidth = width / generation;
        line.GetComponent<LineRenderer>().material = material;
        lines.Add(line);
        GenerateMesh(line, currentLine);
        line.GetComponent<LineRenderer>().enabled = false;
        currentLine++;
    }

    private void GenerateMesh(GameObject line, int currentLine)
    {
        GameObject meshObject = new GameObject($"Mesh_{currentLine}", typeof(MeshRenderer), typeof(MeshFilter));
        meshObject.transform.SetParent(transform, false);
        Renderer renderer = meshObject.GetComponent<Renderer>();
        renderer.sharedMaterial = line.GetComponent<LineRenderer>().sharedMaterial;

        MeshFilter meshFilter = meshObject.GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mesh.name = $"Mesh_{currentLine}";
        line.GetComponent<LineRenderer>().BakeMesh(mesh, camera, true);
        meshFilter.mesh = mesh;
        Vector3[] vertices;
        vertices = mesh.vertices;

        Vector3[] verticesNew = {
            vertices[0],
            vertices[1],
            vertices[2],
            vertices[3],
            new Vector3 (vertices[3].x, vertices[3].y,  vertices[3].z - 1f),
            new Vector3 (vertices[2].x, vertices[2].y,  vertices[2].z - 1f),
            new Vector3 (vertices[1].x, vertices[1].y,  vertices[1].z - 1f),
            new Vector3 (vertices[0].x, vertices[0].y,  vertices[0].z - 1f),
        };
        //Debug.Log("start");
        //foreach (Vector3 v in vertices)
        //{

        //    Debug.Log(v);
        //}
        int[] triangles = {
            0, 2, 1, //face front
	        0, 3, 2,
            2, 3, 4, //face top
	        2, 4, 5,
            1, 2, 5, //face right
	        1, 5, 6,
            0, 7, 4, //face left
	        0, 4, 3,
            7, 4, 5, //face back
	        7, 6,4,
            0, 6, 7, //face bottom
	        0, 1, 6
        };
        mesh.Clear();
        mesh.vertices = verticesNew;
        mesh.triangles = triangles;
        mesh.Optimize();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshes.Add(meshObject);
    }

    private void DrawLines(int generation)
    {
        string sentence = lSystem.sentence;
        for (int i = 0; i < sentence.Length; i++)
        {
            switch (sentence[i])
            {
                case 'F':
                    Line(generation);
                    break;
                case 'X':
                    break;
                case '+':
                    transform.Rotate(Vector3.back * (angle / 10));
                    break;
                case '-':
                    transform.Rotate(Vector3.forward * (angle / 10));
                    break;
                case '*':
                    transform.Rotate(Vector3.up * 120);
                    break;
                case '/':
                    transform.Rotate(Vector3.down * 120);
                    break;
                case '[':
                    stack.Push(new LSystemTransform()
                    {
                        position = transform.position,
                        rotation = transform.rotation
                    });
                    break;
                case ']':
                    LSystemTransform ti = stack.Pop();
                    transform.position = ti.position;
                    transform.rotation = ti.rotation;
                    break;
                default:
                    throw new InvalidOperationException("Invalid L-tree operation");
            }
        }
    }
}
