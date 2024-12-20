

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UIElements;

namespace MiniDungeon { 
    public class MazeBuilder : MonoBehaviour
    {
        private Maze maze;
        Vector3 wallSize = new Vector3(5f, 4f, 0.5f);
        [SerializeField] private GameObject cellGameObject;

        private void Start()
        {
            BuildMaze(10, 10);
        }

        private void BuildMaze(int numberOfRows, int numberOfCols)
        {
            maze = new Maze(numberOfRows, numberOfCols);

            Cell[,] generatedMazeCells = maze.GetMazeCells();

            Debug.Log("============ Start Building ==============");

            // for each cell
            foreach (Cell cell in generatedMazeCells) {

                GameObject cellCurrentGameObject = InstantiateCellOnPosition(cell);

                BuildCellWalls(cell, cellCurrentGameObject);
            }

            Debug.Log("Maze Created!");

            
            //xxx maze to null for garbage collector
            //maze = null;
        }


        private GameObject InstantiateCellOnPosition(Cell cell)
        {
            // calculate position based on size column and row
            float xSize = cellGameObject.GetComponent<CellGameObject>().GetSize().x;
            float zSize = cellGameObject.GetComponent <CellGameObject>().GetSize().z;
            Vector3 position = new Vector3(cell.GetIdCell().idColumn * xSize , 0f, cell.GetIdCell().idRow * zSize * -1);
            
            // instantiate
            return Instantiate(cellGameObject, position , Quaternion.Euler(0, 0, 0));
        }

        private void BuildCellWalls(Cell cell, GameObject cellCurrentGameObject)
        {
            Debug.Log($"cell=>{cell}, cell.GetHasNorthWall()=>{cell.GetHasNorthWall()}");
            Debug.Log($"cellCurrentGameObject.transform.position=>{cellCurrentGameObject.transform.position}");
            
            CellGameObject myCellGameObject = cellCurrentGameObject.GetComponent<CellGameObject>();

            if (cell.GetHasNorthWall() == false) { 
                myCellGameObject.DestroyNorthWall();
            }

            if (cell.GetHasSouthWall() == false)
            {
                myCellGameObject.DestroySouthWall();
            }

            if (cell.GetHasEastWall() == false) { myCellGameObject.DestroyEastWall(); }
            if(cell.GetHasWestWall() == false) {  myCellGameObject.DestroyWestWall(); }
            
        }

        // TODO implement a visible process of the maze building using coroutines
        private void BuildMazeStepByStep(int numberOfRows, int numberOfCols, float timeSteps)
        {
            throw new NotImplementedException();
        }

       


    }// class
}
