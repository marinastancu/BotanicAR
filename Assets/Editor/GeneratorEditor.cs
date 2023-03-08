using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LSystemTurtle))]
public class GeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LSystemTurtle lSystemTurtle = (LSystemTurtle)target;
        if (DrawDefaultInspector())
        {
            if (lSystemTurtle.autoUpdate)
            {
                lSystemTurtle.angle = Random.Range(lSystemTurtle.minAngle, lSystemTurtle.maxAngle);
                lSystemTurtle.Generate(clean: true);
            }
        }
        if (GUILayout.Button("Generate"))
        {
            lSystemTurtle.angle = Random.Range(lSystemTurtle.minAngle, lSystemTurtle.maxAngle);
            lSystemTurtle.Generate(clean: true);
        }
    }
}

