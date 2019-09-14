using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class PlatformManager : PlatformGenericSinglton<PlatformManager>
{
    // custom event handlers
    public delegate void PlatformManagerUpdateUI(PlatformConfigurationData pcd);
    public static event PlatformManagerUpdateUI OnPlatformManagerUpdateUI;

    public delegate void PlatformManagerUpdateCamera(PlatformConfigurationData pcd);
    public static event PlatformManagerUpdateCamera OnPlatformManagerUpdateCamera;

    // the actual platform for the nodes
    GameObject[,] platform;
    GameObject currentSelection;

    // platform that stores just NextPosition of platform's nodes
    PlatformDataNode[,] platformData;

    // default material of the nodes
    public Material myMaterial;

    // used to determine we are allowed to program nodes or not
    public bool Program;
    public bool SimulateTest;
    public bool ResetSimulation;

    public PlatformConfigurationData configurationData;

    // used for the update function to access specific nodes without using a for loop
    int i = 0, j = 0;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        #region Object Selection
        if (Input.GetMouseButtonUp(0))
        {
            if (Program)
            {
                #region Screen To World
                RaycastHit hitInfo = new RaycastHit();
                bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
                if (!IsPointerOverUIObject())
                {
                    if (hit)
                    {
                        Debug.Log("Hit " + hitInfo.transform.gameObject.name);

                        #region COLOR
                        hitInfo.transform.gameObject.GetComponent<Renderer>().material.color = Color.blue;

                        if (currentSelection != null)
                        {
                            //De-select old one
                            currentSelection.GetComponent<PlatformDataNode>().Selected = false;

                            //Now assign the new selection
                            currentSelection = hitInfo.transform.gameObject;

                            //Do select stuff, set selected to true & update UI
                            currentSelection.GetComponent<PlatformDataNode>().SelectNode();
                        }
                        else
                        {
                            currentSelection = hitInfo.transform.gameObject;
                            currentSelection.GetComponent<PlatformDataNode>().SelectNode();
                        }
                        #endregion
                    }
                    else
                    {
                        Debug.Log("No hit");
                    }
                    #endregion
                }
            }
            #endregion
        }
        if (SimulateTest)
        {
            // used "if platform != null" here
            // to handle "Missing reference exception error at line w/ platform[i,j]"
            if (platform !=  null)
            {
                if (i < configurationData.M)
                {
                    if (j < configurationData.N)
                    {
                        string s = string.Format("{0},{1}", i, j);
                        Debug.Log(s);

                        platform[i, j].GetComponent<PlatformDataNode>().NextPosition = platformData[i, j].NextPosition;
                        if (platform[i, j].GetComponent<PlatformDataNode>().NextPosition == 0)
                        {
                            platform[i, j].GetComponent<PlatformDataNode>().Simulate = false;
                        }
                        else
                        {
                            platform[i, j].GetComponent<PlatformDataNode>().Simulate = true;
                        }

                        //Debug.Log(platform[i, j].GetComponent<PlatformDataNode>().NextPosition);
                        j++;
                    }
                    if (j == configurationData.N)
                    {
                        // End of row, move onto next row
                        i++;
                        j = 0;
                    }
                }
                else
                {
                    // Restart indexes and update positions
                    Debug.Log("End of loop, updating positions");
                    UpdatePositions();
                    i = 0;
                    j = 0;

                }
            }
        }
    }

    // used for the event handlers
    private void OnEnable()
    {
        UIManagerV2.BuildPlatformOnClicked += UIManagerV2_OnBuildPlatformClicked;
        UIManagerV2.OnWriteProgramData += UIManagerV2_OnWriteProgramData;
        UIManagerV2.OnReadWriteLinesData += ReadFile;
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    // used for the event handlers
    private void OnDisable()
    {
        UIManagerV2.BuildPlatformOnClicked -= UIManagerV2_OnBuildPlatformClicked;
        UIManagerV2.OnWriteProgramData -= UIManagerV2_OnWriteProgramData;
        UIManagerV2.OnReadWriteLinesData -= ReadFile;
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }

    // used when build platform button is clicked
    public void UIManagerV2_OnBuildPlatformClicked(PlatformConfigurationData pcd)
    {
        DestroyPlatform();

        configurationData = pcd;

        BuildPlatform();
    }

    // used to manage the scene switching
    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        //handling simulate scene
        //which reads from txt file
        //************READ THIS******************
        //some kind of fuckery is afoot here
        switch (SceneManager.GetActiveScene().name)
        {
            case "Main Menu":
                //Don't build
                break;
            case "Simulate Scene":
                Program = false;
                SimulateTest = true;
                //reset i and j to start from beginning again
                i = 0;
                j = 0;
                //read program data before building
                ReadFile();
                BuildPlatform();
                break;
            case "Configuration Scene":
                SimulateTest = false;
                Program = false;
                if (platform != null)
                {
                    BuildPlatform();
                }
                break;
            case "Programming Scene":
                SimulateTest = false;
                Program = true;
                if (platform != null)
                {
                    BuildPlatform();
                }
                break;
        }

        //update camera position to center of platform
        if (OnPlatformManagerUpdateCamera != null)
        {
            //Update Camera, is called only in Programming & Configuration Scene
            OnPlatformManagerUpdateCamera(configurationData);
        }
        //update UI
        if (OnPlatformManagerUpdateUI != null)
        {
            OnPlatformManagerUpdateUI(configurationData);
        }

    }

    // used to create that one instance of the platform
    public void BuildPlatform()
    {
        platform = new GameObject[configurationData.M, configurationData.N];
        for (int i = 0; i < configurationData.M; i++)
        {
            for (int j = 0; j < configurationData.N; j++)
            {
                // create the actual cubes that will be used as nodes
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(i + (i * configurationData.deltaSpacing), 0, j + (j * configurationData.deltaSpacing));
                cube.transform.rotation = Quaternion.identity;
                cube.transform.localScale = new Vector3(1, 0.1f, 1);
                cube.name = string.Format("Cube-{0}-{1}", i, j);
                cube.GetComponent<Renderer>().material = myMaterial;

                // set the i, j, next position, and next color of each node
                cube.AddComponent<PlatformDataNode>();
                PlatformDataNode pdn = cube.GetComponent<PlatformDataNode>();
                pdn.NextColor = Color.white;
                pdn.NextPosition = cube.transform.position.y;
                pdn.node = cube;

                pdn.i = i;
                pdn.j = j;

                // finally add it to the game object platform
                platform[i, j] = cube;
            }
        }
    }

    // used to destroy any platform if needed
    public void DestroyPlatform()
    {
        if (platform != null)
        {
            for (int i = 0; i < configurationData.M; i++)
            {
                for (int j = 0; j < configurationData.N; j++)
                {
                    Destroy(platform[i, j]);
                }
            }
        }
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

    // write either to a new or existing text file
    public void UIManagerV2_OnWriteProgramData()
    {
        using (StreamWriter outputFile = new StreamWriter(Path.Combine(Application.dataPath, "WriteLines.txt")))
        {
            outputFile.WriteLine(configurationData.ToString());
            for (int i = 0; i < configurationData.M; i++)
            {
                for (int j = 0; j < configurationData.N; j++)
                {
                    outputFile.WriteLine(platform[i, j].GetComponent<PlatformDataNode>().ToString());
                }
            }
        }
        Debug.Log("Program has finished writing txt file");
    }

    // read from the text file
    public void ReadFile()
    {
        Debug.Log("Reader reached");
        StreamReader reader = new StreamReader(Path.Combine(Application.dataPath, "WriteLines.txt"));

        using (reader)
        {
            // prep the reader
            string line = reader.ReadLine();
            string[] entries;
            entries = line.Split(',');
            
            // 1st line is configuration data
            Debug.Log(entries[0] + "," + entries[1] + "," + entries[2] + "," + entries[3]);
            
            // Configure
            PlatformConfigurationData pcd = new PlatformConfigurationData();
            pcd.M = Convert.ToInt32(entries[0]);
            pcd.N = Convert.ToInt32(entries[1]);
            pcd.deltaSpacing = (float)Convert.ToDouble(entries[2]);
            pcd.RandomHeight = (float)Convert.ToDouble(entries[3]);

            // set global configurationData to local pcd
            // so we can access the correct information
            configurationData = pcd;

            // used to map each node accordingly
            platformData = new PlatformDataNode[pcd.M, pcd.N];

            // read through the text until complete
            while ((line = reader.ReadLine()) != null)
            {
                // create a new pdn to access i, j, and next position
                PlatformDataNode pdn = new PlatformDataNode();
                entries = line.Split(',');
                
                // rest of lines are now platform data nodes
                Debug.Log(entries[0] + "," + entries[1] + "," + entries[2]);
                
                // grab the i, j and next position of each node
                pdn.i = Convert.ToInt32(entries[0]);
                pdn.j = Convert.ToInt32(entries[1]);
                pdn.NextPosition = (float)Convert.ToDouble(entries[2]);

                // assign it accordingly to the platformData array
                platformData[pdn.i, pdn.j] = pdn;
            }
        }
        reader.Close();

    }

    public void UpdatePositions()
    {
        //updates program data at end of loops
        for (int i = 0; i < configurationData.M; i++)
        {
            for (int j = 0; j < configurationData.N; j++)
            {
                if (i == configurationData.M - 1)
                {
                    // last index so first row should be taking next position from last row
                    platformData[0, j].NextPosition = platform[i, j].GetComponent<PlatformDataNode>().NextPosition;
                }
                else
                {
                    // every other row pushes to the next row
                    platformData[i+1, j].NextPosition = platform[i, j].GetComponent<PlatformDataNode>().NextPosition;
                }
            }
        }

        // gotta change something to the first if statement here
    }
}
