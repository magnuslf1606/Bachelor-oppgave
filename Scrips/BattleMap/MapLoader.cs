using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    public GameObject[] maps; // Array holding references to map prefabs
    private DataAccessor dataAccessor; // Reference to the DataAccessor component
    public GameObject grid; // Reference to the grid GameObject

    // Start is called before the first frame update
    void Start()
    {
        dataAccessor = GameObject.FindWithTag("DataAccessor").GetComponent<DataAccessor>();
        Debug.Log("Before everything");
        if(dataAccessor != null) {
            Debug.Log("FOUND ACCESS");
            LoadMap(dataAccessor.Info[1]);
        }
    }

    // Method to load a map based on the provided mapString
    public void LoadMap(string mapString)
    {
        // Extract the value from the mapString
        string value = mapString;

        switch(value)
        {
            case "grass":
            case "forest":
            case "water":
            case "tundra":
            case "dungeon":

                List<GameObject> maps = FilterMaps(value);

                InstantiateRandomMap(maps);
                break;
            default:
                Debug.Log("Uknown map type: " + value);
                break;
        }
    }

    // Method to filter maps based on the given value
    private List<GameObject> FilterMaps(string value)
    {
        List<GameObject> filteredMaps = new List<GameObject>();

        // Iterate through all maps and filter based on the provided value
        foreach (GameObject prefab in maps)
        {
            if (prefab.name.StartsWith(value))
            {
                filteredMaps.Add(prefab);
            }
        }
        return filteredMaps;
    }

    // Method to instantiate a random map from the given list
    private void InstantiateRandomMap(List<GameObject> mapList)
    {
        if (mapList.Count > 0)
        {
            GameObject randomMap = mapList[UnityEngine.Random.Range(0, mapList.Count)];

            // Instantiate the selected map
            if (randomMap != null)
            {
                Debug.Log("Instantiated!");
                var ins = Instantiate(randomMap, transform.position, Quaternion.identity);
                ins.transform.parent = grid.transform;
            }
            else
            {
                Debug.LogError("Failed to instantiate random map!");
            }
        }
        else
        {
            Debug.LogError("No map prefabs found!");
        }
    }
}
