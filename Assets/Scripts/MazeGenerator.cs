using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniDungeon { 
    public class MazeGenerator : MonoBehaviour
    {
        private Maze maze;
        private int wallSize = 1;
        [SerializeField] private GameObject wall;


        private void GenerateMaze()
        {
            maze = new Maze(3, 3);

            List<Cell> generatedMazeCells = maze.GetMazeCells();
            int currentRow = 0;

            // for each cell
            foreach (Cell cell in generatedMazeCells) {

                if (cell.GetIdRow() == 0)
                {
                    // build up
                }

                if (cell.GetIdCol() == 0)
                {
                    // build left
                }

                if (cell.GetCurrentDirection() != Direction.Right)
                {
                    // check if there's a path pointing to the current cell
                    Cell downCell = maze.FindGeneratedCellByIdRowIdCol(cell.GetIdRow(), cell.GetIdCol()+1);
                    if (downCell.GetCurrentDirection() != Direction.Up)
                    {
                        // build a wall down
                    }
                }

                // OJO QUE PASA CUANDO LLEGAMOS ABAJO DEL TODO!?!?!?!
                if (cell.GetCurrentDirection() != Direction.Down)
                {
                    // check if there's a path pointing to the current cell
                    Cell downCell = maze.FindGeneratedCellByIdRowIdCol(cell.GetIdRow() - 1, cell.GetIdCol());
                    if (downCell.GetCurrentDirection() != Direction.Up)
                    {
                        // build a wall down
                    }
                }
                
                
            }
        }
    }
}
