using UnityEngine;
using Grid;
using System.Collections.Generic;

public class FireVolumeControl : MonoBehaviour
{
    private AudioSource fireSource;
    [SerializeField] private GridManager gridManager;

    [SerializeField] private float audioRadius = 45.0f;

    [SerializeField] private int maxTrees = 10;

    [SerializeField] private float minVolume = .3f, maxVolume = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridManager = GridManager.GetInstance();
        fireSource = SoundManager.GetFireSource();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, audioRadius);
    }

    // Update is called once per frame
    void Update()
    {
        int foundTrees = 0;
        List<Tile> foundTiles = gridManager.TryGetTileInRange(transform.position, audioRadius);
        for (int i = 0; i < foundTiles.Count; i++)
        {
            if (foundTiles[i].GetTileType() == TileType.Fire)
            {
                Debug.DrawLine(transform.position, foundTiles[i].getWorldPosition(), Color.red);
                foundTrees++;
                if (foundTrees >= maxTrees)
                    break;
            }
        }

        float volume = 0f;
        if (foundTrees >= 1)
        {
            float foundTreesVolumeLerp = Mathf.InverseLerp(1, maxTrees, foundTrees);
            volume = Mathf.Lerp(minVolume, maxVolume, foundTreesVolumeLerp);
            
        }
        else if (fireSource.volume > 0f)
        {
            volume = 0f;
        }

        fireSource.volume = Mathf.Lerp(fireSource.volume, volume, 1f - Mathf.Exp(-1f * Time.deltaTime));
    }
}
