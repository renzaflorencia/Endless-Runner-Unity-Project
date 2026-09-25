using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerControl : MonoBehaviour 
{
    private CharacterController _characterController;
    private Vector3 _direction;
    public float ForwardSpeed;
    public float MaxSpeed;

    private int _desiredLane = 1; // 0: Kiri, 1: Tengah, 2: Kanan
    public float LaneDistance;
    public float LaneChangeSpeed;

    private Vector3 _targetPosition;
    private Vector3 _initialPosition;

    public float JumpForce;
    public float MaxJumpForce;

    public float Gravity = -10;
    private bool isJumping = false;

    private Animator _animator; 

    public float NormalHeight;
    public float SlideHeight;
    private float _originalHeight;
    private float _originalCenterY;

    private bool isSliding = false;

    // Distance
    public TextMeshProUGUI distanceTextGO; // UI Text untuk menampilkan jarak
    public TextMeshProUGUI distanceTextWin;
    private Vector3 lastPosition; // Posisi terakhir pemain
    private float distanceTraveled; // Jarak yang sudah ditempuh

    public HealthManager HealthManager;
    public ScoreSystem ScoreSystem;
    public PlayerManager PlayerManager;

    void Start()
    {

        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _initialPosition = transform.position;
        _originalHeight = _characterController.height;
        _originalCenterY = _characterController.center.y;

        HealthManager = FindObjectOfType<HealthManager>();
        PlayerManager = FindObjectOfType<PlayerManager>();

        lastPosition = transform.position;
        distanceTraveled = 0f;
    }

    void Update()
    {
        Debug.DrawRay(transform.position, Vector3.down * 0.1f, Color.red); ; // Untuk debuggung saja
        Debug.Log("isGrounded: " + _characterController.isGrounded); // Untuk debugging saja

        //Periksa status permainan
        if (!PlayerManager.IsGameStarted || PlayerManager.GameOver)
        {
            HandleGameNotStartedOrOver();
            return;
        }

        _animator.SetBool("isStarted", true);
        IncreaseSpeed();
        _direction.z = ForwardSpeed;
        _direction.y += Gravity * Time.deltaTime;

        if (_characterController.isGrounded && isJumping)
        {
            isJumping = false;
            ReturnToRunningAnimation();
        }

        HandleKeyboardInput();
        UpdateLanePosition(); 

        //Menghitung jarak yang ditempuh
        float distanceThisFrame = Vector3.Distance(lastPosition, transform.position);
        distanceTraveled += distanceThisFrame;
        lastPosition = transform.position;

        //Menampillkan jarak di UI
        UpdateDistanceUI();
    }

    private void FixedUpdate()
    {
        if(PlayerManager.IsGameStarted && !PlayerManager.GameOver)
        {
            Vector3 currentPosition = transform.position;
            Vector3 nextPosition = Vector3.Lerp(currentPosition, _targetPosition, LaneChangeSpeed * Time.deltaTime);
            Vector3 moveDirection = nextPosition - currentPosition;

            _characterController.Move(moveDirection + _direction * Time.deltaTime);
        }
    }

    // Menangani input keyboard
    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            OnSwipeLeft();
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            OnSwipeRight();
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            OnSwipeUp();
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            OnSwipeDown();
    }

    private void HandleGameNotStartedOrOver()
    {
        _direction.z = 0; // Hentikan pergerakan

        // Mulai permainan dengan Enter key
        if (Input.GetKeyDown(KeyCode.Return) && !PlayerManager.IsGameStarted)
        {
            PlayerManager.IsGameStarted = true;
            Destroy(PlayerManager.startingText);
        }
    }

    private void IncreaseSpeed()
    {
        if (ForwardSpeed < MaxSpeed)
            ForwardSpeed += 0.1f * Time.deltaTime;

        if (JumpForce < MaxJumpForce)
            JumpForce += 0.01f * Time.deltaTime;
    }

    private void UpdateLanePosition()
    {
        // Menghitung posisi target berdasarkan lane yang diinginkan
        Vector3 lanePosition = new Vector3((_desiredLane - 1) * LaneDistance, transform.position.y, transform.position.z);
        _targetPosition = Vector3.Lerp(transform.position, lanePosition, Time.deltaTime * LaneChangeSpeed);
    }

    private void UpdateDistanceUI()
    {
        distanceTextGO.text = $"Distance: {distanceTraveled:F2} m";
        distanceTextWin.text = $"Distance: {distanceTraveled:F2} m";
    }

    void OnSwipeLeft()
    {
        if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
        {
            _desiredLane = Mathf.Max(0, _desiredLane - 1);
        }
    }

    void OnSwipeRight()
    {
        if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
        {
            _desiredLane = Mathf.Min(2, _desiredLane + 1);
        }
    }

    void OnSwipeUp()
    {
        if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
            Jump();
    }

    void OnSwipeDown()
    {
        if (PlayerManager.IsGameStarted && !PlayerManager.GameOver && !isSliding)
        {
            _animator.SetTrigger("Slide");
            StartCoroutine(SlideCoroutine());
        }
    }

    private void Jump()
    {
        if (_characterController.isGrounded)
        {
            _direction.y = JumpForce;
            isJumping = true;
            _animator.SetTrigger("Jump");
        }
    }

    private void ReturnToRunningAnimation()
    {
        _targetPosition.z += transform.position.z - _initialPosition.z;
    }

    private IEnumerator SlideCoroutine()
    {
        isSliding = true;
        _characterController.height = SlideHeight;
        _characterController.center = new Vector3(_characterController.center.x, 0.4f, _characterController.center.z);
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length); 
        _characterController.height = _originalHeight;
        _characterController.center = new Vector3(_characterController.center.x, _originalCenterY, _characterController.center.z);
        isSliding = false;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.CompareTag("Obstacle"))
        {
            if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
            {
                HealthManager.ObstacleHit();
                ScoreSystem.ObstacleHit();
                _animator.SetTrigger("Stumble");
                Destroy(hit.gameObject);
            }
        }
    }
}