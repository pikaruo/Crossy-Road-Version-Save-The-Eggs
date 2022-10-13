using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject grass;
    [SerializeField] GameObject road;
    [SerializeField] int extent;
    [SerializeField] int frontDistance = 10;
    [SerializeField] int minZPos = -5;
    [SerializeField] int maxSameTerrainRepeat = 3;

    Dictionary<int, TerrainBlock> map = new Dictionary<int, TerrainBlock>(50);

    private void Start()
    {

        // * belakang
        for (int z = minZPos; z <= 0; z++)
        {
            CreateTerrain(grass, z);
        }

        // * depan
        for (int z = 1; z < frontDistance; z++)
        {
            // * menentukan terrain block dengan probabilitas 50% 
            var prefab = GetNextRandomTerrainPrefab(z);

            // * Instantiate block
            CreateTerrain(prefab, z);
        }

        // Debug.Log(Tree.AllPositions.Count);

        // foreach (var treePos in Tree.AllPositions)
        // {
        //     Debug.Log(treePos);
        // }

    }

    private void CreateTerrain(GameObject prefab, int zPos)
    {
        // * Instantiate block
        var go = Instantiate(prefab, new Vector3(0, 0, zPos), Quaternion.identity);
        var tb = go.GetComponent<TerrainBlock>();
        tb.Build(extent);

        map.Add(zPos, tb);
        Debug.Log(map[zPos].GetType());
        Debug.Log(map[zPos] is Road);
    }

    private GameObject GetNextRandomTerrainPrefab(int nextpos)
    {
        bool isUniform = true;
        var tbRef = map[nextpos - 1];
        for (int distance = 2; distance <= maxSameTerrainRepeat; distance++)
        {
            if (map[nextpos - distance].GetType() != tbRef.GetType())
            {
                isUniform = false;
                break;
            }
        }

        if (isUniform)
        {
            if (tbRef is Grass)
            {
                return road;
            }
            return grass;
        }

        // * menentukan terrain block dengan probabilitas 50% 
        return Random.value > 0.5f ? road : grass;
    }
}
