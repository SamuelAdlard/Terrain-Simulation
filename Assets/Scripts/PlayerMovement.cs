
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    public CharacterController Controller;
    public float Speed = 12f;
    

    
    void Update()
    {
        float X = Input.GetAxis("Horizontal");
        float Z = Input.GetAxis("Vertical");
        Vector3 MoveZ = transform.forward * Z * Speed * Time.deltaTime;
        Vector3 MoveX = transform.right * X * Speed * Time.deltaTime;
        Controller.Move(MoveZ);
        Controller.Move(MoveX);
        

        
    }
    

}
