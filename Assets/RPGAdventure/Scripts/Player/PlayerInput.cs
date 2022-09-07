using System.Collections;
using UnityEngine;

namespace RPGAdventure
{
    public class PlayerInput : MonoBehaviour
    {
        private Vector3 m_Movement;
        private bool m_IsAttacking;
        public Vector3 MoveInput
        {
            get { return m_Movement; }
        }

        public bool IsMoving
        {
            get
            {
                return !Mathf.Approximately(MoveInput.magnitude, 0);
            }
        }

        public bool IsAttacking
        {
            get
            {
                return m_IsAttacking;
            }
        }
        void Update()
        {
            m_Movement.Set(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            if(Input.GetButtonDown("Fire1") && !m_IsAttacking)
            {
                StartCoroutine(AttackAndWait());
            }
        }

        private IEnumerator AttackAndWait()
        {
            m_IsAttacking = true;
            yield return new WaitForSeconds(0.03f);
            m_IsAttacking = false;
        }
    }
}