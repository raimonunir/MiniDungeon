using MiniDungeon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        const string wall = "#";

        string test1 = "abcd";
        //Debug.LogWarning(test1.Substring(1, 1));

        Debug.Log(wall+wall+wall);
        Debug.Log("--");
        Debug.Log("|  |");
        Debug.Log("__");
        Debug.LogWarning("You must delete TestScript");

        int[] arrayA = { 6, 2, 3, 4 };
        int[] arrayB = { 6, 2, 3, 4 };

        int[,] arrayBi;

        arrayBi = new int[2,2];


        arrayBi[1,1] = 69;

        Debug.Log($"arrayBi[1,1]=>{arrayBi[1, 1]}, {arrayBi}");

        if (Enumerable.SequenceEqual(arrayA, arrayB))
        {
            Debug.Log("They are equal");
        }
        else
        {
            Debug.Log("Not equal");
        }

        /*
        List<Cell> listA = new List<Cell>();

        listA.Add(new Cell(0, 0));
        listA.Add(new Cell(0, 1));

        //Debug.Log($"cell=>{listA[0]}");

        
        List<Cell> listB = new List<Cell>(listA.Count);

        listA.ForEach(cell => listB.Add(cell));

        Debug.Log($"listA[0].GetInMaze() => {listA[0].GetInMaze()}");
        Debug.Log($"listA[1].GetInMaze() => {listA[1].GetInMaze()}");
        listB[0].SetInMaze();
        Debug.Log($"listA[0].GetInMaze() => {listA[0].GetInMaze()}");
        Debug.Log($"listA[1].GetInMaze() => {listA[1].GetInMaze()}");


        listB.RemoveAt(1);
        Debug.Log($"ListB.count={listB.Count} // ListA.count={listA.Count}");
        */
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
