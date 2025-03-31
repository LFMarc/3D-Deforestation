using UnityEngine;
using DG.Tweening;
using System;

namespace Deforestation.Audio
{
    public class AudioController : MonoBehaviour
    {
        public static AudioController Instance { get; private set; } // Singleton

        const float MAX_VOLUME = 0.1f;

        #region Fields
        [Header("FX")]
        [SerializeField] private AudioSource _steps;
        [SerializeField] private AudioSource _machineOn;
        [SerializeField] private AudioSource _machineOff;
        [SerializeField] private AudioSource _shoot;

        [Space(10)]

        [Header("Music")]
        [SerializeField] private AudioSource _musicMachine;
        [SerializeField] private AudioSource _musicHuman;
        [SerializeField] private AudioClip _winMusic;

        [Space(10)]

        [Header("Tutorial Voice Lines")]
        [SerializeField] private AudioSource _voiceSource; // Fuente de audio para las voces
        [SerializeField] private AudioClip[] _tutorialVoiceLines; // Array con las líneas de voz

        private bool[] _voicePlayed; // Controla qué líneas de voz ya se reprodujeron
        #endregion

        #region Unity Callbacks	
        private void Awake()
        {
            // Configuración del Singleton
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject); // Evita duplicados

            _voicePlayed = new bool[_tutorialVoiceLines.Length]; // Inicializa el control de voz
        }

        private void Start()
        {
            _musicHuman.Play();

            // Reproducir la PRIMERA línea de voz al iniciar el juego
            PlayTutorialVoice(0);
        }
        #endregion

        #region Public Methods
        public void PlayTutorialVoice(int index)
        {
            if (index >= 0 && index < _tutorialVoiceLines.Length && !_voicePlayed[index] && _tutorialVoiceLines[index] != null)
            {
                _voiceSource.PlayOneShot(_tutorialVoiceLines[index]);
                _voicePlayed[index] = true; // Marcar la línea como reproducida
            }
        }

        public void PlayWinMusic()
        {
            // Detener cualquier otra música
            _musicHuman.Stop();
            _musicMachine.Stop();

            if (_winMusic != null)
            {
                // Crear un nuevo AudioSource temporal para reproducir la música de victoria
                AudioSource winSource = gameObject.AddComponent<AudioSource>();
                winSource.clip = _winMusic;
                winSource.loop = false; // Si quieres que la música solo suene una vez
                winSource.volume = MAX_VOLUME;
                winSource.Play();

                // Opcional: destruir el AudioSource cuando termine la canción
                Destroy(winSource, _winMusic.length);
            }
        }

        #endregion
    }
}
