﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreferenceLoader : MonoBehaviour {
	// Constant:
	public const float tanFiveOverSixty = 0.001454442f;
    public const float cm2ft = 0.032808399f;

	// Training Variables
	public float trainSpeedThresMin;
	public float trainSpeedThresMax;
	public float trainAccThresMin;
	public float trainAccThresMax;
	public float trainVariableGain;

	// SVA Variables
	public float svaInitialOpSize;
	public float svaTestResult;

	// DVA Variables
	public float dvaSpeedResultMin;
	public float dvaSpeedResultMax;
	public float dvaAcuityResult;
    public float dvaHeadSpeedTriggerThreshold;
	public float dvaLowerHeadSpeedWindow;
	public float dvaUpperHeadSpeedWindow;
	public float dvaSpeedThresStepMin;
	public float dvaSpeedThresStepMax;
	public float dvaAccThresMin;
	public float dvaAccThresMax;
	public float leftGain;
    public float rightGain;
    public int dvaLookBackAmount;
    //public float UnityperPixel = 60.0f / 1920.0f;

	// GamePlay Variables
	public float gpOptotypeSize;
	public float gpSpeedThresMin;
	public float gpSpeedThresMax;
	public float gpVariableGain;
	public float gpOptotypeSizeGain;
	public float gpSpeedThresReduce;
    public float PlayerDistance;
    public int OptotypeWindow;
    public float CorrectUpperBound;
    public float CorrectLowerBound;

    // Shared Variables
	public int optotypeShowDelayTime;
	public List<string> headDirectionOptions; // Specifies Head Direction Option
	public int headDirection;
	public List<string> optotypeShowTimesOptions; // Specifies Head Turn Mechanisms
	public int optotypeShowTimes;
	public List<string> trackerOptions;
	public int trackerType;
    public List<string> GainActivate;
    public int blackout;
    public List<string> GameType;
    public int gametype;
    public List<string> DuoGain;
    public int duogain;
    public int testInterval;
	public float patientToScreenDistance;
	public float dpi;  // Screen dpi information
	public float objectSizeScalerForStandardVision; // Given screen's dpi info and testing distance from patient, it stores the opject size scale so that optotype is the size of standard vision (20/20)
	public float keepHeadSteadyTime;
    public List<float> optotypeSize;
    public int optytpeSizeChoice = 5;
    public int DeterminedOptotypeSize;
    public float eps = 2.2204f * Mathf.Pow(10, -16);

	// Use this for initialization
	void Awake () { // Here must use Awake to initalize all variables. If use Start(), it will cause synchronize issue with other Start() functions call in other scripts

		DontDestroyOnLoad (this);

		trainSpeedThresMin = 30f;
		trainSpeedThresMax = 150f;
		trainAccThresMin = 1500f;
		trainAccThresMax = 4000f;
		trainVariableGain = 0f;

		svaInitialOpSize = 0.5f;
		svaTestResult = 0.5f; // Give an initial value for develop convenience



        //DVA options
		dvaSpeedResultMin = 60f;
		dvaSpeedResultMax = 80f;
		dvaSpeedThresStepMin = 8f;
		dvaSpeedThresStepMax = 10f;
		dvaAcuityResult = .5f; // Give an initial value for develop convenience
		dvaAccThresMin = 1500f;
		dvaAccThresMax = 4000f;
        
        //Non-Deprecated DVA Options
        dvaHeadSpeedTriggerThreshold = 100f;
        dvaLowerHeadSpeedWindow = 5f;
        dvaUpperHeadSpeedWindow = 100f;
        leftGain = 0f;
        rightGain = 0f;
        dvaLookBackAmount = 4;
        OptotypeWindow = 10;
        CorrectUpperBound = 0.8f;
        CorrectLowerBound = 0.5f;


        gpOptotypeSizeGain = .3f;
		gpSpeedThresReduce = 10f;
		gpSpeedThresMin = 100f; // Give an initial value for develop convenience
		gpSpeedThresMax = 120f; // Give an initial value for develop convenience
		gpVariableGain = 1f;

		optotypeShowDelayTime = 100;
		optotypeShowTimes = 1;
        testInterval = 3;
        patientToScreenDistance = 10f;    //100f * cm2ft; // 1 m
		keepHeadSteadyTime = 1f;
		dpi = Screen.dpi;
		if (dpi == 0) {
			Debug.LogError ("Screen Dpi info not known");
		}

		headDirectionOptions = new List<string> () {
			"Left Only", 
			"Right Only", 
			"Up Only", 
			"Down Only", 
			"Left and Right", 
			"Up and Down", 
			"Four Directions", 
			"Left or Right Randomly", 
			"Up or Down Randomly", 
			"Four Directions Randomly"
		};

        GainActivate = new List<string>()
        {
            "Activate",
            "De-Activate"
        };

		optotypeShowTimesOptions = new List<string> () {
			"1",
			"2",
			"3",
			"4",
			"5",
			"6"
		};

		trackerOptions = new List<string> () {
			"Polhemus",
            "Mouse"
		};

        optotypeSize= new List<float>()
        {
            1, 2, 3, 4, 5, 6, 7, 8 ,9 ,10, 11, 12
        };

        GameType = new List<string>()
        {
            "VR",
            "Monitor",
            "Polhemus"
        };

        DuoGain = new List<string>()
        {
            "Left Side Gain",
            "Right Side Gain",
            "Both Sides Gain",
            "No Gain"
        };
        //values
        dvaLookBackAmount = 4;
	}

	/*
	 * 
	 *  GUIRectWithObject and WorldToGUIPoint funtions
	 *  are used to calculate MeshRenderer size in pixels
	 *  given a MeshRenderer.
	 * 
	 *  Use it to help calculate real optotype size on screen
	 */
	public Rect GUIRectWithObject(MeshRenderer mesh)
	{
		Vector3 cen = mesh.bounds.center;
		Vector3 ext = mesh.bounds.extents;
		Vector2[] extentPoints = new Vector2[8]
		{
			WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z-ext.z)),
			WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z-ext.z)),
			WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y-ext.y, cen.z+ext.z)),
			WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y-ext.y, cen.z+ext.z)),
			WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z-ext.z)),
			WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z-ext.z)),
			WorldToGUIPoint(new Vector3(cen.x-ext.x, cen.y+ext.y, cen.z+ext.z)),
			WorldToGUIPoint(new Vector3(cen.x+ext.x, cen.y+ext.y, cen.z+ext.z))
		};
		Vector2 min = extentPoints[0];
		Vector2 max = extentPoints[0];
		foreach (Vector2 v in extentPoints)
		{
			min = Vector2.Min(min, v);
			max = Vector2.Max(max, v);
		}
		return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
	}

	public static Vector2 WorldToGUIPoint(Vector3 world)
	{
		Vector2 screenPoint = Camera.main.WorldToScreenPoint(world);
		screenPoint.y = (float) Screen.height - screenPoint.y;
		return screenPoint;
	}
}
