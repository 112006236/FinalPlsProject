using UnityEngine;

public class AutoDestroyAfterAnim : MonoBehaviour
{
    private void Start()
    {
        Animator animator = GetComponent<Animator>();
        float animLength = animator.runtimeAnimatorController.animationClips[0].length;
        Destroy(gameObject, animLength);
    }
}
