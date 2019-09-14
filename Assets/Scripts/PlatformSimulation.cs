using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSimulation: MonoBehaviour
{
    public int M = 16;
    public int N = 9;
    public float spacing = 0.1f;

    GameObject[,] platform;
    float[,] nextPosition;
    Color[,] nextColor;
    public Material myMaterial;

    public bool simulate;
    public bool built;
    public string ColorScale;
    public float range = 1.0f;

    public UIManager ui;

    // Use this for initialization
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        if (built)
        {
            //float color = Random.Range(0f, 1.0f);
            if (Input.GetKeyDown(KeyCode.T)) //Key to start simulation
            {
                simulate = !simulate;
            }
            if (Input.GetKeyDown(KeyCode.W)) //Increase random range of displacement on y-axis
            {
                if (range < 7)
                {
                    range += 1.0f;
                    ui.Y_Slider.value += 1.0f;
                    ui.YRange.text = "Y-Range: " + range;
                }
            }
            if (Input.GetKeyDown(KeyCode.S)) //Decrease random range of displacement on y-axis
            {
                if (range > 0)
                {
                    range -= 1.0f;
                    ui.Y_Slider.value -= 1.0f;
                    ui.YRange.text = "Y-Range: " + range;
                }
            }
            if (Input.GetKeyDown(KeyCode.Q)) //End the whole project
            {
                Application.Quit();
            }
            if (Input.GetKeyDown(KeyCode.R)) //Generate colors in the green spectrum
            {
                ColorScale = "RedScale Spectrum";
                ui.myDropdown.value = 1;
            }
            if (Input.GetKeyDown(KeyCode.G)) //Generate colors in the green spectrum
            {
                ColorScale = "GreenScale Spectrum";
                ui.myDropdown.value = 2;
            }
            if (Input.GetKeyDown(KeyCode.B)) //Generate colors in the blue spectrum
            {
                ColorScale = "BlueScale Spectrum";
                ui.myDropdown.value = 3;
            }
            if (Input.GetKeyDown(KeyCode.H)) //Generate colors in the gray spectrum
            {
                ColorScale = "GrayScale Spectrum";
                ui.myDropdown.value = 0;
            }
            if (Input.GetKeyDown(KeyCode.E)) //Generate random RGB colors
            {
                ColorScale = "Random Spectrum";
                ui.myDropdown.value = 4;
            }

            if (simulate) //start the simulation
            {
                for (int i = 0; i < M; i++)
                {
                    for (int j = 0; j < N; j++)
                    {
                        //if current position = next position
                        //time to get a new position
                        if (Threshold(platform[i, j].transform.position.y, nextPosition[i, j]) >= 0
                            && Threshold(platform[i, j].transform.position.y, nextPosition[i, j]) <= 0.003)
                        {
                            nextPosition[i, j] = UnityEngine.Random.Range(-range, range);
                            // next color ... 
                            float c = Random.Range(0f, 1.0f);
                            if (ColorScale == "RedScale Spectrum")
                            {
                                nextColor[i, j] = new Color(c, 0, 0);
                            }
                            if (ColorScale == "GreenScale Spectrum")
                            {
                                nextColor[i, j] = new Color(0, c, 0);
                            }
                            if (ColorScale == "BlueScale Spectrum")
                            {
                                nextColor[i, j] = new Color(0, 0, c);
                            }
                            if (ColorScale == "GrayScale Spectrum")
                            {
                                nextColor[i, j] = new Color(c, c, c);
                            }
                            if (ColorScale == "Random Spectrum")
                            {
                                nextColor[i, j] = new Color(Random.Range(0f, 1.0f), Random.Range(0f, 1.0f), Random.Range(0f, 1.0f));
                            }
                        }

                        //smooth transition the color
                        platform[i, j].transform.gameObject.GetComponent<Renderer>().material.color =
                        Color.Lerp(platform[i, j].transform.gameObject.GetComponent<Renderer>().material.color, nextColor[i, j], Time.deltaTime);

                        //smooth transition the position
                        platform[i, j].transform.position = Vector3.Lerp(platform[i, j].transform.position,
                            new Vector3(platform[i, j].transform.position.x, nextPosition[i, j], platform[i, j].transform.position.z),
                            Time.deltaTime);
                    }
                }
            }
            else
            {
                //Go back to original state
                for (int i = 0; i < M; i++)
                {
                    for (int j = 0; j < N; j++)
                    {
                        //Set position of all in double loop
                        //like when it was initialized
                        //smooth transition the position
                        platform[i, j].transform.position = Vector3.Lerp(platform[i, j].transform.position,
                            new Vector3(i + (i * spacing), 0, j + (j * spacing)),
                            Time.deltaTime);
                        nextPosition[i, j] = platform[i, j].transform.position.y;
                        //from whatever position you're at
                        //go back to default y position at 0
                        //smooth transition the color back to white
                        platform[i, j].transform.gameObject.GetComponent<Renderer>().material.color =
                        Color.Lerp(platform[i, j].transform.gameObject.GetComponent<Renderer>().material.color, Color.white, Time.deltaTime);
                        //ColorScale = "GrayScale Spectrum";
                    }
                }
            }
        }
    }

    //return a number for the platforms to determine
    //if they should move
    float Threshold(float a, float b)
    {
        return Random.Range(0.00299f, 0.005f);
    }

    //New Code added for Phase 2, Part A

    public void BuildSim(int M, int N, float spacing)
    {
        //Destroy former platforms
        if (built)
        {
            DestroyOldStuff();
        }

        //create new plaftform with new size and spacing
        this.M = M;
        this.N = N;
        this.spacing = spacing;
        platform = new GameObject[M, N];
        nextPosition = new float[M, N];
        nextColor = new Color[M, N];
        for (int i = 0; i < M; i++)
        {
            for (int j = 0; j < N; j++)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(i + (i * spacing), 0, j + (j * spacing));
                cube.transform.rotation = Quaternion.identity;
                cube.transform.localScale = new Vector3(1, 0.1f, 1);
                cube.name = string.Format("Node-{0}-{1}", i, j);
                cube.GetComponent<Renderer>().material = myMaterial;

                platform[i, j] = cube;
                nextPosition[i, j] = cube.transform.position.y;
                nextColor[i, j] = Color.white;
            }
        }
        
        built = true;
    }


    public void DestroyOldStuff()
    {
        for (int i = 0; i < M; i++)
        {
            for (int j = 0; j < N; j++)
            {
                Destroy(platform[i, j]);
            }
        }
    }

    //End of new code from Phase 2, Part A
}
