
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Assertions;

namespace MiniDungeon
{

    /*!****
    * @brief CellMazeDirection contains all the posible directions
    * **/
    public enum CellMazeDirection { West, East, North, South };

    /*!***
     * @brief Maze uses cells to calculate a maze
     *  **/
    public class Maze
    {
        private int numberOfRows;
        private int numberOfColumns;
        private int numberOfFloors;
        private bool mazeHasBeenCalculated = false;
        private Cell[,] cellMazeMatrix;
        private List<Cell> cellsNotYetInMaze;

        public Maze(int numberOfRows, int numberOfCols, int numberOfLevels = 1)
        {
            if (numberOfRows < 2 || numberOfCols < 2)
            {
                Debug.LogError("numberOfRows and numberOfColumns must be greater than 1");
            }

            if (numberOfLevels < 1)
            {
                Debug.LogError("numberOfLevel must be greater than 0");
            }

            this.numberOfRows = numberOfRows;
            this.numberOfColumns = numberOfCols;
            this.numberOfFloors = numberOfLevels;

            // Populate cellMazeMatrix
            PopulateCellMazeMatrix();

            // Calculate a random Maze
            CalculateMaze();

            // Design MazeWalls
            DesignCellsWalls();
        }

        public int GetNumberOfRows() { return numberOfRows; }
        public int GetNumberOfCols() { return numberOfColumns; }
        public int GetNumberOfLevels() { return numberOfFloors; }

        //TODO implement: it is possible to populate the cellMazeMatrix from an existing array as parameter
        private void PopulateCellMazeMatrix()
        {
            cellMazeMatrix = new Cell[numberOfColumns, numberOfRows];

            // starts from top-left, then right until end of the row, then down to the next row
            /*  [row,col] = row, col
             *  
             *  [0,0][0,1][0,2]  
             *  [1,0][1,1][1,2]
             *  [2,0][2,1][2,2]
             */
            int floor = 0;

            for (int row = 0; row < numberOfRows; row++)
            {
                for (int col = 0; col < numberOfColumns; col++)
                {
                    Cell newCell = new Cell(col, row, floor);

                    if (row == 0)
                    {
                        newCell.SetNotAllowedMazeDirection(CellMazeDirection.North);
                    }
                    else if (row == numberOfRows - 1)
                    {
                        newCell.SetNotAllowedMazeDirection(CellMazeDirection.South);
                    }

                    if (col == 0)
                    {
                        newCell.SetNotAllowedMazeDirection(CellMazeDirection.West);
                    }
                    else if (col == numberOfColumns - 1)
                    {
                        newCell.SetNotAllowedMazeDirection(CellMazeDirection.East);
                    }

                    // add cell to the matrix
                    Debug.Log($"{newCell} added to cellMazeMatrix");
                    cellMazeMatrix[col,row] = newCell;
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
            cellsNotYetInMaze = new List<Cell>(cellMazeMatrix.Length);
            foreach (Cell cell in cellMazeMatrix)
            {
                cellsNotYetInMaze.Add(cell);
            }

            // pick randomly a cell and add it to the maze
            AddCellToMaze(UnityEngine.Random.Range(0, cellsNotYetInMaze.Count));

            while(cellsNotYetInMaze.Count > 0)
            {
                // pick randomly a not in maze cell
                int firstCellIndexNotInMaze = UnityEngine.Random.Range(0, cellsNotYetInMaze.Count);
                int currentCellIndexNotInMaze = firstCellIndexNotInMaze;
                Cell currentCellNotInMaze = cellsNotYetInMaze[currentCellIndexNotInMaze];


                while (true) {

                    // set randomly a direction
                    SetRandomlyCellMazeDirection(currentCellNotInMaze);

                    // walk to the next cell
                    int nextIdRow = currentCellNotInMaze.GetNextRowIdByCurrentMazeDirection();
                    int nextIdCol = currentCellNotInMaze.GetNextIdColByCurrentMazeDirection();

                    Cell nextCell = FindGeneratedCellByIdColIdRow(nextIdRow, nextIdCol);      
                    
                    if (nextCell.IsInMaze())
                    {
                        break;// if the nextCell is in Maze we are done!
                    }
                    // if not walk another step
                    currentCellIndexNotInMaze = FindCellIndexNotInMazeByIdRowIdCol(nextIdRow, nextIdCol);
                    currentCellNotInMaze = cellsNotYetInMaze[currentCellIndexNotInMaze];
                }

                // Set the final walked path
                currentCellIndexNotInMaze = firstCellIndexNotInMaze;
                while (true)
                {
                    Cell currentCell = cellsNotYetInMaze[currentCellIndexNotInMaze];
                   
                    // first get the next cell
                    int nextIdRow = currentCell.GetNextRowIdByCurrentMazeDirection();
                    int nextIdCol = currentCell.GetNextIdColByCurrentMazeDirection();

                    Cell nextCell = FindGeneratedCellByIdColIdRow(nextIdRow, nextIdCol);

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

            // mark maze has been calculated
            mazeHasBeenCalculated = true;
        }

        private void DesignCellsWalls()
        {
            if (!mazeHasBeenCalculated)
            {
                Debug.LogWarning("DesignCellsWalls must be executed after maze has been Calculated");
            }

            foreach (Cell cell in cellMazeMatrix) 
            {
                cell.SetHasAllWalls(true);

                CellMazeDirection direction = cell.GetCurrentCellMazeDirection();

                // remove wall according to cell direction
                if (direction == CellMazeDirection.North)
                {
                    cell.SetHasNorthWall(false);
                }
                else if (direction == CellMazeDirection.South)
                {
                    cell.SetHasSouthWall(false);
                }
                else if (direction == CellMazeDirection.East) { cell.SetHasEastWall(false); }
                else if (direction == CellMazeDirection.West) { cell.SetHasWestWall(false); }

                // removes cell walls according to neighboring cells
                int cellIdColumn = cell.GetIdColumn();
                int cellIdRow = cell.GetIdRow();

                // checks north wall if it is not a top north row cell and has northwall
                if (cell.GetHasNorthWall() && cellIdRow > 0)
                {
                    //  if his northCell is poiting to this cell destroy north wall
                    Cell northCell = FindGeneratedCellByIdColIdRow(cellIdColumn, cellIdRow - 1);
                    if (northCell.GetCurrentCellMazeDirection() == CellMazeDirection.South)
                    {
                        cell.SetHasNorthWall(false) ;
                    }
                }

                // if  it is not a top south row cell and has sourthwall
                if (cell.GetHasSouthWall() && cellIdRow < numberOfRows - 1) { 
                    
                    // if the cell has a southcell pointing to north destroy southwall
                    Cell southCell = FindGeneratedCellByIdColIdRow(cellIdColumn, cellIdRow + 1);
                    if (southCell.GetCurrentCellMazeDirection() == CellMazeDirection.North)
                    {
                        cell.SetHasSouthWall(false) ;
                    }
                }

                if (cell.GetHasEastWall() && cellIdColumn > 0) { 
                    Cell eastCell = FindGeneratedCellByIdColIdRow(cellIdColumn-1, cellIdRow);
                    if (eastCell.GetCurrentCellMazeDirection() == CellMazeDirection.West) { 
                        cell.SetHasEastWall(false) ;
                    }
                }

                if(cell.GetHasWestWall() && cellIdColumn < numberOfColumns - 1)
                {
                    Cell westCell = FindGeneratedCellByIdColIdRow(cellIdColumn + 1, cellIdRow);
                    if (westCell.GetCurrentCellMazeDirection() == CellMazeDirection.East) { 
                        cell.SetHasWestWall(false) ;
                    }
                }
            }
        }

        public Cell[,] GetMazeCells()
        {
            return cellMazeMatrix;
        }

        private void SetRandomlyCellMazeDirection(Cell cell)
        {
            List<CellMazeDirection> allowedCellMazeDirections = cell.GetAllowedMazeDirections();
            int randomIndex = UnityEngine.Random.Range(0, allowedCellMazeDirections.Count);
            CellMazeDirection randomDirection = allowedCellMazeDirections[randomIndex];
            cell.SetCurrentMazeDirection(randomDirection);
        }

        private void AddCellToMaze(int indexAtCellsNotInMaze)
        {
            cellsNotYetInMaze[indexAtCellsNotInMaze].SetInMaze();
            cellsNotYetInMaze.RemoveAt(indexAtCellsNotInMaze);
        }

        /****
         * return -1 if there is no cell inside the list with that idRow and idColumn
         */
        private int FindCellIndexNotInMazeByIdRowIdCol(int idRow, int idCol)
        {
            for (int i = 0; i < cellsNotYetInMaze.Count; i++)
            {
                if (cellsNotYetInMaze[i].GetIdRow() == idRow && cellsNotYetInMaze[i].GetIdColumn() == idCol)
                {
                    return i;
                }
            }

            return -1;
        }

        public Cell FindGeneratedCellByIdColIdRow(int idCol,  int idRow)
        {
            if(idCol >= cellMazeMatrix.GetLength(0))
            {
                throw new ArgumentOutOfRangeException("idCol", idCol, $"idCol out of range");
            }

            if(idRow >= cellMazeMatrix.GetLength(1))
            {
                throw new ArgumentOutOfRangeException("idRow", idRow, $"idRow out of range");

            }

            return cellMazeMatrix[idCol, idRow];  
        }

        private void PrintMaze()
        {
            string mazeCurrentRow = "";
            foreach (Cell cell in cellMazeMatrix)
            {
                // inicially all directions are allowed
                if(cell.GetCurrentCellMazeDirection() == CellMazeDirection.North)
                {
                    mazeCurrentRow += "[^]";
                }
                //TODO
            }
        }
    }


}

