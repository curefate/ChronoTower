using UnityEngine;
using System.Collections;

public class SignifierManager : MonoBehaviour
{
    [Header("Particle System")]
    public GameObject signifierPrefab;

    [Header("Glow Settings")]
    public float glowScale = 0.05f;

    [Header("Animation")]
    public float scaleDuration = 0.4f;
    public float scaleMultiplier = 1.15f;

    [Header("Rim FX")]
    public Material rimMaterial;
    //public GameObject rimTarget;

    private GameObject currentSignifierInstance;
    private GameObject rimInstance;
    private Coroutine scaleRoutine;

    void Start()
    {
        CreateRimInstance();
    }

    void CreateRimInstance()
    {
        // 🔥 Automatically find the FIRST mesh in parents
        MeshFilter mf = GetComponentInParent<MeshFilter>();
        MeshRenderer mr = GetComponentInParent<MeshRenderer>();

        if (mf == null || mr == null)
        {
            Debug.LogWarning("No MeshFilter/MeshRenderer found in parents of " + gameObject.name);
            return;
        }

        GameObject target = mf.gameObject;

        rimInstance = new GameObject("RimGlow");
        rimInstance.transform.SetParent(target.transform, false);
        rimInstance.transform.localPosition = Vector3.zero;
        rimInstance.transform.localRotation = Quaternion.identity;
        rimInstance.transform.localScale = Vector3.one;

        MeshFilter rimMF = rimInstance.AddComponent<MeshFilter>();
        MeshRenderer rimMR = rimInstance.AddComponent<MeshRenderer>();

        rimMF.sharedMesh = mf.sharedMesh;

        // Match material count
        Material[] mats = new Material[mr.materials.Length];
        for (int i = 0; i < mats.Length; i++)
        {
            mats[i] = rimMaterial;
        }

        rimMR.materials = mats;

        rimInstance.SetActive(false);
    }

    public void ActivateSignifier()
    {
        // Rim ON
        if (rimInstance != null)
            rimInstance.SetActive(true);

        // Create glow if needed
        if (currentSignifierInstance == null)
        {
            currentSignifierInstance = Instantiate(signifierPrefab, transform);
            currentSignifierInstance.transform.localPosition = Vector3.zero;
            currentSignifierInstance.transform.localScale = Vector3.one * glowScale;
        }

        currentSignifierInstance.SetActive(true);

        // 🔥 Smooth scale UP
        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);

        scaleRoutine = StartCoroutine(ScaleRoutine(
            Vector3.one * glowScale * 0.9f,
            Vector3.one * glowScale * scaleMultiplier
        ));

        /*
        var ps = currentSignifierInstance.GetComponentInChildren<ParticleSystem>();
        if (ps != null)
            ps.Play();
        */
        // 🔥 Force correct transform before playing
        currentSignifierInstance.transform.localPosition = Vector3.zero;
        currentSignifierInstance.transform.localRotation = Quaternion.identity;

        // Reset particles BEFORE playing
        var ps = currentSignifierInstance.GetComponentInChildren<ParticleSystem>();
        if (ps != null)
        {
            ps.Clear(true);
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play();
        }

    }

    public void DeactivateSignifier()
    {
        // Rim OFF
        if (rimInstance != null)
            rimInstance.SetActive(false);

        if (currentSignifierInstance == null)
            return;

        // 🔥 Smooth scale DOWN
        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);

        scaleRoutine = StartCoroutine(ScaleRoutine(
            currentSignifierInstance.transform.localScale,
            Vector3.one * glowScale * 0.8f,
            true
        ));
    }

    IEnumerator ScaleRoutine(Vector3 start, Vector3 end, bool disableAtEnd = false)
    {
        float time = 0f;

        while (time < scaleDuration)
        {
            time += Time.deltaTime;
            float t = time / scaleDuration;

            // ✨ Ease-out (feels smoother than linear)
            t = 1f - Mathf.Pow(1f - t, 2f);

            currentSignifierInstance.transform.localScale = Vector3.Lerp(start, end, t);
            yield return null;
        }

        currentSignifierInstance.transform.localScale = end;

        if (disableAtEnd)
            currentSignifierInstance.SetActive(false);
    }
}