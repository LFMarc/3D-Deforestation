using UnityEngine;
using TMPro;
using Deforestation.Recolectables;
using System;
using Deforestation.Interaction;
using UnityEngine.UI;
using UnityEngine.Audio;

namespace Deforestation.UI
{
    public class UIGameController : MonoBehaviour
    {
        #region Fields
        private Inventory _inventory => GameController.Instance.Inventory;
        private InteractionSystem _interactionSystem => GameController.Instance.InteractionSystem;

        [Header("Settings")]
        [SerializeField] private AudioMixer _mixer;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private GameObject _settingsPanel;
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _fxSlider;

        [Header("Inventory")]
        [SerializeField] private TextMeshProUGUI _crystal1Text;
        [SerializeField] private TextMeshProUGUI _crystal2Text;
        [SerializeField] private TextMeshProUGUI _crystal3Text;

        [Header("Interaction")]
        [SerializeField] private InteractionPanel _interactionPanel;

        [Header("Health")]
        [SerializeField] private Slider _machineSlider;
        [SerializeField] private Slider _playerSlider;

        [SerializeField] private GameObject _defeatPanel;
        [SerializeField] private GameObject _defeatCamera;

        private bool _settingsOn = false;
        #endregion

        #region Unity Callbacks
        // Start is called before the first frame update
        void Start()
        {
            _settingsPanel.SetActive(false);

            // Suscribirse a los eventos del inventario y la interacción
            _inventory.OnInventoryUpdated += UpdateUIInventory;
            _interactionSystem.OnShowInteraction += ShowInteraction;
            _interactionSystem.OnHideInteraction += HideInteraction;

            // Suscribirse al evento de cambio de vida del jugador
            HealthSystem playerHealth = GameController.Instance.PlayerHealth;
            if (playerHealth != null)
            {
                playerHealth.OnHealthChanged += UpdatePlayerHealth;
            }
            else
            {
                Debug.LogError("UIGameController: No se encontró el sistema de salud del jugador.");
            }

            // Configurar eventos de configuración de sonido
            _settingsButton.onClick.AddListener(SwitchSettings);
            _musicSlider.onValueChanged.AddListener(MusicVolumeChange);
            _fxSlider.onValueChanged.AddListener(FXVolumeChange);
        }

        private void SwitchSettings()
        {
            _settingsOn = !_settingsOn;
            _settingsPanel.SetActive(_settingsOn);
        }

        internal void UpdatePlayerHealth(float value)
        {
            _playerSlider.value = value;

            if (value <= 0)
            {
                ActivateDefeatScreen();
            }
        }

        internal void UpdateMachineHealth(float value)
        {
            _machineSlider.value = value;

            if (value <= 0)
            {
                ActivateDefeatScreen();
            }
        }

        private void ActivateDefeatScreen()
        {
            _defeatPanel.SetActive(true);  // Activar UI de derrota
            _defeatCamera.SetActive(true); // Activar cámara especial

            // Opcional: Desactivar el HUD o controles
            GameController.Instance.DisablePlayerControls();
        }
        #endregion

        #region Public Methods
        public void ShowInteraction(string message)
        {
            _interactionPanel.Show(message);
        }

        public void HideInteraction()
        {
            _interactionPanel.Hide();
        }
        #endregion

        #region Private Methods
        private void UpdateUIInventory()
        {
            if (_inventory.InventoryStack.ContainsKey(RecolectableType.SuperCrystal))
                _crystal1Text.text = _inventory.InventoryStack[RecolectableType.SuperCrystal].ToString();
            else
                _crystal1Text.text = "0";

            if (_inventory.InventoryStack.ContainsKey(RecolectableType.HyperCrystal))
                _crystal2Text.text = _inventory.InventoryStack[RecolectableType.HyperCrystal].ToString();
            else
                _crystal2Text.text = "0";

            if (_inventory.InventoryStack.ContainsKey(RecolectableType.SparklingCrystal))
                _crystal3Text.text = _inventory.InventoryStack[RecolectableType.SparklingCrystal].ToString();
            else
                _crystal3Text.text = "0";
        }

        private void FXVolumeChange(float value)
        {
            _mixer.SetFloat("FXVolume", Mathf.Lerp(-60f, 0f, value));
        }

        private void MusicVolumeChange(float value)
        {
            _mixer.SetFloat("MusicVolume", Mathf.Lerp(-60f, 0f, value));
        }
        #endregion
    }
}
