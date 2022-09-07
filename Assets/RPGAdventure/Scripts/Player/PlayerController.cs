using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGAdventure
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance
        {
            get { return s_Instance; }
        }

        private static PlayerController s_Instance;

        public MeleeWeapon meleeWeapon;
        public float maximumForwardSpeed = 8;
        public float rotationSpeed;
        public float m_MaximumRotationSpeed = 1200;
        public float m_MinimumRotationSpeed = 800;
        public float gravity = 20;

        private PlayerInput m_PlayerInput;
        private CameraController m_CameraController;
        private CharacterController m_ChController;
        private Animator m_Animator;

        private Quaternion m_TargetRotation;

        private float m_DesiredForwardSpeed;
        private float m_ForwardSpeed;
        private float m_VerticalSpeed;

        private readonly int m_HashForwardSpeed = Animator.StringToHash("ForwardSpeed");
        private readonly int m_HashMeleeAttack = Animator.StringToHash("MeleeAttack");

        const float k_Acceleration = 20f;
        const float k_Deceleration = 35f;

        private void Awake()
        {
            m_ChController = GetComponent<CharacterController>();
            m_CameraController = Camera.main.GetComponent<CameraController>();
            m_PlayerInput = GetComponent<PlayerInput>();
            m_Animator = GetComponent<Animator>();

            s_Instance = this;
        }
        private void FixedUpdate()
        {
            ComputeForwardMovement();
            ComputeVerticalMovement();
            ComputeRotation();

            if(m_PlayerInput.IsMoving)
            {
                float rotationSpeed = Mathf.Lerp(m_MaximumRotationSpeed, m_MinimumRotationSpeed, m_ForwardSpeed / m_DesiredForwardSpeed);
                m_TargetRotation = Quaternion.RotateTowards(transform.rotation, m_TargetRotation, rotationSpeed * Time.fixedDeltaTime);
                transform.rotation = m_TargetRotation;
            }

            m_Animator.ResetTrigger(m_HashMeleeAttack);
            if(m_PlayerInput.IsAttacking)
            {
                m_Animator.SetTrigger(m_HashMeleeAttack);
            }
        }

        private void OnAnimatorMove()
        {
            Vector3 movement = m_Animator.deltaPosition;
            movement += m_VerticalSpeed * Vector3.up * Time.fixedDeltaTime;
            m_ChController.Move(movement);
        }

        public void MeleeAttackStart()
        {
            meleeWeapon.BeginAttack();
        }

        public void MeleeAttackEnd()
        {
            meleeWeapon.EndAttack();
        }

        private void ComputeVerticalMovement()
        {
            m_VerticalSpeed = -gravity;
        }

        private void ComputeForwardMovement()
        {
            Vector3 moveInput = m_PlayerInput.MoveInput.normalized;
            m_DesiredForwardSpeed = moveInput.magnitude * maximumForwardSpeed;

            float acceleration = m_PlayerInput.IsMoving ? k_Acceleration : k_Deceleration;

            m_ForwardSpeed = Mathf.MoveTowards(m_ForwardSpeed, m_DesiredForwardSpeed, Time.fixedDeltaTime * acceleration);

            m_Animator.SetFloat(m_HashForwardSpeed, m_ForwardSpeed);
        }

        private void ComputeRotation()
        {
            Vector3 moveInput = m_PlayerInput.MoveInput.normalized;

            //Use xAxis here in the float y slot, player rotation left and right = y slot, camera rotation left and right = x slot, so camera x rotation = player y rotation
            Vector3 cameraDirection = Quaternion.Euler(0, m_CameraController.PlayerCam.m_XAxis.Value, 0) * Vector3.forward;
            
            Quaternion targetRotation;

            if(Mathf.Approximately(Vector3.Dot(moveInput, Vector3.forward), -1))
            {
                //Going backwards
                targetRotation = Quaternion.LookRotation(-cameraDirection);
            }
            else
            {
                Quaternion movementRotation = Quaternion.FromToRotation(Vector3.forward, moveInput);
                targetRotation = Quaternion.LookRotation(movementRotation * cameraDirection);
            }

            m_TargetRotation = targetRotation;
        }
    }
}
