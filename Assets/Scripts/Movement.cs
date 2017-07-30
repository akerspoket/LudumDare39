using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {
    public float m_movementSpeed = 5;
    public float m_sprintSpeed = 10;
    public float m_rotationSpeed = 10;
    public float rotationX = 0;
    public float rotationY = 0;
    CharacterController m_controller;


	// Use this for initialization
	void Start () {
        m_controller = GetComponent<CharacterController>();

    }
	
	// Update is called once per frame
	void Update () {
        UpdateRotation();
        UpdateMovement();

    }

    private void UpdateMovement()
    {
        Vector3 speed = Vector3.zero;
        float speedToUse = m_movementSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speedToUse = m_sprintSpeed;
        }

        if (Input.GetKey(KeyCode.W))
        {
            speed += transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            speed += -transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            speed += -transform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            speed += transform.right;
        }
        m_controller.SimpleMove(speed.normalized * speedToUse);
    }

    private void UpdateRotation()
    {
        rotationX += Input.GetAxis("Mouse X") * m_rotationSpeed;
        rotationY -= Input.GetAxis("Mouse Y") * m_rotationSpeed;
        rotationY = Mathf.Clamp(rotationY, -80, 80);

        Vector3 eulerAngles = new Vector3(rotationY, rotationX, 0);
        
        transform.rotation = Quaternion.Euler(eulerAngles);
    }
}
