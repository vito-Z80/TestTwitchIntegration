using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

namespace Avatars
{
    public class Avatar : MonoBehaviour
    {
        [SerializeField] Material shadeMaterial;
        public string avatarName;

        PixelPerfectCamera m_pixelPerfectCamera;
        Dictionary<AvatarState, int[]> m_avatars = new();
        SpriteRenderer m_spriteRenderer;

        int m_currentFrame; //  текущий кадр анимации

        float m_angle;
        float m_leaveTime;  //  время до уничтожения аватара
        float m_frameTime;  //  время кадра анимации
        float m_stateTime;  //  время состояния анимации одного направления
        float m_idleTime;   //  время idle анимации
        float m_flyCurrentSpeed; // Текущая скорость атакованной цели

        float m_bottomOffset;
        
        const float StrikingDistance = 1.0f;    //  расстояние удара
        const float FlyDeceleration = 1.5f; // Скорость замедления атакованной цели
        
        bool m_canDiagonalMovable;  //  true = можно перемещаться по диагонали
        bool m_wasAttacked; //  true = был атакован
        bool m_isAttackPermitted;   //  true = атака разрешена
        bool m_isPursue;    //  true = преследует цель
        bool m_gotHit;  //  true = получил в кабину
        bool m_iFree;   //  true = можно переиспользовать (не реализовано)

        Avatar m_targetAvatar;

        AvatarState m_currentState;
        AvatarState[] m_randomStates;
        Rect m_area;

        AvatarsController m_avatarsController;

        Vector3 m_randomTargetDirection;
        Vector3 m_flyDirection; // Направление движения атакованной цели

        AvatarMove m_avatarMovement;


        void Start()
        {
            m_iFree = false;
            m_spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
            m_spriteRenderer.sharedMaterial = shadeMaterial;
            SetIdleTime();
            m_isAttackPermitted = true;
            m_isPursue = false;
        }

        public SpriteRenderer GetSpriteRenderer() => m_spriteRenderer ?? GetComponentInChildren<SpriteRenderer>();

        public void Init(PixelPerfectCamera pixelPerfectCamera, Rect area, string avatarName, AvatarsController avatarsController, Dictionary<AvatarState, int[]> avatarIndices)
        {
            m_iFree = false;
            m_avatars = avatarIndices;
            m_avatarsController = avatarsController;
            this.avatarName = avatarName;
            m_pixelPerfectCamera = pixelPerfectCamera;
            m_area = area;
            m_leaveTime = 5.0f * 30.0f;
            m_stateTime = GetRandomStateTime();

            if (avatarIndices.ContainsKey(AvatarState.Idle))
            {
                var values = Enum.GetValues(typeof(AvatarMove));
                var randomIndex = Random.Range(0, values.Length);
                m_avatarMovement = (AvatarMove)values.GetValue(randomIndex);
            }
            else
            {
                m_avatarMovement = AvatarMove.Random;
            }
        }

        void Update()
        {
            SetBottomOffset();
            var deltaTime = 1.0f / m_pixelPerfectCamera.assetsPPU * 60.0f * Time.deltaTime;

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
                    SetState();
                    AvatarAnimation();
                    if (!m_isPursue) Move(deltaTime);
                }
            }

            if (m_gotHit)
            {
                Fly(deltaTime);
                if (m_flyCurrentSpeed <= 0.0f)
                {
                    m_wasAttacked = false;
                    m_gotHit = false;
                }
            }
        }

        void SetBottomOffset()
        {
            if (m_spriteRenderer?.sprite == null)
            {
                m_bottomOffset = 4.0f;
                return;
            }
            m_bottomOffset = m_spriteRenderer.sprite.bounds.size.y;
        }

        void Fly(float deltaTime)
        {
            if (m_flyCurrentSpeed > 0)
            {
                transform.position += m_flyDirection * (m_flyCurrentSpeed * deltaTime);
                
                if (transform.position.x < m_area.xMin)
                {
                    transform.position = new Vector3(m_area.xMin, transform.position.y, transform.position.z);
                    m_flyDirection.x = Mathf.Abs(m_flyDirection.x); 
                }
                else if (transform.position.x > m_area.xMax)
                {
                    transform.position = new Vector3(m_area.xMax, transform.position.y, transform.position.z);
                    m_flyDirection.x = -Mathf.Abs(m_flyDirection.x); 
                }

                if (transform.position.y < m_area.yMin + m_bottomOffset)
                {
                    transform.position = new Vector3(transform.position.x, m_area.yMin + m_bottomOffset, transform.position.z);
                    m_flyDirection.y = Mathf.Abs(m_flyDirection.y);
                }
                else if (transform.position.y > m_area.yMax)
                {
                    transform.position = new Vector3(transform.position.x, m_area.yMax, transform.position.z);
                    m_flyDirection.y = -Mathf.Abs(m_flyDirection.y); 
                }

                m_flyCurrentSpeed -= FlyDeceleration * Time.deltaTime;
                if (m_flyCurrentSpeed < 0)
                {
                    m_flyCurrentSpeed = 0;
                }
            }
        }


        void Move(float deltaTime)
        {
            switch (m_avatarMovement)
            {
                case AvatarMove.Oval:
                    OvalMovement(deltaTime);
                    break;
                case AvatarMove.Random:
                    RandomMovement(deltaTime);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        void OvalMovement(float deltaTime)
        {
            m_angle += deltaTime / 8.0f;
            if (m_angle > Mathf.PI * 2)
                m_angle -= Mathf.PI * 2;
            var a = m_area.width / 2f;
            var b = m_area.height / 2f;
            var center = new Vector2(m_area.x + a, m_area.y + b);
            var x = center.x + a * Mathf.Cos(m_angle);
            var y = center.y + b * Mathf.Sin(m_angle);
            transform.position = new Vector3(x, y, transform.position.z);
        }

        void RandomMovement(float deltaTime)
        {
            m_idleTime -= deltaTime;
            if (m_avatars.ContainsKey(AvatarState.Idle))
            {
                if (m_idleTime < 0.0f && m_idleTime > -6.0f)
                {
                    if (Random.value < 0.93f)
                    {
                        return;
                    }

                    m_randomTargetDirection = transform.position;
                }
                else if (m_idleTime <= -6.0f)
                {
                    SetIdleTime();
                }
            }

            transform.position = Vector3.MoveTowards(transform.position, m_randomTargetDirection, deltaTime);
            if (Mathf.Abs(transform.position.x - m_randomTargetDirection.x) < 0.00625f)
            {
                m_randomTargetDirection.x = Random.Range(m_area.min.x, m_area.max.x);
            }

            if (Mathf.Abs(transform.position.y - m_randomTargetDirection.y) < 0.00625f)
            {
                m_randomTargetDirection.y = Random.Range(m_area.min.y  + m_bottomOffset, m_area.max.y);
            }
            m_currentState = transform.position.x > m_randomTargetDirection.x ? AvatarState.Left : AvatarState.Right;
        }

        void SetIdleTime()
        {
            m_idleTime = Random.Range(3, 10);
        }


        bool IsStateTimeOver()
        {
            m_stateTime -= Time.deltaTime;
            if (m_stateTime < 0.0f)
            {
                m_stateTime = GetRandomStateTime();
                return true;
            }

            return false;
        }


        void SetState()
        {
            var lastState = m_currentState;
            if (IsStateTimeOver())
            {
                m_currentState = GetRandomState();
                if (m_currentState != lastState)
                {
                    m_currentFrame = 0;
                }
            }
        }

        void AvatarAnimation()
        {
            if (!m_avatars.ContainsKey(m_currentState))
            {
                m_currentState = GetRandomState();
                return;
            }

            if (!IsPlayingAnimation())
            {
                m_currentState = AvatarState.Idle;
                m_frameTime = 0.0f;
                m_currentFrame = 0;
                IsPlayingAnimation();
            }
        }

        bool IsPlayingAnimation()
        {
            if (m_frameTime > 1.0f / 8.0f)
            {
                m_currentFrame++;

                if (m_currentFrame >= m_avatars[m_currentState].Length) return false;
                var renderId = m_avatars[m_currentState][m_currentFrame];
                var sprite = m_avatarsController.GetSprite(renderId);
                m_spriteRenderer.sprite = sprite;
                m_frameTime = 0.0f;
            }

            m_frameTime += Time.deltaTime * 1.45f;
            return true;
        }

        AvatarState GetRandomState()
        {
            m_randomStates ??= new[] { AvatarState.Idle, AvatarState.Left, AvatarState.Right };
            var randomIndex = Random.Range(0, m_randomStates.Length);
            return m_randomStates[randomIndex];
        }

        float GetRandomStateTime()
        {
            return Random.Range(3.0f, 8.0f);
        }

        void DestroyAvatar()
        {
            Debug.Log($"TODO: Destroyed avatar: {avatarName}");
        }


        IEnumerator Pursuit()
        {
            var distance = Vector3.Distance(transform.position, m_targetAvatar.transform.position);
            var direction = Vector3.zero;
            //  движемся к цели.
            m_isPursue = true;
            while (distance > StrikingDistance)
            {
                distance = Vector3.Distance(transform.position, m_targetAvatar.transform.position);
                var deltaTime = 1.0f / m_pixelPerfectCamera.assetsPPU * 60.0f * Time.deltaTime * 2.0f;
                direction = (m_targetAvatar.transform.position - transform.position).normalized;
                transform.position += direction * deltaTime;
                m_currentState = direction.x <= 0.0f ? AvatarState.Left : AvatarState.Right;
                yield return null;
            }

            //  цель должна остановиться когда ее настигли.
            m_targetAvatar.WasAttacked();
            //  анимации атаки.
            m_currentState = m_currentState == AvatarState.Left ? AvatarState.AttackLeft : AvatarState.AttackRight;
            m_frameTime = 0.0f;
            m_currentFrame = 0;
            while (IsPlayingAnimation())
            {
                yield return null;
            }

            // отправляем цель в космос.
            m_targetAvatar.GotHit(direction);
            //  последний кадр кулака показать 1 сек.
            yield return new WaitForSeconds(1.0f);
            // движемся дальше по своим делам.
            m_currentState = GetRandomState();
            m_stateTime = GetRandomStateTime();
            m_isAttackPermitted = true;
            m_isPursue = false;
            yield return null;
        }

        void WasAttacked()
        {
            m_wasAttacked = true;
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
            StartCoroutine(Pursuit());
        }
    }
}