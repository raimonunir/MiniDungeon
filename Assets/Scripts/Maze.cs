using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace MiniDungeon
{
    public class Maze
    {
        private int numberOfRows;
        private int numberOfCols;
        private int numberOfLevels;
        private List<Cell> generatedCells;
        private List<Cell> cellsNotInMaze;

        public Maze(int numberOfRows, int numberOfCols, int numberOfLevels = 1)
        {
            if (numberOfRows < 2 || numberOfCols < 2)
            {
                Debug.LogError("numberOfRows and numberOfCols must be greater than 1");
            }

            if (numberOfLevels < 1)
            {
                Debug.LogError("numberOfLevel must be greater than 0");
            }

            this.numberOfRows = numberOfRows;
            this.numberOfCols = numberOfCols;
            this.numberOfLevels = numberOfLevels;

            // Add new generatedCells to the List
            GenerateCells();

            // Calculate a random Maze
            CalculateMaze();
        }

        public int GetNumberOfRows() { return numberOfRows; }
        public int GetNumberOfCols() { return numberOfCols; }
        public int GetNumberOfLevels() { return numberOfLevels; }

        private void GenerateCells()
        {
            // starts from top-left, then right until end of the row, then down to the next row
            /*  [row,col] = row, col
             *  
             *  [0,0][0,1][0,2]  
             *  [1,0][1,1][1,2]
             *  [2,0][2,1][2,2]
             */
            for (int row = 0; row < numberOfRows; row++)
            {
                for (int col = 0; col < numberOfCols; col++)
                {
                    Cell newCell = new Cell(row, col);

                    if (row == 0)
                    {
                        newCell.SetNotAllowedDirection(Direction.Up);
                    }
                    else if (row == numberOfRows - 1)
                    {
                        newCell.SetNotAllowedDirection(Direction.Down);
                    }

                    if (col == 0)
                    {
                        newCell.SetNotAllowedDirection(Direction.Left);
                    }
                    else if (col == numberOfCols - 1)
                    {
                        newCell.SetNotAllowedDirection(Direction.Right);
                    }

                    // add cell to the matrix
                    generatedCells.Add(newCell);
                }
            }
        }

        /****
         * CalculateMaze() - based on Wilson's algorithm (random walk)
         * 
         * https://weblog.jamisbuck.org/2011/1/20/maze-generation-wilson-s-algorithm
         * */
        private void CalculateMaze()
        {
            // populate list of cells not yet in the maze
            cellsNotInMaze = new List<Cell>(generatedCells.Count);
            generatedCells.ForEach(cell => cellsNotInMaze.Add(cell));

            // pick randomly a cell and add it to the maze
            AddCellToMaze(UnityEngine.Random.Range(0, cellsNotInMaze.Count));

            while(cellsNotInMaze.Count > 0)
            {
                // pick randomly a not in maze cell
                int firstCellIndexNotInMaze = UnityEngine.Random.Range(0, cellsNotInMaze.Count);
                int currentCellIndexNotInMaze = firstCellIndexNotInMaze;
                

                while (true) {
                    // set randomly a direction
                    cellsNotInMaze[currentCellIndexNotInMaze].SetCurrentDirectionRandomly();

                    // walk to the next cell
                    //TODO improve performance (implement Cell.nextCellLeft(), Cell.nextCellUp(), etc.)
                    int nextIdRow = cellsNotInMaze[currentCellIndexNotInMaze].GetNextRowIdByCurrentDirection();
                    int nextIdCol = cellsNotInMaze[currentCellIndexNotInMaze].GetNextIdColByCurrentDirection();

                    Cell nextCell = FindGeneratedCellByIdRowIdCol(nextIdRow, nextIdCol);      
                    
                    if (nextCell.IsInMaze())
                    {
                        break;// if the nextCell is in Maze we are done!
                    }
                    // if not walk another step
                    currentCellIndexNotInMaze = FindCellIndexNotInMazeByIdRowIdCol(nextIdRow, nextIdCol);
                }

                // Set the final walked path
                currentCellIndexNotInMaze = firstCellIndexNotInMaze;
                while (true)
                {
                    // first get the next cell
                    //TODO improve performance (implement Cell.nextCellLeft(), Cell.nextCellUp(), etc.)
                    int nextIdRow = cellsNotInMaze[currentCellIndexNotInMaze].GetNextRowIdByCurrentDirection();
                    int nextIdCol = cellsNotInMaze[currentCellIndexNotInMaze].GetNextIdColByCurrentDirection();

                    Cell nextCell = FindGeneratedCellByIdRowIdCol(nextIdRow, nextIdCol);

                    // add the current cell to the maze
                    AddCellToMaze(currentCellIndexNotInMaze);

                    if (nextCell.IsInMaze())
                    {
                        // if the nextCell is in Maze we are done...
                        break;
                    }

                    //...if not, walk to the next cell
                    currentCellIndexNotInMaze = FindCellIndexNotInMazeByIdRowIdCol(nextIdRow, nextIdCol);
                }
                
            }
        }

        public List<Cell> GetMazeCells()
        {
            return generatedCells;
        }

        private void AddCellToMaze(int indexAtCellsNotInMaze)
        {
            cellsNotInMaze[indexAtCellsNotInMaze].SetInMaze();
            cellsNotInMaze.RemoveAt(indexAtCellsNotInMaze);
        }

        /****
         * return -1 if there is no cell inside the list with that idRow and idCol
         */
        private int FindCellIndexNotInMazeByIdRowIdCol(int idRow, int idCol)
        {
            for (int i = 0; i < cellsNotInMaze.Count; i++)
            {
                if (cellsNotInMaze[i].GetIdRow() == idRow && cellsNotInMaze[i].GetIdCol() == idCol)
                {
                    return i;
                }
            }

            return -1;
        }

        public Cell FindGeneratedCellByIdRowIdCol(int idRow,  int idCol)
        {
            foreach (Cell cell in generatedCells) { 
                if(cell.GetIdRow() == idRow && cell.GetIdCol() == idCol) return cell;
            }

            Debug.LogError($"Imposible to Find Cell with idRow={idRow} and idCol={idCol} in generatedCells");
            return null;    
        }

        private void PrintMaze()
        {
            int currentRow = 0;
            string mazeCurrentRow = "";
            foreach (Cell cell in cellsNotInMaze)
            {
                // inicially all directions are allowed
                if(cell.GetCurrentDirection() == Direction.Up)
                {
                    mazeCurrentRow += "[^]";
                }
                //TODO
            }
        }
    }


}

