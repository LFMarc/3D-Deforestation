using UnityEngine;
using System.Collections;

namespace Deforestation
{
    [RequireComponent(typeof(HealthSystem))]
    public class Tower : MonoBehaviour
    {
        #region Fields
        private HealthSystem _health;
        [SerializeField] private Camera towerCamera; // Cámara a activar
        private static bool voicePlayed = false; // Control de la tercera línea de voz
        #endregion

        #region Unity Callbacks
        void Awake()
        {
            _health = GetComponent<HealthSystem>();
            _health.OnDeath += DestroyTower;

            if (towerCamera != null)
                towerCamera.gameObject.SetActive(false); // Asegurar que la cámara está inactiva al inicio
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(ActivateTowerCamera());
            }
        }
        #endregion

        #region Private Methods
        private void DestroyTower()
        {
            GameController.Instance.RegisterTowerDestruction();
            Destroy(gameObject);
        }

        private IEnumerator ActivateTowerCamera()
        {
            towerCamera.gameObject.SetActive(true); // Activa la cámara
            towerCamera.transform.LookAt(transform.position); // La orienta hacia la torre

            if (!voicePlayed && Deforestation.Audio.AudioController.Instance != null)
            {
                Deforestation.Audio.AudioController.Instance.PlayTutorialVoice(2); // Reproducir tercera línea
                voicePlayed = true;
            }

            yield return new WaitForSeconds(3); // Espera 3 segundos

            towerCamera.gameObject.SetActive(false); // Desactiva la cámara
        }
        #endregion
    }
}
