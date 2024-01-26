using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Selecetor : MonoBehaviour
{
    private Camera cam;
    public static Selecetor instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        cam = Camera.main;
    }
    public Vector3 GetCurTilePosition()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return new Vector3(0, -99, 0);
            
        }
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = cam.ScreenPointToRay(Input.mousePosition); // rayi mosuepozisyonundan aldým dokunduðum yeri
        float rayOut = 0.0f;
        if (plane.Raycast(ray,out rayOut))
        {
            Vector3 newPos = ray.GetPoint(rayOut) - new Vector3(0.05f,0.0f,0.5f);

            newPos = new Vector3(Mathf.CeilToInt(newPos.x), 0.0f, Mathf.CeilToInt(newPos.z)); // burada ise yerleþtimede tam deðerlere tamamlamasýný yazdýk 

            return newPos;
        }
        return new Vector3(0, -99, 0);
    }
}
