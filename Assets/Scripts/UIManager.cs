using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{

    //All text variables that get updated when changed are made
    public Text PlatformSize;
    public Text DeltaSpacing;
    public Text YRange;

    //Two sliders, for spacing and y-range
    public Slider DeltaSlider;
    public Slider Y_Slider;

    //Two input fields, for dimensions
    public InputField M_Input;
    public InputField N_Input;
    public Dropdown myDropdown;

    //Instance of other script to access all components from previous build
    public PlatformSimulation simulation;

    //Panels that will be hidden or unhidden
    public RectTransform left;
    public RectTransform right;

    //Boolean variable that determines if the side panels are hidden
    bool DisplayConfigPanelFlag = true;

    //Used to pass dimensions to other script
    int M;
    int N;

    //Didn't know how to get names from dropdown, so I use their values as an index to retrieve the name
    string[] dropdown = { "GrayScale Spectrum", "RedScale Spectrum", "GreenScale Spectrum", "BlueScale Spectrum", "Random Spectrum" };

    // Use this for initialization
    void Start()
    {
        //Display default sizing. changes after inputs from user
        PlatformSize.text = "Platform Size: MxN";
        DeltaSpacing.text = "Spacing: " + simulation.spacing + "";
        YRange.text = "Y-Range: " + simulation.range;
        DeltaSlider.value = simulation.spacing;
        Y_Slider.value = simulation.range;
    }

    //All of my Click methods here***********************************************************************************
    public void StartButtonClick() //Button click method to start the simulation
    {
        if (simulation.built)
        {
            if (simulation.simulate == true)
            {
                Debug.Log("Sim Button clicked, sim ending");
            }
            else
            {
                Debug.Log("Sim Button clicked, sim start");
            }
            simulation.simulate = !simulation.simulate;
        }
    }

    public void SetupButtonClick() //hide or unhide the panels
    {
        Debug.Log("Setup Button clicked");
        DisplayConfigPanelFlag = !DisplayConfigPanelFlag;
        if (DisplayConfigPanelFlag)
        {
            left.gameObject.SetActive(true);
            right.gameObject.SetActive(true);
        }
        else
        {
            left.gameObject.SetActive(false);
            right.gameObject.SetActive(false);
        }
    }

    public void BuildButtonClick()
    {
        Debug.Log("Build Button clicked");
        //start conditional here to change code
        //if(M and N are empty fields) //user hasn't entered new dimensions
        if (M_Input.text != "" && N_Input.text == "") //N field empty
        {
            //Fill N field with default value
            N_Input.text = simulation.N.ToString();
            N = simulation.N;
        }
        if (M_Input.text == "" && N_Input.text != "") //M field empty
        {
            //Fill M field with default value
            M_Input.text = simulation.M.ToString();
            M = simulation.M;
        }
        if (M_Input.text == "" && N_Input.text == "") //Both fields empty
        {
            //  fill the text fields with default dimensions
            M_Input.text = simulation.M.ToString();
            N_Input.text = simulation.N.ToString();
            //Set M and N values to default values
            M = simulation.M;
            N = simulation.N;
            //No need to build new objects, old ones will be fine
            //since there were no new dimensions specified
        }
        simulation.BuildSim(M, N, DeltaSlider.value);

        //change color scheme if dropdown value was changed
        simulation.ColorScale = dropdown[myDropdown.value];
        //Update PlatformSize text field
        //Delta spacing text and Y-range changes dynamically, no need to update
        PlatformSize.text = "Platform Size:" + M + "x" + N;
        Debug.Log("BuldButtonClick() - Color Scale: " + simulation.ColorScale + "Dropdown value: " + myDropdown.value);
    }

    public void ExitButtonClick()
    {
        Debug.Log("Exit Button clicked");
        Application.Quit();
    }
    //End of my button click methods here***********************************************************************************

    public void SliderValueChage() //Change delta spacing of the tiles
    {
        Debug.Log(DeltaSlider.value);
        //simulation.spacing = DeltaSlider.value;
        DeltaSpacing.text = "Spacing: " + DeltaSlider.value + "";
    }

    public void Y_SliderValueChange() //Change Y-range displacement of the tiles
    {
        Debug.Log(Y_Slider.value);
        simulation.range = Y_Slider.value;
        YRange.text = "Y-Range: " + simulation.range;
    }

    public void OnDropDownChange() //Change color scale based on drop down value
    {
        if (myDropdown.value == 0)
        {
            //Gray Scale Selected
            simulation.ColorScale = "GrayScale Spectrum";
        }
        if (myDropdown.value == 1)
        {
            //Red Scale Selected
            simulation.ColorScale = "RedScale Spectrum";
        }
        if (myDropdown.value == 2)
        {
            //Green Scale Selected
            simulation.ColorScale = "GreenScale Spectrum";
        }
        if (myDropdown.value == 3)
        {
            //Blue Scale Selected
            simulation.ColorScale = "BlueScale Spectrum";
        }
        if (myDropdown.value == 4)
        {
            //Random Scale Selected
            simulation.ColorScale = "Random Spectrum";
        }
        Debug.Log(myDropdown.value + ": " + simulation.ColorScale);
    }

    public void OnM_InputEntered()
    {
        M = int.Parse(M_Input.text);
    }

    public void OnN_InputEntered()
    {
        N = int.Parse(N_Input.text);
    }

    //Used to determine if UI is over a game object
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        foreach (var result in results)
        {
            Debug.Log(result.gameObject.name);
        }
        return results.Count > 0;
    }

    //Start of new code here ***********************************************************************************

    public void OnClick(Button b)
    {
        switch (b.name)
        {
            case "Configuration":
                //Go to Configuration scene
                SceneManager.LoadScene("Platform Configuration");
                break;
            case "Program":
                //Go to Program scene
                //SceneManager.LoadScene("Program Scene");
                break;
            case "Simulate":
                //Go to Simulate scene

                SceneManager.LoadScene("Platform Simulation");
                break;
            case "Exit":
                //Go to Exit scene
                Application.Quit();
                break;


        }
    }
}

    //End of new code here **************************************************************************************