using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGAdventure
{
    public class MeleeWeapon : MonoBehaviour
    {
        [System.Serializable]
        public class AttackPoint
        {
            public float radius;
            public Vector3 offset;
            public Transform rootTransform;
        }

        public int damage = 10;
        public AttackPoint[] attackPoints = new AttackPoint[0];

        private bool m_IsAttacking = false;
        private Vector3[] m_OriginAttackPosition;

        private void FixedUpdate()
        {
            if(m_IsAttacking)
            {
                for(int i = 0; i < attackPoints.Length; i++)
                {
                    AttackPoint ap = attackPoints[i];
                    Vector3 worldPosition = ap.rootTransform.position + ap.rootTransform.TransformDirection(ap.offset);
                    Vector3 attackVector = worldPosition - m_OriginAttackPosition[i];
                    Ray r = new Ray(worldPosition, attackVector);
                    Debug.DrawRay(worldPosition, attackVector, Color.red);
                }
            }
        }
        public void BeginAttack()
        {
            m_IsAttacking = true;
            m_OriginAttackPosition = new Vector3[attackPoints.Length];
            for(int i = 0; i < attackPoints.Length; i++)
            {
                AttackPoint ap = attackPoints[i];
                m_OriginAttackPosition[i] = ap.rootTransform.position + ap.rootTransform.TransformDirection(ap.offset);
            }
        }

        public void EndAttack()
        {
            m_IsAttacking = false;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            foreach(AttackPoint attackPoint in attackPoints)
            {
                if(attackPoint.rootTransform != null)
                {
                    Vector3 worldPosition = attackPoint.rootTransform.TransformVector(attackPoint.offset);
                    Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.6f);
                    Gizmos.DrawSphere(attackPoint.rootTransform.position + worldPosition, attackPoint.radius);
                }
            }
        }
#endif
    }
}

