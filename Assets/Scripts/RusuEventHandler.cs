/*==============================================================================
Copyright (c) 2021 PTC Inc. All Rights Reserved.

Confidential and Proprietary - Protected under copyright and other laws.
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
==============================================================================*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Vuforia;




/// <summary>
/// A custom handler that implements the ITrackableEventHandler interface.
///
/// Changes made to this file could be overwritten when upgrading the Vuforia version.
/// When implementing custom event handler behavior, consider inheriting from this class instead.
/// </summary>
public class RusuEventHandler : MonoBehaviour
{
    public enum TrackingStatusFilter
    {
        Tracked,
        Tracked_ExtendedTracked
        
    }

    /// <summary>
    /// A filter that can be set to either:
    /// - Only consider a target if it's in view (TRACKED)
    /// - Also consider the target if's outside of the view, but the environment is tracked (EXTENDED_TRACKED)
    /// - Even consider the target if tracking is in LIMITED mode, e.g. the environment is just 3dof tracked.
    /// </summary>
    public TrackingStatusFilter StatusFilter = TrackingStatusFilter.Tracked_ExtendedTracked;
    public UnityEvent OnTargetFound;
    public UnityEvent OnTargetLost;
 
    public static GameObject RaduBRusu;
    public static GameObject Found;
    public static GameObject Name;
    public static GameObject Description;
    public static GameObject Employer;
    public static GameObject Work;
    protected ObserverBehaviour mObserverBehaviour;
    protected TargetStatus mPreviousTargetStatus = TargetStatus.NotObserved;
    protected bool mCallbackReceivedOnce;


    string URL = "https://query.wikidata.org/sparql?query=SELECT ?personLabel ?personDescription ?employerLabel (GROUP_CONCAT(DISTINCT ?itemLabel; separator= ', ') as ?workLabel) WHERE {?person wdt:P106 wd:Q15976092. ?person wdt:P18 <http://commons.wikimedia.org/wiki/Special:FilePath/Radu%20Rusu%20Fyusion.jpg>. ?person wdt:P108 ?employer. ?person wdt:P800 ?item.SERVICE wikibase:label { bd:serviceParam wikibase:language 'en'. ?person rdfs:label ?personLabel. ?person schema:description ?personDescription. ?employer rdfs:label ?employerLabel. ?item rdfs:label ?itemLabel.}.} GROUP BY ?personLabel ?personDescription ?employerLabel";
    
    protected virtual void Start()
    {
        Found = GameObject.Find("Found");
        Name = GameObject.Find("Name");
        Description = GameObject.Find("Description");
        Employer = GameObject.Find("Employer");
        Work = GameObject.Find("Work");
        mObserverBehaviour = GetComponent<ObserverBehaviour>();

        if (mObserverBehaviour)
        {
            mObserverBehaviour.OnTargetStatusChanged += OnObserverStatusChanged;
            mObserverBehaviour.OnBehaviourDestroyed += OnObserverDestroyed;

            OnObserverStatusChanged(mObserverBehaviour, mObserverBehaviour.TargetStatus);
        }
    }

    protected virtual void OnDestroy()
    {
        if (mObserverBehaviour)
            OnObserverDestroyed(mObserverBehaviour);
    }

    void OnObserverDestroyed(ObserverBehaviour observer)
    {
        mObserverBehaviour.OnTargetStatusChanged -= OnObserverStatusChanged;
        mObserverBehaviour.OnBehaviourDestroyed -= OnObserverDestroyed;
        mObserverBehaviour = null;
    }

    void OnObserverStatusChanged(ObserverBehaviour behaviour, TargetStatus targetStatus)
    {
        var name = mObserverBehaviour.TargetName;
        if (mObserverBehaviour is VuMarkBehaviour vuMarkBehaviour && vuMarkBehaviour.InstanceId != null)
        {
            name += " (" + vuMarkBehaviour.InstanceId + ")";
        }

        Debug.Log($"Target status: { name } { targetStatus.Status } -- { targetStatus.StatusInfo }");

        HandleTargetStatusChanged(mPreviousTargetStatus.Status, targetStatus.Status);
        HandleTargetStatusInfoChanged(targetStatus.StatusInfo);

        mPreviousTargetStatus = targetStatus;
    }

    protected virtual void HandleTargetStatusChanged(Status previousStatus, Status newStatus)
    {
        var shouldBeRendererBefore = ShouldBeRendered(previousStatus);
        var shouldBeRendererNow = ShouldBeRendered(newStatus);
        if (shouldBeRendererBefore != shouldBeRendererNow)
        {
            if (shouldBeRendererNow)
            {
                OnTrackingFound();
            }
            else
            {
                OnTrackingLost();
            }
        }
        else
        {
            if (!mCallbackReceivedOnce && !shouldBeRendererNow)
            {
                // This is the first time we are receiving this callback, and the target is not visible yet.
                // --> Hide the augmentation.
                OnTrackingLost();
            }
        }

        mCallbackReceivedOnce = true;
    }

    protected virtual void HandleTargetStatusInfoChanged(StatusInfo newStatusInfo)
    {
        if (newStatusInfo == StatusInfo.WRONG_SCALE)
        {
            Debug.LogErrorFormat("The target {0} appears to be scaled incorrectly. " +
                                 "This might result in tracking issues. " +
                                 "Please make sure that the target size corresponds to the size of the " +
                                 "physical object in meters and regenerate the target or set the correct " +
                                 "size in the target's inspector.", mObserverBehaviour.TargetName);
        }
    }

    protected bool ShouldBeRendered(Status status)
    {
        if (status == Status.TRACKED)
        {
            // always render the augmentation when status is TRACKED, regardless of filter
            return true;
        }

    if (StatusFilter == TrackingStatusFilter.Tracked_ExtendedTracked && status == Status.EXTENDED_TRACKED)
        {
            // also return true if the target is extended tracked
            return true;
        }

        return false;
    }

    protected virtual void OnTrackingFound()
    {
        
        string encoded = Uri.EscapeUriString(URL);
        StartCoroutine(getData(encoded));
        if (mObserverBehaviour)
        {
            var rendererComponents = mObserverBehaviour.GetComponentsInChildren<Renderer>(true);
            var colliderComponents = mObserverBehaviour.GetComponentsInChildren<Collider>(true);
            var canvasComponents = mObserverBehaviour.GetComponentsInChildren<Canvas>(true);

            // Enable rendering:
            foreach (var component in rendererComponents)
                component.enabled = true;

            // Enable colliders:
            foreach (var component in colliderComponents)
                component.enabled = true;

            // Enable canvas':
            foreach (var component in canvasComponents)
                component.enabled = true;
        }

        OnTargetFound?.Invoke();
    }

    protected virtual void OnTrackingLost()
    {
        Found.transform.parent = null;
        Name.transform.parent = null;
        Description.transform.parent = null;
        Employer.transform.parent = null;
        Work.transform.parent = null;
        if (mObserverBehaviour)
        {
            var rendererComponents = mObserverBehaviour.GetComponentsInChildren<Renderer>(true);
            var colliderComponents = mObserverBehaviour.GetComponentsInChildren<Collider>(true);
            var canvasComponents = mObserverBehaviour.GetComponentsInChildren<Canvas>(true);

            // Disable rendering:
            foreach (var component in rendererComponents)
                component.enabled = false;

            // Disable colliders:
            foreach (var component in colliderComponents)
                component.enabled = false;

            // Disable canvas':
            foreach (var component in canvasComponents)
                component.enabled = false;
        }

        OnTargetLost?.Invoke();
    }
    public static string res = "";
    public IEnumerator getData(string uri)
    {
        Debug.Log("Processing Website");

        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Call/Request website and wait to finish download

            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log("No response from website");
            }
            else
            {
                string intresponse = webRequest.downloadHandler.text;
                Debug.Log(intresponse);
                setRes(intresponse);
                processResponse(intresponse);
                
                RaduBRusu = GameObject.Find("Radu B Rusu");

                ScanScript.Text();

                FoundScript.SetParent(RaduBRusu);
                FoundScript.Text();
                
                NameScript.SetParent(RaduBRusu);
                NameScript.Text(intresponse);

                DescriptionScript.SetParent(RaduBRusu);
                DescriptionScript.Text(intresponse);

                EmployerScript.SetParent(RaduBRusu);
                EmployerScript.Text(intresponse);

                WorkScript.SetParent(RaduBRusu);
                WorkScript.Text(intresponse);

            }
        }
    }

    public static void setRes(string intresponse)
    {
        res = intresponse;

    }

    public static string getRes()
    {
        return res;

    }
    private void processResponse(string responsetext)
    {
        //if you get a json response you neet to implement the json handler: 
        //jsonData respdata = JsonUtility.FromJson<jsonData>(responsetext);


        //Handler to go through result list
        if (responsetext.Contains("Radu B. Rusu"))
        {
            Debug.Log("Researcher found");
        }
        else
        {
            Debug.Log("Researcher not found");
        }

    }


}
