//This script is attached only on some models that do not perfectly fit.
//The character customization script will check if the objects have this script attached to them and it will move them accordingly.
using UnityEngine;
using System.Collections;

public class MeshOffset : MonoBehaviour {
    public Vector3 posOffset;  //Insert here the offset that i want the model to move
    public Vector3 rotationOffset;
    public Vector3 scaleOffset;
}
