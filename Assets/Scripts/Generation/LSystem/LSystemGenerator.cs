using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.InputSystem;

public class LSystemGenerator : MonoBehaviour
{
    public Camera camera;
    private Controls controls;
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
    [Range(1, 50)]
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

    private void Awake()
    {
        controls = new Controls();
    }

    private void OnEnable()
    {
        controls.Player.Tap.performed += Tap;
        controls.Player.Enable();
    }

    private void Tap(InputAction.CallbackContext obj)
    {
        Generate(clean: true);
    }

    private void OnDisable()
    {
        controls.Player.Tap.performed -= Tap;
    }

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
            DestroyImmediate(line, true);
        }
        foreach (GameObject mesh in meshes)
        {
            DestroyImmediate(mesh, true);
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
    }

    private void Line(int generation)
    {
        initialPosition = transform.position;
        transform.Translate(Vector3.up * (length / 500));
        GameObject line = Instantiate(branch);
        line.transform.parent = transform;
        line.name = $"Line_{currentLine}";
        line.GetComponent<LineRenderer>().SetPosition(0, initialPosition);
        line.GetComponent<LineRenderer>().SetPosition(1, transform.position);
        line.GetComponent<LineRenderer>().startWidth = (width / 50) / (generation * 10);
        line.GetComponent<LineRenderer>().endWidth = (width / 50) / (generation * 10);
        line.GetComponent<LineRenderer>().material = material;
        lines.Add(line);
        GenerateMesh(line, currentLine);
        line.GetComponent<LineRenderer>().enabled = false;
        currentLine++;

        //GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        //cylinder.transform.localScale = new Vector3(0.001f, 0.01f, 0.001f);
        //cylinder.transform.position = line.GetComponent<LineRenderer>().GetPosition(1);
        //Debug.Log(Quaternion.Euler(line.GetComponent<LineRenderer>().GetPosition(0)));
        //cylinder.transform.rotation = Quaternion.Euler((line.GetComponent<LineRenderer>().GetPosition(0)) * 1000);
    }

    private void GenerateMesh(GameObject line, int currentLine)
    {
        GameObject meshObject = new GameObject($"Mesh_{currentLine}", typeof(MeshRenderer), typeof(MeshFilter));

        Renderer renderer = meshObject.GetComponent<Renderer>();
        renderer.sharedMaterial = line.GetComponent<LineRenderer>().sharedMaterial;

        MeshFilter meshFilter = meshObject.GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mesh.name = $"Mesh_{currentLine}";
        line.GetComponent<LineRenderer>().BakeMesh(mesh, camera, true);
        meshFilter.mesh = mesh;
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
