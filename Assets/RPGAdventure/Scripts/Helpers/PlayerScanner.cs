using RPGAdventure;
using UnityEngine;

[System.Serializable]
public class PlayerScanner
{
    public float meleeDetectionRadius = 2;
    public float detectionRadius = 10;
    public float detectionAngle = 90;
    public PlayerController FindTarget(Transform target)
    {
        if (PlayerController.Instance == null) { return null; }

        Vector3 toPlayer = PlayerController.Instance.transform.position - target.position;
        toPlayer.y = 0;

        if (toPlayer.magnitude <= detectionRadius)
        {
            if ((Vector3.Dot(toPlayer.normalized, target.forward) > Mathf.Cos(detectionAngle * 0.5f * Mathf.Deg2Rad)) || toPlayer.magnitude <= meleeDetectionRadius)
            {
                return PlayerController.Instance;
            }
        }
        return null;
    }
}
