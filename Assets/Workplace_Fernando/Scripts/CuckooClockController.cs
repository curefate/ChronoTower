using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class CuckooClockController : MonoBehaviour
{
    [Header("References")]
    public Transform pendulum;
    //public Transform[] gears;
    public Transform hourHand;
    public Transform minuteHand;
    public Transform spring;
    public Transform bird;
    public Transform leftDoor;
    public Transform rightDoor;

    [Header("Idle Settings")]
    public float pendulumAngle = 30f;
    public float pendulumSpeed = 2f;
    public float minuteSpeed = 30f;
    public float hourSpeed = 10f;

    [Header("Cuckoo Settings")]
    public float cuckooSpeedMultiplier = 10f;
    public float doorOpenAngle = 90f;
    public float birdMoveDistance = 0.2f;
    public float springStretch = 1f;

    private bool isAnimating = false;
    private float time;

    [Header("Audio")]
    public AudioSource cuckooAudioSource;
    public AudioClip cuckooClip;
    public AudioSource timeAudioSource;    
    public AudioClip forwardTimeClip;
    public AudioClip rewindTimeClip;

    void Start()
    {
        //For Testing
        //InvokeRepeating(nameof(AnimateCuckoo), 10f, 10f);
        StartCoroutine(TimeLoop());
    }

    IEnumerator TimeLoop()
    {
        while (true)
        {
            ForwardTime();           // first call
            yield return new WaitForSeconds(10f);

            RewindTime();            // second call
            yield return new WaitForSeconds(10f);
        }
    }

    void Update()
    {
        IdleCuckoo();

        //For Testing
        //if (Keyboard.current.digit1Key.wasPressedThisFrame) AnimateCuckoo();
    }


    //IDLE LOOP
    public void IdleCuckoo()
    {
        //if (isAnimating) return;

        time += Time.deltaTime * pendulumSpeed;

        // Pendulum swing (sin wave)
        float angle = Mathf.Sin(time) * pendulumAngle;
        //pendulum.localRotation = Quaternion.Euler(0, 0, angle);
        pendulum.localRotation = Quaternion.Euler(angle, 0, 0);

        // Clock hands
        minuteHand.Rotate(Vector3.up, -minuteSpeed * Time.deltaTime);
        hourHand.Rotate(Vector3.up, hourSpeed * Time.deltaTime);
        //minuteHand.Rotate(Vector3.forward, -minuteSpeed * Time.deltaTime, Space.Self);
        //hourHand.Rotate(Vector3.forward, hourSpeed * Time.deltaTime, Space.Self);

        // Optional: gears rotation
        //foreach (var gear in gears)
        //for (int i = 0; i < gears.Length; i++)
        //{
            //gear.Rotate(Vector3.forward, minuteSpeed * Time.deltaTime);
            //gear.Rotate(gear.forward, direction * minuteSpeed * Time.deltaTime, Space.World);
            //gear.Rotate(Vector3.forward, direction * minuteSpeed * Time.deltaTime, Space.Self);
            //float direction = (i % 2 == 0) ? 1f : -1f;
            //gears[i].Rotate(Vector3.forward, direction * minuteSpeed * Time.deltaTime, Space.Self);
        //}

    }

    //MAIN ANIMATION
    public void AnimateCuckoo()
    {
        if (!isActiveAndEnabled) return;
        if (!isAnimating)
            StartCoroutine(CuckooRoutine());
    }

    IEnumerator CuckooRoutine()
    {
        isAnimating = true;
    
        float originalPendulum = pendulumSpeed;
        pendulumSpeed *= cuckooSpeedMultiplier;

        float originalMinute = minuteSpeed;
        minuteSpeed *= cuckooSpeedMultiplier;

        float originalHour = hourSpeed;
        hourSpeed *= cuckooSpeedMultiplier;

        for (int i = 0; i < 3; i++)
        {
            //audioSource.PlayOneShot(cuckooClip);
            cuckooAudioSource.PlayOneShot(cuckooClip);

            yield return StartCoroutine(OpenDoors());
            yield return StartCoroutine(CuckooPop());
            yield return StartCoroutine(CloseDoors());

            yield return new WaitForSeconds(0.2f);
        }

        pendulumSpeed = originalPendulum;
        minuteSpeed = originalMinute;
        hourSpeed = originalHour;

        isAnimating = false;
    }

    IEnumerator OpenDoors()
    {
        float t = 0;
        Quaternion leftStart = leftDoor.localRotation;
        Quaternion rightStart = rightDoor.localRotation;

        //Quaternion leftTarget = Quaternion.Euler(0, -doorOpenAngle, 0);
        //Quaternion rightTarget = Quaternion.Euler(0, doorOpenAngle, 0);
        Quaternion leftTarget = Quaternion.Euler(0, doorOpenAngle, 0);
        Quaternion rightTarget = Quaternion.Euler(0, -doorOpenAngle, 0);

        while (t < 1)
        {
            t += Time.deltaTime * 3f;

            leftDoor.localRotation = Quaternion.Slerp(leftStart, leftTarget, t);
            rightDoor.localRotation = Quaternion.Slerp(rightStart, rightTarget, t);

            yield return null;
        }
    }

    IEnumerator CloseDoors()
    {
        float t = 0;
        Quaternion leftStart = leftDoor.localRotation;
        Quaternion rightStart = rightDoor.localRotation;

        Quaternion leftTarget = Quaternion.identity;
        Quaternion rightTarget = Quaternion.identity;

        while (t < 1)
        {
            t += Time.deltaTime * 3f;

            leftDoor.localRotation = Quaternion.Slerp(leftStart, leftTarget, t);
            rightDoor.localRotation = Quaternion.Slerp(rightStart, rightTarget, t);

            yield return null;
        }
    }

    IEnumerator CuckooPop()
    {
        float t = 0;

        Vector3 birdStart = bird.localPosition;
        //Vector3 birdTarget = birdStart + Vector3.forward * birdMoveDistance;
        Vector3 birdTarget = birdStart + Vector3.right * birdMoveDistance;

        Vector3 springStart = spring.localScale;
        //Vector3 springTarget = new Vector3(
        //    springStart.x,
        //    springStart.y + springStretch,
        //    springStart.z
        //);
        Vector3 springTarget = new Vector3(
            springStart.x + springStretch,
            springStart.y,
            springStart.z
        );

        while (t < 1)
        {
            t += Time.deltaTime * 4f;

            bird.localPosition = Vector3.Lerp(birdStart, birdTarget, t);
            spring.localScale = Vector3.Lerp(springStart, springTarget, t);

            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        // Return
        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 4f;

            bird.localPosition = Vector3.Lerp(birdTarget, birdStart, t);
            spring.localScale = Vector3.Lerp(springTarget, springStart, t);

            yield return null;
        }
    }

    public void ForwardTime()
    {
        timeAudioSource.PlayOneShot(forwardTimeClip);
        //timeAudioSource.pitch = 0.5f; // slower (lower pitch)
        //timeAudioSource.clip = forwardTimeClip;
        //timeAudioSource.Play();

        AnimateCuckoo();
    }

    public void RewindTime()
    {
        timeAudioSource.PlayOneShot(rewindTimeClip);
        //timeAudioSource.pitch = 0.5f; // slower (lower pitch)
        //timeAudioSource.clip = rewindTimeClip;
        //timeAudioSource.Play();

        AnimateCuckoo();
    }

}