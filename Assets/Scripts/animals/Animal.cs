using System.Collections;
using UnityEngine;
using Lean.Pool;

public enum States {
    Wander,
    Alert,
    Escape,
    Aggressive
}

public abstract class Animal : MonoBehaviour {

    private enum Directions {
        Up,
        Right,
        Down,
        Left
    }

    protected States _state;
    public States State {
        get {
            return _state;
        }
        set {
            if (_state == value)
                return;

            _state = value;
            UpdateAction();
        }
    }

    private Directions direction;
    
    private Rigidbody2D _rb;
    private BoxCollider2D _collider;
    private Animator _animator;

    [SerializeField]
    private PlayerDetection _playerDetectionObject;
    [SerializeField]
    private Transform _alertSprite;
    [SerializeField]
    private Transform _aggressiveSprite;

    [SerializeField]
    protected Sprite[] spriteDirections;

    [SerializeField]
    protected float _health;
    [SerializeField]
    public const float ALERT_TIMER = 0.55f;
    [SerializeField]
    private bool _isAggressive;
    public int spawnWeight;

    protected float minTimeBetweenMove = 1f, maxTimeBetweenMove = 6f;

    protected float CurrentMovementSpeed { get; set; }
    protected float RegularMovementSpeed { get; set; } = 5;
    protected float HostileMovementSpeed { get; set; } = 7.5f;
    protected float RetreatMovementSpeed { get; set; } = 15f;

    protected int moveRadius = 5; // 14x14

    protected const int MAX_DISTANCE_DIFFERENCE = 50;

    protected virtual void Start() {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
        _animator = GetComponent<Animator>();
    }

    private void OnEnable() {
        AnimalControl.Instance.AliveAnimals.Add(this);
        StartCoroutine(MovementRoutine());
        StartCoroutine(_playerDetectionObject.LookForPlayer(PlayerDetection.DETECTION_DELAY));
        State = States.Wander;
        CurrentMovementSpeed = RegularMovementSpeed;
    }


    public void Despawn() {
        AnimalControl.Instance.AliveAnimals.Remove(this);
        LeanPool.Despawn(this.gameObject);
    }

    public IEnumerator MovementRoutine() {
        yield return new WaitForSeconds(Random.Range(minTimeBetweenMove, maxTimeBetweenMove));
        while(State == States.Wander) {
            if (Vector3.Distance(transform.position, Player.Instance.transform.position) > MAX_DISTANCE_DIFFERENCE) {
                Despawn();
                break;
            }
            StartCoroutine(AdvanceToNextPosition(GetNextPosition()));
            yield return new WaitForSeconds(Random.Range(minTimeBetweenMove, maxTimeBetweenMove));
        }
    }

    private IEnumerator AdvanceToNextPosition(Vector3 nextPosition) {
        _animator.SetBool("isMoving", true);
        States state = State;

        float sqrRemainingDistance = (transform.position - nextPosition).sqrMagnitude;
        while(sqrRemainingDistance > float.Epsilon && State == state) {
            Vector3 newPosition = Vector3.MoveTowards(_rb.position, nextPosition, CurrentMovementSpeed * Time.deltaTime);
            _rb.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - nextPosition).sqrMagnitude;
            UpdateAnimation(nextPosition - transform.position);
            yield return null;
        }
        
        ResetAnimation();
    }

    private void SetAnimFloat(Vector2 vector) {
        _animator.SetFloat("moveX", vector.x);
        _animator.SetFloat("moveY", vector.y);
    }

    private void ResetAnimation() {
        _animator.SetFloat("moveX", 0);
        _animator.SetFloat("moveY", 0);
        _animator.SetBool("isMoving", false);
        
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        switch(direction) {
            case Directions.Up:
                renderer.sprite = spriteDirections[0];
                break;
            case Directions.Right:
                renderer.sprite = spriteDirections[1];
                break;
            case Directions.Down:
                renderer.sprite = spriteDirections[2];
                break;
            case Directions.Left:
                renderer.sprite = spriteDirections[3];
                break;
        }
    }

    private void UpdateAnimation(Vector3 direction) {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) {
            if (direction.x > 0) {
                SetAnimFloat(Vector2.right);
                this.direction = Directions.Right;
                SetRotation(270);
            } else {
                SetAnimFloat(Vector2.left);
                this.direction = Directions.Left;
                SetRotation(80);
            }
        } else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y)) {
            if (direction.y > 0) {
                SetAnimFloat(Vector2.up);
                this.direction = Directions.Up;
                SetRotation(0);
            } else {
                SetAnimFloat(Vector2.down);
                this.direction = Directions.Down;
                SetRotation(180);
            }
        }
    }

    protected void UpdateAction() {
        ResetAnimation();
        StopAllCoroutines();
        switch(State) {
            case States.Wander:
                StartCoroutine(MovementRoutine());
                StartCoroutine(_playerDetectionObject.LookForPlayer(PlayerDetection.DETECTION_DELAY));
                break;
            case States.Alert:
                _alertSprite.gameObject.SetActive(true);
                StartCoroutine(Utility.CallMethodAfterTime(Escape, ALERT_TIMER));
                break;
            case States.Escape:
                CurrentMovementSpeed = RetreatMovementSpeed;
                StartCoroutine(AdvanceToNextPosition(Vector2.up * 100));
                UpdateAnimation(Vector2.up);
                break;
            case States.Aggressive:
                Attack();
                break;
        }
    }

    private void Escape() {
        _alertSprite.gameObject.SetActive(false);
        if (_playerDetectionObject.CanSeePlayer()) {
            State = (_isAggressive) ? States.Aggressive : States.Escape;
        } else {
            State = States.Wander;
        }
    }

    private void Attack() {
        _alertSprite.gameObject.SetActive(false);
        _aggressiveSprite.gameObject.SetActive(true);
        CurrentMovementSpeed = HostileMovementSpeed;
        
        StartCoroutine(AdvanceToNextPosition(Player.Instance.transform.position));
    }

    private void SetRotation(int rotation) {
        _playerDetectionObject.transform.eulerAngles = new Vector3(0, 0, rotation);
    }

    protected Vector3 GetNextPosition() {
        return new Vector3(Random.Range(transform.position.x - moveRadius, transform.position.x + moveRadius), 
                            Random.Range(transform.position.y - moveRadius, transform.position.y + moveRadius));
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Tree") || collision.CompareTag("Water")) {
            Despawn();
        }
    }
}
