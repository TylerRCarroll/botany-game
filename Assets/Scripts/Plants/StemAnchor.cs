using UnityEngine;

public class StemAnchor : MonoBehaviour
{

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, (transform.position + transform.forward));
    }
}
