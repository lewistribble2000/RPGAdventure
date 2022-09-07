using Cinemachine;
using UnityEngine;

namespace RPGAdventure
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private CinemachineFreeLook freeLook;

        public CinemachineFreeLook PlayerCam
        {
            get { return freeLook; }
        }
        void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                freeLook.m_XAxis.m_MaxSpeed = 400;
                freeLook.m_YAxis.m_MaxSpeed = 10;
            }
            if (Input.GetMouseButtonUp(1))
            {
                freeLook.m_XAxis.m_MaxSpeed = 0;
                freeLook.m_YAxis.m_MaxSpeed = 0;
            }
        }
    }
}
