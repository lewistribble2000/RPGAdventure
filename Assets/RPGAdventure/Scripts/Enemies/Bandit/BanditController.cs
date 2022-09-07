using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace RPGAdventure
{
    public class BanditController : MonoBehaviour
    {
        public PlayerScanner playerScanner;
        public float timeToStopPersuit = 2;
        public float waitOnPersuitTime = 2;
        public float distanceToAttack = 1.1f;

        public bool HasFollowTarget
        {
            get
            {
                return m_FollowTarget != null;
            }
        }

        private PlayerController m_FollowTarget;
        private EnemyController m_EnemyController;
        private Animator m_Animator;
        private float m_TimeSinceLostTarget = 0;
        private Vector3 m_OriginPosition;
        private Quaternion m_OriginRotation;

        private readonly int m_HashInPursuit = Animator.StringToHash("InPursuit");
        private readonly int m_NearBase = Animator.StringToHash("NearBase");
        private readonly int m_HashAttack = Animator.StringToHash("Attack");
        private void Awake()
        {
            m_EnemyController = GetComponent<EnemyController>();
            m_OriginPosition = transform.position;
            m_OriginRotation = transform.rotation;
            m_Animator = GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            GuardPosition();
        }

        private void GuardPosition()
        {
            var detectedTarget = playerScanner.FindTarget(transform);
            bool hasDetectedTarget = detectedTarget != null;
            if (hasDetectedTarget) { m_FollowTarget = detectedTarget; }

            if (HasFollowTarget)
            {
                AttackOrFollowTarget();

                if (hasDetectedTarget)
                {
                    m_TimeSinceLostTarget = 0;
                }
                else
                {
                    StopPursuit();
                }
            }

            CheckIfNearBase();
        }

        private void AttackOrFollowTarget()
        {
            Vector3 toTarget = m_FollowTarget.transform.position - transform.position;
            if (toTarget.magnitude <= distanceToAttack)
            {
                AttackTarget(toTarget);
            }
            else
            {
                FollowTarget();
            }
        }

        private void StopPursuit()
        {
            m_TimeSinceLostTarget += Time.fixedDeltaTime;

            if (m_TimeSinceLostTarget >= timeToStopPersuit)
            {
                m_FollowTarget = null;
                m_Animator.SetBool(m_HashInPursuit, false);
                m_EnemyController.ReturnToBase(m_OriginPosition);
            }
        }

        private void AttackTarget(Vector3 toTarget)
        {
            var toTargetRotation = Quaternion.LookRotation(toTarget);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toTargetRotation, 360 * Time.fixedDeltaTime);
            m_EnemyController.StopFollowTarget();
            m_Animator.SetTrigger(m_HashAttack);
        }

        private void FollowTarget()
        {
            m_Animator.SetBool(m_HashInPursuit, true);
            m_EnemyController.FollowTarget(m_FollowTarget.transform.position);
        }

        private void CheckIfNearBase()
        {
            Vector3 toBase = m_OriginPosition - transform.position;
            toBase.y = 0;

            bool nearBase = toBase.magnitude < 0.01f;
            m_Animator.SetBool(m_NearBase, nearBase);

            if (nearBase)
            {
                Quaternion targetRotation = Quaternion.RotateTowards(transform.rotation, m_OriginRotation, 360 * Time.fixedDeltaTime);
                transform.rotation = targetRotation;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Color c = new Color(255, 0, 0, 0.4f);
            UnityEditor.Handles.color = c;

            Vector3 rotatedForward = Quaternion.Euler(0, -playerScanner.detectionAngle * 0.5f, 0) * transform.forward;

            UnityEditor.Handles.DrawSolidArc(
                transform.position,
                Vector3.up,
                rotatedForward,
                playerScanner.detectionAngle,
                playerScanner.detectionRadius);

            UnityEditor.Handles.DrawSolidArc(
                transform.position,
                Vector3.up,
                rotatedForward,
                360,
                playerScanner.meleeDetectionRadius);
        }
#endif
    }
}