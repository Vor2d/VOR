using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR;
//top level wrapper class to retrieve all data information from any data stream typing
//*** To Use Instructions:
//Put in same object as the VRController, CoilController, or Quaternion Controller Script 
//in the scene, then set the source field in the script to correct type: VR, Coil, or Quaternion respectively
public class DataSource : MonoBehaviour {
    //QuaternionController qController;
    CoilController coilController;
    VRController vrController;
    QuaternionController qController;

    //TODO insert callers to retrieve values from assigned controller
    public Vector3 angularVelocityRead = new Vector3();
    public Quaternion currentRotation = new Quaternion();
    public Vector3 currentPosition = new Vector3();
    public DynamicAcuityController dynamicReference;
    public PreferenceLoader pl;
    public UInt32 streamSample;
    public string fixedtime;
    [SerializeField]
    private int source;
    private int resource = 0;

    //variables used for loggin
    public string speedEvaluationHash = null;

    private void Awake()
    {
        pl = GameObject.Find("PreferenceLoader").GetComponent<PreferenceLoader>();
    }

    // Use this for initialization
    void Start () {

        source = pl.gametype;
        initializeSource(source);
    }

    public void initializeSource(int sourceType)
    {
        if (source == 0)
        {
            vrController = GetComponent<VRController>();
        }
        else if (source == 1)
        {
            coilController = GetComponent<CoilController>();
        }
        else if (source == 2)
            qController = GetComponent<QuaternionController>();
    }
	
	// Update is called once per frame
	void Update () {
        if (source == 2)
        {
            pullQuaternionControllerData();
        }
        else if (source == 1)
        {
            pullCoilControllerData();
        }
        else if (source == 0)
        {
            pullVRControllerData();
        }
    }

    void pullQuaternionControllerData()
    {
        currentRotation = qController.getRotation(); 
        angularVelocityRead = qController.angularVelocityRead;
        qController.speedEvaluationHash = speedEvaluationHash; 
    }

    void pullCoilControllerData()
    {
        currentRotation = coilController.currentRotation;
        angularVelocityRead = coilController.angularVelocityRead;
        coilController.speedEvaluationHash = speedEvaluationHash;
        streamSample = coilController.streamSample;
    }
    
    void pullVRControllerData()
    {
        currentRotation = vrController.currentRotation;
        currentPosition = vrController.currentPosition;
        angularVelocityRead = vrController.angularVelocityRead;
        vrController.speedEvaluationHash = speedEvaluationHash;
        fixedtime = vrController.fixedtime;
    } 
    public void QuitStream()
    {
        if (source == 2)
        {
            //qController.QuitStream();
        }
        else if (source == 1)
        {
            coilController.QuitStream();
        }
        else if (source == 0)
        {
            vrController.QuitStream();
        }
    }


    public void calibrate()
    {
        if (source == 2)
        {
            qController.calibrate();
        }
        else if (source == 1)
        {
            coilController.calibrate();
        }
        else if (source == 0)
        {
            //InputTracking.Recenter();
        }
    }


    public enum Source
    {
        VR = 0,  Coil = 1, Polhemus = 2, None = 3
    }
}
