using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell : MonoBehaviour
{

    public GameObject leftWall;
    public GameObject rightWall;
    public GameObject bottomWall;
    public GameObject topWall;
    public GameObject floor;

    public bool visited = false;
    public int x;
    public int z;


    public void Initialize(int xPos, int zPos)      //셀 초기화
    {
        x = xPos;
        z = zPos;
        visited = false;
        ShowAllWalls();
    }

    public void ShowAllWalls()          //모든 벽 표시
    {
        leftWall.SetActive(true);
        rightWall.SetActive(true);
        bottomWall.SetActive(true);
        topWall.SetActive(true);
        floor.SetActive(true);
    }

    public void RemoveWall(string direction)            //특정 방향 벽 제거
    {
        switch (direction)
        {
            case "left":
                leftWall.SetActive(false);
                break;
            case "right":
                rightWall.SetActive(false);
                break;
            case "top":
                topWall.SetActive(false);
                break;
            case "bottom":
                bottomWall.SetActive(false);
                break;
        }
    }

    public void SetColor(Color color)       //셀 색상 변경(경로 표시용)
    {
        floor.GetComponent<Renderer>().material.color = color;
    }           
}
