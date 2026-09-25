using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetManager : MonoBehaviour
{
    public GameObject[] JalanPrefabs;
    public float ZSpawn;          
    public float JalanLength;     
    public int NumberOfJalan;

    private List<GameObject> _activeJalan = new List<GameObject>();
    private Queue<int> _lastSpawnedIndexes = new Queue<int>();

    public Transform PlayerTransform;

    void Start()
    {
        // Spawn jalan awal
        for (int i = 0; i < NumberOfJalan; i++)
        {
            SpawnJalan(i == 0 ? 0 : Random.Range(0, JalanPrefabs.Length));
        }
    }

    void Update() 
    {
        // Periksa apakah posisi pemain sudah melewati batas untuk spawn jalan baru
        if (PlayerTransform.position.z - 35 > ZSpawn - (NumberOfJalan * JalanLength))
        {
            SpawnJalan(GetRandomJalanIndex());
            DeleteJalan();
        }
    }

    public void SpawnJalan(int JalanIndex)
    {
        // Instansiasi jalan baru dan menambahkannya ke daftar aktif
        GameObject go = Instantiate(JalanPrefabs[JalanIndex], transform.forward * ZSpawn, transform.rotation);
        _activeJalan.Add(go);
        ZSpawn += JalanLength; // Perbarui posisi spawn

        // Tambahkan indeks ke antrian dan pastikan antrian tidak terlalu panjang
        _lastSpawnedIndexes.Enqueue(JalanIndex);
        if (_lastSpawnedIndexes.Count > 3) // Komentar disesuaikan dengan kode
        {
            _lastSpawnedIndexes.Dequeue();
        }
    }

    private void DeleteJalan()
    {
        // Hapus jalan yang paling awal dari daftar aktif
        Destroy(_activeJalan[0]);
        _activeJalan.RemoveAt(0);
    }

    private int GetRandomJalanIndex()
    {
        // Jika sudah menggunakan semua indeks yang tersedia, kosongkan antrian
        if (_lastSpawnedIndexes.Count >= JalanPrefabs.Length)
        {
            _lastSpawnedIndexes.Clear();
        }

        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, JalanPrefabs.Length);
        } while (_lastSpawnedIndexes.Contains(randomIndex) && _lastSpawnedIndexes.Count < JalanPrefabs.Length);

        return randomIndex;
    }
}