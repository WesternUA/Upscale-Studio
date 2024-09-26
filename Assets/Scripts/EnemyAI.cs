using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public EnemyState currentState;
    public Transform[] patrolPoints;
    public float chaseRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1f; // Затримка між атаками
    public int attackDamage = 10; // Сума шкоди, яку завдає ворог
    public Transform player;
    public DamageTextSpawner damageTextSpawner; // Додано DamageTextSpawner
    public AudioClip[] FootstepAudioClips; // Масив аудіокліпів для кроків
    public AudioClip attackAudioClip; // Звуковий ефект для атаки
    public AudioClip hitAudioClip; // Звуковий ефект для завдання шкоди
    public float FootstepAudioVolume = 0.5f; // Гучність звуку кроків
    public float audioVolume = 1.0f; // Гучність звукових ефектів атаки та шкоди
    private bool canAttack = true; // Чи може ворог атакувати
    private bool isAttacking = false; // Чи ворог зараз атакує
    private int currentPatrolIndex = 0;
    private NavMeshAgent agent;
    private PlayerHealth playerHealth;
    private AudioSource audioSource; // Джерело звуку

    // Змінні для анімацій
    private Animator _animator;
    private int _animIDSpeed;
    private int _animIDAttack;
    private int _animIDGrounded;
    private int _animIDMotionSpeed;
    private bool _hasAnimator;

    private float _speed;
    private float _animationBlend;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>(); // Отримуємо компонент аніматора
        audioSource = GetComponent<AudioSource>(); // Отримуємо компонент AudioSource
        _hasAnimator = _animator != null;

        if (_animator == null)
        {
            Debug.LogError("Animator component not found on the enemy.");
        }

        AssignAnimationIDs(); // Присвоюємо ID для анімацій

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
                playerHealth = playerObject.GetComponent<PlayerHealth>();
            }
            else
            {
                Debug.LogError("Player object with tag 'Player' not found.");
            }
        }
        else
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }

        currentState = EnemyState.Patrolling;
        GoToNextPatrolPoint();
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patrol();
                break;
            case EnemyState.Chasing:
                Chase();
                break;
            case EnemyState.Attacking:
                Attack();
                break;
        }

        CheckTransitions();
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed"); // Використовується для Blend Tree
        _animIDAttack = Animator.StringToHash("Attack");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPatrolPoint();
        }

        UpdateMovementAnimation(agent.velocity.magnitude);
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length > 0)
        {
            agent.destination = patrolPoints[currentPatrolIndex].position;
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    void Chase()
    {
        if (player != null && !isAttacking) // Переслідуємо, якщо не атакуємо
        {
            agent.isStopped = false;
            agent.destination = player.position;
        }

        UpdateMovementAnimation(agent.velocity.magnitude);
    }

    void Attack()
    {
        if (canAttack && playerHealth != null)
        {
            // Зупиняємо ворога перед атакою і перевіряємо відстань перед атакою
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= attackRange)
            {
                agent.isStopped = true; // Зупиняємо ворога під час атаки
                isAttacking = true; // Вказуємо, що ворог атакує

                // Обертаємо ворога в напрямку гравця перед атакою
                LookAtPlayer();

                Debug.Log("Attack the player");

                _animator.SetTrigger(_animIDAttack); // Запускаємо анімацію атаки
                //PlayAttackSound(); // Програємо звуковий ефект атаки
                Debug.Log("Attack animation triggered");

                // Після атаки починається затримка
                StartCoroutine(AttackCooldown());
            }
        }
    }

    // Метод для обертання ворога до гравця
    private void LookAtPlayer()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0; // Ігноруємо вісь Y для горизонтального обертання
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f); // Плавне обертання
        }
    }

    // Оновлюємо анімацію руху в залежності від швидкості
    void UpdateMovementAnimation(float speed)
    {
        _speed = speed;
        _animationBlend = Mathf.Lerp(_animationBlend, _speed, Time.deltaTime * 10.0f);

        // Оновлюємо параметри аніматора
        if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, _speed > 0 ? 1 : 0);
        }
    }

    // Корутин для затримки між атаками
    private System.Collections.IEnumerator AttackCooldown()
    {
        canAttack = false; // Забороняємо атаку
        yield return new WaitForSeconds(attackCooldown); // Затримка 1 секунда
        canAttack = true; // Дозволяємо атакувати знову
        isAttacking = false; // Вказуємо, що атака завершена
        agent.isStopped = false; // Продовжуємо рух після атаки
    }

    void CheckTransitions()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Patrolling:
                if (distanceToPlayer < chaseRange)
                {
                    currentState = EnemyState.Chasing;
                }
                break;
            case EnemyState.Chasing:
                if (distanceToPlayer < attackRange && !isAttacking) // Перехід до атаки, якщо не атакуємо
                {
                    currentState = EnemyState.Attacking;
                }
                else if (distanceToPlayer > chaseRange)
                {
                    currentState = EnemyState.Patrolling;
                    GoToNextPatrolPoint();
                }
                break;
            case EnemyState.Attacking:
                if (!isAttacking && distanceToPlayer > attackRange) // Якщо атака завершена і гравець поза зоною атаки
                {
                    currentState = EnemyState.Chasing;
                    agent.isStopped = false;
                }
                break;
        }
    }

    // Метод, який спрацьовує при Animation Event 'OnFootstep'
    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f && FootstepAudioClips.Length > 0)
        {
            int index = Random.Range(0, FootstepAudioClips.Length); // Вибираємо випадковий звук кроку
            AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.position, FootstepAudioVolume); // Програємо звук кроку
        }
    }

    // Метод, який спрацьовує при Animation Event 'ApplyDamage'
    private void ApplyDamage()
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage); // Наносимо шкоду гравцеві
            //PlayHitSound(); // Програємо звуковий ефект нанесення шкоди

            // Спавнимо текст дамага
            if (damageTextSpawner != null)
            {
                damageTextSpawner.Spawn(attackDamage);
            }
            else
            {
                Debug.LogError("DamageTextSpawner is not assigned.");
            }
        }
    }

    public void PlayAttackSound()
    {
        if (audioSource != null && attackAudioClip != null)
        {
            audioSource.PlayOneShot(attackAudioClip, audioVolume); // Програємо звук атаки
        }
    }

    private void PlayHitSound()
    {
        if (audioSource != null && hitAudioClip != null)
        {
            audioSource.PlayOneShot(hitAudioClip, audioVolume); // Програємо звук при нанесенні шкоди
        }
    }
}
