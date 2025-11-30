using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordProjectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private float speed = 10f;
    private float createdTime;
    public float attackDamage = 15f;

    // Start is called before the first frame update
    void Start()
    {
        createdTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - createdTime >= lifetime)
        {
            Destroy(gameObject);
        } 
        else
        {
            transform.position += speed * Time.deltaTime * transform.up;
        }
    }
}
