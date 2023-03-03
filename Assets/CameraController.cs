using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject UI;
    public bool pauseWithUI = false;

    // Update is called once per frame
    void Update()
    {
        float speed = 10;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed *= 5;
        }
        
        // Allow the camera to be moved around the scene
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * (Time.unscaledDeltaTime * speed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * (Time.unscaledDeltaTime * speed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * (Time.unscaledDeltaTime * speed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * (Time.unscaledDeltaTime * speed);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.position -= transform.up * (Time.unscaledDeltaTime * speed);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.position += transform.up * (Time.unscaledDeltaTime * speed);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            transform.position += transform.up * (Time.unscaledDeltaTime * speed);
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.position -= transform.up * (Time.unscaledDeltaTime * speed);
        }
        
        float rotationSpeed = 75;
        // Allow the camera to be rotated around the scene
        Quaternion rotation = transform.rotation;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rotation.eulerAngles += Vector3.right * (-Time.unscaledDeltaTime * rotationSpeed);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            rotation.eulerAngles += Vector3.right * (Time.unscaledDeltaTime * rotationSpeed);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rotation.eulerAngles += Vector3.up * (Time.unscaledDeltaTime * -rotationSpeed);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rotation.eulerAngles += Vector3.up * (Time.unscaledDeltaTime * rotationSpeed);
        }
        // Rotate with mouse
        if (Input.GetMouseButton(1) || Input.GetMouseButton(0))
        {
            rotation.eulerAngles += Vector3.up *  (Input.GetAxis("Mouse X") * Time.unscaledDeltaTime * -rotationSpeed * 2);
            rotation.eulerAngles += Vector3.right *  (Input.GetAxis("Mouse Y") * Time.unscaledDeltaTime * rotationSpeed * 2);
        }
        transform.rotation = rotation;
        
        // Allow the camera to be zoomed in and out
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            transform.position += transform.forward * (Time.unscaledDeltaTime * 100);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            transform.position -= transform.forward * (Time.unscaledDeltaTime * 100);
        }
        
        // Toggle the UI on and off with the esc key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UI.SetActive(!UI.activeSelf);
            // Freeze the app when the UI is open
            if (UI.activeSelf && pauseWithUI)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
        
    }
    public void setPauseWithUI(bool pause)
    {
        pauseWithUI = pause;
        if (UI.activeSelf && pauseWithUI)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
    
}
