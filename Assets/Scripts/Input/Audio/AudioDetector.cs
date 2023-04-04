using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Android;

public class AudioDetector : MonoBehaviour
{
    public int sampleWindow = 64;
    private AudioClip microphoneClip;

    async void Start()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            await Task.Run(() => Permission.RequestUserPermission(Permission.Microphone));
        }

        MicrophoneToAudioClip();
    }

    public void MicrophoneToAudioClip()
    {
        string microphoneName = Microphone.devices[0];
        microphoneClip = Microphone.Start(microphoneName, true, 20, AudioSettings.outputSampleRate);
    }

    public float GetLoudnessFromMicrophone()
    {
        return GetLoudnessFromAudioclip(Microphone.GetPosition(Microphone.devices[0]), microphoneClip);
    }

    public float GetLoudnessFromAudioclip(int clipPosition, AudioClip clip)
    {
        int startPosition = clipPosition - sampleWindow;
        if (startPosition < 0)
        {
            return 0;
        }
        float[] waveData = new float[sampleWindow];
        clip.GetData(waveData, startPosition);

        float totalLoudness = 0;

        for (int i = 0; i < sampleWindow; i++)
        {
            totalLoudness += Mathf.Abs(waveData[i]);
        }

        return totalLoudness / sampleWindow;
    }
}
