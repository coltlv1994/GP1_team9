using UnityEngine;

public class BucketScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("FreshWater"))
        {
            print("GetWaterInBucket");
        }
    }
}
