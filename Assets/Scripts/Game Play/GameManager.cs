using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject grass;
    [SerializeField] GameObject road;
    [SerializeField] int extent;
    [SerializeField] int frontDistance = 10;
    [SerializeField] int backDistance = -5;
    [SerializeField] int maxSameTerrainRepeat = 3;

    private int playerLastMaxTravel;

    Dictionary<int, TerrainBlock> map = new Dictionary<int, TerrainBlock>(50);

    TMP_Text gameOverText;

    private void Start()
    {
        // setup gameover panel
        gameOverPanel.SetActive(false);
        gameOverText = gameOverPanel.GetComponentInChildren<TMP_Text>();

        // * belakang
        for (int z = backDistance; z <= 0; z++)
        {
            CreateTerrain(grass, z);
        }

        // * depan
        for (int z = 1; z <= frontDistance; z++)
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

        player.SetUp(backDistance, extent);

    }

    private void Update()
    {
        // cek player masih hidup?
        if (player.IsDie && gameOverPanel.activeInHierarchy == false)
            StartCoroutine(ShowGameOverPanel());

        // Infinite Terrain system
        if (player.MaxTravel == playerLastMaxTravel)
            return;


        playerLastMaxTravel = player.MaxTravel;

        // bikin kedepan
        var randTbPrefab = GetNextRandomTerrainPrefab(player.MaxTravel + frontDistance);
        CreateTerrain(randTbPrefab, player.MaxTravel + frontDistance);

        // hapus dibelakang
        var lastTB = map[player.MaxTravel - 1 + backDistance];

        // hapus dari daftar
        map.Remove(player.MaxTravel - 1 + backDistance);
        // hilangkan dari scene
        Destroy(lastTB.gameObject);
        // setup lagi supaya player gk bisa gerak kebelakang
        player.SetUp(player.MaxTravel + backDistance, extent);
    }

    IEnumerator ShowGameOverPanel()
    {
        yield return new WaitForSeconds(1);

        gameOverText.text = "Your Score : " + player.MaxTravel;
        gameOverPanel.SetActive(true);
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

    // TODO berganti scene
    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
        Debug.Log("Ini adalah Scene ke-" + sceneIndex);
    }
}
