using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacement : MonoBehaviour
{
    private bool currentlyPlacing; //vamý yomhu kontrol
    private bool currentlyBuldozering; //silinmiþmi

    private BuildingPreset curBuildindinPreset;
    private float IndicatorUpdateTime = 0.05f;
    private float lastUpdateTime;
    private Vector3 curIndicatorPos;

    public GameObject placementIndicator;
    public GameObject buldozerIndicator;

    public void BeginNewBuildingPlacement(BuildingPreset preset) 
    {
        /*if (City.insance.money<preset)
        {
            return;
        }*/
        currentlyPlacing = true;
        curBuildindinPreset = preset;
        placementIndicator.SetActive(true);

    } 

    void CancelBuildinPlacement()
    {
        currentlyPlacing = false;
        placementIndicator.SetActive(false);
    }

    public void ToogleBulldoze()
    {
        currentlyBuldozering = !currentlyBuldozering;
        buldozerIndicator.SetActive(currentlyBuldozering);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelBuildinPlacement();
        }
        if (Time.time-lastUpdateTime>IndicatorUpdateTime)
        {
            lastUpdateTime = Time.time;
            curIndicatorPos = Selecetor.instance.GetCurTilePosition();

            if (currentlyPlacing)
            {
                placementIndicator.transform.position = curIndicatorPos;
            }
            else if (currentlyBuldozering)
            {
                buldozerIndicator.transform.position = curIndicatorPos;
            }
        }
        if (Input.GetMouseButtonDown(0) && currentlyPlacing)
        {
            PlaceBuilding();
        }
        else if (Input.GetMouseButtonDown(0) && currentlyBuldozering)
        {
            Buldoz();
        }
    }
    void PlaceBuilding()
    {
        GameObject buildingObj = Instantiate(curBuildindinPreset.prefab, curIndicatorPos, Quaternion.identity);
        City.instance.OnplaceBuilding(buildingObj.GetComponent<Building>());
        CancelBuildinPlacement();
    }
    void Buldoz()
    {
        Building buildingToDestroy = City.instance.buildings.Find(x => x.transform.position == curIndicatorPos);
        if (buildingToDestroy != null)
        {
            City.instance.OnRemoveBuilding(buildingToDestroy);
        }
    }
}
