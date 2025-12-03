// using UnityEngine;

// public class HealthOrb : MonoBehaviour
// {
//     [Header("References")]
//     public PlayerCombat playerCombat;   // Drag player here

//     [Header("Settings")]
//     public float healAmount = 25f;
//     public float moveSpeed = 6f;
//     public float attractRange = 5f;

//     private Transform player;

//     void Start()
//     {
//         if (playerCombat != null)
//             player = playerCombat.transform;
//         else
//             Debug.LogWarning("HealthOrb: No PlayerCombat reference assigned!");
//     }

//     void Update()
//     {
//         if (player == null) return;

//         float dist = Vector3.Distance(transform.position, player.position);

//         if (dist <= attractRange)
//         {
//             transform.position = Vector3.MoveTowards(
//                 transform.position,
//                 player.position,
//                 moveSpeed * Time.deltaTime
//             );
//         }
//     }

//     private void OnTriggerEnter(Collider other)
//     {
//         if (other.gameObject == playerCombat.gameObject)
//         {
//             playerCombat.Heal(healAmount);
//             Destroy(gameObject);
//         }
//     }
// }
