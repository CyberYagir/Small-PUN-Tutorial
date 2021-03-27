using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpCamera : MonoBehaviour
{
    public PlayerLocal playerLocal;
    public float speed;



    public void LateUpdate()
    {
        if (playerLocal == null)
        {
            playerLocal = GetComponentInParent<PlayerLocal>();
            if (playerLocal == null) { Destroy(gameObject);  return; }
            transform.parent = null;
            return;
        }
        
        transform.position = Vector3.Lerp(transform.position, playerLocal.transform.position + new Vector3(0, 0, -10), speed);
    }
}
