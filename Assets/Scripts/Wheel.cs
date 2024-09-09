using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public float wheelRadius= 0.3677536f;
    int layer_mask;
    private Vector3 origin;
    public RaycastHit wheelHits;//¹ÙÄû Áß½É, red»ö
    public RaycastHit wheelHits_f;//¹ÙÄû Àü¹æ, blue»ö
    public RaycastHit wheelHits_l;//¹ÙÄû ÁÂÃø, green»ö
    // Start is called before the first frame update
    void Start()
    {
        layer_mask = 1 << LayerMask.NameToLayer("Terrain");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        origin = transform.position;
        
        //¹ÙÄû Áß½É
        if (Physics.Raycast(origin, -transform.up, out wheelHits, wheelRadius+10.0f, layer_mask))
        {
            Debug.DrawLine(origin, wheelHits.point, Color.red);
        }
        else
        {
            Debug.DrawLine(origin, origin + -transform.up * wheelRadius, Color.black);
        }

        if(Physics.Raycast(origin + transform.forward*0.1f,-transform.up, out wheelHits_f, wheelRadius + 10.0f, layer_mask))
        {
            Debug.DrawLine(origin + transform.forward * 0.1f, wheelHits_f.point, Color.blue);
        }
        else
        {
            Debug.DrawLine(origin + transform.forward * 0.1f, origin + transform.forward * 0.1f - transform.up * wheelRadius, Color.black);
        }

        if (Physics.Raycast(origin - transform.right * 0.1f, -transform.up, out wheelHits_l, wheelRadius + 10.0f, layer_mask))
        {
            Debug.DrawLine(origin - transform.right * 0.1f, wheelHits_l.point, Color.green);
        }
        else
        {
            Debug.DrawLine(origin - transform.right * 0.1f, origin - transform.right*0.1f - transform.up * wheelRadius, Color.black);
        }

    }
}
