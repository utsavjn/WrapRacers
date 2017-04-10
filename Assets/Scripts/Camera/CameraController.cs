using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CameraController : MonoBehaviour {


    // Sets the camera to follow a target by Unity's parenting system
    [SerializeField]
    public Transform Target
    {
        set { transform.SetParent(value); }
    }

    //public Vector3 distanceFromPlayer = new Vector3(0, 80, -150);
}
