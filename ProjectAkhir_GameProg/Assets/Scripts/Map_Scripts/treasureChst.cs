using UnityEngine;
using System.Collections;

public class treasureChst : MonoBehaviour
{
    [Header("Settings")]
    public float openRange = 3f;
    public bool openOnce = true;

    [Header("Treasure Settings")]
    [Tooltip("Drag and drop prefabs from the Project folder here.")]
    public GameObject[] treasurePrefabs; // These can be prefabs
    public Transform spawnPoint;

    [Header("Animation")]
    public Animator chestAnimator;
    public string openBoolName = "isOpen";

    [Header("Jump Effect Settings")]
    public float squashAmount = 0.7f;
    public float jumpHeight = 0.3f;
    public float animationDuration = 0.2f;

    private bool isOpened = false;
    private Transform player;
    private Vector3 originalScale;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        originalScale = transform.localScale;

        if (chestAnimator != null)
            chestAnimator.SetBool(openBoolName, false);
    }

    void Update()
    {
        if (player == null || (isOpened && openOnce)) return;

        if (Vector3.Distance(transform.position, player.position) <= openRange)
        {
            OpenChest();
        }
    }

    void OpenChest()
    {
        isOpened = true;

        if (chestAnimator != null)
            chestAnimator.SetBool(openBoolName, true);

        StartCoroutine(SquashAndJump());

        if (treasurePrefabs.Length > 0)
        {
            int index = Random.Range(0, treasurePrefabs.Length);
            GameObject prefab = treasurePrefabs[index];
            if (prefab != null)
                Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    IEnumerator SquashAndJump()
    {
        float elapsed = 0f;
        Vector3 stretchScale = new Vector3(originalScale.x * 1.5f, originalScale.y * squashAmount, originalScale.z);
        Vector3 jumpPosition = transform.position + Vector3.up * jumpHeight;

        Vector3 startScale = originalScale;
        Vector3 startPos = transform.position;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            transform.localScale = Vector3.Lerp(startScale, stretchScale, t);
            transform.position = Vector3.Lerp(startPos, jumpPosition, t);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            transform.localScale = Vector3.Lerp(stretchScale, originalScale, t);
            transform.position = Vector3.Lerp(jumpPosition, startPos, t);
            yield return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, openRange);
    }
}
