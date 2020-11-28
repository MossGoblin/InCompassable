using UnityEngine;
using Random = UnityEngine.Random;

public class SquircleRadar : MonoBehaviour
{
    public LayerMask targetLayer;

    public float GetRadius(Vector3 direction3d)
    {
        float distance = 0;
        // 3d to 2d
        Vector3 direction = new Vector3(direction3d.x, 0, direction3d.z) *100;

        Debug.Log($"origin: {transform.position.x}/{transform.position.z}");
        Debug.Log($"dir: {direction.x}/{direction.y}/{direction.z}");
        // raycast from center in direction

        RaycastHit hit;
        bool gotHit = Physics.Raycast(transform.position, direction, out hit, 100f, targetLayer);
        Debug.DrawRay(transform.position, direction * 100, Color.red, 100f);

        if (gotHit)
        {
            Debug.Log($"hit: {hit.collider.name}");
            distance = Vector3.Distance(transform.position, hit.transform.position);
        }

        return distance;
    }

    void Start()
    {
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Debug.Log("Hit space");
            GetRadius(new Vector3(Random.Range(0, 100), 0, Random.Range(0, 100)));
        }
    }
}
