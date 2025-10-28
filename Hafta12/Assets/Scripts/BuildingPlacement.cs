using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacement : MonoBehaviour
{
    private bool currentlyPlacing;
    private bool currentlyBuldozering;

    private BuildingPreset curBuildindinPreset;
    private float IndicatorUpdateTime = 0.05f;
    private float lastUpdateTime;
    private Vector3 curIndicatorPos;
    private Quaternion curIndicatorRotation = Quaternion.identity;

    public GameObject placementIndicator;
    public GameObject buldozerIndicator;

    public void BeginNewBuildingPlacement(BuildingPreset preset)
    {
        if (preset == null)
        {
            Debug.LogWarning("No preset supplied for placement.");
            return;
        }

        if (!City.instance.CanAfford(preset))
        {
            Debug.LogWarning("Insufficient funds to place " + preset.name + ".");
            return;
        }

        currentlyPlacing = true;
        curBuildindinPreset = preset;
        placementIndicator.SetActive(true);
        curIndicatorRotation = Quaternion.identity;
        placementIndicator.transform.rotation = curIndicatorRotation;
    }

    void CancelBuildinPlacement()
    {
        currentlyPlacing = false;
        placementIndicator.SetActive(false);
        curBuildindinPreset = null;
    }

    public void ToogleBulldoze()
    {
        currentlyBuldozering = !currentlyBuldozering;
        buldozerIndicator.SetActive(currentlyBuldozering);
        if (currentlyBuldozering)
        {
            CancelBuildinPlacement();
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentlyBuldozering)
            {
                currentlyBuldozering = false;
                buldozerIndicator.SetActive(false);
            }
            else
            {
                CancelBuildinPlacement();
            }
        }

        if (currentlyPlacing && (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.R)))
        {
            float direction = 0f;
            if (Input.GetKeyDown(KeyCode.Q))
            {
                direction = -90f;
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                direction = 90f;
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                direction = 90f;
            }

            curIndicatorRotation = Quaternion.Euler(0f, curIndicatorRotation.eulerAngles.y + direction, 0f);
            placementIndicator.transform.rotation = curIndicatorRotation;
        }

        if (Time.time - lastUpdateTime > IndicatorUpdateTime)
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
        if (curBuildindinPreset == null)
        {
            return;
        }

        if (!City.instance.IsValidPlacementPosition(curIndicatorPos))
        {
            Debug.LogWarning("Can not place building outside the map.");
            return;
        }

        if (!City.instance.CanPlaceBuilding(curBuildindinPreset, curIndicatorPos))
        {
            Debug.LogWarning("Tile is already occupied or you can not afford this building.");
            return;
        }

        GameObject buildingObj = Instantiate(curBuildindinPreset.prefab, curIndicatorPos, curIndicatorRotation);
        City.instance.OnplaceBuilding(buildingObj.GetComponent<Building>());
        CancelBuildinPlacement();
    }
    void Buldoz()
    {
        if (City.instance.TryGetBuildingAt(curIndicatorPos, out Building buildingToDestroy))
        {
            City.instance.OnRemoveBuilding(buildingToDestroy);
        }
        else
        {
            Debug.Log("No building to bulldoze at " + curIndicatorPos);
        }
    }
}
