using UnityEngine;

public class MaterialAudioBehaviour : MonoBehaviour
{
    public AudioDetector detector;

    public float loudnessSensitivity = 0.1f;
    public float threshold = 0.1f;
    private Renderer renderer;
    public void Start()
    {
        renderer = GetComponent<Renderer>();
    }
    public void Update()
    {
        float loudness = detector.GetLoudnessFromMicrophone() * loudnessSensitivity;

        if (loudness > threshold)
        {
            Color previousColor = renderer.material.color;
            Color newColor = Color.Lerp(previousColor, Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), Mathf.PingPong(Time.time, 1));
            renderer.material.color = newColor;
        }
    }
}
