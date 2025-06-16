using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using UnityEngine.SceneManagement;

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



    [Header("Cercas")]
    public GameObject fencePrefab;      // Cerca genérica de 3x1
    public GameObject gatePrefab;       // Puerta de salida
    public GameObject cornerPrefab; // Prefab de esquina

    public bool colocarPuerta = true;

    [Header("Nav mesh surface")]
    public NavMeshSurface navMeshSurface;

    [Header("Objetos del mapa")]
    public GameObject vidasPrefab;
    public GameObject balasPrefab;
    public GameObject runaPrefab;

    [Tooltip("Cuántas vidas spawnean")]
    public int cantidadVidas = 5;
    [Tooltip("Cuántas balas spawnean")]
    public int cantidadBalas = 5;
    [Tooltip("Radio desde el centro para colocar pickups")]
    public float mapRadius = 100f;

    [Tooltip("Layer donde está el suelo")]
    public LayerMask groundLayer;

    [Tooltip("Objetos sobre los que no deben aparecer pickups")]
    public LayerMask blockedLayer;
    void Start()
    {
        SelectUniquePositions();
        GenerateGrid();
        PlaceUniquePrefabs();

        PlaceFences();

        StartCoroutine(GenerateFarmRoutine());
    }

    HashSet<Vector2Int> reservedPositions = new HashSet<Vector2Int>();

    IEnumerator GenerateFarmRoutine()
    {
        // Construir el NavMesh
        navMeshSurface.BuildNavMesh();

        // Esperar hasta que el NavMesh esté listo
        yield return new WaitUntil(() => navMeshSurface.navMeshData != null);

        // Esperar un frame adicional para asegurar que todo está inicializado
        yield return null;

        // Activar todos los spawners
        ActivateAllSpawners();

        // Spawnear ítems del mapa (vidas, balas y la runa)
        SpawnItems(vidasPrefab, cantidadVidas);
        yield return null;

        SpawnItems(balasPrefab, cantidadBalas);
        yield return null;

        // Runa especial: solo una vez
        SpawnRunaSegura();
    }
    //nuevo
    void SpawnItems(GameObject prefab, int cantidad)
    {
        for (int i = 0; i < cantidad; i++)
        {
            Vector3 spawnPos = GetRandomNavMeshPosition();
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }

    void SpawnRunaSegura()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (GameManager.gameManager.HasRuneForLevel(sceneIndex))
        {
            Debug.Log("✅ Runa ya recolectada en este nivel. No se genera.");
            return;
        }
        Vector3 spawnPos = GetRandomNavMeshPosition();
        if (spawnPos != Vector3.zero)
        {
            Instantiate(runaPrefab, spawnPos, Quaternion.identity);
            Debug.Log($"✅ Runa instanciada en {spawnPos}");
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró una posición válida para la runa.");
        }
    }

    //Vector3 GetRandomNavMeshPosition()
    //{
    //    const float verticalOffset = 0.3f; // Subida inicial para que no atraviese el suelo
    //    int maxTries = 30;

    //    for (int tries = 0; tries < maxTries; tries++)
    //    {
    //        float safeMargin = 2f; // Evita extremos del mapa
    //        float safeMinX = tileSize * safeMargin;
    //        float safeMaxX = tileSize * (gridSize - safeMargin);
    //        float safeMinZ = tileSize * safeMargin;
    //        float safeMaxZ = tileSize * (gridSize - safeMargin);

    //        float randomX = Random.Range(safeMinX, safeMaxX);
    //        float randomZ = Random.Range(safeMinZ, safeMaxZ);

    //        Vector3 probe = new Vector3(randomX, 50f, randomZ);

    //        if (Physics.Raycast(probe, Vector3.down, out RaycastHit hit, 100f, groundLayer))
    //        {
    //            if (NavMesh.SamplePosition(hit.point, out NavMeshHit nav, 1.5f, NavMesh.AllAreas))
    //            {
    //                Vector3 finalPos = nav.position + Vector3.up * verticalOffset;
    //                return finalPos;
    //            }
    //        }
    //    }

    //    Debug.LogWarning("⚠️ No se encontró posición segura sobre el mapa.");
    //    return Vector3.zero;
    //}
    Vector3 GetRandomNavMeshPosition()
    {
        const float verticalOffset = 0.3f;
        int maxTries = 30;

        for (int tries = 0; tries < maxTries; tries++)
        {
            float safeMargin = 2f;
            float safeMinX = tileSize * safeMargin;
            float safeMaxX = tileSize * (gridSize - safeMargin);
            float safeMinZ = tileSize * safeMargin;
            float safeMaxZ = tileSize * (gridSize - safeMargin);

            float randomX = Random.Range(safeMinX, safeMaxX);
            float randomZ = Random.Range(safeMinZ, safeMaxZ);

            Vector3 probe = new Vector3(randomX, 50f, randomZ);

            if (Physics.Raycast(probe, Vector3.down, out RaycastHit hit, 100f, groundLayer))
            {
                if (NavMesh.SamplePosition(hit.point, out NavMeshHit nav, 1.5f, NavMesh.AllAreas))
                {
                    Vector3 finalPos = nav.position + Vector3.up * verticalOffset;

                    // ✅ Check para evitar zonas bloqueadas
                    if (Physics.CheckSphere(finalPos, 0.5f, blockedLayer))
                    {
                        continue; // Estaba dentro de un edificio inaccesible
                    }

                    return finalPos;
                }
            }
        }

        Debug.LogWarning("⚠️ No se encontró posición segura sobre el mapa.");
        return Vector3.zero;
    }



    //fin nuevo
    void ActivateAllSpawners()
    {
        // Buscar todos los ZombieSpawners en la escena
        ZombieSpawner[] spawners = FindObjectsOfType<ZombieSpawner>();

        foreach (ZombieSpawner spawner in spawners)
        {
            spawner.ZSpawner();
        }

        Debug.Log($"Activados {spawners.Length} spawners de zombies");
    }
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

    void PlaceFences()
    {
        int lado = Random.Range(0, 4); // 0=Sur, 1=Este, 2=Norte, 3=Oeste
        int puertaIndex = gridSize / 2;

        for (int x = 0; x < gridSize; x++)
        {
            Vector3 posSur = new Vector3(x * tileSize, 0, -tileSize);
            Vector3 posNorte = new Vector3(x * tileSize, 0, gridSize * tileSize);

            GameObject prefabSur = (lado == 0 && x == puertaIndex) ? gatePrefab : fencePrefab;
            GameObject prefabNorte = (lado == 2 && x == puertaIndex) ? gatePrefab : fencePrefab;

            Instantiate(prefabSur, posSur, Quaternion.identity, transform);
            Instantiate(prefabNorte, posNorte, Quaternion.Euler(0, 180, 0), transform);
        }

        for (int z = 0; z < gridSize; z++)
        {
            Vector3 posOeste = new Vector3(-tileSize, 0, z * tileSize);
            Vector3 posEste = new Vector3(gridSize * tileSize, 0, z * tileSize);

            GameObject prefabOeste = (lado == 3 && z == puertaIndex) ? gatePrefab : fencePrefab;
            GameObject prefabEste = (lado == 1 && z == puertaIndex) ? gatePrefab : fencePrefab;

            Quaternion rot = Quaternion.Euler(0, 90, 0);
            Instantiate(prefabOeste, posOeste, rot, transform);
            Instantiate(prefabEste, posEste, rot, transform);
        }
        PlaceFenceCorners();
    }
    void PlaceFenceCorners()
    {
        float s = tileSize;
        float max = gridSize * s;

        // Posiciones de esquina
        Vector3 esquinaSO = new Vector3(-s, 0, -s);
        Vector3 esquinaSE = new Vector3(max, 0, -s);
        Vector3 esquinaNO = new Vector3(-s, 0, max);
        Vector3 esquinaNE = new Vector3(max, 0, max);

        // Instanciamos cada esquina con su rotación correspondiente
        Instantiate(cornerPrefab, esquinaSO, Quaternion.Euler(0, 180, 0), transform);
        Instantiate(cornerPrefab, esquinaSE, Quaternion.Euler(0, 90, 0), transform);
        Instantiate(cornerPrefab, esquinaNO, Quaternion.Euler(0, 270, 0), transform);
        Instantiate(cornerPrefab, esquinaNE, Quaternion.identity, transform);
    }

}

