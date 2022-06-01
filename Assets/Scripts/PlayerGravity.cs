using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGravity : MonoBehaviour
{
    public CharacterController Controller;
    
    Vector3 Velocity;
    public float Gravity = -10f;
    void FixedUpdate()
    {
        
        Velocity.y += Gravity * Time.deltaTime;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.7f))
        {
            if (hit.rigidbody != null)
            {
                
                Velocity.x = hit.rigidbody.velocity.x;
                Velocity.z = hit.rigidbody.velocity.z;
            }
            else
            {
                
                Velocity.x = 0;
                Velocity.z = 0;
            }
            if (hit.transform.tag != "Water")
            {
                Velocity.y = 0;
            }
            
            if (Input.GetKey(KeyCode.Space) && hit.transform.tag != "Water")
            {
                Velocity.y = 6;
            }

        }


        Controller.Move(Velocity * Time.deltaTime);
    }
}
