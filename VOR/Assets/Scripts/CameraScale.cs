using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System;

public class CameraScale: MonoBehaviour {

    //public GameObject main_camera;
    private PreferenceLoader pl;
    private DataSource dataSource;
    private GameObject optotype;
    public Quaternion LeftAnchor;
    public Vector3 LeftError;
    public Quaternion RightAnchor;
    public Vector3 RightError;
    public GameObject BlackCurtain;
    public GameObject BackGround;
    private bool errorflag = false;
    public Quaternion centerRotation;
    public Quaternion tempRotation;
    public Vector3 rotation;
    public float DeltaAngleY;
    public float deltaY;
    public float UnityperCm;
    private Quaternion Qdelta;
    private Vector3 tempVec;
    public float DeltaAngle;
    private Vector3 temp;
    private float temp1;
    public GameObject RecenterPoint;
    private float DeltaDistance;
    private float CurrentDistance;
    private float LastDistance;
    private Quaternion LastRotation;
    private Vector3 lastPosition;
    private Vector3 currentPosition;
    public Transform target;
    private float DeltaDistanceX;
    private float DeltaDistanceZ;

    // Use this for initialization
    void Awake () {
        if (GameObject.Find("PreferenceLoader") == null)
        {
            GameObject obj = Instantiate(Resources.Load("PreferenceLoader")) as GameObject;
            obj.name = "PreferenceLoader";
        }
        pl = GameObject.Find("PreferenceLoader").GetComponent<PreferenceLoader>(); // Get PreferenceLoader script which stores all user setting.
        optotype = GameObject.Find("OptotypeE");
        if(optotype != null)
        {
            dataSource = optotype.GetComponent<DataSource>();
        }
        else
        {
            Debug.Log("Null");
        }
        if (pl.gametype == 1)
        {
            BackGround.transform.position = new Vector3(0, 0, 0);
        }
        centerRotation = RecenterPoint.transform.rotation;
        //Application.targetFrameRate = 30;
        UnityperCm = 120.0f / 121.0f;
        LastDistance = 0.0f;
        LastRotation = new Quaternion(0, 0, 0, 0);
        lastPosition = BackGround.transform.position;
        //Debug.Log("Screen height : " + Screen.height);
        //Debug.Log("Screen Width : " + Screen.width);
    }
	
	// Update is called once per frame
	void Update () {
        tempRotation = dataSource.currentRotation;
        //Debug.Log(UnityperCm);

        if (pl.gametype == 0)
        {
            Qdelta = tempRotation * Quaternion.Inverse(LastRotation);
            //Debug.Log(centerRotation);
            Qdelta.ToAngleAxis(out DeltaAngle, out tempVec);
            DeltaAngleY = DeltaAngle * tempVec.y;//tempRotation.eulerAngles.y - lastRotation.eulerAngles.y;
                                                 /*tempRotation.ToAngleAxis(out temp1, out temp);
                                                 rotation = new Vector3 (temp1 * temp.x, temp1 * temp.y, temp1 * temp.z);*/
            CurrentDistance = Mathf.Tan(DeltaAngleY * Mathf.PI / 180) * (pl.patientToScreenDistance);
            DeltaDistance = CurrentDistance;
            //DeltaDistanceX = Mathf.Sin(DeltaAngleY * Mathf.PI / 180) * (pl.patientToScreenDistance);
            //DeltaDistanceZ = (pl.patientToScreenDistance) - Mathf.Cos(DeltaAngleY * Mathf.PI / 180) * (pl.patientToScreenDistance);
            currentPosition = target.transform.position;
        }
        if (pl.gametype == 1)
        {
            Qdelta = tempRotation * Quaternion.Inverse(centerRotation);
            //Debug.Log(centerRotation);
            Qdelta.ToAngleAxis(out DeltaAngle, out tempVec);
            DeltaAngleY = DeltaAngle * tempVec.y;//tempRotation.eulerAngles.y - lastRotation.eulerAngles.y;
                                                 /*tempRotation.ToAngleAxis(out temp1, out temp);
                                                 rotation = new Vector3 (temp1 * temp.x, temp1 * temp.y, temp1 * temp.z);*/
            //CurrentDistance = Mathf.Tan(DeltaAngleY * Mathf.PI / 180) * (pl.patientToScreenDistance); //flat screen calculation;
            CurrentDistance = (DeltaAngleY * Mathf.PI / 180) * (pl.patientToScreenDistance); //curve screen calculation with a proper radius;
            DeltaDistance = CurrentDistance - LastDistance;
        }

        //Debug.Log(DeltaDistance);

        if (pl.gametype == 0)
        {
            if (pl.duogain == 0)
            {
                if (!BlackCurtain.activeSelf)
                {

                    if (DeltaDistance < -0.01f)
                    {
                        BackGround.transform.RotateAround(Vector3.zero, Vector3.up, DeltaAngleY * pl.rightGain);
                    }
                }
                else
                {
                    BackGround.transform.localPosition = new Vector3(0, 0, pl.patientToScreenDistance);
                }
            }
            if (pl.duogain == 1)
            {
                if (!BlackCurtain.activeSelf)
                {
                    if (DeltaDistance > 0.01f)
                    {
                        BackGround.transform.RotateAround(Vector3.zero, Vector3.up, DeltaAngleY * pl.leftGain);
                    }
                }
                else
                {
                    BackGround.transform.localPosition = new Vector3(0, 0, pl.patientToScreenDistance);
                }
            }
            if (pl.duogain == 2)
            {
                if (!BlackCurtain.activeSelf)
                {
                    if (DeltaDistance < -0.01f)
                    {
                        BackGround.transform.RotateAround(Vector3.zero, Vector3.up, DeltaAngleY * pl.rightGain);
                    }
                    if (DeltaDistance > 0.01f)
                    {
                        BackGround.transform.RotateAround(Vector3.zero, Vector3.up, DeltaAngleY * pl.leftGain);
                    }
                }
                else
                {
                    BackGround.transform.localPosition = new Vector3(0, 0, 0);
                }
            }
        }
        if (pl.gametype == 1)
        {
            if (pl.duogain == 0)
            {
                if (!BlackCurtain.activeSelf)
                {

                    if (DeltaDistance < -0.01f)
                    {
                        BackGround.transform.position += new Vector3(pl.leftGain * DeltaDistance * UnityperCm, 0, 0);
                    }
                }
                else
                {
                    BackGround.transform.position += new Vector3(0, 0, 0);
                }
            }
            if (pl.duogain == 1)
            {
                if (!BlackCurtain.activeSelf)
                {
                    if (DeltaDistance > 0.01f)
                    {
                        BackGround.transform.position += new Vector3(pl.rightGain * DeltaDistance * UnityperCm, 0, 0);
                    }
                }
                else
                {
                    BackGround.transform.position += new Vector3(0, 0, 0);
                }
            }
            if (pl.duogain == 2)
            {
                if (!BlackCurtain.activeSelf)
                {
                    if (DeltaDistance > 0.01f)
                    {
                        //deltaY = Mathf.Tan((Mathf.Ceil(DeltaAngleY * 10) / 10) * Mathf.PI / 180) * (pl.patientToScreenDistance);
                        //deltaY = Mathf.Tan(DeltaAngleY * Mathf.PI / 180) * (pl.patientToScreenDistance);
                        BackGround.transform.position += new Vector3(pl.rightGain * DeltaDistance * UnityperCm, 0, 0);
                        Debug.Log("Left" + pl.rightGain + " | " + BackGround.transform.position.x);
                        //lastRotation = tempRotation;
                    }
                    if (DeltaDistance < -0.01f)
                    {
                        // deltaY = -Mathf.Tan((Mathf.Ceil(Mathf.Abs(DeltaAngleY) * 10)/10) * Mathf.PI / 180) * (pl.patientToScreenDistance);
                        //deltaY = Mathf.Tan(DeltaAngleY * Mathf.PI / 180) * (pl.patientToScreenDistance);
                        BackGround.transform.position += new Vector3(pl.leftGain * DeltaDistance * UnityperCm, 0, 0);
                        //Debug.Log(Screen.width);
                        Debug.Log("Right" + pl.leftGain + " | " + BackGround.transform.position.x);
                        //lastRotation = tempRotation;
                    }
                }
                else
                {
                    BackGround.transform.position = new Vector3(0, 0, 0);
                }
            }
        }
        //polhemus;
        /*if (pl.gametype == 2)
        {
            if (pl.duogain == 0)
            {
                if (!BlackCurtain.activeSelf)
                {
                    if (DeltaAngleY <= 0f)
                    {
                        deltaY = Mathf.Tan(tempRotation.eulerAngles.y * Mathf.PI / 180) * (pl.patientToScreenDistance);
                        BackGround.transform.position = new Vector3(pl.leftGain * deltaY * UnityperCm, 0, 0);
                        //lastRotation = tempRotation;
                    }
                }
                else
                {
                    BackGround.transform.position = new Vector3(0, 0, 0);
                }
            }
            if (pl.duogain == 1)
            {
                if (!BlackCurtain.activeSelf)
                {
                    if (DeltaAngleY >= 0f)
                    {
                        deltaY = Mathf.Tan(tempRotation.eulerAngles.y * Mathf.PI / 180) * (pl.patientToScreenDistance);
                        BackGround.transform.position = new Vector3(pl.rightGain * deltaY * UnityperCm, 0, 0);
                        //lastRotation = tempRotation;
                    }
                }
                else
                {
                    BackGround.transform.position = new Vector3(0, 0, 0);
                }
            }
            if (pl.duogain == 2)
            {
                if (!BlackCurtain.activeSelf)
                {
                    if (DeltaAngleY <= 0.05f)
                    {
                        deltaY = Mathf.Tan(DeltaAngleY * Mathf.PI / 180) * (pl.patientToScreenDistance);
                        BackGround.transform.position += new Vector3((pl.leftGain * deltaY * UnityperCm), 0, 0);
                        //Debug.Log("Left"+ pl.leftGain+" | " + BackGround.transform.position.x);
                        //lastRotation = tempRotation;
                    }
                    if (DeltaAngleY >= 0.05f)
                    {
                        deltaY = Mathf.Tan(DeltaAngleY * Mathf.PI / 180) * (pl.patientToScreenDistance);
                        BackGround.transform.position += new Vector3((pl.rightGain * deltaY * UnityperCm), 0, 0);
                        //Debug.Log(Screen.width);
                        //Debug.Log("Right"+ pl.rightGain + " | "+ BackGround.transform.position.x);
                        //lastRotation = tempRotation;
                    }
                }
                else
                {
                    BackGround.transform.position = new Vector3(0, 0, 0);
                }
            }
        }*/
        centerRotation = RecenterPoint.transform.rotation;
        LastDistance = CurrentDistance;
        LastRotation = tempRotation;
        lastPosition = currentPosition;
    }
}
