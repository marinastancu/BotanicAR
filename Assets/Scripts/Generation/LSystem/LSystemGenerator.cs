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

    // Used for controlling the generation
    public bool autoUpdate;

    [Header("Randomization")]
    [SerializeField]
    [Range(-80, 80)]
    public float minAngle = -10;

    [SerializeField]
    [Range(-80, 80)]
    public float maxAngle = 10;

    [Header("Generation")]
    [SerializeField]
    private LSystem lSystem;

    [SerializeField]
    [Range(1, 100)]
    private float length = 5.0f;

    [SerializeField]
    [Range(1, 100)]
    private float width = 5.0f;

    [SerializeField]
    [Range(-100, 100)]
    public float angle = 10;

    [SerializeField]
    [Range(1, 5)]
    private int numberOfGenerations = 1;

    private LSystemTransform point = new LSystemTransform();
    private Stack<LSystemTransform> stack = new Stack<LSystemTransform>();
    private Vector3 initialPosition = Vector3.zero;
    private int currentLine = 0;
    private List<GameObject> lines = new List<GameObject>();

    private void Start() { }

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
        lSystem.Generate(numberOfGenerations);
        DrawLines();
    }

    private void Line()
    {
        initialPosition = transform.position;
        transform.Translate(Vector3.up * length);
        GameObject line = Instantiate(branch);
        line.transform.parent = transform;
        line.name = $"Line_{currentLine}";
        line.GetComponent<LineRenderer>().SetPosition(0, initialPosition);
        line.GetComponent<LineRenderer>().SetPosition(1, transform.position);
        line.GetComponent<LineRenderer>().startWidth = width;
        line.GetComponent<LineRenderer>().endWidth = width;
        line.GetComponent<LineRenderer>().material = material;
        lines.Add(line);
        currentLine++;
    }

    private void DrawLines()
    {
        string sentence = lSystem.sentence;
        for (int i = 0; i < sentence.Length; i++)
        {
            switch (sentence[i])
            {
                case 'F':
                    Line();
                    break;
                case 'X':
                    break;
                case '+':
                    transform.Rotate(Vector3.back * (angle + (Random.Range(minAngle, maxAngle) / 100)));
                    break;
                case '-':
                    transform.Rotate(Vector3.forward * (angle + (Random.Range(minAngle, maxAngle) / 100)));
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
