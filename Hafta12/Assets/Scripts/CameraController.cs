using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed;
    public float minXRot;
    public float maxXrot;
    private float curXrot;

    public float minZoom;
    public float maxZoom;
    public float zoomSpeed;
    public float rotateSpeed;
    private float curZoom;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        curZoom = cam.transform.localPosition.y;
        curXrot = -50;
    }
    private void Update()
    {
        curZoom += Input.GetAxis("Mouse ScrollWheel") * -zoomSpeed;  // zoom de�erine mousen orta tu�lundan ald���m�z de�eri atad�k
        curZoom = Mathf.Clamp(curZoom, minZoom, maxZoom);

        cam.transform.localPosition = Vector3.up * curZoom; // buradada cameran�n pozisyonunu up y�nde zoom de�eriyle �arpt�k
        if (Input.GetMouseButton(1))
        {
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");
            curXrot += -y * rotateSpeed;
            curXrot = Mathf.Clamp(curXrot, minXRot, maxXrot);
            transform.eulerAngles = new Vector3(curXrot, transform.eulerAngles.y + (x * rotateSpeed), 0.0f); // yde kendi yesini ekleyip x le rotatespeed i �arp
        }

        //Movement

        Vector3 forward = cam.transform.forward;  // forward de�i�eknine cam �n forward�n� ileri de�eini ekle
        forward.y = 0.0f; // y yi s�f�rla
        forward.Normalize();

        Vector3 right = cam.transform.right; // buradada sa� solunu ald�k
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 dir = forward * moveZ + right * moveX;
        dir.Normalize();

        dir *= moveSpeed * Time.deltaTime;
        transform.position += dir;

    }
}
