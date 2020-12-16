using UnityEngine;
using Random = UnityEngine.Random;

public class Radar : MonoBehaviour
{
    public LayerMask targetLayer;

    public float GetDistanceAtAngle(float angle)
    {
        float distance = 0;

        Vector3 directionFromAngle = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));

        // Vector3 directionFromAngle = Quaternion.AngleAxis(angle, Vector2.right).eulerAngles.normalized;
        // Vector3 directionFromAngle = Quaternion.Euler(angle, 0, 0).eulerAngles.normalized;
        Vector3 direction = directionFromAngle;
        // Vector3 direction = new Vector3(directionFromAngle.x, 0, directionFromAngle.z);
        RaycastHit hit;
        bool gotHit = Physics.Raycast(transform.position, direction, out hit, 10f, targetLayer);
        Debug.DrawRay(transform.position, direction * 10, Color.red, 0.05f);

        if (gotHit)
        {
            distance = Vector3.Distance(transform.position, hit.point);
            // Debug.Log($"hit: {hit.point.x}/{hit.point.z}");
            Debug.Log($"dist: {distance}");
        }

        return distance;
    }


    public float GetDistanceAtAngle(Vector3 direction)
    {
        float distance = 0;

        // Vector3 directionFromAngle = Quaternion.AngleAxis(angle, Vector3.right).eulerAngles.normalized;
        // Vector3 direction = new Vector3(directionFromAngle.x, 0, directionFromAngle.z);
        RaycastHit hit;
        bool gotHit = Physics.Raycast(transform.position, direction, out hit, 10f, targetLayer);
        Debug.DrawRay(transform.position, direction * 10, Color.red, 0.05f);

        if (gotHit)
        {
            distance = Vector3.Distance(transform.position, hit.point);
            // Debug.Log($"hit: {hit.point.x}/{hit.point.z}");
            Debug.Log($"dist: {distance}");
        }

        return distance;
    }
}
