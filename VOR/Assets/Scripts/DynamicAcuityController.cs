using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text;
using System.IO;
using System.Linq;
using UnityEngine.XR;
using System.Threading;


public class DynamicAcuityController : MonoBehaviour
{

    //references to Unity Values
    private static readonly float rotationUpperBound = 360f;

    //mode to use the script in
    [SerializeField]
    private EnumTypes.GameMode gameMode;
    private static float _ControllerThreshold = 0.7f;
    private float timer;
    private float timer1;
    private float timer2;
    private bool optotype_showed_once;
    private bool speedvalidated;
    private bool recordtime;
    private float startTime;
    private float returnTime;
    private bool keypressed;
    private bool ballrecentered;
    private bool resettime;
    private bool joystickinput = false;
	private bool headturned = false;
	private bool headstopped = false;

    //Most important: Connection to the data stream
    static public DataSource dataSource;  // get Controller script for streamed coil data
    public VRController vrController;
    public CoilController coilController;
    //System.Diagnostics.Stopwatch fixtimecounter = new System.Diagnostics.Stopwatch();



    // Public Variables
    public float speedThreshold;
    public float speedDifferenceMax = 40;
    public float speedDifferenceMin = 25;
    private float PeakVelocityMax = 180f;
    private float PeakVeclocityMin = 140f;
    private string speedMessage;
    private float TimeOutAngle = 80f;
    private float TimeOutTime = 2f;
	private int JumpoutLength = 15;
	private int SameCountLenght = 5;
    private Sprite Optotype;
    private SpriteRenderer SpriteR;
    public GameObject recabdot;
    public GameObject background;
    public GameObject gloves;
    public GameObject soccer;
    public GameObject dot;
	public GameObject HeadDot;
    public GameObject canvas;
    public GameObject blackout;
    public GameObject RecenterPoint;
    public GameObject DecisionMaking;
    public SpriteRenderer DR;
    public Text savesText;
    public Text goalsText;
    public Text response;
    public Text warning;
    public int init;
    public bool speedflag = true;
    public string fixedtime;
    public int textshowtime;
    public bool optotypedisplayed;
    public bool responded;
    public float waitingtime = 0.5f;
    public Text response1;
    public Text VA;
    public Text speed;
    public float BackGroundResizeRatio;



    //public setttings for new trial head recentering windows
    public float headRecenteringThreshold = 15f;
    public float evaluationRotation = 0f;


    // Private Variables
    private float headSpeed; // Rotational head speed in desired head rotation direction
    private float lookbackSpeed; // lookback speed in desired head rotation direction
    private string fixtime;
    private System.Timers.Timer _delayTimer;

    // private float initSpeedThresMin;
    // private float initSpeedThresMax;
    // private float speedThresStepMin;
    // private float speedThresStepMax;
    // private float lastSpeedThresMin;
    // private float lastSpeedThresMax;
    private float lastOptotypeSize;
    private float decimalScale;    
    private float optotypeShowDelayTime;
    private Vector3 angularVelocity;

    // translationOnScreen - Linear distance on screen that corresponds to current head orientation
    private Vector3 translationOnScreen = Vector3.zero; 
    private float pixels2World; // used for calculating translationOnScreen
    private const int optotypeNumber = 8;
    private MeshRenderer[] meshRenderer; // array of MeshRenderers for all elements in Optotype prefab
    private bool showTestInfo = false;
    private bool evaluationStart = false; // For enable/disable test mode
    private bool paused = false; // For enable/disable input
    private bool DVATestStart = false;
    private string testInfo = "";
    private float PeakVelocity = 0f;

    //Persistent Variables
    private int optotypeSize;
    public static float logmarScale = 1.0f;
    //public PreferenceLoader pl;

    //TODO 
    //opptotype[randomkey] represents the direction/orientation of the correct choice for the current trial
    private string[] optotypeDirection = new string[optotypeNumber];
    private int randomKey = 0;
    private bool showOptionButtons = false;
    private bool animationIsOn = false;
    private bool showHUD = true;
    private float ballAnimationTime = 1f;
    private float moveBackTime = 1f;
    private int saves;
    private int goals;
    private PreferenceLoader pl;
    private string keyClicked;
    private Vector3 lastMousePosition = Vector3.zero;
    private ArrayList directionIndicator;
    private string directionIndicatorLabel;
    private int optotypeShowTimesRemaining;
    private Vector3 glovesInitPosition;
    private float ballSpeed = .1f;
    private bool movebackFinish = false; // indicated whether a moveBack() coroutine is finished
    private bool recalibrateTimeUp = false; // check if time is up in recalibrate() function
    private float bestDVA;
    private int bestDVACounter;
    private int bestDVAReset;
    private float backgroundScale;
    private string info;
    private bool arrowDisappear;
    private float accelerationComponent;
    public GameObject savebox;
    public GameObject goalbox;
    public Text LastThreePeakVelocities;
    private string PeakVelocityDisplay;
    private string Save_Goal;
    // private float maxHeadSpeed;
    // private float maxHeadAcc;

    // Buttons
    // for single screen play
    /*
    private Rect RectMainMenu = new Rect(Screen.width / 30, Screen.height / 25, Screen.width / 15, Screen.height / 20); 
    private Rect RectRecalibrate = new Rect(Screen.width / 30 + Screen.width / 15 + 10, Screen.height / 25, Screen.width / 15, Screen.height / 20);
    private Rect RectLogResult= new Rect(Screen.width / 30 + Screen.width * 2 / 15 + 20, Screen.height / 25, Screen.width / 15, Screen.height / 20);
    */
    private Rect RectMainMenu = new Rect(Screen.width / 2 - Screen.width / 8, Screen.height * 2 / 6, Screen.width / 8, Screen.height / 10);
    private Rect RectRecalibrate = new Rect(Screen.width / 2 - Screen.width / 8, Screen.height * 3 / 6, Screen.width / 8, Screen.height / 10);
    private Rect RectLogResult = new Rect(Screen.width / 2 - Screen.width / 8, Screen.height * 4 / 6, Screen.width / 8, Screen.height / 10);
    // for dual screen
    /*GameObject Cube1Hid;
    GameObject Cube2Hid;*/

    //conditional String to show if movement too fast or slow
    private string speedEvaluationMessage = "";

    //switch to say if a new trial needs to be restared
    private bool needNewTrial = false;

    //Logging variables
    private StringBuilder trialLogger;
    private StringBuilder speedLogger;
    private bool alreadyLogged;
    private int logFileCounter = 0;
    private int rawDataLogCounter = 0;
    private bool endtrial = false;
    //holding values for trial logging
    private int speedEvaluationRecord = 0;
    private int WindowCounter = 0;
    private int CorrectCounter = 0;

    // local class variables for current and past head velocity for readFromCoil()
    public Vector3 currentHeadVelocity; // this is the current head velocity that is pulled from the stream
    private Vector3 pastHeadVelocity; // this is the lookback velocity that is pulled from the velocity queue
    private string currentfixedtime;
    private Queue<Vector3> headVelocityHistory;  // this stores recent head velocity for lookback
    // lookbackWindow is the number of frames to look back when calculating the acceleration window checks
    // To do this, it is the capacity of the headVelocityHistory queue.
    private int lookbackWindow = 10;

    private uint streamSampleAtOptotypeAppearance;
    private uint streamSample;

    public string trialLogFile;
    public string speedfile;
    private bool recentered = false;
    private bool arrowshowed;
    private bool jumpout;
    private bool SavePeak;
    private bool optotypeInit;
    private int correct;
    private int[] CorrectOpt = new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
    private int[] InCorrectOpt = new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
    private float[] lgit = new float[12] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f};
    private Queue<float> LastThreePeaks = new Queue<float>();
    private int nOpt;
    private int last_nOpt;
    private float L;
    private float Lmax;
    private float VAmax;
    private int nMax;
    private int sameCount;
    private int OptotypeResult;
    private Queue<int> Allinputs = new Queue<int>();
    private Queue<int> Unique;
    private int[] UniqueList;
    private int[] AllList;

    public AudioClip Kick;
    public AudioClip Miss;
    public AudioClip Catch;
    public AudioClip Crowd;
    public AudioSource KickSource;
    public AudioSource ResponseSource;
    public AudioSource CrowdSource;
    private bool PlayResponse;

    // Use this for initialization
    void Start()
    {
        //Display.displays[1].Activate();
        optotypeInit = false;
        //Debug.Log(gameMode);
        Time.fixedDeltaTime = 1.0f / 240.0f;
        warning = GameObject.Find("warning").GetComponent<Text>();
        SpriteR = GetComponent<SpriteRenderer>();
        DR = GameObject.Find("DecisionMaking").GetComponent<SpriteRenderer>();
        //Cube1Hid = GameObject.Find("Cube1Hid");
        //Cube2Hid = GameObject.Find("Cube2Hid");
        //vrController = GetComponent<VRController>();
        pl = StartingSceneCanvas.pl;
        //To Do
        optotypeShowDelayTime = (float)pl.optotypeShowDelayTime / 1000;
        optotypeSize = pl.optytpeSizeChoice;
        pl.objectSizeScalerForStandardVision = PreferenceLoader.tanFiveOverSixty * (pl.patientToScreenDistance);
        //Debug.Log(String.Format("Screen DPI:{0}, StandardVisionSize{1}: ", Screen.dpi, pl.objectSizeScalerForStandardVision));
        nOpt = 5;
        saves = 0;
        goals = 0;
        optotypeDirection[0] = "Right";
        optotypeDirection[1] = "UpRight";
        optotypeDirection[2] = "Up";
        optotypeDirection[3] = "UpLeft";
        optotypeDirection[4] = "Left";
        optotypeDirection[5] = "DownLeft";
        optotypeDirection[6] = "Down";
        optotypeDirection[7] = "DownRight";
        randomKey = UnityEngine.Random.Range(0, optotypeNumber);
        /*if(optotypeSize == 0)
        {
            this.transform.Rotate(new Vector3(0, 0, 90) * randomKey);
        }
        else
        {
            this.transform.Rotate(new Vector3(0, 0, 45) * randomKey); // randomly chooses the optotype orientation
        }*/
        //this.transform.Rotate(new Vector3(0, 0, 45) * randomKey);
        //Debug.Log(randomKey);
        if (optotypeSize == 0)
        {
            //Debug.Log("zero statement");
            this.transform.Rotate(new Vector3(0, 0, 90) * (randomKey/2));
        }// randomly chooses the optotype orientation
        else
        {
            //Debug.Log("Non-zero:" + randomKey);
            this.transform.Rotate(new Vector3(0, 0, 45) * randomKey);
        }
        directionIndicator = new ArrayList(); // stores direction sign objects which are created and destroyed at runtime
        dot.SetActive(false);
        dot.transform.position = Vector3.zero;
        glovesInitPosition = gloves.transform.localPosition;
        gloves.GetComponent<SpriteRenderer>().enabled = false;
        pixels2World = (Camera.main.orthographicSize * 2.0f) / Camera.main.pixelHeight; // screen space to world space ratio

        headVelocityHistory = new Queue<Vector3>();
        if (pl.gametype == 1)
        {
            background.transform.position = new Vector3(0, 0, 0);
        }
        
        //switch for how to set logmar initially depending on the mode
        /*if (gameMode != EnumTypes.GameMode.Game)
        {
            optotypeSize = pl.optotypeSize[4];//pl.objectSizeScalerForStandardVision * 10;
        }
        else // MFW - even for game mode, initial acuity should be what is set in the preferences (but it should not go down to SVA)
        {
            //decimalScale = Mathf.Pow(10, logmarScale);
            if (pl.DeterminedOptotypeSize != 0f)
            {
                optotypeSize = pl.DeterminedOptotypeSize;
            }
            else
            {
                optotypeSize = pl.optotypeSize[4];
            }//pl.objectSizeScalerForStandardVision * 10;
        }*/
        //Debug.Log(decimalScale);
        //Debug.Log(pl.objectSizeScalerForStandardVision);
        //Debug.Log(optotypeSize);

        // WHAT DOES THIS DO?
        //background scale?
        //backgroundScale = background.transform.localScale.x;
        //this.transform.localScale = new Vector3(optotypeSize, optotypeSize, optotypeSize) /backgroundScale;
                
        bestDVA = logmarScale;
        bestDVACounter = 0;
        bestDVAReset = 0;
        
        Lmax = 0f;
        VAmax = 0.5f;
        nMax = 0;
        sameCount = 0;
        arrowDisappear = false;

        //headSpeedWindow Threshold and settings for lookback
        speedThreshold = pl.dvaHeadSpeedTriggerThreshold;
        speedDifferenceMin = pl.dvaLowerHeadSpeedWindow;
        speedDifferenceMax = pl.dvaUpperHeadSpeedWindow;

        //history settings
        lookbackWindow = pl.dvaLookBackAmount;


        vrController = GetComponent<VRController>();
        coilController = GetComponent<CoilController>();
        //Polhemus Controller Link
        dataSource = GetComponent<DataSource>();
        dataSource.dynamicReference = this;
        // coilController.changeVelocityHistory(lookbackWindow);

        //Logging setup
        trialLogger = new StringBuilder();
        speedLogger = new StringBuilder();
        // rawDataLogger = new StringBuilder();
        //BackGroundResizeRatio = 2;//Screen.width / 1920.0f;
        //background.transform.localScale = background.transform.localScale * BackGroundResizeRatio;
        background.transform.position = recabdot.transform.position;
        background.transform.rotation = recabdot.transform.rotation;
        //canvas.transform.localScale = canvas.transform.localScale / BackGroundResizeRatio;
        //canvas.transform.localPosition = Vector3.zero;
        ballrecentered = false;
        keypressed = false;
        recordtime = false;
        resettime = false;
        speedvalidated = false;
        optotype_showed_once = false;
        timer = 0;
        optotypedisplayed = false;
        responded = false;
        recentered = false;
        arrowshowed = false;
        correct = 0;
        blackout.SetActive(false);
        //fixtimecounter.Start();
        savebox.SetActive(false);
        goalbox.SetActive(false);
        DecisionMaking.SetActive(false);
        recalibrate();
        jumpout = false;
        SavePeak = false;
        if(optotypeSize == 0)
        {
            if(randomKey%2 == 1)
            {
                Optotype = Resources.Load<Sprite>("Sprites/Transparant Cs/rotate");
            }
            else
            {
                Optotype = Resources.Load<Sprite>("Sprites/Transparant Cs/0");
            }
        }
        else
        {
            Optotype = Resources.Load<Sprite>("Sprites/Transparant Cs/" + optotypeSize.ToString());
        }
        //Optotype = Resources.Load<Sprite>("Sprites/Transparant Cs/" + optotypeSize.ToString());
        CrowdSource.Play();
        //this.transform.localPosition = new Vector3(0, 0, 0);
        //Debug.Log("optotype size:" + optotypeSize * 5f);
        //Debug.Log("optotype size in cm:" + optotypeSize * 5f * 38.2f / 60f);
    }

    // FixedUpdate is called once per frame
    void FixedUpdate()
    {
        Debug.Log("op" + optotypeSize);
        //Debug.Log(pl.optytpeSizeChoice);
        //speed.text = background.transform.position.z.ToString();
        //Debug.Log(currentHeadVelocity.y.ToString());
        readTracker();
        //speed.text = currentHeadVelocity.y.ToString();
        if (!optotypeInit)
        {
            if (optotypeSize == 0)
            {
                if (randomKey % 2 == 1)
                {
                    Optotype = Resources.Load<Sprite>("Sprites/Transparant Cs/rotate");
                }
                else
                {
                    Optotype = Resources.Load<Sprite>("Sprites/Transparant Cs/0");
                }
            }
            else
            {
                Optotype = Resources.Load<Sprite>("Sprites/Transparant Cs/" + optotypeSize.ToString());
            }
            //Optotype = Resources.Load<Sprite>("Sprites/Transparant Cs/" + optotypeSize.ToString());
            optotypeInit = true;
        }
        if( gameMode == EnumTypes.GameMode.Static)
        {
            blackout.SetActive(false);
        }
        if (gameMode != EnumTypes.GameMode.Static) {
			HeadDot.transform.rotation = dataSource.currentRotation;
		}
        BlackOut();
        //Debug.Log(Screen.dpi);
        savebox.SetActive(false);
        goalbox.SetActive(false);
        /*Vector3 point1 = Cube1Hid.transform.position;
        Vector3 point2 = Cube2Hid.transform.position;
        Vector2 ScreenPoint1 = Camera.main.WorldToScreenPoint(point1);
        Vector2 ScreenPoint2 = Camera.main.WorldToScreenPoint(point2);
        Debug.Log((ScreenPoint1 - ScreenPoint2).magnitude);*/
        // Wait for countdown to finish, is called every frame
        //showOptotype(true);
        if (gameMode == EnumTypes.GameMode.Calibration)
        {
            if (!jumpout)
            {
                /*terminate if the same VA has been found for five trials in a row
                or if there have been at least 10 trials going back and forth between
                two adjacent VAs, but only if one smaller is wrong*/
                /*if ((sameCount >= 3 && nOpt > 1) || (((Allinputs.Count > 9) && (Unique.Count == 2) && (UniqueList[1] - UniqueList[0] == 1)) && (nOpt > 1)))
                {
                    optotypeSize = nOpt - 1;
					jumpout = true;
                }*/
                //recenter judgement
                if (!recentered)
                {
                    recenterHeadForNewTrial(headRecenteringThreshold, Vector3.zero, pl.keepHeadSteadyTime);
                    response1.text = " ";
                    PeakVelocityDisplay = "";
                }
                if (recentered)
                {
                    if (!arrowDisappear)
                    {
                        if (!arrowshowed)
                        {
                            directionIndicatorDisplayer();
                            arrowshowed = true;
                        }
                        response.text = "";
                    }
                    // rotate direction renderer according to head configuration
                    if (headSpeed > speedThreshold * 0.4)
                    {
                        directionIndicatorDestroy();
                        JudgeHeadDirection();
                    }
                    if (arrowDisappear && !speedvalidated)//if arrow appeared and head speed reached threshold then continue this trial
                    {
                        if (validateSpeed() && !speedvalidated)//display optotype
                        {
                            responded = false;
                            if (!optotype_showed_once)
                            {
                                    showOptotype(true);
                                    optotype_showed_once = true;
                            }
                      

                            //response.text = "good";
                            optotypedisplayed = true;
                        }
                        if (!keypressed && !speedvalidated)//set optotype display time
                        {
                            timer += Time.deltaTime;
                            if (timer > 0.2)
                            {
                                showOptotype(false);
                                timer = 0f;
                            }
                        }
                        giveFeedback();
                        if (keypressed && !speedvalidated)//after user input
                        {
                            playAnimation();
                            if (!animationIsOn)
                            {
                                showHUD = true;
                                //display latest three peaks
                                if (!SavePeak)
                                {
                                    LastThreePeaks.Enqueue(PeakVelocity);
                                    if (LastThreePeaks.Count > 3)
                                    {
                                        LastThreePeaks.Dequeue();
                                    }
                                    foreach (float number in LastThreePeaks)
                                    {
                                        PeakVelocityDisplay += number.ToString("F4") + "\n";
                                    }
                                    SavePeak = true;
                                }
                                //display all GUI stuffs for a certain time (waitingtime) then reset the trial.
                                showspeedresult();
                                savesText.text = saves.ToString();
                                goalsText.text = goals.ToString();
                                savebox.SetActive(true);
                                goalbox.SetActive(true);
                                timer += Time.deltaTime;
                                if (timer > waitingtime)
                                {
                                    showOptotype(false);
                                    recenterball(Vector3.zero, pl.keepHeadSteadyTime);
                                    if (ballrecentered)
                                    {
                                        resetflags();
                                        timer = 0f;
                                    }
                                }
                            }
                        }
                    }
                    if (needNewTrial && !optotypedisplayed)//reset trial if head speed threshold is not met.
                    {

                        speedvalidated = true;
                        showHUD = true;
                        showSpeedEvaluationMessage();
                        timer1 += Time.deltaTime;
                        if (timer1 > waitingtime)
                        {
                            logTrialData(speedEvaluationMessage);
                            recentered = false;
                            needNewTrial = false;
                            arrowDisappear = false;
                            speedvalidated = false;
                            timer1 = 0f;
                            speedflag = true;
                            arrowshowed = false;
                        }
                    }
                }
            }
            else
            {
                VA.text = "VA = " + OptotypeResult.ToString();
                response.text = "Finished Testing Dynamic Acuity";
            }
        }
        else if(gameMode == EnumTypes.GameMode.Game)
        {
            if (!recentered)
            {
                recenterHeadForNewTrial(headRecenteringThreshold, Vector3.zero, pl.keepHeadSteadyTime);
                response1.text = " ";
                PeakVelocityDisplay = "";
            }
            if (recentered)
            {
                if (!arrowDisappear)
                {
                    if (!arrowshowed)
                    {
                        directionIndicatorDisplayer();
                        arrowshowed = true;
                    }
                    response.text = "";
                }
                // rotate direction renderer according to head configuration
                if (headSpeed > speedThreshold * 0.4)
                {
                    directionIndicatorDestroy();
                    JudgeHeadDirection();
                }
                if (arrowDisappear && !speedvalidated)
                {
                    // if (correctDirection() && validateSpeed())
                    if (validateSpeed() && !speedvalidated)
                    { // or use checkSpeedAndAcceleration function?
                        responded = false;
                        if (!optotype_showed_once)
                        {
                            showOptotype(true);
                            optotype_showed_once = true;
                        }
                        // pull streamSample at time of optotype appearance to be saved
                        // in trial log
                        //TODO
                        //StartCoroutine(timer(2));

                        //response.text = "good";
                        //streamSampleAtOptotypeAppearance = dataSource.streamSample;
                        optotypedisplayed = true;
                    }
                    if (!keypressed && !speedvalidated)
                    {
                        timer += Time.deltaTime;
                        if (timer > 0.2)
                        {
                            showOptotype(false);
                            timer = 0f;
                        }
                    }
                    giveFeedback();
                    if (keypressed && !speedvalidated)
                    {
                        playAnimation();
                        if (!animationIsOn)
                        {
                            showHUD = true;
                            if (!SavePeak)
                            {
                                LastThreePeaks.Enqueue(PeakVelocity);
                                if (LastThreePeaks.Count > 3)
                                {
                                    LastThreePeaks.Dequeue();
                                }
                                foreach (float number in LastThreePeaks)
                                {
                                    PeakVelocityDisplay += number.ToString("F4") + "\n";
                                }
                                SavePeak = true;
                            }
                            showspeedresult();
                            savesText.text = saves.ToString();
                            goalsText.text = goals.ToString();
                            savebox.SetActive(true);
                            goalbox.SetActive(true);
                            if (WindowCounter >= pl.OptotypeWindow)
                            {
                                Debug.Log("W =" + WindowCounter);
                                Debug.Log("C =" + CorrectCounter);
                                Debug.Log((float)CorrectCounter / WindowCounter);
                                OptotypeSizeChangeInGamePlay();
                                WindowCounter = 0;
                                CorrectCounter = 0;
                            }
                            timer += Time.deltaTime;
                            if (timer > waitingtime)
                            {
                                showOptotype(false);
                                recenterball(Vector3.zero, pl.keepHeadSteadyTime);
                                if (ballrecentered)
                                {
                                    resetflags();
                                    timer = 0f;
                                }
                            }
                        }
                    }
                }
                if (needNewTrial && !optotypedisplayed)
                {

                    speedvalidated = true;
                    showHUD = true;
                    showSpeedEvaluationMessage();
                    timer1 += Time.deltaTime;
                    if (timer1 > waitingtime)
                    {
                        logTrialData(speedEvaluationMessage);
                        recentered = false;
                        needNewTrial = false;
                        arrowDisappear = false;
                        speedvalidated = false;
                        timer1 = 0f;
                        speedflag = true;
                        arrowshowed = false;
                    }
                }
            }
        }
		else if(gameMode == EnumTypes.GameMode.Static)
        {

            if (!jumpout)
            {
               /*if ((sameCount >= 3 && nOpt >= 1) || (((Allinputs.Count > 9) && (Unique.Count == 2) && (UniqueList[1] - UniqueList[0] == 1)) && (nOpt >= 1)))
                {
                    optotypeSize = nOpt - 1;
					jumpout = true;
                }*/
                showOptotype(true);
                if (!keypressed)
                {
                    //Debug.Log (keyClicked);
                    OptotypeDisplayForDecisionMaking();
                    ControllerSettings();
                    // Here order matters, must first check whether irrelavent key is clicked
                    //Debug.Log(optotypeDirection[randomKey]);
                    if (getIrreleventKeyClicked() && joystickinput)
                    {
                        response.text = "You clicked the wrong key!";
                        keypressed = true;
                    }
                    else if (getCorrectKeyClicked() && joystickinput)
                    {
                        response.text = "You saved the ball!";
                        saves++;
                        showResponse("correct");
                        keypressed = true;
                    }
                    else if (getWrongKeyClicked() && joystickinput)
                    {
                        response.text = "You missed the ball!";
                        goals++;
                        showResponse("wrong");
                        keypressed = true;
                    }
                }
                if (keypressed)
                {
                    showOptotype(false);
                    playAnimation();
                    if (!animationIsOn)
                    {
                        showHUD = true;
                        showspeedresult();
                        savesText.text = saves.ToString();
                        goalsText.text = goals.ToString();
                        savebox.SetActive(true);
                        goalbox.SetActive(true);
                        timer += Time.deltaTime;
                        if (timer > 0.5f)
                        {
                            showOptotype(false);
                            recenterball(Vector3.zero, pl.keepHeadSteadyTime);
                            if (ballrecentered)
                            {
                                resetflags();
                                timer = 0f;
                            }
                            response.text = "";
                        }
                    }
                }
            }
            else
            {
                VA.text = "VA = " + OptotypeResult.ToString();
                response.text = "Finished Testing Static Acuity";
            }
        }
    }

    void BlackOut()
    {
        if(pl.blackout == 0)//black out = 0 means black out is active, 1 is not.
        {
            if (!recentered)
            {
                blackout.SetActive(true);
            }
            if (recentered)
            {
                blackout.SetActive(false);
            }
        }
        else
        {
            blackout.SetActive(false);
        }
    }

    void JudgeHeadDirection()
    {
        switch (pl.headDirection)
        {

            case 0: // Leftward only
                if (currentHeadVelocity.y < -40f)
                {
                    speedEvaluationMessage = "Wrong Way!";
                    directionIndicatorDestroy();
                    needNewTrial = true;
                }
                break;

            case 1: // Rightward only
                if (currentHeadVelocity.y > 40f)
                {
                    speedEvaluationMessage = "Wrong Way!";
                    directionIndicatorDestroy();
                    needNewTrial = true;
                }
                break;

            case 2: // Upward only
                if (currentHeadVelocity.x < -40)
                {
                    speedEvaluationMessage = "Wrong Way!";
                    directionIndicatorDestroy();
                    needNewTrial = true;
                }
                break;

            case 3: // Downward only
                if (currentHeadVelocity.x > 40)
                {
                    speedEvaluationMessage = "Wrong Way!";
                    directionIndicatorDestroy();
                    needNewTrial = true;
                }
                break;

            case 4: // Rightward or Leftward
                break;

            case 5: // Upward or Downward
                break;

            case 6: // Any direction
                break;

            case 7: // Random horizontal direction
                if (pl.gametype == 0)
                {
                    if (directionIndicatorLabel == "left")
                    {
                        if (currentHeadVelocity.y > 40f)
                        {
                            speedEvaluationMessage = "Wrong Way!";
                            needNewTrial = true;
                        }
                    }
                    else
                    {
                        if (currentHeadVelocity.y < -40f)
                        {
                            speedEvaluationMessage = "Wrong Way!";
                            needNewTrial = true;
                        }
                    }
                }
                else if (pl.gametype == 1)
                {
                    if (directionIndicatorLabel == "left")
                    {
                        if (currentHeadVelocity.y < -40f)
                        {
                            speedEvaluationMessage = "Wrong Way!";
                            directionIndicatorDestroy();
                            needNewTrial = true;
                        }
                    }
                    else
                    {
                        if (currentHeadVelocity.y > 40f)
                        {
                            speedEvaluationMessage = "Wrong Way!";
                            directionIndicatorDestroy();
                            needNewTrial = true;
                        }
                    }
                }
                break;

            case 8: // Random vertical direction
                if (directionIndicatorLabel == "up")
                {
                    if (currentHeadVelocity.x < -10f)
                    {
                        speedEvaluationMessage = "Wrong Way!";
                        directionIndicatorDestroy();
                        needNewTrial = true;
                    }
                }
                else
                {
                    if (currentHeadVelocity.x > 10f)
                    {
                        speedEvaluationMessage = "Wrong Way!";
                        directionIndicatorDestroy();
                        needNewTrial = true;
                    }
                }
                break;

            case 9: // Random any direction
                if (directionIndicatorLabel == "left")
                {
                    if (currentHeadVelocity.y < -10f)
                    {
                        speedEvaluationMessage = "Wrong Way!";
                        directionIndicatorDestroy();
                        needNewTrial = true;
                    }
                }
                else if (directionIndicatorLabel == "right")
                {
                    if (currentHeadVelocity.y > 10f)
                    {
                        speedEvaluationMessage = "Wrong Way!";
                        directionIndicatorDestroy();
                        needNewTrial = true;
                    }
                }
                else if (directionIndicatorLabel == "up")
                {
                    if (currentHeadVelocity.x < -10f)
                    {
                        speedEvaluationMessage = "Wrong Way!";
                        directionIndicatorDestroy();
                        needNewTrial = true;
                    }
                }
                else
                {
                    if (currentHeadVelocity.x > 10f)
                    {
                        speedEvaluationMessage = "Wrong Way!";
                        directionIndicatorDestroy();
                        needNewTrial = true;
                    }
                }
                break;
        }
    }

    void resetflags()
    {
        rotateBackAndRandom();
        Optotype = Resources.Load<Sprite>("Sprites/Transparant Cs/" + optotypeSize.ToString());
        responded = true;
        recentered = false;
        needNewTrial = false;
        arrowDisappear = false;
        optotype_showed_once = false;
        optotypedisplayed = false;
        keypressed = false;
        gloves.SetActive(false);
        ballrecentered = false;
        resettime = false;
        recordtime = false;
        speedflag = true;
        arrowshowed = false;
        PeakVelocity = 0f;
        SavePeak = false;
		headturned = false;
        headstopped = false;
        optotypeInit = false;
        joystickinput = false;
        PlayResponse = false;   
        DecisionMaking.SetActive(false);
        timer2 = 0f;
    }
    // reads in motion information from tracker
    void readTracker()
    {
        //Vector3 headAngle;
        Vector3 translationOnScreen = Vector3.zero;

        readFromDataSource();

        // translation on screen stores how many pixels the objects will move per head's angular position
        // translation along the x-axis depends on rotation about the y-axis
        // translation along the y-axis depends on rotation about the x-axis

        //headAngle = Quaternion2GazeAngleRad(dataSource.currentRotation);
        //to do (1)
        //translationOnScreen.x = -Mathf.Tan(headAngle.y * pl.sceneGain) * pl.patientToScreenDistance * 12 * Screen.dpi * pixels2World;
        //translationOnScreen.y = - Mathf.Tan(headAngle.x * pl.sceneGain) * pl.patientToScreenDistance * 12 * Screen.dpi * pixels2World;

        //background.transform.position = translationOnScreen;
    }

    /* Deprecated:
	 *
	 * Name: recordMaxSpeedAndAcc
	 * Functionality: record the maximum speed and acceleration during one session
	 *
	 */
    /*
   void recordMaxSpeedAndAcc()
   {
       if (headSpeed.magnitude > maxHeadSpeed)
           maxHeadSpeed = headSpeed.magnitude;
       if (accelerationComponent > maxHeadAcc)
           maxHeadAcc = accelerationComponent;
   }
   */

    /*
	 * 
	 * Name: showOptotype
	 * Functionality: helper function to show or hide optotype
	 * 
	 */
    void showOptotype(bool status)
    {
        if (status)
        {
            SpriteR.enabled = true;
            SpriteR.sprite = Optotype;
        }
        else if (!status)
        {
            SpriteR.enabled = false;
        }

    }

    /* 
     * method to show message of the speedEvaluationResponse if head movement was too fast or too slow
     * */
    private void showSpeedEvaluationMessage()
    {
        response.text = speedEvaluationMessage;
    }

    private void showspeedresult()
    {
        response1.text = speedMessage;
        LastThreePeakVelocities.text = PeakVelocityDisplay;
        //Debug.Log(PeakVelocityDisplay);
    }


    /*
	 *
	 * Name: recalibrate
	 * Functionality: 1: automatically moves object to destination. 
	 * 2: detects if objects are within a certain range of destination
	 * for a certain amount of time.
	 * 
	 */
    void recenterHeadForNewTrial(float headRecenteringThreshold, Vector3 destination, float time)
    {
        if (!recentered)
        {
            showHUD = true;
            //StopCoroutine("recalibrateTimer"); // Only use StartCoroutine(string name, object value) and StopCoroutine(string name) would work here.
            dot.SetActive(true);
            if(pl.blackout == 0)
            {
                response.text = " ";
            }
            else
            {
                response.text = "Center Head";
            }
            // Debug.Log(recentered);
            if (headRotationCheck(headRecenteringThreshold))
            {
                //Debug.Log("CHECK" + headRotationCheck(headRecenteringThreshold));
                response.text = "Hold Still";
                timer += Time.deltaTime;
                if(timer > waitingtime)
                {
                    recentered = true;
                    timer = 0f;
                }
                //StartCoroutine(timer(2));
            }
        }
            //showHUD = false;
            // maxHeadSpeed = 0f;
            // maxHeadAcc = 0f;
            //response.text = null;
        else
        {
        }
        dot.SetActive(false);
        movebackFinish = true;
    }
    //finds the difference between the polhemus value for calibration and the absolute current position
    //TODO
    private bool headRotationCheck(float headRecenteringThreshold)
    {
       // Debug.Log(dataSource.currentRotation);

        // this.evaluationRotation = coilController.currentRotation.eulerAngles.x;
        //response.text = "Center Head"; //  (Mathf.Rad2Deg * Mathf.Acos( coilController.currentRotation.w )).ToString();
                                       //return (dataSource.currentRotation.eulerAngles.magnitude < headRecenteringThreshold); // || (Mathf.Abs(Mathf.Abs(this.evaluationRotation) - rotationUpperBound) < headRecenteringThreshold);

        /*Quaternion trueRotation = new Quaternion(dataSource.currentRotation.x - pl.sceneGain * dataSource.currentRotation.x,
            dataSource.currentRotation.y - pl.sceneGain * dataSource.currentRotation.y,
            dataSource.currentRotation.z - pl.sceneGain * dataSource.currentRotation.z,
            dataSource.currentRotation.w);*/
        if (Quaternion.Angle(dataSource.currentRotation, RecenterPoint.transform.rotation) < headRecenteringThreshold)
        {
            response.text = "Keep Steady";
        }
        return (Quaternion.Angle(dataSource.currentRotation, RecenterPoint.transform.rotation) < headRecenteringThreshold);
    }


    void recenterball(Vector3 destination, float time)
    {
        Vector3 optotypePosition = this.transform.localPosition;
        Vector3 soccerPosition = soccer.transform.localPosition;
        Vector3 glovesPosition = gloves.transform.localPosition;
        //Vector3 containerPostion = canvas.transform.localPosition;
        if (!resettime)
        {
            returnTime = Time.time;
            resettime = true;
        }
        if (Time.time < returnTime + time / 2)
        {
            this.transform.localPosition = Vector3.Lerp(optotypePosition, destination, (Time.time - returnTime));
            soccer.transform.localPosition = Vector3.Lerp(soccerPosition, destination, (Time.time - returnTime));
            //canvas.transform.localPosition = Vector3.Lerp (containerPostion, destination, (Time.time - startTime)/(time/3f));
            gloves.transform.localPosition = Vector3.Lerp(glovesPosition, destination + glovesInitPosition, (Time.time - returnTime));
        }
        else
        {
            ballrecentered = true;
        }

    }

    /**
     * 
     * Name: validateSpeed
     * Functionality: tracks when the velocityProfile is above the threshold
     * Sets string message and shows optotype or resets trial depending on if 
     * 
     * */
    bool validateSpeed()
    {
        timer2 += Time.deltaTime;
        //float difference = Mathf.Abs(headSpeed - lookbackSpeed);
        if (Mathf.Abs(headSpeed) > PeakVelocity)
        {
            PeakVelocity = Mathf.Abs(headSpeed);
        }
        //Debug.Log(PeakVelocity);
        //speed.text = currentHeadVelocity.y.ToString();
        //Debug.Log(currentHeadVelocity.y);
		if (Mathf.Abs(headSpeed) >= 20f) 
		{
			headturned = true;
		}
		if (Mathf.Abs(headSpeed) < 20f && headturned) 
		{
			headstopped = true;
		}
        if((headSpeed < speedThreshold) && headstopped && (!keypressed))
        {
            speedEvaluationMessage = "Time Out";
            needNewTrial = true;
            headstopped = false;
            timer2 = 0f;
            return false;
        }
		if ((headSpeed < speedThreshold) && (Quaternion.Angle(dataSource.currentRotation, RecenterPoint.transform.rotation) > TimeOutAngle) && (!keypressed))
        {
            speedEvaluationMessage = "Time Out";
            needNewTrial = true;
			headstopped = false;
            timer2 = 0f;
            return false;
        }
		if ((headSpeed < speedThreshold) && (timer2 > TimeOutTime) && (!keypressed))
        {
            speedEvaluationMessage = "Time Out";
            timer2 = 0f;
            needNewTrial = true;
			headstopped = false;
            return false;
        }
		else if (headSpeed > speedThreshold)
        {
            if (speedflag)
            {
                if(pl.gametype == 0)
                {
                    fixedtime = currentfixedtime;
                }
                else if(pl.gametype == 1)
                {
                    fixedtime = "NaN";
                    streamSample = streamSampleAtOptotypeAppearance;
                }
                speedflag = false;
            }
            if (PeakVelocity < PeakVeclocityMin)
            {
                speedMessage = "Too Slow";
                speedEvaluationRecord = -1;
                //needNewTrial = true;
                //logTrialData("slow");
                //StartCoroutine(logTrial(false));
            }
            else if (PeakVelocity > 200f)
            {
                speedMessage = "Too Fast";
                speedEvaluationRecord = 1;
                //needNewTrial = true;
                //logTrialData("fast");
                //StartCoroutine(logTrial(false));
            }
            else
            {
                speedMessage = "Head Speed is Good";
                speedEvaluationRecord = 0;
                needNewTrial = false;
            }
            dataSource.speedEvaluationHash = "" + speedEvaluationRecord;
            timer2 = 0f;
            return true;
        }
        else
        {
            timer2 = 0f;
            return false;
        }
    }

    /*
     *   used during a trial evaluation
     *   Convention: 
     *   FixedTime  VariableGain   HeadSpeedTrigger     HeadSpeedLowerWindow    HeadSpeedUpperWindow    LookBackWindow state   Right/Wrong optotypeDirection   optotypeSize    userResponse   Score_saves   Score_goals
     * */
    private void logTrialData(string state)
    {
        //non-dynamic values
        //fixTime = Time.fixedTime.ToString();
        trialLogger.Append(fixedtime + "\t");
        trialLogger.Append(streamSample + "\t");
        trialLogger.Append(pl.leftGain + "\t");
        trialLogger.Append(pl.rightGain + "\t");
        trialLogger.Append(pl.dvaHeadSpeedTriggerThreshold + "\t");
        trialLogger.Append(pl.dvaLowerHeadSpeedWindow + "\t");
        trialLogger.Append(pl.dvaUpperHeadSpeedWindow + "\t");
        trialLogger.Append(pl.dvaLookBackAmount + "\t");

        //dynamic values
        trialLogger.Append(state + "\t");
        trialLogger.Append(optotypeDirection[randomKey] + "\t");
        trialLogger.Append(optotypeSize + "\t");
        trialLogger.Append(keyClicked + "\t");
        trialLogger.Append(saves + "\t");
        trialLogger.Append(goals + "\t");
        trialLogger.AppendLine();
    }

    /*
     * logs the data pulled from Polhemus and that was used in the determination of correct head speed thresholds 
     * Convention
     * FixedTime    velocityMagnitude   acceleration    QuaternionPosition.w    x   y   z  
     * */
    //private void logRawData(float magnitude, float acceleration, Quaternion rotation)
    //{
    //    rawDataLogger.Append(Time.fixedTime + " ");
    //    rawDataLogger.Append(magnitude + " ");
    //    rawDataLogger.Append(rotation.w + " ");
    //    rawDataLogger.Append(rotation.x + " ");
    //    rawDataLogger.Append(rotation.y + " ");
    //    rawDataLogger.Append(rotation.z + " ");
    //    rawDataLogger.AppendLine();
    //}

    /*  Deprecated
	bool checkSpeedAndAcceleration(float speedThresMin, float SpeedThresMax, float accThresMin, float accThresMax, ref string s){
		int result = 0;
		if (maxHeadSpeed >= speedThresMin && maxHeadSpeed <= SpeedThresMax) {
			s = "Head Speed is good";
			result++;
		}
		else if (maxHeadSpeed > SpeedThresMax)
			s = "Head Speed Is Too Fast";
		else
			s = "Head Speed Is Too Slow";
		if (pl.trackerType == 1) {
			if (maxHeadAcc >= accThresMin && accelerationComponent <= accThresMax) {
				s += "\nHead Acceleration Rate Is Good";
				result++;
			} else if (maxHeadAcc < accThresMin) {
				s += "\n Head Acceleration Rate Is Too Low";
				result--;
			} else {
				s += "\n Head Acceleration Rate Is Too High";
				result--;
			}
		}
		return(result == 1 || result == 2);

	}
	*/


    /*
	 * 
	 * Name: directionIndicatorDisplayerHelper
	 * Functionality: Helper function of directionIndicatorDisplayer 
	 * to help display direction arrow
	 * 
	 */
    void directionIndicatorDisplayerHelper(string direction, Vector3 position)
    {
        GameObject arrow = new GameObject(direction);
        arrow.transform.parent = background.transform; // Make it a child of background so it can use its parent world position
        arrow.transform.rotation = background.transform.rotation;
        arrow.transform.localPosition = position;
        SpriteRenderer renderer = arrow.AddComponent<SpriteRenderer>();
        renderer.sortingLayerName = "UI";
        Sprite arrowSprite = Resources.Load<Sprite>("Sprites/Arrow/arrow-" + direction);
        renderer.sprite = arrowSprite;
        directionIndicator.Add(arrow);
        directionIndicatorLabel = direction;
		arrow.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f) / 5; // backgroundScale;
    }

    

    /*
	 * 
	 * Name: directionIndicatorDisplayer
	 * Functionality: display direction arrows upon configuration
	 * 
	 */
    void directionIndicatorDisplayer()
    {
        //arrowDisappear = false;
        switch (pl.headDirection)
        {
            case 0:
                directionIndicatorDisplayerHelper("left", new Vector3(-0.3f, 0, 0));
                break;

            case 1:
                directionIndicatorDisplayerHelper("right", new Vector3(0.3f, 0, 0));
                break;

            case 2:
                directionIndicatorDisplayerHelper("up", new Vector3(0, 0, 0));
                break;

            case 3:
                directionIndicatorDisplayerHelper("down", new Vector3(0, 0, 0));
                break;

            case 4:
                directionIndicatorDisplayerHelper("left", new Vector3(-0.1f, 0, 0));
                directionIndicatorDisplayerHelper("right", new Vector3(0.1f, 0, 0));
                break;

            case 5:
                directionIndicatorDisplayerHelper("up", new Vector3(0, 0.1f, 0));
                directionIndicatorDisplayerHelper("down", new Vector3(0, -0.1f, 0));
                break;

            case 6:
                directionIndicatorDisplayerHelper("left", new Vector3(-0.3f, 3, 0));
                directionIndicatorDisplayerHelper("right", new Vector3(0.3f, 1, 0));
                directionIndicatorDisplayerHelper("up", new Vector3(0, 0.3f, 0));
                directionIndicatorDisplayerHelper("down", new Vector3(0, -0.3f, 0));
                break;

            case 7:
                float a = UnityEngine.Random.value;
                if (a < .5f)
                {
                    directionIndicatorDisplayerHelper("left", new Vector3(-0.1f, 0, 0));
                    //Debug.Log("Left turn!");
                }
                else
                {
                    directionIndicatorDisplayerHelper("right", new Vector3(0.1f, 0, 0));
                    //Debug.Log("right turn!");
                }
                break;

            case 8:
                a = UnityEngine.Random.value;
                if (a < .5f)
                {
                    directionIndicatorDisplayerHelper("up", new Vector3(0, 0.1f, 0));
                }
                else
                {
                    directionIndicatorDisplayerHelper("down", new Vector3(0, -0.1f, 0));
                }
                break;

            case 9:
                a = UnityEngine.Random.value;
                if (a < .25f)
                {
                    directionIndicatorDisplayerHelper("left", new Vector3(-0.1f, 0, 0));
                }
                else if (a >= .25f && a < .5f)
                {
                    directionIndicatorDisplayerHelper("right", new Vector3(0.1f, 0, 0));
                }
                else if (a >= .5f && a < .75f)
                {
                    directionIndicatorDisplayerHelper("up", new Vector3(0, 0.1f, 0));
                }
                else
                {
                    directionIndicatorDisplayerHelper("down", new Vector3(0, -0.1f, 0));
                }
                break;
        }
    }

    /*
	 * 
	 * Name: directionIndicatorDestroy
	 * Funtionality: direction arrow is a game object that is created dynamically during run time
	 * It needs to be destroyed before respawning new direction arrow game object for other test sessions
	 * 
	 */
    void directionIndicatorDestroy()
    {
        foreach (GameObject g in directionIndicator)
        {
            Destroy(g);
        }
        directionIndicator.Clear();
        arrowDisappear = true;
    }

    /* 
	 *
	 * Name: giveFeedback
	 * Functionality: giveFeedback() is called every frame
	 * giveFeedback() will call playAnimation() later,
	 * all objects should not be set back to original 
	 * position when animation is on
	 *
	 */
    void giveFeedback()
    {
        // Test user response
        if (!responded && !keypressed)
        {
            OptotypeDisplayForDecisionMaking();
            ControllerSettings();
            //mapping joystick input
            // Here order matters, must first check whether irrelavent key is clicked
            if (getIrreleventKeyClicked() && (joystickinput))
            {
                response.text = "You clicked the wrong key!";
                keypressed = true;
            }
            else if (getCorrectKeyClicked() && (joystickinput))
            {
                //playAnimation();
                //paused = true;
                //showHUD = false; // will be reset to TRUE in playAnimation coroutine
                //response.text = "You saved the ball!";
                saves++;
                showResponse("correct");
                keypressed = true;
            }
            else if (getWrongKeyClicked() && (joystickinput))
            {
                //playAnimation();
                //paused = true;
                //showHUD = false; // will be reset to TRUE in playAnimation coroutine
                //response.text = "You missed the ball!";
                goals++;
                showResponse("wrong");
                keypressed = true;
            }
        }
    }

    /*
	 * 
	 * Name: showFinishTest
	 * Functionality: Display and store relevant results when
	 * detects DVA test is over
	 * 
	 */
    /*IEnumerator showFinishTest()
    {
        //response.text = "Your Head Speed Window is: " + lastSpeedThresMin + " - " + lastSpeedThresMax + "\nYour Dynamic Acuity is:\n" + lastOptotypeSize + " in LogMAR";
        response.text = "";
        StartCoroutine(recenterHeadForNewTrial(0f, Vector3.zero, moveBackTime)); // 0f: default setting always recalibrate
        showOptotype(false);
        //pl.dvaSpeedResultMin = lastSpeedThresMin;
        //pl.dvaSpeedResultMax = lastSpeedThresMax;
        pl.dvaAcuityResult = lastOptotypeSize;
        logmarScale = lastOptotypeSize;
        decimalScale = Mathf.Pow(10, logmarScale);
        optotypeSize = decimalScale * pl.objectSizeScalerForStandardVision;

        yield return new WaitForSeconds(1.5f);
        showOptionButtons = true;
    }*/


    /*
	 * 
	 * Name: showResponse
	 * Functionality: show response upon patient's input
	 * 
	 */
    /*void ControllerSettings()
{
    if(Mathf.Abs(Input.GetAxis("Horizontal")) > 0.8f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.8f)
    {
        joystickinput = true;
    }
    else
    {
        keyClicked = KeyCode.None;
        joystickinput = false;
    }
    if (Input.GetAxis("Horizontal") >= 0.8f || Input.GetKeyDown(KeyCode.RightArrow))
    {
        keyClicked = KeyCode.RightArrow;
        //Debug.Log("Right");
    }
    else if (Input.GetAxis("Horizontal") <= -0.8f || Input.GetKeyDown(KeyCode.LeftArrow))
    {
        keyClicked = KeyCode.LeftArrow;
        //Debug.Log("Left");
    }
    if (Input.GetAxis("Vertical") >= 0.8f || Input.GetKeyDown(KeyCode.UpArrow))
    {
        keyClicked = KeyCode.UpArrow;
        //Debug.Log("Up");
    }
    else if (Input.GetAxis("Vertical") <= -0.8f || Input.GetKeyDown(KeyCode.DownArrow))
    {
        keyClicked = KeyCode.DownArrow;
        //Debug.Log("Down");
    }JOYSTICK BACKUP, THIS IS FOR NORMAL JOYSTICKS;
}*/
    /*void ControllerSettings()
    {
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > _ControllerThreshold || Mathf.Abs(Input.GetAxis("Vertical")) > _ControllerThreshold)
        {
            joystickinput = true;
        }
        else
        {
            keyClicked = KeyCode.None;
            joystickinput = false;
        }
        if (Input.GetAxis("Horizontal") >= _ControllerThreshold || Input.GetKeyDown(KeyCode.DownArrow))
        {
            keyClicked = KeyCode.DownArrow;
            //Debug.Log("Right");
        }
        else if (Input.GetAxis("Horizontal") <= -_ControllerThreshold || Input.GetKeyDown(KeyCode.UpArrow))
        {
            keyClicked = KeyCode.UpArrow;
            //Debug.Log("Left");
        }
        if (Input.GetAxis("Vertical") >= _ControllerThreshold || Input.GetKeyDown(KeyCode.RightArrow))
        {
            keyClicked = KeyCode.RightArrow;
            //Debug.Log("Up");
        }
        else if (Input.GetAxis("Vertical") <= -_ControllerThreshold || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            keyClicked = KeyCode.LeftArrow;
            //Debug.Log("Down");
        }
    }*/

    void OptotypeDisplayForDecisionMaking()
    {
        if((Mathf.Abs(Input.GetAxis("Horizontal")) > _ControllerThreshold || Mathf.Abs(Input.GetAxis("Vertical")) > _ControllerThreshold) && joystickinput == false)
        {
            DecisionMaking.SetActive(true);
            if (SpriteR.enabled)
            {
                SpriteR.enabled = false;
            }
        }
        else
        {
            DecisionMaking.SetActive(false);
            //SpriteR.enabled = true;
        }
        if (Input.GetAxis("Vertical") > _ControllerThreshold)
        {
            DR.sprite = Resources.Load<Sprite>("Sprites/DecisionMaking/Up");
        }
        if (Input.GetAxis("Vertical") < -_ControllerThreshold)
        {
            DR.sprite = Resources.Load<Sprite>("Sprites/DecisionMaking/Down");
        }
        if (Input.GetAxis("Horizontal") > _ControllerThreshold)
        {
            DR.sprite = Resources.Load<Sprite>("Sprites/DecisionMaking/Right");
        }
        if (Input.GetAxis("Horizontal") < -_ControllerThreshold)
        {
            DR.sprite = Resources.Load<Sprite>("Sprites/DecisionMaking/Left");
        }
        if (Input.GetAxis("Horizontal") > _ControllerThreshold && Input.GetAxis("Vertical") > _ControllerThreshold)
        {
            DR.sprite = Resources.Load<Sprite>("Sprites/DecisionMaking/UpRight");
        }
        if (Input.GetAxis("Horizontal") < -_ControllerThreshold && Input.GetAxis("Vertical") > _ControllerThreshold)
        {
            DR.sprite = Resources.Load<Sprite>("Sprites/DecisionMaking/UpLeft");
        }
        if (Input.GetAxis("Horizontal") > _ControllerThreshold && Input.GetAxis("Vertical") < -_ControllerThreshold)
        {
            DR.sprite = Resources.Load<Sprite>("Sprites/DecisionMaking/DownRight");
        }
        if (Input.GetAxis("Horizontal") < -_ControllerThreshold && Input.GetAxis("Vertical") < -_ControllerThreshold)
        {
            DR.sprite = Resources.Load<Sprite>("Sprites/DecisionMaking/DownLeft");
        }
        if (Input.GetButton("Confirm"))
        {
            joystickinput = true;
            DecisionMaking.SetActive(false);
            SpriteR.enabled = true;
        }
    }
    void ControllerSettings()
{
        if (Input.GetAxis("Horizontal") > _ControllerThreshold && Input.GetAxis("Vertical") > _ControllerThreshold)
        {
            keyClicked = "UpRight";
        }
        else if (Input.GetAxis("Horizontal") < -_ControllerThreshold && Input.GetAxis("Vertical") > _ControllerThreshold)
        {
            keyClicked = "UpLeft";
        }
        else if (Input.GetAxis("Horizontal") < -_ControllerThreshold && Input.GetAxis("Vertical") < -_ControllerThreshold)
        {
            keyClicked = "DownLeft";
        }
        else if (Input.GetAxis("Horizontal") > _ControllerThreshold && Input.GetAxis("Vertical") < -_ControllerThreshold)
        {
            keyClicked = "DownRight";
        }
        else if (Input.GetAxis("Vertical") < -_ControllerThreshold)
        {
            keyClicked = "Down";
        }
        else if (Input.GetAxis("Vertical") >_ControllerThreshold)
        {
            keyClicked = "Up";
        }
        else if (Input.GetAxis("Horizontal") > _ControllerThreshold)
        {
            keyClicked = "Right";
        }
        else if (Input.GetAxis("Horizontal") < -_ControllerThreshold)
        {
            keyClicked = "Left";
        }
        else
        {
            keyClicked = "None";
            joystickinput = false;
        }
    }

    void OptotypeSizeChangeInGamePlay()
    {
        if ((pl.CorrectUpperBound <= (float)CorrectCounter / WindowCounter))
        {
            Debug.Log("Increased");
            if(optotypeSize >= 1)
            {
                optotypeSize--;//between 0 and 11;
            }
        }
        else if ((pl.CorrectLowerBound >= (float)CorrectCounter / WindowCounter))
        {
            Debug.Log("Decreased");
            if (optotypeSize <= 10)
            {
                optotypeSize++;//between 0 and 11;
            }
        }
    }

    void showResponse(string r)
    {
        KickSource.PlayOneShot(Kick);
        if (gameMode == EnumTypes.GameMode.Game)
        {
            WindowCounter++;
        }
        //Debug.Log(optotypeSize);
        //Debug.Log ("Key" + keyClicked);
        //lastSpeedThresMin = speedDifferenceMin;
        //lastSpeedThresMax = speedDifferenceMax;
        //lastOptotypeSize = logmarScale;
        switch (r)
        {
            case "correct":
                //scaleOptotypeCorrect();
                logTrialData(r);
                CorrectOpt[nOpt]++;
                Save_Goal = "Save";
                if (gameMode == EnumTypes.GameMode.Game)
                {
                    CorrectCounter++;
                }
                /* if (bestDVA > logmarScale)
                 { // new best is reached
                     //TO DO LOGMAR CALCULATION :if(logmarScale)
                     bestDVA = logmarScale;
                 }*/

                break;

            case "wrong":
                /*if (bestDVA > logmarScale)
                { // new best is reached
                    bestDVA = logmarScale;  // MFW - why make this bestDVA if the response is wrong?
                    bestDVACounter = 0;
                    bestDVAReset = 0;
                }
                else if (Mathf.Approximately(bestDVA, logmarScale))
                { // best is tied
                    bestDVACounter++;
                }
                else
                { // best is not reached
                    bestDVAReset++;
                    if (bestDVAReset >= 2)
                    { // If two more wrongs guesses made without reaching the best, bestSVA will set to current logmarScale   MFW- but it's not doing this
                        bestDVAReset = 0;
                    }
                }*/
                //print (bestDVA + " " + bestDVACounter + " " + bestDVAReset);
                //scaleOptotypeWrong();
                logTrialData(r);
                InCorrectOpt[nOpt]++;
                Save_Goal = "Goal";
                break;
        }
        if(gameMode != EnumTypes.GameMode.Game)
        {
            last_nOpt = nOpt;
            nOpt = OptotypeCalculation();
			if (nOpt == last_nOpt && (r == "correct" || r == "wrong")) {
				sameCount++;
			} 
			else {
				sameCount = 0;
			}
            if (!jumpout)
            {
                optotypeSize = nOpt;
            }
            else
            {
                //warning.text = "VA = " + optotypeSize;
               // pl.DeterminedOptotypeSize = optotypeSize;
            }
            Allinputs.Enqueue(nOpt);
			if (Allinputs.Count > JumpoutLength)
            {
                Allinputs.Dequeue();
            }
            Unique = new Queue<int>(Allinputs.Distinct());
            var hash = new HashSet<int>(Unique);
            UniqueList = hash.ToArray();
            AllList = Allinputs.ToArray();
			/*int[] frequency = new int[12] {0,0,0,0,0,0,0,0,0,0,0,0};
			int larggeroptotype = 0;
			int smalleroptotype = 0;
			for(int i = 0; i < AllList.Length; i++)
			{
				frequency[AllList[i]]++;
				// get the two optotype size jumping back and forth
				if (frequency [AllList [i]] == (JumpoutLength + 1) / 2) {
					larggeroptotype = frequency [AllList [i]];
				}
				if (AllList[i] < 11 && frequency [AllList [i] + 1] == 2) {
					smalleroptotype = frequency [AllList [i] + 1];
				}
				else if (AllList[i] > 0 && frequency [AllList [i] - 1] == 2) {
					smalleroptotype = frequency [AllList [i] - 1];
				}
			}*/
            for (int i = 0; i < UniqueList.Length; i++)
            {
                //Debug.Log("UniqueList[i]: " + UniqueList[i]);
            }
            for (int i = 0; i < AllList.Length; i++)
            {
                //Debug.Log("AllList: " + AllList[i]);
            }
			//Debug.Log (frequency[0] + " " + frequency[1] + " " + frequency[2] + " "+ frequency[3] + " "+ frequency[4] + " "+ frequency[5] + " "+ frequency[6] + " "+ frequency[7] + " "+ frequency[8] + " "+ frequency[9] + " "+ frequency[10] +  " " + frequency[11] );
            //Debug.Log("Count:" + AllList.Length + ", Unique = " + Unique.Count);
			/*terminate if the same VA has been found for three trials in a row
              or if there have been at least 10 trials going back and forth between
              two adjacent VAs, but only if one smaller is wrong*/
			if (sameCount >= SameCountLenght) //
			{
				if (nOpt >= 1)
				{
					OptotypeResult = nOpt;
					/*if (r == "wrong")
					{
						InCorrectOpt[nOpt - 1] = CorrectOpt[nOpt] + 1;
						optotypeSize = OptotypeResult;
					}
					else
					{
						CorrectOpt[nOpt - 1] = CorrectOpt[nOpt - 1] + 1;
					}*/
					jumpout = true;
				}
				else
				{
					optotypeSize = OptotypeResult;
					jumpout = true;
				}
			}
			else if ((Allinputs.Count == JumpoutLength) && (Unique.Count == 2) && (UniqueList[1] - UniqueList[0] == 1))
			{
				OptotypeResult = UniqueList [1]; //feed the larger VA as the result
				jumpout = true;
				optotypeSize = OptotypeResult;
			}
        }
        //Debug.Log(optotypeSize);
        //Debug.Log(nOpt);
        // If satisfies the stop condition, shows test result
        /*(if (stopCase() && (gameMode != EnumTypes.GameMode.Game))
        {
            StartCoroutine(showFinishTest());
        }*/

        // rotate back to orginal direction and then randomly rotate again.
    }

    /*
     *best PEST
     *VA = visual acuity
     *Opt = range of optotype sizes
     *lgit = 0.25 + 0.75 ./ (1 + (VA ./ Opt).^2); 0.25 is the chance
     *probability for tumbling E, do this up to
    */

    private int OptotypeCalculation()
    {
        Lmax = 0;
        //pl.optotypeSize[0] = pl.eps;
        for (int i = 0; i < pl.optotypeSize.Count; i++)
        {
            for (int j = 0; j < pl.optotypeSize.Count; j++)
            {
                lgit[j] = 0.125f + (0.75f / (1.0f + Mathf.Pow(pl.optotypeSize[i] / pl.optotypeSize[j], 2)));
            }
            for (int k = 0; k < pl.optotypeSize.Count; k++)
            {
                if (k == 0)
                {
                    L = Mathf.Pow(lgit[k], CorrectOpt[k]) * Mathf.Pow(1 - lgit[k], InCorrectOpt[k]);
                }
                else
                {
                    L = L * Mathf.Pow(lgit[k], CorrectOpt[k]) * Mathf.Pow(1 - lgit[k], InCorrectOpt[k]);
                }
                //Debug.Log(CorrectOpt[k]);
            }
            if (L > Lmax)
            {
                Lmax = L;
                VAmax = pl.optotypeSize[i];
                nMax = i;
                //Debug.Log(nMax);
                //Debug.Log(VAmax);
            }
        }
        //pl.optotypeSize[0] = 0.05f;
        return nMax;
    }
    /*private void scaleOptotype()
   {

       if (optotypeSize < 0.0249f)
       {
           if (warning != null)
           {
               warning.text = "Optotype Distorted";
           }
           //Debug.Log("Optotype Distorted");
           //Debug.Log(String.Format("Last undistorted Optotype size:,{0}", Axis_X * 5));
       }
       else if (optotypeSize < 0.05f)
       {
           if(warning != null)
           {
               warning.text = "Reaching Optotype Distortion boundary";
           }
           if (warning == null)
           {
               Debug.Log("1");
           }
           //Debug.Log("Reaching Optotype Distortion boundary");
       }
       else if(optotypeSize >= 0.05f)
       {
           warning.text = "";
       }
   }*/

    /*
	 * 
	 * Name: playAnimation()
	 * Functionality: play soccer ball animation after receive user response
        */

    void playAnimation()
    {
        animationIsOn = true;
        //showHUD = false;
        gloves.GetComponent<SpriteRenderer>().enabled = true;
        Vector3 movementScale = new Vector3(0.5f, 0.5f, 0);
        Vector3[] ballMovement = new[] {
            new Vector3 (1f, 0f, 0f),
            new Vector3 (1f, 1f, 0f),
            new Vector3 (0f, 1f, 0f),
            new Vector3 (-1f, 1f, 0f),
            new Vector3 (-1f, 0f, 0f),
            new Vector3 (-1f, -1f, 0f),
            new Vector3 (0f, -1f, 0f),
            new Vector3 (1f, -1f, 0f),
        };
        Vector3 objectPosition = this.transform.localPosition;
        Vector3 glovesPosition = gloves.transform.localPosition;
        //Vector3 currentOptotypeSize = this.transform.localScale;
        Vector3 currentSoccerSize = soccer.transform.localScale;
        //Vector3 currentCanvasScale = canvas.transform.localScale;
        Vector3 gloveMovement = Vector3.zero;
        Vector3 optotypeSizeChanger;
        switch (keyClicked)
        {
            case "Up":
                gloveMovement = new Vector3(0f, 1f, 0f);
                break;
            case "Down":
                gloveMovement = new Vector3(0f, -1f, 0f);
                break;
            case "Left":
                gloveMovement = new Vector3(-1f, 0f, 0f);
                break;
            case "Right":
                gloveMovement = new Vector3(1f, 0f, 0f);
                break;
            case "UpRight":
                gloveMovement = new Vector3(1f, 1f, 0f);
                break;
            case "DownRight":
                gloveMovement = new Vector3(1f, -1f, 0f);
                break;
            case "UpLeft":
                gloveMovement = new Vector3(-1f, 1f, 0f);
                break;
            case "DownLeft":
                gloveMovement = new Vector3(-1f, -1f, 0f);
                break;
        }
        ballMovement[randomKey] = Vector3.Scale(ballMovement[randomKey], movementScale);
        gloveMovement = Vector3.Scale(gloveMovement, movementScale);

        showOptotype(true);
        gloves.SetActive(true);
        //soccer.GetComponent<SpriteRenderer> ().enabled = true; // Show the soccer ball at this time because its color would mix with the optotype

        if (!recordtime)
        {
            startTime = Time.time;
            recordtime = true;
        }

        if (Time.time < startTime + ballAnimationTime / 2)
        {
            this.transform.localPosition = Vector3.Lerp(objectPosition, objectPosition + ballMovement[randomKey], (Time.time - startTime) / (ballAnimationTime / ballSpeed));
            soccer.transform.localPosition = Vector3.Lerp(objectPosition, objectPosition + ballMovement[randomKey], (Time.time - startTime) / (ballAnimationTime / ballSpeed));
            gloves.transform.localPosition = Vector3.Lerp(glovesPosition, glovesPosition + gloveMovement - glovesInitPosition * 0.5f, (Time.time - startTime) / (ballAnimationTime / ballSpeed));
            optotypeSizeChanger = Vector3.Lerp(Vector3.one, Vector3.one * 2f, (Time.time - startTime) / (ballAnimationTime / ballSpeed));
            soccer.transform.localScale = Vector3.Scale(currentSoccerSize, optotypeSizeChanger);
            //this.transform.localScale = Vector3.Scale(currentOptotypeSize, optotypeSizeChanger);
            if (Time.time > startTime + ballAnimationTime / 4)
            {
                Debug.Log("Sound");
                ResponseSoundPlay();
            }
        }
        else
        {
            animationIsOn = false;
            //gloves.GetComponent<SpriteRenderer>().enabled = false;
        }
        //showHUD = true;
        //this.transform.localScale = currentOptotypeSize;
        soccer.transform.localScale = currentSoccerSize;
        //animationIsOn = false;
    }

    void ResponseSoundPlay()
    {
        if (Save_Goal == "Save" && !PlayResponse)
        {
            ResponseSource.PlayOneShot(Catch);
            PlayResponse = true;
        }
        if (Save_Goal == "Goal" && !PlayResponse)
        {
            ResponseSource.PlayOneShot(Miss);
            PlayResponse = true;
        }
    }
    /* 
	 * 
	 * Name: rotateBackAndRandom
	 * Fucntionality: Clean up work at the end of the 
	 * evaluation procedures
	 *
	 */
    void rotateBackAndRandom()
    {
        if(optotypeSize == 0)
        {
            this.transform.rotation = Quaternion.identity;
            randomKey = UnityEngine.Random.Range(0, optotypeNumber);
            this.transform.Rotate(new Vector3(0, 0, 90) * (randomKey/2)); 
        }
        else
        {
            this.transform.rotation = Quaternion.identity;
            randomKey = UnityEngine.Random.Range(0, optotypeNumber);
            this.transform.Rotate(new Vector3(0, 0, 45) * randomKey);
        }
        /*this.transform.Rotate(new Vector3(0, 0, -45) * randomKey);
        randomKey = UnityEngine.Random.Range(0, optotypeNumber);
        this.transform.Rotate(new Vector3(0, 0, 45) * randomKey);*/
        if (pl.gametype == 1) {
			background.transform.position = new Vector3 (0, 0, 0);
		}
        //this.GetComponent<SpriteRenderer> ().sprite = optotypeSprites [randomKey];
        //this.transform.localScale = new Vector3(optotypeSize, optotypeSize, optotypeSize)/ backgroundScale;
    }

    /*
	 * 
	 * Name: getCorrectKeyClicked
	 * Functionality: Examine whether patient clicked the correct 
	 * key upon optotype direction
	 * 
	 */
    bool getCorrectKeyClicked()
    {
        if (keyClicked == optotypeDirection[randomKey])
        {
            return true;
        }
        else
        {
            return false;
        }
        //keyClicked = optotypeDirection[randomKey];
        //return Input.GetKeyDown(optotypeDirection[randomKey]);
    }

    /*
	 * 
	 * Name: getWrongKeyClicked
	 * Functionality: Examine whether patient clicked the wrong 
	 * key upon optotype direction
	 * 
	 */
    bool getWrongKeyClicked()
    {
        if (keyClicked != optotypeDirection[randomKey])
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /*
	 * 
	 * Name: getIrrelaventKeyClicked
	 * Functionality: Examine whether patient clicked the irrelavent 
	 * key upon optotype direction
	 * 
	 */
    bool getIrreleventKeyClicked()
    {
        if (keyClicked == "None")
        {
            return true;
        }
        else
        {
            return false;
        }
        //return !Input.GetKeyDown(KeyCode.UpArrow) && !Input.GetKeyDown(KeyCode.DownArrow) && !Input.GetKeyDown(KeyCode.LeftArrow) && !Input.GetKeyDown(KeyCode.RightArrow);
    }

    /*
	 * 
	 * Name: stopCase
	 * Functionality: Check if the DVA test has satisfied the 
	 * circumsatance of finishing
	 * 
	 */
    bool stopCase()
    {
        return bestDVACounter >= 2 || logmarScale < -0.1f;  //MFW - changed -0.3f to -0.1f
    }

    /*
	 * 
	 * Name: readFromMouse()
	 * Functionality: this function reads in mouse data of patient's head movement and store it into rawHeadDelta variable,
	 * headDelta variable then uses rawHeadDelta to compute the effective head movement speed according to headDirection option
	 * 
	 */
    

    /*
	 * 
	 * Name: readFromDataSource()
	 * Functionality: this function reads in Data Source's data stream which captures patient's head movement and store it into rawHeadDelta variable,
	 * headDelta variable then uses rawHeadDelta to compute the effective head movement speed according to headDirection option
	 * 
	 */
    void readFromDataSource()
    {
        // WHY DO WE NEED TO DIVIDE INTO X, Y, AND Z AND THEN RECOMBINE?
        currentHeadVelocity = dataSource.angularVelocityRead;//new Vector3(dataSource.angularVelocityRead.x, dataSource.angularVelocityRead.y, dataSource.angularVelocityRead.z);
        currentfixedtime = dataSource.fixedtime;
        streamSampleAtOptotypeAppearance = dataSource.streamSample;
        headVelocityHistory.Enqueue(currentHeadVelocity);
        if (headVelocityHistory.Count > lookbackWindow)
        {
            headVelocityHistory.Dequeue();
        } 
        pastHeadVelocity = headVelocityHistory.Peek();
        /*speedLogger.Append(currentHeadVelocity.x + "\t");
        speedLogger.Append(currentHeadVelocity.y + "\t");
        speedLogger.Append(currentHeadVelocity.z + "\t");
        speedLogger.Append(dataSource.fixedtime);
        speedLogger.AppendLine();*/
        switch (pl.headDirection)
        {
            case 0: // Leftward only

                    headSpeed = Mathf.Abs(currentHeadVelocity.y);
                    lookbackSpeed = Mathf.Abs(pastHeadVelocity.y);

                break;

            case 1: // Rightward only

                    headSpeed = Mathf.Abs(currentHeadVelocity.y);
                    lookbackSpeed = Mathf.Abs(pastHeadVelocity.y);

                break;

            case 2: // Upward only

                    headSpeed = Mathf.Abs(currentHeadVelocity.x);
                    lookbackSpeed = Mathf.Abs(pastHeadVelocity.x);

                break;

            case 3: // Downward only
                    headSpeed = Mathf.Abs(currentHeadVelocity.x);
                    lookbackSpeed = Mathf.Abs(pastHeadVelocity.x);
                break;

            case 4: // Rightward or Leftward
                headSpeed = Mathf.Abs(currentHeadVelocity.y);
                lookbackSpeed = Mathf.Abs(pastHeadVelocity.y);
                break;

            case 5: // Upward or Downward
                headSpeed = Mathf.Abs(currentHeadVelocity.x);
                lookbackSpeed = Mathf.Abs(pastHeadVelocity.x);
                break;

            case 6: // Any direction
                Vector3 headSpeedTot = new Vector3(currentHeadVelocity.x, currentHeadVelocity.y, 0);
                headSpeed = headSpeedTot.magnitude;
                Vector3 lookbackSpeedTot = new Vector3(pastHeadVelocity.x, pastHeadVelocity.y, 0);
                lookbackSpeed = lookbackSpeedTot.magnitude;
                break;

            case 7: // Random horizontal direction

                    if (directionIndicatorLabel == "left")
                    {
                            headSpeed = Mathf.Abs(currentHeadVelocity.y);
                            lookbackSpeed = Mathf.Abs(pastHeadVelocity.y);
                    }
                    else
                    {
                            headSpeed = Mathf.Abs(currentHeadVelocity.y);
                            lookbackSpeed = Mathf.Abs(pastHeadVelocity.y);
                    }
                break;

            case 8: // Random vertical direction
                if (directionIndicatorLabel == "up")
                {
                        headSpeed = Mathf.Abs(currentHeadVelocity.x);
                        lookbackSpeed = Mathf.Abs(pastHeadVelocity.x);
                }
                else
                {
                        headSpeed = Mathf.Abs(currentHeadVelocity.x);
                        lookbackSpeed = Mathf.Abs(pastHeadVelocity.x);
                }
                break;

            case 9: // Random any direction
                if (directionIndicatorLabel == "left")
                {
                        headSpeed = Mathf.Abs(currentHeadVelocity.y);
                        lookbackSpeed = Mathf.Abs(pastHeadVelocity.y);
                }
                else if (directionIndicatorLabel == "right")
                {
                        headSpeed = Mathf.Abs(currentHeadVelocity.y);
                        lookbackSpeed = Mathf.Abs(pastHeadVelocity.y);
                }
                else if (directionIndicatorLabel == "up")
                {
                        headSpeed = Mathf.Abs(currentHeadVelocity.x);
                        lookbackSpeed = Mathf.Abs(pastHeadVelocity.x);
                }
                else
                {
                        headSpeed = Mathf.Abs(currentHeadVelocity.x);
                        lookbackSpeed = Mathf.Abs(pastHeadVelocity.x);
                }
                break;
        }
        // translation on screen stores how many pixels the objects will move per head's angular position
        // translation along the x-axis depends on rotation about the y-axis
        // translation along the y-axis depends on rotation about the x-axis
        float tangentVector = 0;
        if(dataSource.currentRotation.eulerAngles.y != 0)
        {
            tangentVector = -Mathf.Tan(Mathf.Deg2Rad * dataSource.currentRotation.eulerAngles.y);
        }
        translationOnScreen = new Vector3(tangentVector, -Mathf.Tan(Mathf.Deg2Rad * dataSource.currentRotation.eulerAngles.x), 0) * pl.patientToScreenDistance * 12 * Screen.dpi * pixels2World;
        // accelerationComponent = Mathf.Abs(coilController.getAccelerationVector().z);

        //logs raw data from polhemusController
        // logRawData(headSpeed.magnitude, accelerationComponent, polhemusController.getRotation());
    }


    /*void OnGUI()
    {
        /*Event e = Event.current;
        if (e.isKey)
        {
            Debug.Log(e.keyCode);
        }
        GUIStyle countdownStyle = new GUIStyle();
        countdownStyle.normal.textColor = Color.yellow;
        countdownStyle.fontSize = 30;
        countdownStyle.alignment = TextAnchor.MiddleCenter;

        GUIStyle headupStyle = new GUIStyle();
        headupStyle.alignment = TextAnchor.MiddleCenter;
        headupStyle.fontSize = 20;
        headupStyle.normal.textColor = Color.magenta;

        /*
        info = "Head Speed Window:\n"
        + speedDifferenceMin
        + " - "
        + speedDifferenceMax
        + "\nAcuity:\n"
        + logmarScale
        + " in LogMAR\n"
        + pl.patientToScreenDistance
        + "/"
        + pl.patientToScreenDistance * decimalScale
        + " in foot\nCurrent Head Acceleration: "
        + Mathf.Abs(headSpeed.magnitude - pastSpeed.magnitude).ToString()
        + "\nBest DVA: "
        + bestDVA
        + " has reached "
        + bestDVACounter
        + " times";
        */

        // GUI.Box(new Rect(Screen.width * 2 / 3, Screen.height * 3 / 5, Screen.width / 10, Screen.height / 5), info, headupStyle);

        //if (showTestInfo)
        //{
        //    GUI.Box(new Rect(Screen.width / 3, Screen.height / 5, Screen.width / 3, Screen.height / 10), testInfo, countdownStyle);
        //}

        /*if (showHUD)
            canvas.gameObject.SetActive(true);
        else
            canvas.gameObject.SetActive(false);

      

        // Change transparency for buttons
        // Color oldColor = GUI.color;
        // GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);

        // if (GUI.Button(new Rect(Screen.width / 20, Screen.height * 1 / 15, Screen.width / 10, Screen.height / 10), "Main Menu"))
        if (GUI.Button(RectMainMenu, "Main Menu"))
        {
            //dataSource.QuitStream();
            //Destroy(dataSource);
            /*
            if (gameMode != EnumTypes.GameMode.Game)
                SceneManager.LoadSceneAsync("DynamicPrep", LoadSceneMode.Single);
            else
                SceneManager.LoadSceneAsync("GamePlayPrep", LoadSceneMode.Single);
            */
       /*     SceneManager.LoadScene("StartingScene");
        }
        if (GUI.Button(RectRecalibrate, "Reset Head Center"))
        {
            recalibrate();
        }a


        if (GUI.Button(RectLogResult, "Log Result"))
        {
            StartCoroutine(logTrial(true));
            StartCoroutine(vrController.logSpeed(true));
            //StartCoroutine(coilController.logMonitorData(true));
        }

        // Change transparency back
        // GUI.color = oldColor;

    }*/

    /* 
     * 
	 * Name: recalibrate
	 * Functionality: Recalibrates the ball and pollhemus to 
	 * the current head position, zeroes all 
	 * values
	 *
	 */
    public void recalibrate()
    {
        // polhemusController.zero();
        //dataSource.calibrate();
        background.transform.rotation = recabdot.transform.rotation;
        background.transform.position = recabdot.transform.position;
        //Debug.Log("background:"+background.transform.position);
        //Debug.Log("recab:" + recabdot.transform.position);
        this.transform.position = recabdot.transform.position;
        soccer.transform.position = recabdot.transform.position;
        gloves.transform.localPosition = glovesInitPosition;
        canvas.transform.position = new Vector3(recabdot.transform.position.x, recabdot.transform.position.y, recabdot.transform.position.z - 0.2f);
        RecenterPoint.transform.rotation = dataSource.currentRotation;
    }

    public void logger()
    {
        StartCoroutine(logTrial(true));
        StartCoroutine(vrController.logSpeed(true));
        StartCoroutine(coilController.logMonitorData(true));
    }

    /*
     * Method call on button press to put current StringBuilder storage into text file
     * */
    IEnumerator logTrial(bool incrementLog)
    {
        StreamWriter file;
        try
        {

            // create log file if it does not already exist. Otherwise open it for appending new trial
            if (!File.Exists(trialLogFile) || incrementLog)
            {
                trialLogFile = "trialLog_" + String.Format("{0:_yyyy_MM_dd_hh_mm_ss}", DateTime.Now) + ".txt";
                file = new StreamWriter(trialLogFile);
                file.WriteLine("FixedTime\tStreamSample\tleftGain\trightGain\tHeadSpeedTrigger\tHeadSpeedLowerWindow\tHeadSpeedUpperWindow\tLookBackWindow state\tRight/Wrong\toptotypeDirection\toptotypeSize\tuserResponse\tScore_saves\tScore_goals");
            }
            else
            {
                file = File.AppendText(trialLogFile);
            }

            //file = new StreamWriter("triallogs" + logFileCounter + ".txt");
            file.WriteLine(trialLogger.ToString());
            file.Close();
            trialLogger = new StringBuilder();
            if(incrementLog)
                logFileCounter++;
        }
        catch (System.Exception e)
        {
            Debug.Log("Error in accessing file: " + e);
        }
        yield return new WaitForSeconds(.1f);
    }


    public Vector3 Quaternion2GazeAngleRad(Quaternion q)
    {
        Vector3 g = new Vector3(); // gaze vector for output (using Haustein correction)
        Vector3 r = new Vector3(); // rotation vector for calculations

        float x;
        float y;
        float z;

        // convert quaternion to RHR rotation vector
        r[0] = -q.z / q.w;
        r[1] = q.x / q.w;
        r[2] = -q.y / q.w;

        // convert to gaze and assign back to Unity coordinates with LHR
        z = -2 * Mathf.Atan(r[0]);
        if (!float.IsNaN(z))
            g[2] = z;  // roll z

        x = -2 * Mathf.Atan((r[1] - r[0] * r[2]) / (1 + r[0] * r[0]));
        if (!float.IsNaN(x))
            g[0] = x; // pitch x

        y = -2 * Mathf.Atan((r[2] + r[0] * r[1]) / (1 + r[0] * r[0]));
        if(!float.IsNaN(y))
            g[1] = y;  // yaw y

        return (g);
    }


}