using UnityEngine;
using Cinemachine;
using System.Collections.Generic;
using System.Linq;

public class CameraController : Singleton<CameraController>
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float smoothTime;
    [SerializeField] private float movementThreshold;
    [SerializeField] private float minMovementDistance = 0.1f;

    private CinemachineFramingTransposer framingTransposer;
    private List<PlayerCombatAI> playerCharacters = new List<PlayerCombatAI>();
    private Vector3 velocity = Vector3.zero;
    private bool isInitialAlignmentDone = false;
    private Vector3 initialCameraPosition;
    private float lastStationaryX;

    protected override void Awake()
    {
        base.Awake();
        if (virtualCamera == null)
        {
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
        }
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        initialCameraPosition = virtualCamera.transform.position;
        lastStationaryX = initialCameraPosition.x;
    }

    private void Update()
    {
        if (playerCharacters.Count > 0 && GameManager.Instance.isCombatStart)
        {
            if (!isInitialAlignmentDone)
            {
                AlignCameraWithForwardMostCharacter();
            }
            else
            {
                UpdateCameraPosition();
            }
        }
    }

    private void AlignCameraWithForwardMostCharacter()
    {
        var forwardMostCharacter = GetForwardMostCharacter();
        if (forwardMostCharacter != null)
        {
            float targetPositionX = forwardMostCharacter.transform.position.x;
            virtualCamera.transform.position = new Vector3(targetPositionX, virtualCamera.transform.position.y, virtualCamera.transform.position.z);
            framingTransposer.m_TrackedObjectOffset.x = 0;
            isInitialAlignmentDone = true;
            lastStationaryX = targetPositionX;
        }
    }

    private void UpdateCameraPosition()
    {
        var forwardMostCharacter = GetForwardMostCharacter();
        if (forwardMostCharacter != null)
        {
            float targetPositionX = forwardMostCharacter.transform.position.x;
            float currentCameraX = virtualCamera.transform.position.x;

            if (Mathf.Abs(targetPositionX - lastStationaryX) > movementThreshold)
            {
                Vector3 targetPosition = new Vector3(targetPositionX, virtualCamera.transform.position.y, virtualCamera.transform.position.z);
                Vector3 smoothedPosition = Vector3.SmoothDamp(virtualCamera.transform.position, targetPosition, ref velocity, smoothTime);

                virtualCamera.transform.position = smoothedPosition;
                framingTransposer.m_TrackedObjectOffset.x = 0;

               
                if (Mathf.Abs(smoothedPosition.x - currentCameraX) >= minMovementDistance)
                {
                    lastStationaryX = smoothedPosition.x;
                }
            }
        }
    }

    private PlayerCombatAI GetForwardMostCharacter()
    {
        return playerCharacters
            .Where(p => p != null && p.gameObject.activeSelf && p.team == Team.Ally)
            .OrderByDescending(p => p.transform.position.x)
            .FirstOrDefault();
    }

    public void RegisterPlayerCharacter(PlayerCombatAI playerCharacter)
    {
        if (!playerCharacters.Contains(playerCharacter))
        {
            playerCharacters.Add(playerCharacter);
        }
    }

    public void UnregisterPlayerCharacter(PlayerCombatAI playerCharacter)
    {
        playerCharacters.Remove(playerCharacter);
    }

    public void ClearPlayerCharacters()
    {
        playerCharacters.Clear();
    }

    public void ResetCamera()
    {
        StartCoroutine(SmoothResetCamera());
    }

    private System.Collections.IEnumerator SmoothResetCamera()
    {
        float elapsedTime = 0;
        Vector3 startingPos = virtualCamera.transform.position;

        while (elapsedTime < smoothTime)
        {
            virtualCamera.transform.position = Vector3.Lerp(startingPos, initialCameraPosition, (elapsedTime / smoothTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        virtualCamera.transform.position = initialCameraPosition;
        framingTransposer.m_TrackedObjectOffset.x = 0;
        isInitialAlignmentDone = false;
        lastStationaryX = initialCameraPosition.x;
    }
}