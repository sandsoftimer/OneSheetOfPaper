using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class APExtensions
{
    public static void DestroyAllChild(this Transform transform)
    {
        for (int i = transform.childCount - 1; i > -1; i--)
        {
            GameObject.Destroy(transform.GetChild(i).gameObject);
        }
    }

    public static void GetRandomColor(this Color color)
    {
        color = Random.ColorHSV();
    }

    public static RaycastHit? GetRaycastHitPoint(this Transform t, Vector3 direction, int layerMask)
    {
        RaycastHit hit;
        Physics.Raycast(new Ray(t.position, direction), out hit, Mathf.Infinity, layerMask);
        return hit;
    }

    public static void ActiveChildByIndex(this Transform _t, int childIndex)
    {
        for (int i = 0; i < _t.childCount; i++)
        {
            _t.GetChild(i).gameObject.SetActive(i == childIndex);
        }
    }

    public static float DistanceFrom(this Transform _transform, Transform comparingTransform, APAxis aPAxis = APAxis.ALL)
    {
        return DistanceFrom(_transform.position, comparingTransform.position, aPAxis);
    }

    public static float DistanceFrom(this Transform _transform, Vector3 comparingPosition, APAxis aPAxis = APAxis.ALL)
    {
        return DistanceFrom(_transform.position, comparingPosition, aPAxis);
    }

    public static float DistanceFrom(this Vector3 _transform, Vector3 comparingPosition, APAxis aPAxis = APAxis.ALL)
    {
        float distance = Mathf.Infinity;
        switch (aPAxis)
        {
            case APAxis.ALL:
                distance = Vector3.Distance(_transform, comparingPosition);
                break;
            case APAxis.X:
                distance = Mathf.Abs(_transform.x - comparingPosition.x);
                break;
            case APAxis.Y:
                distance = Mathf.Abs(_transform.y - comparingPosition.y);
                break;
            case APAxis.Z:
                distance = Mathf.Abs(_transform.z - comparingPosition.z);
                break;
        }
        return distance;
    }

    public static Vector3 ModifyThisVector(this Vector3 value, float x, float y, float z)
    {
        return new Vector3(value.x + x, value.y + y, value.z + z);
    }

    public static Vector3 ModifyThisVector(this Vector3 value, Vector3 vector)
    {
        return value.ModifyThisVector(vector.x, vector.y, vector.z);
    }

    public static Transform GetClosestTransform(this Transform t, List<Transform> list)
    {
        List<Transform> transforms = list.OrderBy(i => Vector3.Distance(t.position, i.position)).ToList();
        return transforms.Count > 0? transforms[0] : null;
    }
}
