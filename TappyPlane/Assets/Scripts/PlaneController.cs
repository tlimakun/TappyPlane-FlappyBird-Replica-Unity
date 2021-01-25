using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlaneController : MonoBehaviour
{
    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDead;
    public static event PlayerDelegate OnPlayerScored;

    private GameManager game;

    [SerializeField] private float force = 5f;
    [SerializeField] private float tiltSmooth = 5f;
    [SerializeField] private Vector2 startPosition;
    
    [SerializeField] private Quaternion downRotation = Quaternion.Euler(0f, 0f, -70f);
    [SerializeField] private Quaternion forwardRotation = Quaternion.Euler(0f, 0f, 35f);

    [Header("Audio")]
    [SerializeField] private AudioClip scoreAudio;
    [SerializeField] private AudioClip dieAudio;
    [SerializeField] private AudioClip jumpAudio;

    private Rigidbody2D rb;
    private AudioSource audioSource;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.simulated = false;

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        game = GameManager.instance;
    }

    private void OnEnable()
    {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    private void OnDisable()
    {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    private void OnGameStarted()
    {
        rb.velocity = Vector2.zero;
        rb.simulated = true;
    }

    private void OnGameOverConfirmed()
    {
        transform.position = startPosition;
        transform.rotation = Quaternion.identity;
    }

    private void Update()
    {
        if (game.IsGameOver) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            audioSource.clip = jumpAudio;
            audioSource.Play();
            // Time.timeScale += 0.05f;
            transform.rotation = forwardRotation;
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "DeadZone")
        {
            audioSource.clip = dieAudio;
            audioSource.Play();
            rb.simulated = false;
            OnPlayerDead();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ScoreZone")
        {
            audioSource.clip = scoreAudio;
            audioSource.Play();
            OnPlayerScored();
        }
    }
}
