using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using Random = System.Random;


namespace Grid
{
    public class GridManager : MonoBehaviour
    {
        // PUBLIC/EDITOR ---------------------------------------------------------------------------------------------------------------------------
        [Header("Grid setup")]
        [SerializeField] private Vector2Int m_gridSize;
        [SerializeField] private Vector2 m_cellSize;
        [SerializeField] private bool debug_grid = false;
        private float m_timeBeforeFireAgain = 30.0f; // 30 seconds

        private FireKClustering m_kClustering = null;

        public static GridManager GetInstance()
        {
            return m_instance;
        }

        public void InitFireTile()
        {
            foreach (int i in newFire)
            {
                if (m_fireTiles.TryAdd(i, 1))
                {
                    m_tiles[i].SetTileType(TileType.Fire);

                    // Activate the vfx on the props
                    m_tiles[i].SetPropsOnFire(true);

                    FireGroup newGroup = new FireGroup();
                    newGroup.SetHashSet(new HashSet<int>() { i });
                    newGroup.SetCenter(m_tiles[i].getWorldPosition());
                    m_fireGroups.Add(newGroup);
                }
            }
        }

        public void BakeTiles()
        {
            m_tiles.Clear();
            newFire.Clear();

            m_size = m_cellSize;
            m_offset = m_cellSize / 2;

            string[] tileTypeNameString = Enum.GetNames(typeof(TileType));
            HashSet<string> m_availableTileType = new HashSet<string>(tileTypeNameString);

            for (int y = 0; y < m_gridSize.y; y++)
            {
                for (int x = 0; x < m_gridSize.x; x++)
                {
                    // populate the x-axis first, then y-axis; logic of determining the "neighbor" tiles is hardcoded.
                    Tile newTile = new Tile(new Vector3(gameObject.transform.position.x + x * m_size.x + m_offset.x, gameObject.transform.position.y, gameObject.transform.position.z + y * m_size.y + m_offset.y),
                        new Vector2Int(x, y));
                    Collider[] colliders = Physics.OverlapBox(newTile.getWorldPosition(), new Vector3(m_size.x / 2, 0, m_size.y / 2));
                    TileType finalType = TileType.Land;
                    foreach (Collider c in colliders)
                    {
                        if (m_availableTileType.Contains(c.gameObject.tag))
                        {
                            if (c.gameObject.tag == "Fire")
                            {
                                finalType = TileType.Fire;
                            }
                            else if (c.gameObject.tag == "Flammable" && finalType != TileType.Fire)
                            {
                                finalType = TileType.Flammable;
                            }
                            else if (finalType != TileType.Flammable)
                            {
                                finalType = Enum.Parse<TileType>(c.gameObject.tag);
                            }
                            // can set tile's different m_isFlammable property based on tag
                        }

                        var v = c.gameObject.GetComponent<FireVFX_Controller>();
                        if (v != null)
                        {
                            newTile.AddProps(v);
                        }
                    }
                    newTile.SetTileType(finalType);
                    if (finalType == TileType.Fire)
                    {
                        newFire.Add(y * m_gridSize.x + x);
                    }
                    m_tiles.Add(newTile);
                    //m_tiles[m_tiles.Count - 1].Initialize(m_gridSize);
                }
            }

            Debug.Log("Baking Tile Finished");
        }

        /// <summary>
        /// Try to get a tile from a given grid position
        /// </summary>
        /// <param name="x">Grid position x</param>
        /// <param name="y">Grid position y</param>
        /// <returns>Returns the tile on the grid position, if position not in grid range, return null</returns>
        public Tile TryGetTile(int x, int y)
        {
            if (x < 0 || x >= m_gridSize.x || y < 0 || y >= m_gridSize.y) return null;

            int index = y * m_gridSize.x + x;
            return m_tiles[index];
        }

        /// <summary>
        /// Try to get a tile from a given world position
        /// Tiles are retrieved based on the xz value of the input Vector3
        /// </summary>
        /// <param name="position">The world position</param>
        /// <returns>Returns the tile that the input world position is in range of, if position is outside the grid, return null</returns>
        public Tile TryGetTile(Vector3 p_position)
        {
            Vector2 gridPosition = new Vector2((p_position.x - gameObject.transform.position.x) / m_size.x, (p_position.z - gameObject.transform.position.z) / m_size.y);

            if ((int)gridPosition.x < 0 || (int)gridPosition.x >= m_gridSize.x || (int)gridPosition.y < 0 || (int)gridPosition.y >= m_gridSize.y) return null;
            int index = (int)gridPosition.y * m_gridSize.x + (int)gridPosition.x;
            return m_tiles[index];
        }

        public List<Tile> TryGetTileInRange(Vector3 p_position, float p_range)
        {
            List<Tile> result = new List<Tile>();
            Vector2 gridPosition = new Vector2((p_position.x - gameObject.transform.position.x) / m_size.x, (p_position.z - gameObject.transform.position.z) / m_size.y);

            for (int x = (int)gridPosition.x - (int)p_range; x <= (int)gridPosition.x + (int)p_range; x++)
            {
                for (int y = (int)gridPosition.y - (int)p_range; y <= (int)gridPosition.y + (int)p_range; y++)
                {
                    if (x < 0 || x >= m_gridSize.x || y < 0 || y >= m_gridSize.y) continue;

                    int index = y * m_gridSize.x + x;

                    Vector3 tilePosition = new Vector3(m_tiles[index].getWorldPosition().x, 0f, m_tiles[index].getWorldPosition().z);
                    Vector3 start = new Vector3(p_position.x, 0f, p_position.z);

                    if (Vector3.Distance(tilePosition, start) < p_range)
                        result.Add(m_tiles[index]);
                }
            }

            return result;
        }

        public List<int> TryGetTileIndexInRange(Vector3 p_position, float p_range)
        {
            List<int> result = new List<int>();
            Vector2 gridPosition = new Vector2((p_position.x - gameObject.transform.position.x) / m_size.x, (p_position.z - gameObject.transform.position.z) / m_size.y);

            for (int x = (int)gridPosition.x - (int)p_range; x <= (int)gridPosition.x + (int)p_range; x++)
            {
                for (int y = (int)gridPosition.y - (int)p_range; y <= (int)gridPosition.y + (int)p_range; y++)
                {
                    if (x < 0 || x >= m_gridSize.x || y < 0 || y >= m_gridSize.y) continue;
                    int index = y * m_gridSize.x + x;
                    result.Add(index);
                }
            }

            return result;
        }

        public HashSet<int> AddTileToFire(Vector3 p_position, float p_range)
        {
            HashSet<int> result = new HashSet<int>();

            Vector2 gridPosition = new Vector2((p_position.x - gameObject.transform.position.x) / m_size.x, (p_position.z - gameObject.transform.position.z) / m_size.y);

            for (int x = (int)gridPosition.x - (int)p_range; x <= (int)gridPosition.x + (int)p_range; x++)
            {
                for (int y = (int)gridPosition.y - (int)p_range; y <= (int)gridPosition.y + (int)p_range; y++)
                {
                    if (x < 0 || x >= m_gridSize.x || y < 0 || y >= m_gridSize.y) continue;
                    int index = y * m_gridSize.x + x;

                    if (m_tiles[index].IsFlammable())
                        result.Add(index);
                }
            }

            return result;
        }

        public void EvaluateFireGroups()
        {
            if (m_kClustering == null)
            {
                m_kClustering = new FireKClustering(m_tiles, Math.Min(m_fireTiles.Count, 4)); // current level design has 4 groups of fire.
            }
            
            if(!m_kClustering.KClusteringInit(m_fireTiles))
            {
                m_kClustering = null;
            }
                
            List<Vector3> fireCenters = m_kClustering?.KClusteringRun();

            if (fireCenters != null)
            {
                m_fireGroups.Clear();
                foreach (Vector3 fireCenter in fireCenters)
                {
                    FireGroup fireGroup = new FireGroup();
                    fireGroup.SetCenter(fireCenter);
                    m_fireGroups.Add(fireGroup);
                }
            }

            //List<HashSet<int>> mergeGroups = new List<HashSet<int>>();
            //for (int first = 0; first < m_fireGroups.Count - 1; first++)
            //{
            //    for (int second = first + 1; second < m_fireGroups.Count; second++)
            //    {
            //        var intersection = new HashSet<int>(m_fireGroups[first].GetHashSet());
            //        intersection.IntersectWith(m_fireGroups[second].GetHashSet());

            //        if (intersection.Count == 0) continue;

            //        bool inMergeGroup = false;
            //        foreach (var merge in mergeGroups)
            //        {
            //            if (merge.Contains(first) || merge.Contains(second))
            //            {
            //                merge.Add(first);
            //                merge.Add(second);
            //                inMergeGroup = true;
            //                break;
            //            }
            //        }

            //        if (!inMergeGroup)
            //            mergeGroups.Add(new HashSet<int>() { first, second });
            //    }
            //}

            //foreach (var merge in mergeGroups)
            //{
            //    List<int> mergeList = merge.ToList<int>();
            //    for (int i = 1; i < mergeList.Count; i++)
            //    {
            //        m_fireGroups[mergeList[0]].GetHashSet().AddRange(m_fireGroups[mergeList[i]].GetHashSet());
            //        m_fireGroups[mergeList[0]].SetCenter((m_fireGroups[mergeList[0]].GetCenter() + m_fireGroups[mergeList[i]].GetCenter()) / 2);
            //        m_fireGroups[mergeList[i]].MarkDelete();
            //    }
            //}

            //m_fireGroups.RemoveAll(e => e.CanBeDeleted());
        }

        public bool FireSpread()
        {
            // This function can be called by a timer from state machine.

            Dictionary<int, int> fireTilesCopy = new Dictionary<int, int>(m_fireTiles); // copy constructor

            HashSet<int> newFireTiles = new HashSet<int>();

            foreach (KeyValuePair<int, int> kvp in fireTilesCopy)
            {
                if (kvp.Value != 3)
                {
                    // fire grows up 
                    m_fireTiles[kvp.Key] += 1;
                }
                else
                {
                    int fireGroupIndex = -1;
                    for (int i = 0; i < m_fireGroups.Count; i++)
                    {
                        if (m_fireGroups[i].GetHashSet().Contains(kvp.Key))
                        {
                            fireGroupIndex = i;
                            break;
                        }
                    }

                    HashSet<int> result = AddTileToFire(m_tiles[kvp.Key].getWorldPosition(), 3);
                    //foreach (int index in result.ToList<int>())
                    //{
                    //    m_fireGroups[fireGroupIndex].GetHashSet().Add(index);
                    //}
                    newFireTiles.AddRange(result);
                }
            }

            // deal with new tiles
            return AddFire(newFireTiles.ToList<int>());
        }

        public bool AddFire(List<int> p_newFireTile)
        {
            foreach (int i in p_newFireTile)
            {
                if (!m_tiles[i].IsFlammable()) continue;

                if (m_fireTiles.TryAdd(i, 1))
                {
                    if (m_tiles[i].GetTileType() == TileType.BigTree)
                    {
                        // game over
                        return true;
                    }
                    m_tiles[i].SetTileType(TileType.Fire);

                    // Activate the vfx on the props
                    m_tiles[i].SetPropsOnFire(true);
                }
            }
            return false;
        }

        public bool FireHaveBeenPutOut()
        {
            return m_fireTiles.Count <= 0;
        }

        public bool OnWaterComing(List<int> p_waterSprinkling)
        {
            foreach (int tileIndex in p_waterSprinkling)
            {
                if (m_fireTiles.ContainsKey(tileIndex))
                {
                    m_fireTiles.Remove(tileIndex);
                    Tile currentTile = m_tiles[tileIndex];
                    currentTile.SetFireLevel(0);
                    currentTile.SetPropsOnFire(false);
                    currentTile.SetWetTimer(Time.time + m_timeBeforeFireAgain);

                    // Deactivate the fire vfx
                }
            }

            return m_fireTiles.Count == 0;
        }

        // PRIVATE ---------------------------------------------------------------------------------------------------------------------------------
        private static GridManager m_instance;
        [SerializeField, HideInInspector] private List<Tile> m_tiles = new List<Tile>();
        [SerializeField, HideInInspector] private Vector2 m_size, m_offset;
        private Dictionary<int, int> m_fireTiles = new Dictionary<int, int>(); // first int means tileIndex in m_tiles, second is fire level from 0 to 3 (no fire to most severe)
        private List<Tile> m_gameMap = new List<Tile>();
        [SerializeField] private List<int> newFire = new List<int>();
        public List<FireGroup> m_fireGroups = new List<FireGroup>();

        private void Awake()
        {
            if (m_instance == null)
            {
                m_instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void OnDrawGizmos()
        {
            if (!debug_grid) return;

            foreach (var tile in m_tiles)
            {
                switch (tile.GetTileType())
                {
                    case TileType.Flammable:
                        Gizmos.color = Color.green;
                        break;
                    case TileType.Water:
                        Gizmos.color = Color.cyan;
                        break;
                    case TileType.Fire:
                        Gizmos.color = Color.red;
                        break;
                    case TileType.Obstacle:
                        Gizmos.color = Color.gray;
                        break;
                    case TileType.Barrier:
                        Gizmos.color = Color.black;
                        break;
                    case TileType.BigTree:
                        Gizmos.color = Color.white;
                        break;
                    case TileType.Land:
                        Gizmos.color = Color.yellow;
                        break;
                }
                Gizmos.DrawCube(tile.getWorldPosition(), new Vector3(m_size.x, 0.0f, m_size.y));
            }
        }
        class FireKClustering
        {
            private int m_clusters;
            private List<Tile> m_gameMap;
            private List<int> m_fireTiles;
            private List<Vector3> m_fireTileWorldPos = new List<Vector3>();

            private List<int> m_centroids = new List<int>(); // by tile index
            private List<Vector3> m_centroidsWorldPos = new List<Vector3>();
            private List<List<Vector3>> m_clusteredTiles = new List<List<Vector3>>();

            public FireKClustering(List<Tile> p_gameMap, int k) // k means number of clusters
            {
                m_gameMap = p_gameMap;
                m_clusters = k;
            }

            public List<Vector3> KClusteringRun()
            {
                while (true)
                {
                    KClusteringAssign();
                    if (KClusteringUpdate())
                    {
                        break;
                    }
                }
                return m_centroidsWorldPos;
            }

            public bool KClusteringInit(Dictionary<int, int> p_fireTiles)
            {
                m_centroids.Clear();
                m_clusteredTiles.Clear();
                m_centroidsWorldPos.Clear();
                m_fireTileWorldPos.Clear();

                if (p_fireTiles.Count < m_clusters) return false;

                m_fireTiles = p_fireTiles.Keys.ToList<int>();

                foreach (int i in m_fireTiles)
                {
                    m_fireTileWorldPos.Add(m_gameMap[i].getWorldPosition());
                }

                // Randomly select m_clusters data points as initial centroids
                Random rnd = new Random();
                HashSet<int> ints = new HashSet<int>();
                while (true)
                {
                    ints.Add(m_fireTiles[rnd.Next(0, m_fireTiles.Count)]);
                    if (ints.Count == m_clusters)
                    {
                        break;
                    }
                }

                for (int i = 0; i < ints.Count; i++)
                {
                    m_centroids.Add(ints.ElementAt(i));
                    m_centroidsWorldPos.Add(m_gameMap[m_centroids[i]].getWorldPosition());
                    m_clusteredTiles.Add(new List<Vector3>());
                }

                return true;
            }

            public void KClusteringAssign()
            {
                // clear each list in m_clusteredTiles
                for (int i = 0; i < m_clusters; i++)
                {
                    m_clusteredTiles[i].Clear();
                }

                foreach (Vector3 v3 in m_fireTileWorldPos)
                {
                    List<float> distanceToCentriods = new List<float>();
                    for (int i = 0; i < m_clusters; i++)
                    {
                        distanceToCentriods.Add(DistanceBetween(v3, m_centroidsWorldPos[i]));
                    }
                    int closest = distanceToCentriods.IndexOf(distanceToCentriods.Min());
                    m_clusteredTiles[closest].Add(v3);
                }
            }

            public bool KClusteringUpdate()
            {
                List<Vector3> newCentroids = new List<Vector3>();

                foreach (List<Vector3> cluster in m_clusteredTiles)
                {
                    float sumX = 0.0f;
                    float sumZ = 0.0f;
                    foreach (Vector3 v3 in cluster)
                    {
                        sumX += v3.x;
                        sumZ += v3.z;
                    }
                    sumX /= (float)cluster.Count;
                    sumZ /= (float)cluster.Count;
                    newCentroids.Add(new Vector3(sumX, 0, sumZ));
                }

                for (int i = 0; i < m_clusters; i++)
                {
                    if (DistanceBetween(newCentroids[i], m_centroidsWorldPos[i]) > 1.0f) // that's the threshold
                    {
                        // centroid has moved over the threshold, the clustering algorithm will continue
                        m_centroidsWorldPos = newCentroids;
                        return false;
                    }
                }

                // stabled
                m_centroidsWorldPos = newCentroids;
                return true;
            }

            public float DistanceBetween(Vector3 v1, Vector3 v2)
            {
                return Mathf.Sqrt((v1.x - v2.x) * (v1.x - v2.x) + (v1.z - v2.z) * (v1.z - v2.z));
            }
        }
    }

    public enum TileType
    {
        Flammable,
        Water,
        Fire,
        Obstacle, // this type should not make the helo crash, but may stop the fire
        Barrier,
        BigTree, // when a tile with this tag is on flame, game loses
        Land
    }

    [Serializable]
    public class Tile
    {
        // PUBLIC ------------------------------------------------------------------------------------------------------------------------------
        public Tile(Vector3 p_worldPosition, Vector2Int p_gridPosition)
        {
            m_worldPosition = p_worldPosition;
            m_gridPosition = p_gridPosition;
            m_tileType = TileType.Land;
        }

        public TileType GetTileType()
        {
            return m_tileType;
        }

        public void SetTileType(TileType p_tileType)
        {
            m_tileType = p_tileType;
        }

        public Vector3 getWorldPosition()
        {
            return m_worldPosition;
        }

        public void SetPropsOnFire(bool p_setOnFire)
        {
            if (m_controllers == null) return;

            foreach (FireVFX_Controller props in m_controllers)
            {
                if (p_setOnFire)
                {
                    props.StartFire();
                }
                else
                {
                    props.StopFire();
                }
            }
        }

        public bool IsFlammable()
        {
            //        public enum TileType
            //{
            //    Land,
            //    SeaWater,
            //    FreshWater,
            //    Fire,
            //    Obstacle, // this type should not make the helo crash, but may stop the fire
            //    Barrier,
            //    BigTree // when a tile with this tag is on flame, game loses
            //}
            switch (m_tileType)
            {
                case TileType.Flammable:
                    if (m_wetUntilTimer < Time.time)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case TileType.BigTree:
                    return true;
                default:
                    return false;
            }
        }

        public void SetFireLevel(int p_fireLevel)
        {
            // placeholder function to handle the fire level effects.
            SetTileType((TileType)p_fireLevel);
            return;
        }

        public Vector2Int GetGridPosition()
        {
            return m_gridPosition;
        }

        public void AddProps(FireVFX_Controller p_props)
        {
            if (m_controllers == null) m_controllers = new List<FireVFX_Controller>();

            m_controllers.Add(p_props);
        }

        public void SetWetTimer(float p_wetTimer)
        {
            m_wetUntilTimer = p_wetTimer;
        }

        // PRIVATE -----------------------------------------------------------------------------------------------------------------------------
        [SerializeField, HideInInspector] private TileType m_tileType;
        [SerializeField, HideInInspector] private Vector3 m_worldPosition;
        [SerializeField, HideInInspector] private Vector2Int m_gridPosition;
        [SerializeField, HideInInspector] private float m_wetUntilTimer = 0.0f;

        // Most tiles will have eight neighbors; use fixed size of List<> to avoid re-allocation in run time
        //[SerializeField, HideInInspector] private List<int> m_neighborList = new List<int>(8);
        [SerializeField, HideInInspector] private List<FireVFX_Controller> m_controllers;
    }

    public class FireGroup
    {
        private Vector3 m_centerOfGroup = Vector3.zero;
        private HashSet<int> m_fireTileIndex = new HashSet<int>();
        private bool m_delete = false;

        public void SetCenter(Vector3 p_centerOfGroup)
        {
            m_centerOfGroup = p_centerOfGroup;
        }

        public Vector3 GetCenter()
        {
            return m_centerOfGroup;
        }

        public void SetHashSet(HashSet<int> p_hashSet)
        {
            m_fireTileIndex = p_hashSet;
        }

        public HashSet<int> GetHashSet()
        {
            return m_fireTileIndex;
        }

        public void MarkDelete()
        {
            m_delete = true;
        }

        public bool CanBeDeleted()
        {
            return m_delete;
        }
    }
}