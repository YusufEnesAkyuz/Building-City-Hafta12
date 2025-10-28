using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class City : MonoBehaviour
{
    public int money;
    public int day;
    public int curPopulation;
    public int curJobs;
    public int curFood;
    public int maxPopulation;
    public int maxJobs;
    public int incomePerJob;
    public int powerSupply;
    public int powerDemand;
    public int curPower;
    public int pollution;
    [Range(0, 100)]
    public int happiness;
    [Tooltip("Percentage of the original cost refunded when bulldozing a building.")]
    [Range(0, 100)]
    public int buldozeRefundPercent = 50;
    public TextMeshProUGUI statsText;
    public List<Building> buildings = new List<Building>();
    public static City instance;

    private readonly Dictionary<Vector3Int, Building> buildingsByPosition = new Dictionary<Vector3Int, Building>();

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        RefreshCitySystems();
        UpdateStatText();

    }
    public bool CanAfford(BuildingPreset preset)
    {
        if (preset == null)
        {
            return false;
        }

        return money >= preset.cost;
    }

    public bool CanPlaceBuilding(BuildingPreset preset, Vector3 position)
    {
        if (!CanAfford(preset))
        {
            return false;
        }

        if (!IsValidPlacementPosition(position))
        {
            return false;
        }

        Vector3Int gridPos = WorldToGrid(position);
        return !buildingsByPosition.ContainsKey(gridPos);
    }

    public bool IsValidPlacementPosition(Vector3 position)
    {
        return position.y > -10f;
    }

    Vector3Int WorldToGrid(Vector3 position)
    {
        return Vector3Int.RoundToInt(position);
    }

    public bool TryGetBuildingAt(Vector3 position, out Building building)
    {
        return buildingsByPosition.TryGetValue(WorldToGrid(position), out building);
    }

    public void OnplaceBuilding(Building building)
    {
        money -= building.preset.cost;
        maxPopulation += building.preset.population;
        maxJobs += building.preset.jobs;
        buildings.Add(building);
        Vector3Int gridPos = WorldToGrid(building.transform.position);
        buildingsByPosition[gridPos] = building;
        RefreshCitySystems();
        UpdateStatText();
    }

    void UpdateStatText()
    {
        List<string> warnings = GenerateWarnings();

        statsText.text = string.Format(
            "Day: {0} Money: {1}\nPop: {2} / {3}   Jobs: {4} / {5}\nFood: {6}   Power: {7} / {8}\nHappiness: {9}%   Pollution: {10}\n{11}",
            day,
            money,
            curPopulation,
            maxPopulation,
            curJobs,
            maxJobs,
            curFood,
            curPower,
            Mathf.Max(powerDemand, 0),
            happiness,
            pollution,
            warnings.Count > 0 ? string.Join("\n", warnings) : "All systems stable.");
    }
    public void OnRemoveBuilding(Building building)
    {
        maxPopulation -= building.preset.population;
        maxJobs -= building.preset.jobs;
        if (buldozeRefundPercent > 0)
        {
            money += Mathf.RoundToInt(building.preset.cost * (buldozeRefundPercent / 100f));
        }
        buildings.Remove(building);
        Vector3Int gridPos = WorldToGrid(building.transform.position);
        buildingsByPosition.Remove(gridPos);
        curPopulation = Mathf.Min(curPopulation, maxPopulation);
        curJobs = Mathf.Min(curJobs, maxJobs);
        Destroy(building.gameObject);
        RefreshCitySystems();
        UpdateStatText();
    }
    public void EndTurn()
    {
        CalculateFood();
        CalculatePower();
        CalculatePollution();
        CalculatePopulations();
        CalculateJobs();
        CalculateHappiness();
        CalculateMoney();
        UpdateStatText();
        day++;
    }
    void CalculateMoney()
    {
        int effectiveJobs = curJobs;

        if (powerDemand > 0 && powerSupply < powerDemand)
        {
            float powerRatio = Mathf.Clamp01((float)powerSupply / powerDemand);
            effectiveJobs = Mathf.RoundToInt(effectiveJobs * powerRatio);
        }

        float happinessModifier = Mathf.Lerp(0.5f, 1.5f, happiness / 100f);

        money += Mathf.RoundToInt(effectiveJobs * incomePerJob * happinessModifier);
        foreach (Building building in buildings)
        {
            money -= building.preset.costPerTurn;
        }
    }
    void CalculatePopulations()
    {
        if (curFood>=curPopulation && curPopulation<maxPopulation)
        {
            curFood -= curPopulation / 4;
            curPopulation = Mathf.Min(curPopulation + (curFood / 4), maxPopulation);
        }
        else if (curFood<curPopulation)
        {
            curPopulation = curFood;
        }
    }
    void CalculateJobs()
    {
        curJobs = Mathf.Min(curPopulation, maxJobs);
    }
    void CalculateFood()
    {
        curFood = 0;
        foreach (Building building in buildings)
        {
            curFood += building.preset.food;
        }
    }
    void CalculatePower()
    {
        powerSupply = 0;
        powerDemand = 0;
        foreach (Building building in buildings)
        {
            powerSupply += building.preset.powerProduction;
            powerDemand += building.preset.powerConsumption;
        }

        curPower = Mathf.Max(0, powerSupply - powerDemand);
    }

    void CalculatePollution()
    {
        pollution = 0;
        foreach (Building building in buildings)
        {
            pollution += building.preset.pollution;
        }
    }

    void CalculateHappiness()
    {
        float employmentRate = maxJobs == 0 ? 1f : (float)curJobs / maxJobs;
        float housingRate = maxPopulation == 0 ? 1f : (float)curPopulation / maxPopulation;
        float foodRate = curPopulation == 0 ? 1f : Mathf.Clamp01((float)curFood / curPopulation);
        float powerRate = powerDemand == 0 ? 1f : Mathf.Clamp01((float)powerSupply / Mathf.Max(powerDemand, 1));

        int buildingHappiness = 0;
        foreach (Building building in buildings)
        {
            buildingHappiness += building.preset.happiness;
        }

        float composite = 50f + (employmentRate + housingRate + foodRate + powerRate) * 12.5f + buildingHappiness;
        composite -= pollution * 0.5f;
        happiness = Mathf.Clamp(Mathf.RoundToInt(composite), 0, 100);
    }

    void RefreshCitySystems()
    {
        buildingsByPosition.Clear();
        foreach (Building building in buildings)
        {
            Vector3Int gridPos = WorldToGrid(building.transform.position);
            buildingsByPosition[gridPos] = building;
        }
        CalculateFood();
        CalculatePower();
        CalculatePollution();
        CalculateJobs();
        CalculateHappiness();
    }

    List<string> GenerateWarnings()
    {
        List<string> warnings = new List<string>();

        if (curFood < curPopulation)
        {
            warnings.Add("Warning: Food production is too low!");
        }

        if (powerSupply < powerDemand)
        {
            warnings.Add("Warning: Power grid is overloaded!");
        }

        if (curJobs < maxJobs)
        {
            warnings.Add("Warning: Unfilled jobs are hurting efficiency.");
        }

        if (pollution > 0 && pollution >= buildings.Count * 2)
        {
            warnings.Add("Warning: Pollution is starting to affect happiness.");
        }

        if (warnings.Count == 0 && happiness < 40)
        {
            warnings.Add("Citizens feel unsettled. Improve living conditions!");
        }

        return warnings;
    }
}
