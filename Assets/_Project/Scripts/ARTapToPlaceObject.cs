using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARTapToPlaceObject : MonoBehaviour
{
    [SerializeField] private ARSessionOrigin arSessionOrigin;
    [SerializeField] private ARRaycastManager arRaycastManager;
    [SerializeField] private GameObject placementIndicator;
    [SerializeField] private GameObject[] objectsToPlace;
    private bool _placementPoseIsValid = false;
    private Pose _placementPose;
    private float lastYUpdatedTime = 0f;
    private float lastY = 0f;
    private bool placementIndicatorIsStable = false;
    private State _state = State.PlacingAnObject;
    private List<GameObject> _placedObjects = new List<GameObject>();

    private enum State
    {
        PlacingAnObject,
        Inactive
    }

    public void Reset()
    {
        _state = State.PlacingAnObject;

        foreach (var o in _placedObjects)
        {
            if (!o.IsDestroyed())
                Destroy(o);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_state == State.PlacingAnObject)
        {
            UpdatePlacementPose();
            UpdatePlacementIndicator();

            if (_placementPoseIsValid && placementIndicatorIsStable && Input.touchCount > 0 &&
                Input.GetTouch(0).phase == TouchPhase.Began)
            {
                PlaceObject();
            }
        }
    }

    private void PlaceObject()
    {
        foreach (var o in objectsToPlace)
        {
            this._placedObjects.Add(Instantiate(o, _placementPose.position, _placementPose.rotation));
        }
        _state = State.Inactive;
        placementIndicator.SetActive(false);
    }

    private void UpdatePlacementIndicator()
    {
        if (_placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(_placementPose.position, _placementPose.rotation);
            
            if (Mathf.Abs(this.lastY - _placementPose.position.y) > 0.1f)
            {
                this.lastYUpdatedTime = Time.time;
            }

            this.placementIndicatorIsStable = Time.time - this.lastYUpdatedTime > 3f;
            this.lastY = _placementPose.position.y;
        }
        else
        {
            this.placementIndicatorIsStable = false;
            placementIndicator.SetActive(false);
        }

        placementIndicator.GetComponent<PlacementIndicator>().Stable = this.placementIndicatorIsStable;
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        arRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);
        _placementPoseIsValid = hits.Count > 0;
        if (_placementPoseIsValid)
        {
            this._placementPose = hits[0].pose;
            var cameraForward = Camera.main.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            _placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
}