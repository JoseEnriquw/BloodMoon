using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.AI;

public class FarmGenerator : MonoBehaviour
{
    [Header("Tiles de base")]
    public GameObject[] tilePrefabs; // Tiles genéricos

    [Header("Prefabs únicos")]
    public GameObject casaPrefab; // Donde aparece el jugador
    public GameObject[] edificioPrefabs; // Edificios únicos

    [Header("Configuración de grilla")]
    public int gridSize = 10;
    public float tileSize = 1f;

    [SerializeField] bool gira = false;

    private List<Vector2Int> freePositions = new List<Vector2Int>();

    //public NavMeshSurface navMeshSurface;

    void Start()
    {
        SelectUniquePositions();
        GenerateGrid();
        PlaceUniquePrefabs();

        //navMeshSurface.BuildNavMesh();
    }

    HashSet<Vector2Int> reservedPositions = new HashSet<Vector2Int>();

    void SelectUniquePositions()
    {
        int bordeMin = 1; // Qué tan lejos del borde
        int distanciaMinima = 3; // Distancia mínima entre edificios
        List<Vector2Int> candidatas = new List<Vector2Int>();

        // Generar todas las posiciones válidas (alejadas del borde)
        for (int x = bordeMin; x < gridSize - bordeMin; x++)
        {
            for (int z = bordeMin; z < gridSize - bordeMin; z++)
            {
                candidatas.Add(new Vector2Int(x, z));
            }
        }

        // Barajar las posiciones
        ShuffleList(candidatas);

        // Intentar colocar la casa y edificios
        while (reservedPositions.Count < 1 + edificioPrefabs.Length && candidatas.Count > 0)
        {
            Vector2Int candidata = candidatas[0];
            candidatas.RemoveAt(0);

            bool demasiadoCerca = false;
            foreach (Vector2Int reservada in reservedPositions)
            {
                if (Vector2Int.Distance(candidata, reservada) < distanciaMinima)
                {
                    demasiadoCerca = true;
                    break;
                }
            }

            if (!demasiadoCerca)
            {
                reservedPositions.Add(candidata);
            }
        }

        if (reservedPositions.Count < 1 + edificioPrefabs.Length)
        {
            Debug.LogWarning("No se pudieron colocar todos los edificios con la distancia requerida.");
        }
    }

    void GenerateGrid()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                Vector2Int pos = new Vector2Int(x, z);
                if (reservedPositions.Contains(pos)) continue; // Saltar posiciones reservadas

                Vector3 position = GetWorldPosition(pos);
                GameObject selectedTile = tilePrefabs[Random.Range(0, tilePrefabs.Length)];
                Quaternion rot = gira ? Quaternion.Euler(0, 90 * Random.Range(0, 4), 0) : Quaternion.identity;
                Instantiate(selectedTile, position, rot, transform);

                freePositions.Add(pos);
            }
        }
    }

    void PlaceUniquePrefabs()
    {
        int i = 0;
        foreach (Vector2Int pos in reservedPositions)
        {
            GameObject prefab = i == 0 ? casaPrefab : edificioPrefabs[i - 1];
            Instantiate(prefab, GetWorldPosition(pos), Quaternion.identity, transform);
            i++;
        }
    }
    

    Vector2Int GetRandomFreePosition()
    {
        int index = Random.Range(0, freePositions.Count);
        Vector2Int pos = freePositions[index];
        freePositions.RemoveAt(index); // Remover para evitar repetir
        return pos;
    }

    Vector3 GetWorldPosition(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * tileSize, 0, gridPos.y * tileSize);
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
