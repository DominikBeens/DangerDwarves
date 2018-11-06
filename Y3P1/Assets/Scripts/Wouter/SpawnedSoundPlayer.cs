using UnityEngine;

public class SpawnedSoundPlayer : MonoBehaviour
{
    private Transform parent;

    public float lifetime;
    public bool randomizePitch;
    public float minPitch;
    public float maxPitch;
    public AudioSource myAudioSource;

    private void Awake()
    {
        parent = transform.parent;

        if (lifetime == 0)
        {
            lifetime = 2;
        }
    }

    private void OnEnable()
    {
        if (minPitch == 0)
        {
            minPitch = 0.9f;
        }
        if (maxPitch == 0)
        {
            maxPitch = 1.1f;
        }

        transform.SetParent(null);

        if (randomizePitch)
        {
            myAudioSource.pitch = Random.Range(minPitch, maxPitch);
        }

        Invoke("ReturnToParent", lifetime);
    }

    private void Update()
    {
        if (parent && !transform.parent)
        {
            transform.position = parent.position;
        }
    }

    private void ReturnToParent()
    {
        transform.SetParent(parent);
    }
}
