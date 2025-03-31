using System;
using UnityEngine;
using UnityEngine.AI;

namespace Deforestation.Dinosaurus
{
    public class Raptor : Dinosaur
    {
        #region Fields
        [SerializeField] private float _distanceDetection = 50f;
        [SerializeField] private float _attackDistance = 10f;
        private Transform _playerTransform;

        private bool _chase;
        private bool _attack;

        [SerializeField] private float _attackTime = 2f;
        [SerializeField] private float _attackDamage = 5f;
        private float _attackCooldown;
        #endregion

        #region Unity Callbacks	
        private void Start()
        {
            _attackCooldown = _attackTime;

            // Buscar al jugador por su nombre en la escena
            GameObject player = GameObject.Find("PlayerFPS");
            if (player != null)
            {
                _playerTransform = player.transform;
            }
            else
            {
                Debug.LogError("Raptor: No se encontró el objeto PlayerFPS en la escena.");
            }
        }

        private void Update()
        {
            if (_playerTransform == null) return; // Evita errores si el jugador no fue encontrado

            Vector3 playerPosition = _playerTransform.position;

            // Idle
            if (!_chase && !_attack && Vector3.Distance(transform.position, playerPosition) < _distanceDetection)
            {
                ChasePlayer();
                return;
            }

            // Chase
            if (_chase)
            {
                Vector3 direction = (_playerTransform.position - transform.position).normalized;
                direction.y = 0; // Evita que mire hacia arriba o abajo

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                Quaternion offsetRotation = Quaternion.Euler(0, 180, 0); // Rotación de 180° en Y
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation * offsetRotation, Time.deltaTime * 5f);
            }


            if (_chase && Vector3.Distance(transform.position, playerPosition) < _attackDistance)
            {
                Attack();
                return;
            }
            if (_chase && Vector3.Distance(transform.position, playerPosition) > _distanceDetection)
            {
                Idle();
                return;
            }

            // Attack
            if (_attack)
            {
                _attackCooldown -= Time.deltaTime;
                if (_attackCooldown <= 0)
                {
                    _attackCooldown = _attackTime;

                    HealthSystem playerHealth = _playerTransform.GetComponent<HealthSystem>();
                    if (playerHealth != null)
                    {
                        Debug.Log("Raptor ataca al jugador!");
                        playerHealth.TakeDamage(_attackDamage);
                    }
                    else
                    {
                        Debug.LogWarning("Raptor: No se encontró el HealthSystem en el jugador.");
                    }
                }
            }
            if (_attack && Vector3.Distance(transform.position, playerPosition) > _attackDistance)
            {
                ChasePlayer();
                return;
            }
        }
        #endregion

        #region Private Methods
        private void Idle()
        {
            _anim.SetBool("Run", false);
            _anim.SetBool("Attack", false);
            _chase = false;
            _attack = false;
            _agent.isStopped = true;
        }

        private void ChasePlayer()
        {
            if (_playerTransform == null) return;

            _anim.SetBool("Run", true);
            _anim.SetBool("Attack", false);
            _agent.SetDestination(_playerTransform.position);
            _chase = true;
            _attack = false;
        }

        private void Attack()
        {
            _anim.SetBool("Run", false);
            _anim.SetBool("Attack", true);
            _agent.isStopped = true;

            RotateTowardsPlayer(); // Asegurar que mira en la dirección correcta

            _chase = false;
            _attack = true;
        }

        private void RotateTowardsPlayer()
        {
            Vector3 direction = (_playerTransform.position - transform.position).normalized;
            direction.y = 0; // Mantener solo la rotación en el plano horizontal

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion offsetRotation = Quaternion.Euler(0, 180, 0); // Corregir si el modelo está al revés

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation * offsetRotation, Time.deltaTime * 5f);
        }


        #endregion

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _distanceDetection);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackDistance);
        }
    }
}
