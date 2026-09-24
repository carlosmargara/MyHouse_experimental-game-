using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music")]
    [SerializeField] private EventReference menuMusic;
    [SerializeField] private EventReference timbre;

    private EventInstance currentMusic;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
        // Por ahora podemos dejar esto vacío.
        // Cuando tengamos MainMenu lo utilizaremos para iniciar
        // la música correspondiente.
    }


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Por ahora no hacemos nada.
        // Cuando tengamos MainMenu / Gameplay / GameOver
        // podremos manejar aquí las transiciones globales.
    }


    // =========================================================
    // MUSIC
    // =========================================================

    public void PlayMusic(EventReference musicEvent)
    {
        if (currentMusic.isValid())
        {
            currentMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            currentMusic.release();
            currentMusic.clearHandle();
        }

        currentMusic = RuntimeManager.CreateInstance(musicEvent);
        currentMusic.start();
    }


    public void StopMusic()
    {
        if (!currentMusic.isValid())
            return;

        currentMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        currentMusic.release();
        currentMusic.clearHandle();
    }


    // =========================================================
    // ONE SHOTS
    // =========================================================

    public void PlayOneShot(EventReference sound)
    {
        RuntimeManager.PlayOneShot(sound);
    }


    public void PlayOneShot(EventReference sound, Vector3 position)
    {
        RuntimeManager.PlayOneShot(sound, position);
    }


    // =========================================================
    // CLEANUP
    // =========================================================

    private void OnDestroy()
    {
        if (currentMusic.isValid())
        {
            currentMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            currentMusic.release();
            currentMusic.clearHandle();
        }
    }
}
