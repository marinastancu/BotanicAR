using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LSystemGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject branch;

    [SerializeField]
    private Material material;

    [SerializeField]
    [Range(0, 20)]
    private float seconds = 5.0f;

    private float timer = 0;

    // Used for controlling the generation
    public bool autoUpdate;

    [Header("Randomization")]
    [SerializeField]
    [Range(-100, 100)]
    public float variation = 50f;

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
    [Range(-60, 60)]
    public float angle = 10f;

    [SerializeField]
    [Range(1, 5)]
    private int numberOfGenerations = 3;

    private Stack<LSystemTransform> stack = new Stack<LSystemTransform>();
    private Vector3 initialPosition = Vector3.zero;
    private int currentLine = 0;
    private List<GameObject> lines = new List<GameObject>();

    private void Start()
    {
        Generate(clean: false);
    }

    private void Update()
    {
        if (timer >= seconds)
        {
            Generate(clean: true);
            timer = 0;
        }
        timer += Time.deltaTime * 1.0f;
    }

    private void CleanExistingSystem()
    {
        lSystem.RestoreToOriginalSentence();
        stack.Clear();
        foreach (GameObject line in lines)
        {
            DestroyImmediate(line, true);
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

        variation = Random.Range(-100, 100);
        length = Random.Range(1, 5);
        width = Random.Range(1, 5);
        angle = Random.Range(-60, 60);

        for (int i = 0; i < numberOfGenerations; i++)
        {
            lSystem.Generate();
            DrawLines(i + 1);
        }
    }

    private void Line(int generation)
    {
        initialPosition = transform.position;
        transform.Translate(Vector3.up * (length / 100));
        GameObject line = Instantiate(branch);
        line.transform.parent = transform;
        line.name = $"Line_{currentLine}";
        line.GetComponent<LineRenderer>().SetPosition(0, initialPosition);
        line.GetComponent<LineRenderer>().SetPosition(1, transform.position);
        line.GetComponent<LineRenderer>().startWidth = (width / 100) / (generation * 10);
        line.GetComponent<LineRenderer>().endWidth = (width / 100) / (generation * 10);
        line.GetComponent<LineRenderer>().material = material;
        lines.Add(line);
        currentLine++;
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
                    transform.Rotate(Vector3.back * (angle + variation / 10));
                    break;
                case '-':
                    transform.Rotate(Vector3.forward * (angle + variation / 10));
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
