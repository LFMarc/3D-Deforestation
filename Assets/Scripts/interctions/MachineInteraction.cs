using UnityEngine;
using System;

namespace Deforestation.Interaction
{
    public enum MachineInteractionType
    {
        Door,
        Stairs,
        Machine
    }

    public class MachineInteraction : MonoBehaviour, IInteractable
    {
        #region Fields
        [SerializeField] protected MachineInteractionType _type;
        [SerializeField] protected Transform _target;
        [SerializeField] protected InteractableInfo _interactableInfo;
        #endregion

        #region Public Methods
        public InteractableInfo GetInfo()
        {
            _interactableInfo.Type = _type.ToString();
            return _interactableInfo;
        }

        public virtual void Interact()
        {
            if (_type == MachineInteractionType.Door)
            {
                // Mueve la puerta
                transform.position = _target.position;
            }
            else if (_type == MachineInteractionType.Stairs)
            {
                // Teletransporta al jugador
                GameController.Instance.TeleportPlayer(_target.position);

                // Reproducir la SEGUNDA línea de voz (índice 1)
                if (Deforestation.Audio.AudioController.Instance != null)
                {
                    Deforestation.Audio.AudioController.Instance.PlayTutorialVoice(1);
                }
            }
            else if (_type == MachineInteractionType.Machine)
            {
                GameController.Instance.MachineMode(true);
            }
        }
        #endregion
    }
}
