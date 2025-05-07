using UnityEngine;

public class PhoneRingController : MonoBehaviour
{
    private AudioSource ringSource;

    void Start()
    {
        ringSource = GetComponent<AudioSource>();
        ringSource.Play(); // Start ringing when scene loads
    }

    public void StopRinging()
    {
        if (ringSource.isPlaying)
        {
            ringSource.Stop();
        }
    }
}
