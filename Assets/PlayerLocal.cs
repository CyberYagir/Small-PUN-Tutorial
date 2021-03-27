using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PlayerLocal : MonoBehaviour
{
    public Transform gun;
    public Rigidbody2D rb;
    public float speed, jump;
    public GameObject bullet;
    public float cooldown;
    public float time;
    public Transform bulletPoint;
    public Camera camera;

    public bool onGround;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        

        Vector3 diff = camera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        gun.rotation = Quaternion.Euler(0f, 0f, rot_z);

        rb.AddForce(Vector2.right * Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime, ForceMode2D.Force);

        if (Input.GetKey(KeyCode.Mouse0) && !FindObjectOfType<GameManager>().pause)
        {
            if (time <= 0)
            {
                var bl = PhotonNetwork.Instantiate(bullet.name, bulletPoint.position, gun.rotation);
                bl.GetPhotonView().RPC("Set", RpcTarget.AllBuffered, GetComponent<PhotonView>().Owner.NickName);
                time = cooldown;
            }
        }
        Debug.DrawRay((Vector2)transform.position + Vector2.down * 0.52f, Vector2.down, Color.red);
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + Vector2.down * 0.52f, Vector2.down, 0.1f);
        if (hit.collider != null && hit.collider.transform != transform)
        {
            onGround = true;
        }else
        {
            onGround = false;
        }

        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W))
        {
            if (onGround)
            {
                rb.AddForce(Vector2.up * jump, ForceMode2D.Impulse);
            }
        }
        time -= 1 * Time.deltaTime;
    }
}
