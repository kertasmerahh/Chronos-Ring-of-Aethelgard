using UnityEngine;
public class WorldRotator : MonoBehaviour
{
    [Header("Kecepatan Putar")]

    public float rotationSpeed = 10f;

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}