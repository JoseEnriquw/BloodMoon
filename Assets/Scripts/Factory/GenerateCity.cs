using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

[ExecuteAlways]
public class ModularCityGeneratorWithPerimeterBoundaries : MonoBehaviour
{
    public enum RoadPieceType { Straight, Corner, TJunction, Cross, End }

    [System.Serializable]
    public struct RoadPrefabGroup
    {
        public RoadPieceType type;
        [Tooltip("Variante(s) de prefab para este tipo de calle")]
        public List<GameObject> prefabs;
    }

    [Header("Grid Settings")]
    [Tooltip("Número de ejes de calle en X (>=2)")]
    public int streetsX = 4;
    [Tooltip("Número de ejes de calle en Z (>=2)")]
    public int streetsZ = 4;
    [Tooltip("Cantidad de tramos ‘Straight’ entre intersecciones")]
    public int straightsPerBlock = 2;
    [Tooltip("Semilla (0 = aleatoria cada vez)")]
    public int seed = 0;

    [Header("Road Prefabs")]
    [Tooltip("Para cada RoadPieceType, asigna una lista de prefabs")]
    public List<RoadPrefabGroup> roadPrefabs = new List<RoadPrefabGroup>();

    [Header("Block Prefabs")]
    [Tooltip("Prefabs de manzana para poblar bloques")]
    public List<GameObject> blockPrefabs = new List<GameObject>();

    [Header("Boundary Prefabs")]
    [Tooltip("Prefabs para delimitar el perímetro de la ciudad")]
    public GameObject boundaryStraightPrefab;
    public GameObject boundaryCornerPrefab;

    private Transform cityParent;

    void Start()
    {
        GenerateCity();
    }

    [ContextMenu("Generate City")]
    public void GenerateCity()
    {
        // 1) Inicializar semilla
        if (seed != 0) Random.InitState(seed);
        else seed = Random.Range(1, int.MaxValue);

        // 2) Borrar ciudad previa
        if (cityParent != null)
            DestroyImmediate(cityParent.gameObject);

        // 3) Crear raíz de la ciudad
        cityParent = new GameObject("City").transform;
        cityParent.position = transform.position;

        // 4) Crear contenedores
        var roadsParent = new GameObject("Roads").transform;
        var blocksParent = new GameObject("Blocks").transform;
        var boundariesParent = new GameObject("Boundaries").transform;
        roadsParent.SetParent(cityParent, false);
        blocksParent.SetParent(cityParent, false);
        boundariesParent.SetParent(cityParent, false);

        // 5) Medir tamaños reales de piezas
        GameObject straightSample = GetRandomRoadPrefab(RoadPieceType.Straight);
        GameObject crossSample = GetRandomRoadPrefab(RoadPieceType.Cross);
        if (straightSample == null || crossSample == null)
        {
            Debug.LogError("Faltan prefabs de tipo Straight o Cross en roadPrefabs");
            return;
        }
        float straightLen = straightSample.GetComponent<Renderer>().bounds.size.z;
        float interSize = crossSample.GetComponent<Renderer>().bounds.size.x;

        // 6) Parámetros de rejilla
        int xCount = Mathf.Max(2, streetsX);
        int zCount = Mathf.Max(2, streetsZ);
        int segCount = Mathf.Max(1, straightsPerBlock);
        float step = interSize + straightLen * segCount;
        float halfStep = step * 0.5f;
        Vector3 origin = cityParent.position;

        // 7) Generar intersecciones (nodos)
        for (int ix = 0; ix < xCount; ix++)
            for (int iz = 0; iz < zCount; iz++)
            {
                bool n = iz < zCount - 1;
                bool s = iz > 0;
                bool e = ix < xCount - 1;
                bool w = ix > 0;
                int mask = (n ? 1 : 0) | (e ? 2 : 0) | (s ? 4 : 0) | (w ? 8 : 0);

                RoadPieceType type;
                float rotY;
                switch (mask)
                {
                    case 5: type = RoadPieceType.Straight; rotY = 0; break;
                    case 10: type = RoadPieceType.Straight; rotY = 90; break;
                    case 9: type = RoadPieceType.Corner; rotY = -90; break;
                    case 3: type = RoadPieceType.Corner; rotY = 0; break;
                    case 6: type = RoadPieceType.Corner; rotY = 90; break;
                    case 12: type = RoadPieceType.Corner; rotY = 180; break;
                    case 7: type = RoadPieceType.TJunction; rotY = 90; break;
                    case 14: type = RoadPieceType.TJunction; rotY = 180; break;
                    case 13: type = RoadPieceType.TJunction; rotY = -90; break;
                    case 11: type = RoadPieceType.TJunction; rotY = 0; break;
                    case 15: type = RoadPieceType.Cross; rotY = 0; break;
                    case 1: type = RoadPieceType.End; rotY = 0; break;
                    case 2: type = RoadPieceType.End; rotY = 90; break;
                    case 4: type = RoadPieceType.End; rotY = 180; break;
                    case 8: type = RoadPieceType.End; rotY = 270; break;
                    default: continue;
                }

                GameObject nodePrefab = GetRandomRoadPrefab(type);
                Vector3 nodePos = origin + new Vector3(ix * step, 0, iz * step);
                Spawn(nodePrefab, nodePos, Quaternion.Euler(0, rotY, 0), roadsParent);
            }

        // 8) Generar tramos “Straight” entre nodos
        for (int ix = 0; ix < xCount - 1; ix++)
            for (int iz = 0; iz < zCount; iz++)
                for (int k = 1; k <= segCount; k++)
                {
                    float dx = interSize * 0.5f + straightLen * (k - 0.5f);
                    Vector3 p = origin + new Vector3(ix * step + dx, 0, iz * step);
                    Spawn(GetRandomRoadPrefab(RoadPieceType.Straight), p, Quaternion.identity, roadsParent);
                }
        for (int ix = 0; ix < xCount; ix++)
            for (int iz = 0; iz < zCount - 1; iz++)
                for (int k = 1; k <= segCount; k++)
                {
                    float dz = interSize * 0.5f + straightLen * (k - 0.5f);
                    Vector3 p = origin + new Vector3(ix * step, 0, iz * step + dz);
                    Spawn(GetRandomRoadPrefab(RoadPieceType.Straight), p, Quaternion.Euler(0, 90, 0), roadsParent);
                }

        // 9) Generar manzanas
        for (int ix = 0; ix < xCount - 1; ix++)
            for (int iz = 0; iz < zCount - 1; iz++)
            {
                Vector3 center = origin + new Vector3(
                    ix * step + halfStep,
                    0,
                    iz * step + halfStep
                );
                if (blockPrefabs.Count > 0)
                {
                    GameObject block = blockPrefabs[Random.Range(0, blockPrefabs.Count)];
                    Spawn(block, center, Quaternion.identity, blocksParent);
                }
            }

        // 10) Generar perímetro perfecto: corners + straights (sin solapamiento ni espacios)
        if (boundaryStraightPrefab != null && boundaryCornerPrefab != null)
        {
            float boundaryLen = boundaryStraightPrefab.GetComponent<Renderer>().bounds.size.z; // Asegúrate que mesh y collider coinciden con renderer

            float minX = origin.x - interSize;
            float maxX = origin.x + (xCount - 1) * step + interSize;
            float minZ = origin.z - interSize;
            float maxZ = origin.z + (zCount - 1) * step + interSize;

            // Posiciones de corners
            Vector3 southWest = new Vector3(minX, 0, minZ); // (xMin, zMin)
            Vector3 southEast = new Vector3(maxX, 0, minZ); // (xMax, zMin)
            Vector3 northEast = new Vector3(maxX, 0, maxZ); // (xMax, zMax)
            Vector3 northWest = new Vector3(minX, 0, maxZ); // (xMin, zMax)

            // Spawnear corners
            Spawn(boundaryCornerPrefab, southWest, Quaternion.identity, boundariesParent);           // SW
            Spawn(boundaryCornerPrefab, southEast, Quaternion.Euler(0, 90, 0), boundariesParent);    // SE
            Spawn(boundaryCornerPrefab, northEast, Quaternion.Euler(0, 180, 0), boundariesParent);   // NE
            Spawn(boundaryCornerPrefab, northWest, Quaternion.Euler(0, 270, 0), boundariesParent);   // NW

            // Calcular cuántos straights por lado (solo entre corners)
            int countX = Mathf.RoundToInt((maxX - minX) / boundaryLen);
            int countZ = Mathf.RoundToInt((maxZ - minZ) / boundaryLen);

            // Sur (de SW a SE, sin corners)
            for (int i = 1; i < countX; i++)
            {
                float x = minX + i * boundaryLen;
                Vector3 pos = new Vector3(x, 0, minZ);
                Spawn(boundaryStraightPrefab, pos, Quaternion.identity, boundariesParent);
            }
            // Norte (de NW a NE, sin corners)
            for (int i = 1; i < countX; i++)
            {
                float x = minX + i * boundaryLen;
                Vector3 pos = new Vector3(x, 0, maxZ);
                Spawn(boundaryStraightPrefab, pos, Quaternion.Euler(0, 180, 0), boundariesParent);
            }
            // Oeste (de SW a NW, sin corners)
            for (int i = 1; i < countZ; i++)
            {
                float z = minZ + i * boundaryLen;
                Vector3 pos = new Vector3(minX, 0, z);
                Spawn(boundaryStraightPrefab, pos, Quaternion.Euler(0, 270, 0), boundariesParent);
            }
            // Este (de SE a NE, sin corners)
            for (int i = 1; i < countZ; i++)
            {
                float z = minZ + i * boundaryLen;
                Vector3 pos = new Vector3(maxX, 0, z);
                Spawn(boundaryStraightPrefab, pos, Quaternion.Euler(0, 90, 0), boundariesParent);
            }
        }

    }

    private GameObject GetRandomRoadPrefab(RoadPieceType type)
    {
        var group = roadPrefabs.Find(g => g.type == type);
        if (group.prefabs != null && group.prefabs.Count > 0)
            return group.prefabs[Random.Range(0, group.prefabs.Count)];
        return null;
    }

    private void SpawnRandom(List<GameObject> list, Vector3 pos, Quaternion rot, Transform parent)
    {
        var prefab = list[Random.Range(0, list.Count)];
        if (prefab != null)
            Spawn(prefab, pos, rot, parent);
    }

    private void Spawn(GameObject prefab, Vector3 pos, Quaternion rot, Transform parent)
    {
        if (prefab == null) return;
#if UNITY_EDITOR
        GameObject inst = Application.isPlaying
            ? Instantiate(prefab, pos, rot, parent)
            : (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
#else
        GameObject inst = Instantiate(prefab, pos, rot, parent);
#endif
        inst.transform.SetLocalPositionAndRotation(pos, rot);
    }
}
