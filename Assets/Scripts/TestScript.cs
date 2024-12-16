using MiniDungeon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.LogWarning("You must delete TestScript");

        List<Cell> listA = new List<Cell>();

        listA.Add(new Cell(0, 0));
        listA.Add(new Cell(0, 1));

        //Debug.Log($"cell=>{listA[0]}");

        /*
        List<Cell> listB = new List<Cell>(listA.Count);

        listA.ForEach(cell => listB.Add(cell));

        Debug.Log($"listA[0].GetCurrentDirection() => {listA[0].GetCurrentDirection()}");
        Debug.Log($"listB[0].GetCurrentDirection() => {listB[0].GetCurrentDirection()}\n--- Change Direction ----");
        listA[0].SetDirection(Direction.Down);
        Debug.Log($"listA[0].GetCurrentDirection() => {listA[0].GetCurrentDirection()}");
        Debug.Log($"listB[0].GetCurrentDirection() => {listB[0].GetCurrentDirection()}");


        listA.Clear();
        Debug.Log($"ListB.count={listB.Count} // ListA.count={listA.Count}");
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
