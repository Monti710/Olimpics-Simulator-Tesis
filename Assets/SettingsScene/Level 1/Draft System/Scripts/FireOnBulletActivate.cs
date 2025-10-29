using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using TMPro;

public class FireBulletOnActivate : MonoBehaviour
{
    [Header("Disparo")]
    public GameObject bullet;
    public Transform spawnPoint;
    public float fireSpeed = 20;
    public AudioSource shotSound;
    public float cooldownTime = 0.5f;
    public float hapticDuration = 0.2f;
    public float hapticAmplitude = 0.5f;

    [Header("Control de disparos")]
    public bool useShotLimit = true;
    public int maxShots = 20;
    public TextMeshProUGUI shotsScore; // Texto para mostrar balas restantes

    [Header("Control de tiempo")]
    public bool useTimer = true;
    public float gameDuration = 300f;
    public TextMeshProUGUI timerText; // mostrar tiempo

    [Header("Final del juego")]
    public string nextSceneName = "ScoreScene";
    public float endDelay = 1.0f;

    [Header("Sistema de puntos")]
    public PontCounter pointCounter;

    private int shotsFired = 0;
    private float timeRemaining;
    private bool canShoot = true;
    private bool gameEnded = false;

    void Start()
    {
        timeRemaining = gameDuration;

        XRGrabInteractable grabbable = GetComponent<XRGrabInteractable>();
        grabbable.activated.AddListener(FireBullet);

        if (useTimer && timerText != null)
        {
            UpdateTimerDisplay(); // Mostrar tiempo inicial
        }

        UpdateShotsDisplay(); // Mostrar balas iniciales al iniciar
    }

    void Update()
    {
        if (useTimer && !gameEnded)
        {
            timeRemaining -= Time.deltaTime;

            if (timerText != null)
            {
                UpdateTimerDisplay();
            }

            if (timeRemaining <= 0f)
            {
                StartCoroutine(DelayedEndGame());
            }
        }
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    // Nueva función para actualizar el texto de balas restantes
    private void UpdateShotsDisplay()
    {
        if (shotsScore != null)
        {
            int shotsLeft = maxShots - shotsFired;
            shotsScore.text = $"Balas: {shotsLeft}";
        }
    }

    private void FireBullet(ActivateEventArgs arg)
    {
        if (!canShoot || gameEnded) return;
        if (useShotLimit && shotsFired >= maxShots) return;

        canShoot = false;
        shotsFired++;

        if (shotSound != null) shotSound.Play();

        GameObject spawnedBullet = Instantiate(bullet, spawnPoint.position, Quaternion.identity);
        Rigidbody rb = spawnedBullet.GetComponent<Rigidbody>();
        if (rb != null) rb.linearVelocity = spawnPoint.forward * fireSpeed;
        Destroy(spawnedBullet, 5f);

        if (arg.interactorObject is UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor controllerInteractor)
        {
            controllerInteractor.SendHapticImpulse(hapticAmplitude, hapticDuration);
        }

        UpdateShotsDisplay(); // Actualizar balas restantes al disparar

        StartCoroutine(ResetCooldown());

        if (useShotLimit && shotsFired >= maxShots)
        {
            StartCoroutine(DelayedEndGame());
        }
    }

    private IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(cooldownTime);
        canShoot = true;
    }

    private IEnumerator DelayedEndGame()
    {
        if (gameEnded) yield break;
        gameEnded = true;

        yield return new WaitForSeconds(endDelay);
        EndGame();
    }

    private void EndGame()
    {
        int finalScore = pointCounter != null ? pointCounter.GetTotalPoints() : 0;

        PlayerPrefs.SetInt("FinalScore", finalScore);

        ScoreData newScore = new ScoreData
        {
            playerName = "Jugador1",
            score = finalScore,
            date = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };
        LocalScoreManager.SaveScore(newScore);

        ScoreManager.SaveNewScore(finalScore);

        SceneManager.LoadScene(nextSceneName);
    }
}
