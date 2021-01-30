using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {
    [SerializeField] float rotationSpeed = 5f;
    [Range(-1, 1), SerializeField] int rotationDirection = 1;

    void Update() {
        transform.Rotate(Vector3.forward * rotationSpeed * -rotationDirection * Time.deltaTime);
    }
}
