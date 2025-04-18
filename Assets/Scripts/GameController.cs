﻿using UnityEngine;
using Deforestation.Machine;
using Deforestation.UI;
using Deforestation.Recolectables;
using Deforestation.Interaction;
using Deforestation.Audio;
using Cinemachine;
using System;

namespace Deforestation
{
	public class GameController : Singleton<GameController>
	{
		#region Properties
		public MachineController MachineController => _machine;
		public Inventory Inventory => _inventory;
		public InteractionSystem InteractionSystem => _interactionSystem;
		public TreeTerrainController TerrainController => _terrainController;
		public Camera MainCamera;

		//Events
		public Action<bool> OnMachineModeChange;

		public bool MachineModeOn
		{
			get
			{
				return _machineModeOn;
			}
			private set
			{
				_machineModeOn = value;
				OnMachineModeChange?.Invoke(_machineModeOn);
			}
		}
		#endregion

		#region Fields
		[Header("Player")]
		[SerializeField] protected CharacterController _player;
		public HealthSystem PlayerHealth;
        [SerializeField] protected Inventory _inventory;
		[SerializeField] protected InteractionSystem _interactionSystem;

		[Header("Camera")]
		[SerializeField] protected CinemachineVirtualCamera _virtualCamera;
		[SerializeField] protected Transform _playerFollow;
		[SerializeField] protected Transform _machineFollow;

		[Header("Machine")]
		[SerializeField] protected MachineController _machine;
		[Header("UI")]
		[SerializeField] protected UIGameController _uiController;
		[Header("Trees Terrain")]
		[SerializeField] protected TreeTerrainController _terrainController;

		private bool _machineModeOn;

        //win condition
        private int towersDestroyed = 0;

        [SerializeField] private Camera winCamera;
        [SerializeField] private GameObject winUI;
        #endregion

        #region Unity Callbacks
        // Start is called before the first frame update
        void Start()
		{
            //UI Update
            PlayerHealth.OnHealthChanged += _uiController.UpdatePlayerHealth;
			_machine.HealthSystem.OnHealthChanged += _uiController.UpdateMachineHealth;
			MachineModeOn = false;
		}

		// Update is called once per frame
		void Update()
		{
		}
		#endregion

		#region Public Methods
		public void TeleportPlayer(Vector3 target)
		{
			_player.enabled = false;
			_player.transform.position = target;
			_player.enabled = true;
		}

        public void DisablePlayerControls()
        {
			_player.gameObject.SetActive(false);

            MachineController machine = FindObjectOfType<MachineController>();
            if (machine != null)
            {
                machine.enabled = false; // Desactiva controles de la máquina
            }
        }

        public void RegisterTowerDestruction()
        {
            towersDestroyed++;
            Debug.Log("Torres destruidas: " + towersDestroyed);

            if (towersDestroyed >= 5)
            {
                ActivateWinState();
            }
        }

        internal void MachineMode(bool machineMode)
		{
			MachineModeOn = machineMode;
			//Player
			_player.gameObject.SetActive(!machineMode);
			_player.enabled = !machineMode;

			//Cursor + UI
			if (machineMode)
			{
				//Start Driving
				if (Inventory.HasResource(RecolectableType.HyperCrystal))
					_machine.StartDriving(machineMode);

				_player.transform.parent = _machineFollow;
				_uiController.HideInteraction();
				Cursor.lockState = CursorLockMode.None;
				//Camera
				_virtualCamera.Follow = _machineFollow;

				_machine.enabled = true;
				_machine.WeaponController.enabled = true;
				_machine.GetComponent<MachineMovement>().enabled = true;

			}
			else
			{
				_machine.enabled = false;
				_machine.WeaponController.enabled = false;
				_machine.GetComponent<MachineMovement>().enabled = false;
				_player.transform.parent = null;

				//Camera
				_virtualCamera.Follow = _playerFollow;
				Cursor.lockState = CursorLockMode.Locked;
			}
			Cursor.visible = machineMode;
		}
        #endregion

        #region Private Methods
        private void ActivateWinState()
        {
            Debug.Log("¡Victoria! Se han destruido todas las torres.");

            if (winCamera != null)
                winCamera.gameObject.SetActive(true);

            if (winUI != null)
                winUI.SetActive(true);

            // 🔊 Activamos la música de victoria a través del AudioController
            if (AudioController.Instance != null)
            {
                AudioController.Instance.PlayWinMusic();
            }

            DisablePlayerControls();
        }
        #endregion
    }

}