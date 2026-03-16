using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class VictoryManager : MonoBehaviour
{
    [Header("Victory Music")]
    public AudioSource musicSource;
    public AudioClip victoryMusic;

    [Header("Explosion Sound")]
    public AudioClip explosionSound;
    public float explosionVolume = 1f;

    [Header("Firework Prefabs")]
    public GameObject[] fireworksPrefabs;

    [Header("Firework Brightness")]
    public float emissionMultiplier = 4f;

    [Header("Rocket")]
    public GameObject rocketPrefab;
    public float rocketSpeed = 8f;
    public float rocketHeight = 6f;

    [Header("Spawn Area")]
    public Vector3 areaCenter = Vector3.zero;
    public Vector3 areaSize = new Vector3(5, 0, 5);

    [Header("Timing")]
    public float spawnInterval = 1.5f;

    [Header("Rocket Sound")]
    public AudioClip rocketWhistle;
    public float rocketVolume = 0.25f;

    bool running = false;

    void Start()
    {
        StartCelebration();
    }

    public void StartCelebration()
    {
        if (running) return;
        running = true;

        if (musicSource && victoryMusic)
        {
            musicSource.clip = victoryMusic;
            musicSource.Play();
        }

        StartCoroutine(FireworksRoutine());
    }

    IEnumerator FireworksRoutine()
    {
        while (running)
        {
            SpawnRocket();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnRocket()
    {
        Vector3 randomPos = new Vector3(
            Random.Range(areaCenter.x - areaSize.x / 2, areaCenter.x + areaSize.x / 2),
            transform.position.y,
            Random.Range(areaCenter.z - areaSize.z / 2, areaCenter.z + areaSize.z / 2)
        );

        GameObject rocket = Instantiate(rocketPrefab, randomPos, Quaternion.identity);

        if (rocketWhistle != null)
        {
            AudioSource a = rocket.AddComponent<AudioSource>();
            a.clip = rocketWhistle;
            a.volume = rocketVolume;
            a.spatialBlend = 1f;
            a.Play();
        }

        StartCoroutine(RocketFlight(rocket));
    }

    IEnumerator RocketFlight(GameObject rocket)
    {
        Vector3 start = rocket.transform.position;
        Vector3 target = start + Vector3.up * rocketHeight;

        while (rocket && rocket.transform.position.y < target.y)
        {
            rocket.transform.position += Vector3.up * rocketSpeed * Time.deltaTime;
            yield return null;
        }

        if (rocket)
        {
            SpawnFirework(rocket.transform.position);
            Destroy(rocket);
        }
    }

    void SpawnFirework(Vector3 pos)
    {
        if (fireworksPrefabs.Length == 0) return;

        GameObject prefab = fireworksPrefabs[Random.Range(0, fireworksPrefabs.Length)];
        GameObject firework = Instantiate(prefab, pos, Quaternion.identity);

        // Play explosion sound
        if (explosionSound != null)
            AudioSource.PlayClipAtPoint(explosionSound, pos, explosionVolume);

        // Boost particle brightness (HDR effect)
        ParticleSystem ps = firework.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            Color c = main.startColor.color;
            main.startColor = new ParticleSystem.MinMaxGradient(c * emissionMultiplier);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(areaCenter, areaSize);
    }
}