using UnityEngine;
using System.IO;
using System;
using System.Text;
using System.Threading;
using System.Collections;

public class CoilController : MonoBehaviour
{
    private bool firstBuild = true;
    private PreferenceLoader pl;

    //stream
    private CoilStream clstream;

    //individual components in degrees/s of the quyaternion speed vectors MFW - IT WOULD BE BETTER TO CALL THIS ANGULAR VELOCITY
    public Vector3 angularVelocityRead = new Vector3();

    //streaming time 
    public UInt32 streamSample;

    // public Quaternion oldRotation;
    private StreamWriter file;

    //quaternion representation of angular velocity in radians 
    public Quaternion currentRotation;

    // quaternion for zero position
    private Quaternion referenceOrientation;

    //the Vector3 degrees representation of the angular velocity taken from the differentiation of the quaternion
    //public Vector3 angularVelocity;

    public int logCounter;
    public int logCounterLimit = 14400; // 60 sec * 240 Hz
    private bool logging;
    private StringBuilder logger = new StringBuilder();

    public int startCounter = 0;
    private bool alreadyWritten;

    //stopwatch to keep track of time through system rather than unity
    private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    
    public float time;
    //reference to record DynamicAcuityController decisions
    public DynamicAcuityController dynamicReference;
    public string speedEvaluationHash = null;

    // Use this for initialization
    void Awake()
    {
        // get the stream component
        clstream = GetComponent<CoilStream>();
        if (GameObject.Find("PreferenceLoader") == null)
        {
            GameObject obj = Instantiate(Resources.Load("PreferenceLoader")) as GameObject;
            obj.name = "PreferenceLoader";
        }
        pl = GameObject.Find("PreferenceLoader").GetComponent<PreferenceLoader>();
        if (pl.gametype == 1)
        {
            file = new StreamWriter("logspeedMonitor" + String.Format("{0:_yyyy_MM_dd_hh_mm_ss}", DateTime.Now) + ".txt");
        }
    }

    void Start()
    {
        // initializes arrays, fixes positions
        referenceOrientation = new Quaternion();
        currentRotation = new Quaternion();

        // Initialize reference to zero position
        // This will be updated by FixedUpdate
        // not sure if this is necessary
        referenceOrientation.w = 1f;
        referenceOrientation.x = 0f;
        referenceOrientation.y = 0f;
        referenceOrientation.z = 0f;

        //counter initialization
        logCounter = 0;

        //logging  // ??
        startCounter = 0;
        // logStart = 0;
        logCounterLimit = -1; // turn off raw data logging

        //intializing readable quaternions
        //angularVelocity = new Vector3();

        //initialize stopwatch
        stopwatch.Start();

        //threading clauses
        threadGo = true;

    }

    //Threading utilites
    private Thread _thread1;
    private Thread _thread2;
    // private Mutex _mutex1 = new Mutex();
    // private Mutex _mutex2 = new Mutex();

    private bool threadGo = false;

    //does cleanup whenever application is closed
    void OnApplicationQuit()
    {
        Debug.Log("aborting");
        threadGo = false;
        if (file != null)
            file.Close();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            //Application.Quit();
        }
    }



    // called before performing any physics calculations
    void FixedUpdate()
    {
        /*if (firstBuild)
        {
            //this.zero();
            this.calibrate();
            firstBuild = false;
            //Debug.Log("");
        }*/
    }


    //Main Thread
    //Called in separate process: Child of PlStream job
    public void VelocityThread2()
    {
        //internal variables
        float deltaTime = 0f;
        double currentTimestep = 0;

        //thread variables
        // int i = 0;
        Quaternion coil_orientation;

        if (threadGo)
        {
            stopwatch.Stop();
            currentTimestep = stopwatch.Elapsed.TotalSeconds;
            stopwatch.Reset();
            stopwatch.Start();
            //deltaTime += (float)(currentTimestep);
            deltaTime = (float)currentTimestep;
            // lastTime = (float)currentTimestep;
            //Stopwatch reset


            if (deltaTime >= .001f)
            {
                
               /*
                transfers plstream translated bit information to unity engine parameters
                */
                //pol_position = plstream.positions[i] - prime_position;
                coil_orientation = clstream.currentHeadOrientation; //-calibrate_rotation;

                // Convert quaternion in coil coordinates to Unity coordinates
                // Coil is RHR: X forward, Y left, Z up
                // Unity is LHR: X right, Y up, Z forward
                /*currentRotation.w = coil_orientation[0];
                currentRotation.x = coil_orientation[2]; // Unity X = -coil Y
                currentRotation.y = coil_orientation[3];  // Unity Y = coil Z
                currentRotation.z = coil_orientation[1];  // Unity Z = coil X*/

                currentRotation = new Quaternion(coil_orientation.y, -coil_orientation.z,
                                                        coil_orientation.x, coil_orientation.w);

                //recalibrated rotation
                currentRotation = currentRotation * Quaternion.Inverse(referenceOrientation);

                angularVelocityRead.x = -clstream.currentHeadVelocity[1]; // Unity X = -coil Y
                angularVelocityRead.y = clstream.currentHeadVelocity[2]; // Unity Y = coil Z
                angularVelocityRead.z = clstream.currentHeadVelocity[0]; // Unity Z = coil X
                streamSample = clstream.simulinkSample;
                logger.Append(deltaTime.ToString() + "\t");
                logger.Append(streamSample.ToString() + "\t");
                logger.Append(currentRotation.w + "\t");
                logger.Append(currentRotation.x + "\t");
                logger.Append(currentRotation.y + "\t");
                logger.Append(currentRotation.z + "\t");
                logger.Append(angularVelocityRead.x + "\t");
                logger.Append(angularVelocityRead.y + "\t");
                logger.Append(angularVelocityRead.z + "\t ");
                logger.AppendLine();
                time = deltaTime;
                deltaTime = 0;

            }
        }
        return;
    }

    //method for writing logs into a StringBuilder; Will log all desired data that has been read from the polhemus on a frame-by-frame basis
    //method should be called once per frame as it will only write single line
    public IEnumerator logMonitorData(bool judge)
    {
        file.WriteLine("DeltaTime\tStreamSample\tHeadRotationX\tHeadRotationY\tHeadRotationZ\tHeadSpeedX\tHeadSpeedY\tHeadSpeedZ\t");
        file.WriteLine(logger.ToString());
        file.Close();
        logger = new StringBuilder();
        yield return new WaitForSeconds(0.1f);
    }

  
  

    // sets reference orientation to current value 
    public void calibrate()
    {
        Quaternion zeroOrientation = new Quaternion();
        zeroOrientation = clstream.currentHeadOrientation;
         
        // referenceOrientation is the quaternion of the reference (zero) position 
        referenceOrientation.w = zeroOrientation[0];
        referenceOrientation.x = -zeroOrientation[2]; // Unity Y = coil Z
        referenceOrientation.y = zeroOrientation[3];  // Unity Y = coil Z
        referenceOrientation.z = zeroOrientation[1];  // Unity Z = coil X

    }

    
    //gets the 3 component angular rotation from the Quaternion differentiation
    public Vector3 getQuaternionRotationalSpeed()
    {

        return new Vector3();
    }

    //returns the most recent velocity calculated for the three vector components
    public Vector3 getVelocity()
    {
        return angularVelocityRead;
    }

    public void QuitStream()
    {
        Debug.Log("aborting");
        //file.WriteLine(logger.ToString());
        threadGo = false;
        if (file != null)
        {
            file.Close();
        }
    }
    ////changes the velocityHistory size to desired length 
    //public void changeVelocityHistory(int size)
    //{
    //    this.velocityHistorySize = size;
    //    velocityReadHistory = new Queue<Vector3>();
    //}


}