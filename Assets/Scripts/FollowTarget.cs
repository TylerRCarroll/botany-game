using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;

    private void Start()
    {
        transform.SetParent(null);
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.transform.position, 5f * Time.deltaTime);
    }
}