using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private NavMeshAgent m_NavMeshAgent;
    private Animator m_Animator;
    private float m_SpeedModifier = 0.7f;
    private Vector3 velocity = Vector3.zero;
    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_NavMeshAgent.updatePosition = false;
    }

    private void OnAnimatorMove()
    {
        if(m_NavMeshAgent.enabled) 
        { 
            m_NavMeshAgent.speed = (m_Animator.deltaPosition / Time.fixedDeltaTime).magnitude * m_SpeedModifier; 
        }
    }

    public bool FollowTarget(Vector3 position)
    {
        if(m_NavMeshAgent.updatePosition)
        {
            m_NavMeshAgent.updatePosition = false;
        }
        if (!m_NavMeshAgent.enabled)
        {
            m_NavMeshAgent.enabled = true;
        }

        transform.position = Vector3.SmoothDamp(transform.position, m_NavMeshAgent.nextPosition, ref velocity, 0.1f);
        return m_NavMeshAgent.SetDestination(position);
    }
    public bool ReturnToBase(Vector3 position)
    {
        if (!m_NavMeshAgent.enabled)
        {
            m_NavMeshAgent.enabled = true;
        }
        if(!m_NavMeshAgent.updatePosition)
        {
            m_NavMeshAgent.updatePosition = true;
        }
        return m_NavMeshAgent.SetDestination(position);
    }


    public void StopFollowTarget()
    {
        m_NavMeshAgent.enabled = false;
    }
}
