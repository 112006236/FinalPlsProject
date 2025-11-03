using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LookAtPlayer : MonoBehaviour
{
    public Transform player;
    public Transform camPos;

    // Update is called once per frame
    void Update()
    {
        transform.position = camPos.position;
        transform.LookAt(player);
    }
}
