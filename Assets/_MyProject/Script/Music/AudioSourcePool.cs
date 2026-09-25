using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourcePool : MonoBehaviour
{
    public static AudioSourcePool Instance;

    [SerializeField]
    private int initialPoolSize = 10;
    private List<AudioSource> pool;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePool();
        }
    }

    private void InitializePool()
    {
        pool = new List<AudioSource>(initialPoolSize);
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewAudioSource();
        }
    }

    private AudioSource CreateNewAudioSource()
    {
        AudioSource newSource = new GameObject("PooledAudioSource").AddComponent<AudioSource>();
        newSource.gameObject.SetActive(false);
        newSource.transform.SetParent(transform);
        pool.Add(newSource);
        return newSource;
    }

    public AudioSource GetAudioSource()
    {
        foreach (var source in pool)
        {
            if (!source.gameObject.activeInHierarchy)
            {
                source.gameObject.SetActive(true);
                return source;
            }
        }

        // Jika tidak ada yang tersedia, buat yang baru
        return CreateNewAudioSource();
    }

    public void ReturnAudioSource(AudioSource source)
    {
        source.Stop();
        source.gameObject.SetActive(false);
    }
}