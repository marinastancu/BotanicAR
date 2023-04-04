using UnityEngine;

public class AudioBehaviour : MonoBehaviour
{
    public AudioDetector detector;

    public float loudnessSensitivity = 50f;
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
            Color previousColor = renderer.sharedMaterial.color;
            Color newColor = Color.Lerp(previousColor, Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), Mathf.PingPong(Time.time, 1));
            renderer.sharedMaterial.color = newColor;
        }
    }
}
