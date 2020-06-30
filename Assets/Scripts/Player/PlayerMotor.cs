using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    

    public State Move(bool forward, bool backward, bool left, bool right, float yaw, float pitch)
    {
        Vector3 movingDir = Vector3.zero;
        if (forward ^ backward)
        {
            movingDir += forward ? transform.forward : -transform.forward;
        }
        if (left ^ right)
        {
            movingDir += right ? transform.right : -transform.right;
        }
        movingDir.Normalize();

        //networkBody.MoveForce = new Vector3(movingDir.x, networkBody.MoveForce.y, movingDir.z);
        rb.velocity = movingDir*10f;
        cam.transform.localEulerAngles = new Vector3(pitch, 0f, 0f);
        transform.rotation = Quaternion.Euler(0, yaw, 0);

        State stateMotor = new State();
        stateMotor.position = transform.position;
        stateMotor.rotation = yaw;
        
        return stateMotor;
    }

    public void SetState(Vector3 position, float rotation)
    {
        if (Mathf.Abs(rotation - transform.rotation.y) > 5f)
            transform.rotation = Quaternion.Euler(0, rotation, 0);

        if (Vector3.Distance(transform.position, position) > 0.1f)
        {
            transform.position += (position - transform.position) * BoltNetwork.FrameDeltaTime * 5f;
        }
    }

    public struct State
    {
        public Vector3 position;
        public float rotation;
    }

    public void DisableCamera()
    {
        cam.gameObject.SetActive(false);
    }
}
