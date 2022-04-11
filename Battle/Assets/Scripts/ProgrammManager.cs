using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    private textScript testText;
    private tagTextScript textTag;

    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private GameObject selectedObject;

    private float pressTime;
    private float delta = 0f;

    void Start()
    {
        testText = FindObjectOfType<textScript>();
        textTag = FindObjectOfType<tagTextScript>();

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
    bool Tap()
    {       
        var touch = Input.GetTouch(0);

        delta += Input.GetTouch(0).deltaPosition.magnitude;

        testText.textUI.GetComponent<Text>().text = pressTime.ToString() + "\n" + System.Math.Round(delta / 1000, 2).ToString();
        switch (touch.phase)
        {
            case TouchPhase.Moved:
                {
                    if (System.Math.Round(delta / 1000, 2) < 0.3f)
                    {
                      pressTime = 0f;
                    }
                    else
                    {
                      pressTime = 0.6f;
                    }
                   
                    return false;
                    
                }

            case TouchPhase.Stationary:
                {
                    pressTime += Time.deltaTime;
                    
                    if (pressTime > 0.5f)

                    {
                        return false;
                    }
                    break;
                }

            case TouchPhase.Ended:
                {
                    if (pressTime < 0.5f)
                    {
                        if (System.Math.Round(delta / 1000, 2) < 0.3f)
                        {
                            delta = 0f;
                            pressTime = 0f;
                            return true;
                        }
                    }
                    else
                    {
                        delta = 0f;
                        pressTime = 0f;
                        return false;                       
                    }

                    if (System.Math.Round(delta / 1000, 2) < 0.3f)
                    {
                        return true;
                    }                
                    break;
                }
        }
        return false;
    }//needs float pressTime and float delta variables (return true if tap, false if long touch); 
    void moveObject()
    {      
        if (Input.touchCount > 0)
        {
            
            Touch touch = Input.GetTouch(0);
            TouchPosition = touch.position;
            Ray ray = ARCamera.ScreenPointToRay(touch.position);

            RaycastHit hitObject;
       
            if (Tap())
            {
                if (Physics.Raycast(ray, out hitObject))
                {

                    switch (hitObject.collider.tag)
                    {
                        case "Unselected":
                            {
                                textTag.textUItag.GetComponent<Text>().text = "Selected";
                                hitObject.collider.gameObject.tag = "Selected";
                                break;
                            }
                        case "Selected":
                            {
                                textTag.textUItag.GetComponent<Text>().text = "SelectedRotation";
                                hitObject.collider.gameObject.tag = "SelectedRotation";
                                break;
                            }

                        case "SelectedRotation":
                            {
                                textTag.textUItag.GetComponent<Text>().text = "Unselected";
                                hitObject.collider.gameObject.tag = "Unselected";
                                break;
                            }
                    }
                }
            }



            if (!Tap() && touch.phase == TouchPhase.Moved && System.Math.Round(delta / 1000, 2) > 0.3f)
            {
                if (Physics.Raycast(ray, out hitObject))
                {
                    if (hitObject.collider.CompareTag("Selected"))
                    {
                        ARRaycastManagerScript.Raycast(TouchPosition, hits, TrackableType.Planes);
                        selectedObject = GameObject.FindWithTag("Selected");
                        selectedObject.transform.position = hits[0].pose.position;
                    }
                    if (hitObject.collider.CompareTag("SelectedRotation"))
                    {
                        //Rotation logic
                    }
                }
            }
        }
    }
}