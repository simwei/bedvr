using System;
using System.Collections;
using UnityEngine;

namespace EndlessRunner
{
    [AddComponentMenu("CUSTOM / Player Controller")]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private Transform m_cameraTarget;
        [SerializeField]
        private Renderer[] m_bodyRenderers;
        [SerializeField]
        private float m_horizontalSpeed = 0.2f,
            m_verticalSpeed = 1f, m_fieldOfViewMultiplier = 1f, m_fieldOfViewLimit = 70f;

        private bool m_isMoving = false;

        private const float m_right = 4, m_middle = 0, m_left = -4;
        private float m_movingTarget = 0;
        private float m_currentPosition = 0;

        private Rigidbody m_rigidbody;
        private ParticleSystem[] m_particles;
        private Vector3 m_movement;
        private bool m_canMove = true, m_canJump = true;
        private Vector3 m_playerStartPosition, m_targetStartLocalPosition;
        private Quaternion m_playerStartRotation, m_targetStartLocalRotation;

        public Material CurrentMaterial { get { return m_bodyRenderers[0].sharedMaterial; } }
        public string CurrentMaterialName { get { return m_bodyRenderers[0].sharedMaterial.name; } }

        void Start()
        {
            Initialize();

            SetMaterialColors(GameDirector.RandomColor());

            UIManager.UpdateCurrentScore(0);
            //StartCoroutine(MoveCameraTargetIn());
        }

        void Update()
        {
            if (!m_canMove) return;

            // Jump.
            if (Input.GetButtonDown("Vertical") && m_canJump)
            {
                m_canJump = false;

                DoJump();
            }

            // Get movement value.

            if (Input.GetButtonDown("Fire2"))
            {
                if (m_movingTarget == m_right) return;

                m_movingTarget += m_right;


            }

            if (Input.GetButtonDown("Fire1"))
            {
                if (m_movingTarget == m_left) return;

                m_movingTarget += m_left;

            }
        }

        private void DoMove()
        {
            throw new NotImplementedException();
        }

        void FixedUpdate()
        {
            if (!m_canMove) return;



            // Execute movement
            m_rigidbody.MovePosition(m_rigidbody.position + new Vector3(m_horizontalSpeed * Math.Sign(m_movingTarget - m_rigidbody.position.x), 0, 0));
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.IsGround())
            {
                m_canJump = true;
            }
        }

        void OnTriggerEnter(Collider collider)
        {
            if (collider.IsWater())
            {
                Death();
            }
        }

        public void Reload()
        {
            //StopCoroutine("MoveCameraTargetOut");

            // Restart player position and rotation.
            transform.position = m_playerStartPosition;
            transform.rotation = m_playerStartRotation;

            // Restart target position and rotation.
            m_cameraTarget.localPosition = m_targetStartLocalPosition;
            m_cameraTarget.localRotation = m_targetStartLocalRotation;

            SetMaterialColors(GameDirector.RandomColor());
            HandleComponents(true);

            GameDirector.ResetGame();

            //StartCoroutine(MoveCameraTargetIn());
        }

        public void Death()
        {
            m_rigidbody.velocity = Vector3.zero;

            GameDirector.StopGame();
            GameDirector.DoEffects("PlayerDeath", CurrentMaterial);

            HandleComponents(false);

            //StopCoroutine("Zoom");
            //StartCoroutine("MoveCameraTargetOut");

            GameDirector.ShowScoreResults();
        }

        public void SetMaterialColors(Material materialColor)
        {
            for (int i = 0; i < m_bodyRenderers.Length; i++)
            {
                m_bodyRenderers[i].sharedMaterial = materialColor;
            }

            UIManager.UpdateCurrentMaterial(materialColor);
        }

        private void Initialize()
        {
            m_rigidbody = GetComponent<Rigidbody>();
            m_particles = GetComponentsInChildren<ParticleSystem>();

            // Save player start position and rotation.
            m_playerStartPosition = transform.position;
            m_playerStartRotation = transform.rotation;

            // Save target start position and rotation.
            m_targetStartLocalPosition = m_cameraTarget.localPosition;
            m_targetStartLocalRotation = m_cameraTarget.localRotation;
        }

        private void HandleComponents(bool enable)
        {
            SetRenderersVisibility(false);
           
            m_rigidbody.isKinematic = !enable;
            m_canMove = enable;
            m_canJump = enable;
        }

        private void DoJump()
        {
            // Execute zoom (cam and world scroll).
            //StartCoroutine("Zoom");

            SoundManager.PlaySoundEffect("PlayerJump");

            // Execute jump.
            m_rigidbody.AddRelativeForce(Vector3.up * m_verticalSpeed, ForceMode.Impulse);
        }

        private IEnumerator MoveCameraTargetIn()
        {
            yield return new WaitForSeconds(1f);

            while (m_cameraTarget.localPosition.y > 0.1f)
            {
                m_cameraTarget.localPosition = Vector3.Lerp(m_cameraTarget.localPosition, Vector3.zero, Time.deltaTime);
                m_cameraTarget.localRotation = Quaternion.Lerp(m_cameraTarget.localRotation, Quaternion.identity, Time.deltaTime);

                yield return null;
            }

            m_cameraTarget.localPosition = Vector3.zero;
            m_cameraTarget.localRotation = Quaternion.identity;
        }

        private IEnumerator MoveCameraTargetOut()
        {
            while (m_cameraTarget.localPosition.y < 0.9f)
            {
                m_cameraTarget.localPosition = Vector3.Lerp(m_cameraTarget.localPosition, m_targetStartLocalPosition, Time.deltaTime);
                m_cameraTarget.localRotation = Quaternion.Lerp(m_cameraTarget.localRotation, m_targetStartLocalRotation, Time.deltaTime);

                yield return null;
            }

            m_cameraTarget.localPosition = m_targetStartLocalPosition;
            m_cameraTarget.localRotation = m_targetStartLocalRotation;
        }

        private void SetRenderersVisibility(bool enable)
        {
            for (int i = 0; i < m_bodyRenderers.Length; i++)
            {
                m_bodyRenderers[i].enabled = enable;
            }
        }
    }
}