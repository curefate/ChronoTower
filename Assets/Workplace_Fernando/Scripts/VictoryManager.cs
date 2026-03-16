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

    [Header("Rocket")]
    public GameObject rocketPrefab;
    public float rocketSpeed = 8f;
    public float rocketHeight = 6f;

    [Header("Spawn Area")]
    public Vector3 areaCenter = Vector3.zero;
    public Vector3 areaSize = new Vector3(5, 0, 5);

    [Header("Timing")]
    public float spawnInterval = 1.5f;

    bool running = false;

    [Header("Explosion Light")]
    public float lightRange = 8f;
    public float lightIntensity = 12f;
    public float lightDuration = 0.25f;

    [Header("Rocket Sound")]
    public AudioClip rocketWhistle;
    public float rocketVolume = 1f; 

    void Start()
    {
        StartCelebration();
    }

    // void Update()
    // {
    //     if (Keyboard.current.digit0Key.wasPressedThisFrame) 
    //     {
    //         StartCelebration();
    //     }
    // }

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
            a.spatialBlend = 1f;   // 3D sound
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

        // play explosion sound
        if (explosionSound != null)
            AudioSource.PlayClipAtPoint(explosionSound, pos, explosionVolume);

        // get firework color
        ParticleSystem ps = firework.GetComponent<ParticleSystem>();
        Color fireworkColor = Color.white;

        if (ps != null)
            fireworkColor = ps.main.startColor.color;

        // create temporary light
        StartCoroutine(SpawnExplosionLight(pos, fireworkColor));
    }

    IEnumerator SpawnExplosionLight(Vector3 pos, Color c)
    {
        GameObject lightObj = new GameObject("FireworkLight");
        lightObj.transform.position = pos;

        Light l = lightObj.AddComponent<Light>();
        l.type = LightType.Point;
        l.color = c;
        l.range = lightRange;
        l.intensity = lightIntensity;
        l.shadows = LightShadows.None;

        yield return new WaitForSeconds(lightDuration);

        Destroy(lightObj);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(areaCenter, areaSize);
    }
}