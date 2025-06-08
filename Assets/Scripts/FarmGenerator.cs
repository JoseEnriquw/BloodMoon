using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Start()
    {
        GenerateGrid();
        PlaceUniquePrefabs();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                // Posición
                Vector3 position = new Vector3(x * tileSize, 0, z * tileSize);
                // Instanciar tile genérico
                GameObject selectedTile = tilePrefabs[Random.Range(0, tilePrefabs.Length)];
                if (gira)
                {
                    Quaternion randomRot = Quaternion.Euler(0, 90 * Random.Range(0, 4), 0);
                    Instantiate(selectedTile, position, randomRot, transform);
                }
                else
                {
                    Instantiate(selectedTile, position, Quaternion.identity, transform);
                }
                
                

                // Guardar posición como disponible para objetos únicos
                freePositions.Add(new Vector2Int(x, z));
            }
        }
    }

    void PlaceUniquePrefabs()
    {
        // Colocar la casa del jugador
        Vector2Int casaPos = GetRandomFreePosition();
        Instantiate(casaPrefab, GetWorldPosition(casaPos), Quaternion.identity, transform);

        // Colocar edificios únicos
        foreach (GameObject edificio in edificioPrefabs)
        {
            Vector2Int pos = GetRandomFreePosition();
            Instantiate(edificio, GetWorldPosition(pos), Quaternion.identity, transform);
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

}
