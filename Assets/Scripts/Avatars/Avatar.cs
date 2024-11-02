using System.Collections;
using System.Collections.Generic;
using Data;
using UI;
using UI.AvatarAreaWindow;
using UnityEngine;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

namespace Avatars
{
    public class Avatar : MonoBehaviour
    {
        [SerializeField] Material shadeMaterial;
        public string avatarName;

        PixelPerfectCamera m_pixelPerfectCamera;
        Camera m_camera;
        Dictionary<AvatarState, AvatarAnimationData[]> m_avatarStates;
        AvatarData m_avatarData;
        SpriteRenderer m_spriteRenderer;

        public int m_currentFrame; //  текущий кадр анимации

        float m_angle;
        float m_leaveTime; //  время до уничтожения аватара
        float m_frameTime; //  время кадра анимации
        float m_idleTime; //  время idle анимации
        float m_flyCurrentSpeed; // Текущая скорость атакованной цели

        float m_accumulatedTime; //  Время накопления до смены кадра анимации. Время анимации устанавливается в кадрах в секунду.

        public float randomSpeed;
        // float m_bottomOffset;

        const float StrikingDistance = 1.0f; //  расстояние удара
        const float FlyDeceleration = 1.5f; // Скорость замедления атакованной цели

        bool m_canDiagonalMovable; //  true = можно перемещаться по диагонали
        bool m_wasAttacked; //  true = был атакован
        bool m_isAttackPermitted; //  true = атака разрешена
        bool m_isPursue; //  true = преследует цель
        bool m_gotHit; //  true = получил в кабину
        bool m_iFree; //  true = можно переиспользовать (не реализовано)

        Avatar m_targetAvatar;

        public AvatarState m_currentState;
        int m_currentStateVariant = 0;
        Rect m_flyArea;

        Vector3 m_randomTargetDirection;
        Vector3 m_flyDirection; // Направление движения атакованной цели
        Vector3 m_lastPosition;
        public AreaOffset areaOffset;

        AppSettingsData m_settings;

        Coroutine m_pursuitCoroutine;

        void Start()
        {
            m_iFree = false;
            m_settings = LocalStorage.GetSettings();
            m_camera = Core.Instance.Camera;
            m_pixelPerfectCamera = Core.Instance.Ppc;
            m_spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
            m_spriteRenderer.sharedMaterial = shadeMaterial;
            m_isAttackPermitted = true;
            m_isPursue = false;
        }

        public SpriteRenderer GetSpriteRenderer() => m_spriteRenderer ?? GetComponentInChildren<SpriteRenderer>();


        public bool HasState(AvatarState state)
        {
            return m_avatarStates.ContainsKey(state);
        }

        public void Init(string avatarName, AvatarData avatarData)
        {
            m_currentFrame = 0;
            randomSpeed = 1.0f;
            m_iFree = false;
            m_avatarData = avatarData;
            m_avatarStates = avatarData.Animations;
            SetState();
            this.avatarName = avatarName;
            m_leaveTime = 5.0f * 30.0f;
        }

        void Update()
        {
            if (m_avatarStates == null) return;
            SetAreaVerticalOffset();
            SetRandomSpeed();
            
            var cameraWorldSize = m_camera.orthographicSize * 2;
            var pixelsPerUnit = m_settings.windowWidth / cameraWorldSize;
            var speed = m_avatarData.Animations[m_currentState][m_currentStateVariant].AvatarSpeed;
            var dt = speed / pixelsPerUnit * Time.deltaTime;
            
            // var deltaTime = Time.deltaTime * m_settings.avatarsSpeed * 10.0f * randomSpeed;

            m_leaveTime += Time.deltaTime;
            if (m_leaveTime > 5 * 60)
            {
                // DestroyAvatar();
                // return;
            }

            if (!m_wasAttacked)
            {
                if (m_currentState != AvatarState.AttackRight && m_currentState != AvatarState.AttackLeft)
                {
                    if (!m_isPursue)
                    {
                        Move(dt);
                        SetState();
                        PlayAnimation();
                    }
                }
            }

            if (m_gotHit)
            {
                Fly();
                if (m_flyCurrentSpeed <= 0.0f)
                {
                    m_wasAttacked = false;
                    m_gotHit = false;
                }
            }

            m_lastPosition = transform.position;
        }

        bool m_randomSpeedEnable;

        void SetRandomSpeed()
        {
            if (m_settings.randomSpeedEnabled == m_randomSpeedEnable) return;
            m_randomSpeedEnable = m_settings.randomSpeedEnabled;
            randomSpeed = m_randomSpeedEnable ? Random.Range(0.5f, 3.0f) : 1.0f;
        }

        void SetAreaVerticalOffset()
        {
            if (m_spriteRenderer?.sprite is null) return;

            var spriteSize = m_spriteRenderer.sprite.bounds.size;
            var pivot = m_spriteRenderer.sprite.pivot;


            if (Mathf.Approximately(pivot.y, 0.0f))
            {
                areaOffset.Top = -spriteSize.y;
                areaOffset.Bottom = 0.0f;
            }
            else if (Mathf.Approximately(pivot.y, 1.0f))
            {
                areaOffset.Top = 0.0f;
                areaOffset.Bottom = spriteSize.y;
            }
            else
            {
                areaOffset.Top = -(1.0f - pivot.y) * spriteSize.y;
                areaOffset.Bottom = pivot.y * spriteSize.y;
            }
        }

        void Fly()
        {
            var deltaTime = Time.deltaTime * 4.0f;
            var size = Core.Instance.WorldSize;
            m_flyArea.position = (Vector2)m_pixelPerfectCamera.transform.position - size * 0.5f;
            m_flyArea.size = size;

            if (m_flyCurrentSpeed > 0)
            {
                transform.position += m_flyDirection * (m_flyCurrentSpeed * deltaTime);

                if (transform.position.x < m_flyArea.xMin)
                {
                    transform.position = new Vector3(m_flyArea.xMin, transform.position.y, transform.position.z);
                    m_flyDirection.x = Mathf.Abs(m_flyDirection.x);
                }
                else if (transform.position.x > m_flyArea.xMax)
                {
                    transform.position = new Vector3(m_flyArea.xMax, transform.position.y, transform.position.z);
                    m_flyDirection.x = -Mathf.Abs(m_flyDirection.x);
                }

                if (transform.position.y < m_flyArea.yMin + areaOffset.Bottom)
                {
                    transform.position = new Vector3(transform.position.x, m_flyArea.yMin + areaOffset.Bottom, transform.position.z);
                    m_flyDirection.y = Mathf.Abs(m_flyDirection.y);
                }
                else if (transform.position.y > m_flyArea.yMax + areaOffset.Top)
                {
                    transform.position = new Vector3(transform.position.x, m_flyArea.yMax + areaOffset.Top, transform.position.z);
                    m_flyDirection.y = -Mathf.Abs(m_flyDirection.y);
                }

                m_flyCurrentSpeed -= FlyDeceleration * Time.deltaTime;
                // if (m_flyCurrentSpeed < 0)
                // {
                //     m_flyCurrentSpeed = 0;
                // }
            }
        }


        void Move(float deltaTime)
        {
            RandomMovement(deltaTime);
        }

        void RandomMovement(float deltaTime)
        {
            m_idleTime -= deltaTime;
            if (m_idleTime > 0.0f) return;

            transform.position = Vector3.MoveTowards(transform.position, m_randomTargetDirection, deltaTime);
            if (Mathf.Abs(transform.position.x - m_randomTargetDirection.x) < 0.00625f)
            {
                if (m_avatarStates.ContainsKey(AvatarState.Idle) && Random.value > 0.75f)
                {
                    m_randomTargetDirection = transform.position;
                    m_idleTime = Random.Range(1.5f, 5.0f);
                    m_currentStateVariant = Random.Range(0, m_avatarStates[AvatarState.Idle].Length);
                }
                else
                {
                    m_randomTargetDirection.x = Random.Range(AvatarArea.Rect.min.x, AvatarArea.Rect.max.x);
                    if (HasState(AvatarState.Left) && m_avatarStates[AvatarState.Left].Length == m_avatarStates[AvatarState.Right].Length)
                    {
                        m_currentStateVariant = Random.Range(0, m_avatarStates[AvatarState.Left].Length);
                    }
                    else
                    {
                        m_currentStateVariant = 0;
                    }
                }

                m_currentFrame = 0;
            }

            if (Mathf.Abs(transform.position.y - m_randomTargetDirection.y) < 0.00625f)
            {
                m_randomTargetDirection.y = Random.Range(AvatarArea.Rect.min.y + areaOffset.Bottom, AvatarArea.Rect.max.y + areaOffset.Top);
            }
        }


        void SetState()
        {
            if (transform.position.x > m_lastPosition.x)
            {
                m_currentState = AvatarState.Right;
                if (m_currentStateVariant >= m_avatarStates[AvatarState.Right].Length) m_currentStateVariant = 0;
                m_spriteRenderer.flipX = m_avatarStates[AvatarState.Right][m_currentStateVariant].FlipX;
            }
            else if (transform.position.x < m_lastPosition.x)
            {
                m_currentState = AvatarState.Left;
                if (m_currentStateVariant >= m_avatarStates[AvatarState.Left].Length) m_currentStateVariant = 0;
                m_spriteRenderer.flipX = m_avatarStates[AvatarState.Left][m_currentStateVariant].FlipX;
            }
            else
            {
                if (m_avatarStates.TryGetValue(AvatarState.Idle, out var idle))
                {
                    m_currentState = AvatarState.Idle;
                    if (m_currentStateVariant >= m_avatarStates[AvatarState.Idle].Length) m_currentStateVariant = 0;
                }
                else
                {
                    m_currentState = AvatarState.Left;
                    if (m_currentStateVariant >= m_avatarStates[AvatarState.Left].Length) m_currentStateVariant = 0;
                }
            }
        }


        bool PlayAnimationOnce()
        {
            if (m_frameTime > 1.0f / 8.0f)
            {
                if (m_currentFrame + 1 == m_avatarStates[m_currentState][m_currentStateVariant].AnimationIndices.Length)
                {
                    return false;
                }

                m_frameTime = 0.0f;
                m_currentFrame++;

                if (m_avatarStates.TryGetValue(m_currentState, out var variants))
                {
                    var spriteIndex = variants[m_currentStateVariant].AnimationIndices[m_currentFrame];
                    var sprite = AvatarsStorage.GetSprites()[spriteIndex];
                    m_spriteRenderer.sprite = sprite;
                }
                else
                {
                    Log.LogMessage($"{avatarName}: не имеет анимации '{m_currentState}' но она вызывается волшебным образом ;) ");
                }

                // var frameId = m_avatarStates[m_currentState][m_currentFrame];
                // var sprite = m_avatarsController.GetSprite(frameId);
                // m_spriteRenderer.sprite = sprite;
            }

            m_frameTime += Time.deltaTime * 1.45f;
            return true;
        }


        void PlayAnimation()
        {
            if (m_avatarStates.TryGetValue(m_currentState, out var variants))
            {
                var avatarAnimationData = variants[m_currentStateVariant];
                var animationFrameTime = 1.0f / avatarAnimationData.AnimationSpeed;
                m_accumulatedTime += Time.deltaTime;

                if (m_accumulatedTime < animationFrameTime)
                {
                    return;
                }

                m_accumulatedTime -= animationFrameTime;
                m_currentFrame++;
                if (m_currentFrame >= avatarAnimationData.AnimationIndices.Length)
                {
                    m_currentFrame = 0;
                }

                var spriteIndex = avatarAnimationData.AnimationIndices[m_currentFrame];
                m_spriteRenderer.sprite = AvatarsStorage.GetSprites()[spriteIndex];
            }


            // if (m_frameTime > 1.0f / 8.0f)
            // {
            //     m_frameTime = 0.0f;
            //     m_currentFrame++;
            //     if (m_currentFrame == m_avatarStates[m_currentState][m_currentStateVariant].AnimationIndices.Length)
            //     {
            //         m_currentFrame = 0;
            //         // m_spriteRenderer.sprite = m_avatarsController.GetSprite(m_avatars[m_currentState][m_currentFrame]);
            //         // return;
            //     }
            //
            //     if (m_avatarStates.TryGetValue(m_currentState, out var variants))
            //     {
            //         var spriteIndex = variants[m_currentStateVariant].AnimationIndices[m_currentFrame];
            //         var sprite = m_avatarsController.GetSprite(spriteIndex);
            //         m_spriteRenderer.sprite = sprite;
            //     }
            //     else
            //     {
            //         Log.LogMessage($"{avatarName}: не имеет анимации '{m_currentState}' но она вызывается волшебным образом ;) ");
            //     }
            // }
            //
            // m_frameTime += Time.deltaTime * 1.45f;
        }


        void DestroyAvatar()
        {
            Log.LogMessage($"TODO: Destroyed avatar: {avatarName}");
        }


        IEnumerator Pursuit()
        {
            var distance = Vector3.Distance(transform.position, m_targetAvatar.transform.position);
            var direction = Vector3.zero;
            //  движемся к цели.
            m_isPursue = true;
            while (distance > StrikingDistance)
            {
                if (!HasState(AvatarState.AttackLeft))
                {
                    m_isAttackPermitted = true;
                    m_isPursue = false;
                    yield break;
                }

                distance = Vector3.Distance(transform.position, m_targetAvatar.transform.position);
                var deltaTime = 1.0f / m_pixelPerfectCamera.assetsPPU * 60.0f * Time.deltaTime * 2.0f;
                direction = (m_targetAvatar.transform.position - transform.position).normalized;
                transform.position += direction * deltaTime;
                // m_currentState = direction.x <= 0.0f ? AvatarState.Left : AvatarState.Right;
                SetState();


                PlayAnimation();
                yield return null;
            }


            //  цель должна остановиться когда ее настигли.
            m_targetAvatar.WasAttacked();
            //  анимации атаки.
            var lastState = m_currentState;
            m_currentState = m_currentState == AvatarState.Left ? AvatarState.AttackLeft : AvatarState.AttackRight;
            m_frameTime = 0.0f;
            m_currentFrame = 0;
            while (PlayAnimationOnce())
            {
                yield return null;
            }

            // отправляем цель в космос.
            m_targetAvatar.GotHit(direction);
            //  последний кадр кулака показать 1 сек.
            yield return new WaitForSeconds(1.0f);
            // движемся дальше по своим делам.
            m_currentState = lastState;
            m_isAttackPermitted = true;
            m_isPursue = false;
            yield return null;
        }

        void WasAttacked()
        {
            m_wasAttacked = true;
            m_gotHit = false;
        }

        void GotHit(Vector3 direction)
        {
            m_gotHit = true;
            m_flyCurrentSpeed = 7.0f;
            m_flyDirection = direction;
        }


        public void StartPursuit(Avatar target)
        {
            if (!m_isAttackPermitted) return;
            m_isAttackPermitted = false;
            m_targetAvatar = target;
            if (m_pursuitCoroutine != null)
            {
                StopCoroutine(m_pursuitCoroutine);
            }

            m_pursuitCoroutine = StartCoroutine(Pursuit());
        }
    }
}