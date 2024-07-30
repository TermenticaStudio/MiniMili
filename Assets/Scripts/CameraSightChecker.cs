using UnityEngine;

public class CameraSightChecker
{
    public static bool IsObjectInCameraSight(GameObject obj)
    {
        var cam = Camera.main;

        var directionToObject = obj.transform.position - cam.transform.position;

        if (Vector3.Angle(cam.transform.forward, directionToObject) > cam.fieldOfView)
            return false;

        return true;
    }
}