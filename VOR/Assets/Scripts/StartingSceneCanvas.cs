using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System;

public class StartingSceneCanvas : MonoBehaviour
{
    public GameObject Cav;
    public GameObject ConfigurationPanel;
    public Dropdown headDirectionDropdown;
    public Dropdown GainActivate;
    public Dropdown GameType;
    public Dropdown DuoGain;
    public InputField OptotypeSizeInput;
	public InputField PlayerDistance;
    public InputField LeftGain;
    public InputField RightGain;
    public InputField SpeedThreshold;
    public InputField LookBackFrames;
    public InputField LookBackLowerBound;
    public InputField LookBackUpperBound;
    public InputField OptotypeChangeWindow;
    public InputField CorrectPercentageUpperBound;
    public InputField CorrectPercentageLowerBound;
    public Text CurrentDistance;
    public Text left;
    public Text right;
    public Text SpeedThres;
    public Text Lookbacks;
    public Text lowerbound;
    public Text upperbound;
    public Text Window;
    public Text CorrectUpperBound;
    public Text CorrectLowerBound;
    public Text CurrentOptotypeSize;
    static public PreferenceLoader pl;
    private VRController vrController;

    // private PolhemusController polhemusController;
    private bool showConfigurationPanel = false;
    private bool showConfigurationButton = true;
    private bool showPlayModes = true;
    private string screenDistanceString; // user to screen distance which is needed to calculate variable gain coefficient

    // Use this for initialization
    void Start()
    {
        // polhemusController = DynamicAcuityController.polhemusController;
        if (GameObject.Find("PreferenceLoader") == null)
        {
            GameObject obj = Instantiate(Resources.Load("PreferenceLoader")) as GameObject;
            obj.name = "PreferenceLoader";
        }
        pl = GameObject.Find("PreferenceLoader").GetComponent<PreferenceLoader>(); // Get PreferenceLoader script which stores all user setting.
        Cav = GameObject.Find("Canvas");
        //Display.displays[1].Activate();
        ConfigurationPanel = GameObject.Find("Configuration Panel");                
        headDirectionDropdown.AddOptions(pl.headDirectionOptions);
        headDirectionDropdown.value = 4;
        GainActivate.AddOptions(pl.GainActivate);
        GainActivate.value = 1;
        GameType.AddOptions(pl.GameType);
        GameType.value = 1;
        DuoGain.AddOptions(pl.DuoGain);
        DuoGain.value = 3;
        vrController = GetComponent<VRController>();
    }
    public void Static()
    {
        SceneManager.LoadSceneAsync("Static", LoadSceneMode.Single);
    }
    public void Dynamic()
    {
        SceneManager.LoadSceneAsync("DynamicVR", LoadSceneMode.Single);
    }
    public void Game()
    {
        SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
    }
    public void Configuration()
    {
        showConfigurationPanel = true;
    }
    public void Quit()
    {
        System.Diagnostics.Process.GetCurrentProcess().Kill();
    }
    /*void OnGUI()
    {
        Rect Label1 = new Rect(Screen.width / 10, Screen.height / 10, Screen.width / 8, Screen.height / 10);
        Rect Content1 = new Rect(Screen.width / 10 - 40, Screen.height / 5 + 10, Screen.width / 6, Screen.height / 15);
        Rect Label2 = new Rect(Screen.width * 3 / 10, Screen.height / 10, Screen.width / 8, Screen.height / 10);    
        Rect Content2 = new Rect(Screen.width * 3 / 10, Screen.height / 6, Screen.width / 8, Screen.height / 10);
        Rect Label3 = new Rect(Screen.width * 5 / 10, Screen.height / 10, Screen.width / 8, Screen.height / 10);
        Rect Content3 = new Rect(Screen.width * 5 / 10, Screen.height / 5 + 10, Screen.width / 8, Screen.height / 10);
        Rect Label4 = new Rect(Screen.width * 7 / 10, Screen.height / 10, Screen.width / 8, Screen.height / 10);
        Rect Content4 = new Rect(Screen.width * 7 / 10, Screen.height / 5 + 10, Screen.width / 8, Screen.height / 10);
        Rect Label5 = new Rect(Screen.width / 10, Screen.height * 3 / 10, Screen.width / 8, Screen.height / 10);
        Rect Content5 = new Rect(Screen.width / 10, Screen.height * 4 / 10 + 10, Screen.width / 8, Screen.height / 10);
        Rect Label6 = new Rect(Screen.width * 3 / 10, Screen.height * 3 / 10, Screen.width / 8, Screen.height / 10);
        Rect Content6 = new Rect(Screen.width * 3 / 10, Screen.height * 4 / 10 + 10, Screen.width / 8, Screen.height / 10);
        Rect Label7 = new Rect(Screen.width * 5 / 10, Screen.height * 3 / 10, Screen.width / 8, Screen.height / 10);
        Rect Content7 = new Rect(Screen.width * 5 / 10, Screen.height * 4 / 10 + 10, Screen.width / 8, Screen.height / 10);
        Rect Label8 = new Rect(Screen.width * 7 / 10, Screen.height * 3 / 10, Screen.width / 8, Screen.height / 10);
        Rect Content8 = new Rect(Screen.width * 7 / 10, Screen.height * 4 / 10 + 10, Screen.width / 8, Screen.height / 10);
        Rect Label9 = new Rect(Screen.width  / 10, Screen.height * 5 / 10, Screen.width / 8, Screen.height / 10);
        Rect Content9 = new Rect(Screen.width / 10 - 240, Screen.height / 80 - 315, Screen.width / 8, Screen.height / 15);
        Rect Label10 = new Rect(Screen.width * 3 / 10, Screen.height * 5 / 10, Screen.width / 8, Screen.height / 10);
        Rect Content10 = new Rect(Screen.width / 10 - 50, Screen.height / 10 + 10, Screen.width / 8, Screen.height / 15);
        Rect Label11 = new Rect(Screen.width * 5 / 10, Screen.height * 5 / 10, Screen.width / 8, Screen.height / 10);
        Rect Content11 = new Rect(Screen.width / 20 - 80, Screen.height / 10 + 10, Screen.width / 8, Screen.height / 15);

        // CAN GET RID OF SHOWCONFIGURATION BUTTON AND PUT WITH SHOWPLAYMODES
        if (showConfigurationButton && GUI.Button(new Rect((int) Screen.width * 6.5f / 10, Screen.height * 8 / 10, Screen.width / 8, Screen.height / 10), "Configure"))
        {
            showConfigurationButton = false;
            showConfigurationPanel = true;
            showPlayModes = false;
        }
        // showConfigurationButton = false;
        if (showPlayModes)
        {
            if (GUI.Button(new Rect(Screen.width * 8 / 10, Screen.height * 8 / 10, Screen.width / 8, Screen.height / 10), "Quit"))
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            if (GUI.Button(new Rect(Screen.width / 6, Screen.height * 2 / 5, Screen.width / 7, Screen.height / 10), "Training"))
            {
                showPlayModes = false;
                showConfigurationButton = false;
                SceneManager.LoadSceneAsync("Train", LoadSceneMode.Single);
            }

            if (GUI.Button(new Rect(Screen.width * 2 / 6, Screen.height * 2 / 5, Screen.width / 7, Screen.height / 10), "Static Acuity"))
            {
                showPlayModes = false;
                showConfigurationButton = false;
                SceneManager.LoadSceneAsync("Static", LoadSceneMode.Single);
            }

            if (GUI.Button(new Rect(Screen.width * 3 / 6, Screen.height * 2 / 5, Screen.width / 7, Screen.height / 10), "Dynamic Acuity"))
            {
                showPlayModes = false;
                showConfigurationButton = false;
                SceneManager.LoadSceneAsync("DynamicVR", LoadSceneMode.Single);
            }

            if (GUI.Button(new Rect(Screen.width * 4 / 6, Screen.height * 2 / 5, Screen.width / 7, Screen.height / 10), "Game"))
            {
                showPlayModes = false;
                showConfigurationButton = false;
                SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
            }
            //if (GUI.Button (new Rect ()))

        }
        if (showConfigurationPanel == true)
        {
            bool isNumeric;

            GUIStyle optionStyle = new GUIStyle();
            optionStyle.normal.textColor = Color.red;
            optionStyle.fontSize = 16;
            optionStyle.alignment = TextAnchor.MiddleCenter;

            // Upper Left  -  Head Direction for enabled directions that the head can be moved to track head Speed Thresholds 
            GUI.Box(Label1, "Head Direction\n", optionStyle);
            rectTransformReconstruct(headDirectionDropdown.gameObject.GetComponent<RectTransform>(), Content1);
            headDirectionDropdown.gameObject.SetActive(true);

            // Upper Middle Left - Patient to Screen Distance to determine optotype size and scene motion
            GUI.Box(Label2, "Screen Distance (cm)\n", optionStyle);
            screenDistanceString = GUI.TextField(Content2, screenDistanceString, optionStyle);
            screenDistanceString = Regex.Replace(screenDistanceString, @"[^0-9.]", "");
            float f;
            isNumeric = float.TryParse(screenDistanceString, out f);
            if (isNumeric)
            {
                pl.patientToScreenDistance = f; //* PreferenceLoader.cm2ft; // convert to inches
            }
            else if (screenDistanceString != "")
            {
                screenDistanceString = (pl.patientToScreenDistance /* PreferenceLoader.cm2ft).ToString();/*
            }

            // Upper Middle Right - Initial optotype size in logMAR
            /*GUI.Box(Label3, "Initial Optotype Size (logMAR)\n\n" + pl.svaTestResult, optionStyle);
            pl.svaTestResult = Mathf.Round(10f * GUI.HorizontalSlider(Content3, pl.svaTestResult, 0f, 1f)) / 10f;
            GUI.Box(Label3, "Right Gain\n\n" + pl.rightGain, optionStyle);
            //pl.sceneGain = Mathf.Round(10f * GUI.HorizontalSlider(Content4, pl.sceneGain, -1f, 1f)) / 10f;
            pl.rightGain = (GUI.HorizontalSlider(Content3, pl.rightGain, -1f, 1f));
            pl.rightGain = (float)Math.Round(pl.rightGain, 1);

            // Upper Right - Scene gain
            GUI.Box(Label4, "Left Gain\n\n" + pl.leftGain, optionStyle);
            //pl.sceneGain = Mathf.Round(10f * GUI.HorizontalSlider(Content4, pl.sceneGain, -1f, 1f)) / 10f;
            pl.leftGain = (GUI.HorizontalSlider(Content4, pl.leftGain, -1f, 1f));
            pl.leftGain = (float)Math.Round(pl.leftGain, 1);

            // Center Left - Head Speed Threshold
            GUI.Box(Label5, "Speed Threshold\n\n" + pl.dvaHeadSpeedTriggerThreshold, optionStyle);
            pl.dvaHeadSpeedTriggerThreshold = Mathf.Round(GUI.HorizontalSlider(Content5, pl.dvaHeadSpeedTriggerThreshold, 0f, 200f));

            // Center Middle Left - Velocity Look Back Frames
            GUI.Box(Label6, "Lookback Frames\n\n" + pl.dvaLookBackAmount, optionStyle);
            pl.dvaLookBackAmount = (int)Mathf.Round(GUI.HorizontalSlider(Content6, pl.dvaLookBackAmount, 0f, 20f));

            // Center Middle Right - Lookback Window Lower Bound
            GUI.Box(Label7, "Lookback Lower Bound\n\n" + pl.dvaLowerHeadSpeedWindow, optionStyle);
            pl.dvaLowerHeadSpeedWindow = Mathf.Round(GUI.HorizontalSlider(Content7, pl.dvaLowerHeadSpeedWindow, 0f, 200f));

            // Center Right - Lookback Window Upper Bound
            GUI.Box(Label8, "Lookback Upper Bound\n\n" + pl.dvaUpperHeadSpeedWindow, optionStyle);
            pl.dvaUpperHeadSpeedWindow = Mathf.Round(GUI.HorizontalSlider(Content8, pl.dvaUpperHeadSpeedWindow, 0f, 200f));

            if (GUI.Button(new Rect(Screen.width * 6 / 10, Screen.height * 8 / 10, Screen.width / 8, Screen.height / 10), "Restore Defaults"))
            {
                Destroy(GameObject.Find("PreferenceLoader"));
                GameObject obj = Instantiate(Resources.Load("PreferenceLoader")) as GameObject;
                obj.name = "PreferenceLoader";
                pl = GameObject.Find("PreferenceLoader").GetComponent<PreferenceLoader>(); // Reload PreferenceLoader
            }
            GUI.Box(Label9, "Gain Activate\n", optionStyle);
            rectTransformReconstruct2(GainActivate.gameObject.GetComponent<RectTransform>(), Content9);
            GainActivate.gameObject.SetActive(true);

            GUI.Box(Label10, "Game Type\n", optionStyle);
            rectTransformReconstruct2(GameType.gameObject.GetComponent<RectTransform>(), Content10);
            GameType.gameObject.SetActive(true);

            GUI.Box(Label11, "Duo Gain\n", optionStyle);
            rectTransformReconstruct(DuoGain.gameObject.GetComponent<RectTransform>(), Content11);
            DuoGain.gameObject.SetActive(true);
            OptotypeSizeInput.gameObject.SetActive(true);
			PlayerDistance.gameObject.SetActive(true);
            if (GUI.Button(new Rect(Screen.width * 8 / 10, Screen.height * 8 / 10, Screen.width / 8, Screen.height / 10), "Return"))
            {
                headDirectionDropdown.gameObject.SetActive(false);
                GainActivate.gameObject.SetActive(false);
                GameType.gameObject.SetActive(false);
                DuoGain.gameObject.SetActive(false);
                OptotypeSizeInput.gameObject.SetActive(false);
                OptotypeSizeInput.gameObject.SetActive(false);
				PlayerDistance.gameObject.SetActive(false);
                showConfigurationPanel = false;
                showConfigurationButton = true;
                showPlayModes = true;
            }
        }
    }*/

    // Update is called once per frame
    void Update()
    {
        Debug.Log(showConfigurationPanel);
        if (showConfigurationPanel)
        {
            ConfigurationPanel.SetActive(true);
            Cav.SetActive(false);
        }
        else{
            ConfigurationPanel.SetActive(false);
            Cav.SetActive(true);
        }
        if((pl.duogain == 0) || (pl.duogain == 1) || (pl.duogain == 2))
        {
            GainActivate.value = 0;
        }
        if (pl.duogain == 0)
        {
            pl.rightGain = 0;
        }
        if (pl.duogain == 1)
        {
            pl.leftGain = 0;
        }
		//screenDistanceString = (pl.patientToScreenDistance /* PreferenceLoader.cm2ft*/).ToString();
        CurrentDistance.text = pl.patientToScreenDistance.ToString() + " cm";
        left.text = pl.leftGain.ToString();
        right.text = pl.rightGain.ToString();
        SpeedThres.text = pl.dvaHeadSpeedTriggerThreshold.ToString();
        Lookbacks.text = pl.dvaLookBackAmount.ToString();
        lowerbound.text = pl.dvaLowerHeadSpeedWindow.ToString();
        upperbound.text = pl.dvaUpperHeadSpeedWindow.ToString();
        Window.text = pl.OptotypeWindow.ToString();
        CorrectUpperBound.text = pl.CorrectUpperBound.ToString();
        CorrectLowerBound.text = pl.CorrectLowerBound.ToString();
        CurrentOptotypeSize.text = pl.optytpeSizeChoice.ToString();
    }

    // Helper function used in reconstruct postion for dropdown menus
    void rectTransformReconstruct(RectTransform trans, Rect position)
    {
        trans.sizeDelta = new Vector2(position.width, position.height);
        trans.anchoredPosition = new Vector2(position.x + position.width / 2, -position.y);
    }
    void rectTransformReconstruct2(RectTransform trans, Rect position)
    {
        trans.sizeDelta = new Vector2(position.width, position.height);
        trans.anchoredPosition = new Vector2(-position.x - position.width / 2, -position.y);
    }

    public void headDirectionDropdownEventHandler(int index)
    {
        pl.headDirection = index;
    }
    public void GainActiveDropdownEventHandler(int index)
    {
        pl.blackout = GainActivate.value;
    }
    public void GameTypeEventHandler(int index)
    {
        pl.gametype = GameType.value;
    }
    public void DuoGainEventHandler(int index)
    {
        pl.duogain = DuoGain.value;
    }
    public void OptotypeSizeHandler()
    {
        int.TryParse (OptotypeSizeInput.text, out pl.optytpeSizeChoice);
    }
	public void PlayerDistanceHandler()
	{
		float.TryParse (PlayerDistance.text, out pl.patientToScreenDistance);
	}
    public void LeftGainHandler()
    {
        float.TryParse (LeftGain.text, out pl.leftGain);
    }
    public void RightGainHandler()
    {
        float.TryParse(RightGain.text, out pl.rightGain);
    }
    public void SpeedThresholdHandler()
    {
        float.TryParse(SpeedThreshold.text, out pl.dvaHeadSpeedTriggerThreshold);
    }
    public void LookBackFramesHandler()
    {
        int.TryParse(LookBackFrames.text, out pl.dvaLookBackAmount);
    }
    public void LookBackUpperBoundHandler()
    {
        float.TryParse(LookBackUpperBound.text, out pl.dvaUpperHeadSpeedWindow);
    }
    public void LookBackLowerBoundHandler()
    {
        float.TryParse(LookBackLowerBound.text, out pl.dvaLowerHeadSpeedWindow);
    }
    public void OptotypeChangeWindowHandler()
    {
        int.TryParse(OptotypeChangeWindow.text, out pl.OptotypeWindow);
    }
    public void CorrentPercentageUpperBoundHandler()
    {
        float.TryParse(CorrectPercentageUpperBound.text, out pl.CorrectUpperBound);
    }
    public void CorrentPercentageLowerBoundHandler()
    {
        float.TryParse(CorrectPercentageLowerBound.text, out pl.CorrectLowerBound);
    }
    public void Restore()
    {
        Destroy(GameObject.Find("PreferenceLoader"));
        GameObject obj = Instantiate(Resources.Load("PreferenceLoader")) as GameObject;
        obj.name = "PreferenceLoader";
        pl = GameObject.Find("PreferenceLoader").GetComponent<PreferenceLoader>(); // Reload PreferenceLoader
    }
    public void Return()
    {
        showConfigurationPanel = false;
    }
}
