using Deforestation.Dinosaurus;
using Deforestation.Recolectables;
using UnityEngine;

namespace Deforestation.Machine
{
    public class MachineMovement : MonoBehaviour
    {
        #region Fields
        [SerializeField] private float _speedForce = 50;
        [SerializeField] private float _speedRotation = 15;
        [SerializeField] private float _jumpForce = 15f;

        private Rigidbody _rb;
        private Vector3 _movementDirection;
        private Inventory _inventory => GameController.Instance.Inventory;

        [Header("Energy")]
        [SerializeField] private float energyDecayRate = 20f;
        private float energyTimer = 0f;
        #endregion

        #region Unity Callbacks	
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (_inventory.HasResource(RecolectableType.HyperCrystal))
            {
                // Movimiento
                _movementDirection = new Vector3(Input.GetAxis("Vertical"), 0, 0);
                transform.Rotate(Vector3.up * _speedRotation * Time.deltaTime * Input.GetAxis("Horizontal"));

                // Energía por movimiento
                if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
                {
                    energyTimer += Time.deltaTime;
                    if (energyTimer >= energyDecayRate)
                        _inventory.UseResource(RecolectableType.HyperCrystal);
                }

                // ?? Verificar y consumir SparklingCrystal para saltar
                if (Input.GetKeyDown(KeyCode.Space) && _inventory.HasResource(RecolectableType.SparklingCrystal))
                {
                    Jump();
                    _inventory.UseResource(RecolectableType.SparklingCrystal); // Consumir un recurso
                }
            }
            else
            {
                GameController.Instance.MachineController.StopMoving();
            }
        }

        private void FixedUpdate()
        {
            _rb.AddRelativeForce(_movementDirection.normalized * _speedForce, ForceMode.Impulse);
        }

        private void Jump()
        {
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Tree"))
            {
                int index = other.GetComponent<Tree>().Index;
                GameController.Instance.TerrainController.DestroyTree(index, other.transform.position);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Comprobar si el objeto con el que colisionó tiene un HealthSystem.
            Deforestation.HealthSystem healthSystem = collision.gameObject.GetComponent<Deforestation.HealthSystem>();

            if (healthSystem != null)
            {
                // Verificar si es el jugador
                if (!collision.gameObject.CompareTag("Player"))
                {
                    // Si no es el jugador, aplicamos daño
                    healthSystem.TakeDamage(10);  // 'damageAmount' es el valor que deseas de daño.
                }
            }
        }



    }
}
#endregion