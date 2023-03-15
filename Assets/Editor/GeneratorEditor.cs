using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LSystemGenerator))]
public class GeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LSystemGenerator generator = (LSystemGenerator)target;
        if (DrawDefaultInspector())
        {
            if (generator.autoUpdate)
            {
                generator.Generate(clean: true);
            }
        }
        if (GUILayout.Button("Generate"))
        {
            generator.Generate(clean: true);
        }
    }
}

