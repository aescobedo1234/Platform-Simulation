using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basic : MonoBehaviour
{
    public float spacing = 0.1f;
    public float scaleOnY = 0.1f;
    public float Range = 1.0f;
    public int M = 10;
    public int N = 10;
    GameObject[,] platform;
    float[,] nextPosition;
    public bool Simulate;
    GameObject currentSelection = null;
    Color NextColor;
    float y = 0.0f;
    // Use this for initialization
    void Start()
    {
        platform = new GameObject[M, N];
        nextPosition = new float[M, N];
        for (int i = 0; i < M; i++)
        {
            for (int j = 0; j < N; j++)
            {
                GameObject cube =
                GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(i + (i * spacing),
                0, j + (j * spacing));
                cube.transform.rotation = Quaternion.identity;
                cube.transform.localScale = new Vector3(1, scaleOnY, 1);
                cube.name = string.Format("Cube-{0}{1}", i, j);
                platform[i, j] = cube;
                nextPosition[i, j] = cube.transform.position.y;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Simulate)
        {
            if (currentSelection != null)
            {
                // smooth transition the color ...
                currentSelection.transform.gameObject.GetComponent<Renderer>().material.color =
            Color.Lerp(
            currentSelection.transform.gameObject.GetComponent
            <Renderer>().material.color,
            NextColor,
            Time.deltaTime);
                // smooth transition the position
                currentSelection.transform.position =
                Vector3.Lerp(currentSelection.transform.position,
                new
                Vector3(currentSelection.transform.position.x,
                y,
                currentSelection.transform.position.z),
                Time.deltaTime);
            }
        }
        else
        {
            if (currentSelection != null)
            {
                //float y = UnityEngine.Random.Range(-1.0f, 1.0f);
                // change color
                currentSelection.transform.gameObject.GetComponent<Renderer>().material.color = NextColor;
                // change position
                currentSelection.transform.position =
                new Vector3(currentSelection.transform.position.x, y,
                currentSelection.transform.position.z);
            }
        }
        if (currentSelection != null)
        {
            if (Input.GetKey(KeyCode.X))
                currentSelection.transform.Rotate(new Vector3(1, 0, 0),
                1.0f);
            if (Input.GetKey(KeyCode.Y))
                currentSelection.transform.Rotate(new Vector3(0, 1, 0),
                1.0f);
            if (Input.GetKey(KeyCode.Z))
                currentSelection.transform.Rotate(new Vector3(0, 0, 1),
                1.0f);
        }
        #region Mouse Input
        if (Input.GetMouseButton(0))
        {
            Debug.Log("Pressed left click.");
        }
        #region Object Selection
        if (Input.GetMouseButtonUp(0))
        {
            #region Screen To World
            RaycastHit hitInfo = new RaycastHit();
            bool hit =
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out
            hitInfo);
            if (hit)
            {
                Debug.Log("Hit " + hitInfo.transform.gameObject.name);
                #region COLOR
                //hitInfo.transform.gameObject.GetComponent<Renderer>().material.color = Color.red;
                NextColor = Color.red;
                y = UnityEngine.Random.Range(-Range, Range);
                if (currentSelection != null)
                {
                    currentSelection.transform.gameObject.GetComponent<Renderer>().material.color = Color.white;
                    currentSelection.transform.position = new
                    Vector3(currentSelection.transform.position.x,
                    0, currentSelection.transform.position.z);
                    currentSelection = hitInfo.transform.gameObject;
                }
                else
                {
                    currentSelection = hitInfo.transform.gameObject;
                }
                #endregion
            }
            else
            {
                Debug.Log("No hit");
            }
            #endregion
        }
        //if (currentSelection != null)
        //    
        currentSelection.transform.gameObject.GetComponent<Renderer>().material.color =
    Color.Lerp(currentSelection.transform.gameObject.GetComponent<Renderer>().
    material.color, NextColor, Time.deltaTime);
        #endregion
        if (Input.GetMouseButton(1))
        {
            Debug.Log("Pressed right click.");
        }
        if (Input.GetMouseButton(2))
        {
            Debug.Log("Pressed middle click.");
        }
        #endregion
    }
}