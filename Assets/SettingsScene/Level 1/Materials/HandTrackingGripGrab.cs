using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRHandTrackingEvents), typeof(XRDirectInteractor))]
public class HandTrackingGripGrab : MonoBehaviour
{
    private XRHandTrackingEvents _handEvents;
    private XRDirectInteractor _directInteractor;
    private bool _isGripping;

    void Start()
    {
        _handEvents = GetComponent<XRHandTrackingEvents>();
        _directInteractor = GetComponent<XRDirectInteractor>();

        if (_handEvents != null)
            _handEvents.jointsUpdated.AddListener(OnHandJointsUpdated);
    }

    private void OnHandJointsUpdated(XRHandJointsUpdatedEventArgs args)
    {
        float gripStrength = CalculateGripStrength(args.hand);
        _isGripping = gripStrength > 0.7f;

        if (_isGripping && !_directInteractor.isSelectActive)
        {
            TryGrabNearbyObject();
        }
        else if (!_isGripping && _directInteractor.isSelectActive)
        {
            _directInteractor.interactionManager.SelectExit(
                (IXRSelectInteractor)_directInteractor,
                (IXRSelectInteractable)_directInteractor.firstInteractableSelected
            );
        }
    }

    private float CalculateGripStrength(XRHand hand)
    {
        var middleTip = hand.GetJoint(XRHandJointID.MiddleTip);
        var palm = hand.GetJoint(XRHandJointID.Palm);

        bool isMiddleTracked = middleTip.trackingState != XRHandJointTrackingState.None;
        bool isPalmTracked = palm.trackingState != XRHandJointTrackingState.None;

        if (isMiddleTracked && isPalmTracked)
        {
            Pose middlePose, palmPose;

            if (middleTip.TryGetPose(out middlePose) && palm.TryGetPose(out palmPose))
            {
                Vector3 middleTipPos = middlePose.position;
                Vector3 palmPos = palmPose.position;

                return 1 - Mathf.Clamp01(Vector3.Distance(middleTipPos, palmPos) / 0.1f);
            }
        }

        return 0f;
    }

    private void TryGrabNearbyObject()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.1f);
        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out IXRSelectInteractable grabInteractable))
            {
                _directInteractor.interactionManager.SelectEnter(
                    (IXRSelectInteractor)_directInteractor,
                    grabInteractable
                );
                break;
            }
        }
    }

    void OnDestroy()
    {
        if (_handEvents != null)
            _handEvents.jointsUpdated.RemoveListener(OnHandJointsUpdated);
    }
}