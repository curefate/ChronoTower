using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharlieWaker : MonoBehaviour
{
    [SerializeField] List<GameObject> disableBeforeWake;
    private NodeMover nodeMover;
    private Animator animator;

    private bool _jumped = false;
    private IEnumerator _blendOutCoroutine;

    private void Start()
    {
        nodeMover = GetComponent<NodeMover>();
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
        foreach (var obj in disableBeforeWake)
        {
            obj.SetActive(false);
        }
        //WakeUp();
    }

    private void Update()
    {
        var info = animator.GetCurrentAnimatorStateInfo(1);

        if (info.IsName("Jump") && info.normalizedTime >= 0.4f && !_jumped)
        {
            _jumped = true;
            nodeMover.MoveTo(nodeMover.currentNode.neighbors[0]);
        }

        if (info.IsName("Jump") && info.normalizedTime >= 1)
        {
            if (_blendOutCoroutine == null)
            {
                _blendOutCoroutine = BlendOut(1f);
                StartCoroutine(_blendOutCoroutine);
            }
        }
    }

    public void WakeUp()
    {
        animator.SetTrigger("Wake");
    }

    private IEnumerator BlendOut(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float blend = Mathf.Lerp(1f, 0f, elapsed / duration);
            animator.SetLayerWeight(1, blend);
            yield return null;
        }

        foreach (var obj in disableBeforeWake)
        {
            obj.SetActive(true);
        }
        transform.GetComponent<CharlieWaker>().enabled = false;
    }
}
