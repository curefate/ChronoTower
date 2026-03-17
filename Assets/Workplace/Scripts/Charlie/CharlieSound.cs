using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public class CharlieSound : MonoBehaviour
{
    public AudioClip walkSound;
    public AudioClip climbSound;
    private AudioSource audioSource;
    private Animator animator;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        var info = animator.GetCurrentAnimatorStateInfo(0);

        if (info.IsName("Walking") && info.normalizedTime % 0.5f < 0.1f)
        {
            audioSource.PlayOneShot(walkSound);
        }
        else if (info.IsName("Climbing") && info.normalizedTime % 0.5f < 0.1f)
        {
            audioSource.PlayOneShot(climbSound);
        }
        else if (info.IsName("Jump") && info.normalizedTime > 0.9f)
        {
            audioSource.PlayOneShot(walkSound);
        }
        else if (info.IsName("Stairs") && info.normalizedTime % 0.5f < 0.1f)
        {
            audioSource.PlayOneShot(walkSound);
        }
    }
}
