using UnityEngine;

public class WaterMovement : MonoBehaviour
{
    public float speed = 0.5f; // Velocidad del movimiento en Z
    public float zMovementAmplitude = 1f; // Cantidad de movimiento en Z
    public float waveHeight = 0.2f; // Altura máxima del oleaje en Y
    public float waveSpeed = 1f; // Velocidad del oleaje en Y
    public float damagePerSecond = 10f; // Daño por segundo al estar en contacto

    public GameObject underwaterEffect; // Objeto que se activará/desactivará

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;

        // Asegurar que el efecto esté desactivado al inicio
        if (underwaterEffect != null)
        {
            underwaterEffect.SetActive(false);
        }
    }

    void Update()
    {
        float zOffset = Mathf.Sin(Time.time * speed) * zMovementAmplitude;
        float yOffset = Mathf.Sin(Time.time * waveSpeed) * waveHeight;

        transform.position = startPosition + new Vector3(0, yOffset, zOffset);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Solo activa Underwater si el objeto tiene la etiqueta "Player"
        if (other.CompareTag("Player") && underwaterEffect != null)
        {
            underwaterEffect.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return; // Solo afecta al jugador

        Debug.Log($"Agua tocando al Player: {other.gameObject.name}");

        Deforestation.HealthSystem healthSystem = other.GetComponent<Deforestation.HealthSystem>();
        if (healthSystem != null)
        {
            Debug.Log("Player está recibiendo daño del agua");
            healthSystem.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        // Solo desactiva Underwater si el objeto que salió tiene la etiqueta "Player"
        if (other.CompareTag("Player") && underwaterEffect != null)
        {
            underwaterEffect.SetActive(false);
        }
    }
}