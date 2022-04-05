using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ProgrammManager : MonoBehaviour
{
    [Header("Put your planeMarker here")]
    [SerializeField] private GameObject PlaneMarkerPrefab;

    [SerializeField] private Camera ARCamera;

    private ARRaycastManager ARRaycastManagerScript;

    private Vector2 TouchPosition;

    public GameObject ObjectToSpawn;

    [Header("Put ur scroll View here")]
    public GameObject scrollView;

    public bool chooseObject = false;


    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private GameObject selectedObject;


    void Start()
    {
        ARRaycastManagerScript = FindObjectOfType<ARRaycastManager>();

        PlaneMarkerPrefab.SetActive(false);
        scrollView.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (chooseObject)
        {
            ShowMarkerAndSetObject();
        }

        moveObject();
    }

    void ShowMarkerAndSetObject()
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        ARRaycastManagerScript.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes);

        //show markera
        if (hits.Count > 0)
        {
            PlaneMarkerPrefab.transform.position = hits[0].pose.position;
            PlaneMarkerPrefab.SetActive(true);
        }

        // set object
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            Instantiate(ObjectToSpawn, hits[0].pose.position, ObjectToSpawn.transform.rotation);
            chooseObject = false;
            PlaneMarkerPrefab.SetActive(false);
        }
    }

    void moveObject()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            TouchPosition = touch.position;

            Ray ray = ARCamera.ScreenPointToRay(touch.position);

            RaycastHit hitObject;
            if (touch.phase == TouchPhase.Began)
            {
                if (Physics.Raycast(ray, out hitObject))
                {
                    if (hitObject.collider.CompareTag("Unselected"))
                    {
                        hitObject.collider.gameObject.tag = "Selected";
                    }
                }
            }
            if (touch.phase == TouchPhase.Moved)
            {
                ARRaycastManagerScript.Raycast(TouchPosition, hits, TrackableType.Planes);
                selectedObject = GameObject.FindWithTag("Selected");
                selectedObject.transform.position = hits[0].pose.position;
            }
            
        }
    }
}
 
