using UnityEngine;

public class Buff : MonoBehaviour
{
    AudioSource source;
    public AudioClip bounce;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            source.clip = bounce;
            source.volume = .5f;
            source.Play();
        }
    }
}
