using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    public float rotationSpeedX = 30f;
    public float rotationSpeedY = 45f;
    public float rotationSpeedZ = 60f;

    private float timer = 0f;
    private float changeTime = 10f; // Time in seconds to switch axes
    private int rotationMode = 0;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= changeTime)
        {
            timer -= changeTime; // Reset timer and ensure smooth transition
            rotationMode = (rotationMode + 1) % 3; // Cycle through rotation modes
        }

        ApplyRotation();
    }

    void ApplyRotation()
    {
        float deltaTime = Time.deltaTime;
        Vector3 rotationVector = Vector3.zero;

        switch (rotationMode)
        {
            case 0: // Rotate around X and Y
                rotationVector = new Vector3(rotationSpeedX * deltaTime, rotationSpeedY * deltaTime, 0f);
                break;
            case 1: // Rotate around Y and Z
                rotationVector = new Vector3(0f, rotationSpeedY * deltaTime, rotationSpeedZ * deltaTime);
                break;
            case 2: // Rotate around X and Z
                rotationVector = new Vector3(rotationSpeedX * deltaTime, 0f, rotationSpeedZ * deltaTime);
                break;
        }

        transform.Rotate(rotationVector);
    }
}
