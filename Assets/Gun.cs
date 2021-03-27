using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public List<SpriteRenderer> spriteRenderers;
    public List<int> back;
    public List<int> front;

    // Update is called once per frame
    void Update()
    {
        if (transform.eulerAngles.z < 90 || transform.eulerAngles.z > 270f)
        {
            for (int i = 0; i < spriteRenderers.Count; i++)
            {
                spriteRenderers[i].sortingOrder = front[i];
            }
        }
        else
        {
            for (int i = 0; i < spriteRenderers.Count; i++)
            {
                spriteRenderers[i].sortingOrder = back[i];
            }
        }
    }
}
