
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Assertions;

namespace MiniDungeon
{

    /*!****
    * @brief CellMazeDirection contains all the posible maze directions
    * **/
    public enum CellMazeDirection { West, East, North, South };

    /*!****
    * @brief Directions3D contains all the posible 3D directions
    * **/
    public enum Direcitons3D { North, East, South, West, Up, Down };

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

            // print maze to ASCII
            PrintMazeASCIItoFile(Environment.SpecialFolder.Desktop, "maze");
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
                    Cell newCell = new Cell(new IdCell(col, row, floor));
                    newCell.AllowAllMazeDirections();

                    if (row == 0)
                    {
                        newCell.DisallowMazeDirection(CellMazeDirection.North);
                    }
                    else if (row == numberOfRows - 1)
                    {
                        newCell.DisallowMazeDirection(CellMazeDirection.South);
                    }

                    if (col == 0)
                    {
                        newCell.DisallowMazeDirection(CellMazeDirection.West);
                    }
                    else if (col == numberOfColumns - 1)
                    {
                        newCell.DisallowMazeDirection(CellMazeDirection.East);
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

            // pick randomly a cell and add it to the maze (remove from cellsNotYetInMaze
            Debug.Log("First Cell Added to Maze");
            AddCellToMaze(GetARandomCellFromList(cellsNotYetInMaze));

            while(cellsNotYetInMaze.Count > 0)
            {
                // pick randomly a not in maze cell
                Cell firstRandomCell = GetARandomCellFromList(cellsNotYetInMaze);
                Cell pointerToCurrentCell = firstRandomCell;

                while (true) {

                    // set randomly a direction
                    SetRandomlyCellMazeDirection(pointerToCurrentCell);

                    // walk to the next cell
                    IdCell nextIdCell = GetNextIdCellByCurrentMazeDirecction(pointerToCurrentCell);

                    Cell nextCell = FindCellInMatrixByIdCell(nextIdCell);

                    Debug.Log($"nextCell=>{nextCell} nextCell.IsInMaze()=>{nextCell.IsInMaze()}");
                    
                    if (nextCell.IsInMaze())
                    {
                        Debug.Log("Break!");
                        break;// if the nextCell is in Maze we are done!
                    }
                    Debug.Log("NOOO Break!");
                    // if not walk another step, current cell now is nextCell
                    pointerToCurrentCell = nextCell;
                }

                // Go back to first cell
                pointerToCurrentCell = firstRandomCell;
                while (true)
                {
                    //Cell pointerToCurrentCell = cellsNotYetInMaze[currentCellIndexNotInMaze];

                    // first get the next cell
                    IdCell nextIdCell = GetNextIdCellByCurrentMazeDirecction(pointerToCurrentCell);
                    Cell nextCell = FindCellInMatrixByIdCell(nextIdCell);

                    // add the current cell to the maze
                    AddCellToMaze(pointerToCurrentCell);

                    if (nextCell.IsInMaze())
                    {
                        // if the nextCell is in Maze we are done...
                        break;
                    }

                    //...if not, walk to the next cell
                    pointerToCurrentCell = nextCell;
                }
                
            }

            // mark maze has been calculated
            mazeHasBeenCalculated = true;
        }

        private IdCell GetNextIdCellByCurrentMazeDirecction(Cell currentCell)
        {
            Debug.Log($"GetNextIdCellByCurrentMazeDireciton with currentCell{currentCell} and currentCell.GetCurrentMazeDirection()=>{currentCell.GetCurrentCellMazeDirection()}");

            IdCell nextIdCell;
            int idColumn, idRow;
            if (currentCell.GetCurrentCellMazeDirection() == CellMazeDirection.North)
            {
                idRow = currentCell.GetIdCell().idRow - 1;
            }
            else if (currentCell.GetCurrentCellMazeDirection() == CellMazeDirection.South)
            {
                idRow = currentCell.GetIdCell().idRow + 1;
            }
            else
            {
                idRow = currentCell.GetIdCell().idRow;
            }

            if (currentCell.GetCurrentCellMazeDirection() == CellMazeDirection.East)
            {
                idColumn = currentCell.GetIdCell().idColumn + 1;
            }
            else if (currentCell.GetCurrentCellMazeDirection() == CellMazeDirection.West)
            {
                idColumn = currentCell.GetIdCell().idColumn - 1;
            }
            else
            {
                idColumn = currentCell.GetIdCell().idColumn;
            }

            return nextIdCell = new IdCell(idColumn, idRow, currentCell.GetIdCell().idFloor);
        }

        private Cell GetARandomCellFromList(List<Cell> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
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
                IdCell currentCellId = cell.GetIdCell();

                // checks north wall if it is not a top north row cell and has northwall
                if (cell.GetHasNorthWall() && currentCellId.idRow > 0)
                {
                    //  if his northCell is poiting to this cell destroy north wall
                    Cell northCell = FindCellInMatrixByIdCell(currentCellId.CalculateTheNextIdCellTo(Direcitons3D.North)); //FindGeneratedCellByIdColIdRow(cellIdColumn, cellIdRow - 1);
                    if (northCell.GetCurrentCellMazeDirection() == CellMazeDirection.South)
                    {
                        cell.SetHasNorthWall(false) ;
                    }
                }

                // if  it is not a top south row cell and has sourthwall
                if (cell.GetHasSouthWall() && currentCellId.idRow < numberOfRows - 1) { 
                    
                    // if the cell has a southcell pointing to north destroy southwall
                    Cell southCell = FindCellInMatrixByIdCell(currentCellId.CalculateTheNextIdCellTo(Direcitons3D.South));
                    if (southCell.GetCurrentCellMazeDirection() == CellMazeDirection.North)
                    {
                        cell.SetHasSouthWall(false) ;
                    }
                }

                if (cell.GetHasWestWall() && currentCellId.idColumn > 0) { 
                    Cell eastCell = FindCellInMatrixByIdCell(currentCellId.CalculateTheNextIdCellTo(Direcitons3D.West));
                    if (eastCell.GetCurrentCellMazeDirection() == CellMazeDirection.East) { 
                        cell.SetHasWestWall(false) ;
                    }
                }

                if(cell.GetHasEastWall() && currentCellId.idColumn < numberOfColumns - 1)
                {
                    Cell westCell = FindCellInMatrixByIdCell(currentCellId.CalculateTheNextIdCellTo(Direcitons3D.East));
                    if (westCell.GetCurrentCellMazeDirection() == CellMazeDirection.West) { 
                        cell.SetHasEastWall(false) ;
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

        private void AddCellToMaze(Cell cell)
        {
            //Cell cell = cellsNotYetInMaze[indexAtCellsNotInMaze];

            //cellsNotYetInMaze[indexAtCellsNotInMaze].SetInMaze();
            /*
            Debug.Log($"cell.IsInMaze()=>" +
                $"{cell.IsInMaze()}" +
                $" cell.GetIdColumn()=>{cell.GetIdColumn()}" +
                $" cell.GetIdRow()=>{cell.GetIdRow()}");

            Debug.Log($"cellsNotYetInMaze.Contains(cell)=>{cellsNotYetInMaze.Contains(cell)}");
            */
            cell.SetInMaze();
            cellsNotYetInMaze.Remove(cell);

           // Debug.Log($"cellsNotYetInMaze.Contains(cell)=>{cellsNotYetInMaze.Contains(cell)}");
        }

        /****
         * return -1 if there is no cell inside the list with that idRow and idColumn
         */
        private Cell FindCellNotInMazeByIdCell(IdCell idCell)
        {
            Cell cellToCompare = new Cell(idCell);
            //Debug.Log($"Searching idRow=>{idRow}, idCol=>{idCol} in cellsNotYetInMaze.Count=>{cellsNotYetInMaze.Count}");
            for (int i = 0; i < cellsNotYetInMaze.Count; i++)
            {
                //Debug.Log($"row=>{row} cellsNotYetInMaze[row].GetIdRow()=>{cellsNotYetInMaze[row].GetIdRow()} cellsNotYetInMaze[row].GetIdColumn()=>{cellsNotYetInMaze[row].GetIdColumn()}");
                if (cellsNotYetInMaze[i] == cellToCompare)
                {
                    return cellsNotYetInMaze[i];
                }
            }
            throw new KeyNotFoundException($"idCel={idCell} in cellsNotYetInMaze{cellsNotYetInMaze}");
        }

        public Cell FindCellInMatrixByIdCell(IdCell idCell)
        {
            if(idCell.idColumn >= cellMazeMatrix.GetLength(0))
            {
                throw new ArgumentOutOfRangeException("idCell.idColumn", idCell.idColumn, $"idCell.idColumn=>{idCell.idColumn} out of range");
            }

            if(idCell.idRow >= cellMazeMatrix.GetLength(1))
            {
                throw new ArgumentOutOfRangeException("idCell.idRow", idCell.idRow, $"idCell.idRow out of range");

            }

            return cellMazeMatrix[idCell.idColumn, idCell.idRow];  
        }

        private void PrintMazeASCIItoFile(Environment.SpecialFolder specialFolder, string fileName)
        {
            #if UNITY_EDITOR // just in unity_editor

            const int linesByRow = 3;
            const int charsByColumn = 5;
            const string wallNorthOrSouth = "#####";
            const string noNorthSouthWallHorizontal = "# · #";
            const string north = "^";
            const string east = ">";
            const string west = "<";
            const string south = "v";
            const string wallWest = "# ";
            const string noWallWest = "· ";
            const string noWallEast = " ·";
            const string wallEast = " #";


            // 3 string lines by row, 5 chars by col
            // #=wall
            // .=NOwall
            // v=directionSouth
            // ^=Noth
            /****
             * 
             *  
             *  ##########
             *  # v ·· < #
             *  # · ## · #
             *  # · ## · #
             *  # v ## ^ #
             *  ##########
             */
            string[] lines = new string[linesByRow * numberOfRows];
            string topRowLine = "";
            string midRowLine = "";
            string bottomRowLine = "";
            for (int row = 0; row < numberOfRows; row++)
            {
                for (int col = 0; col < numberOfColumns; col++)
                {
                    Cell cellToPrint = cellMazeMatrix[col, row];
                    CellMazeDirection direction = cellToPrint.GetCurrentCellMazeDirection();

                    Debug.Log($"cellToPrint=>{cellToPrint}, direction=>{direction}, " +
                        $"wallNorth=>{cellToPrint.GetHasNorthWall()}, wallEast=>{cellToPrint.GetHasEastWall()}, " +
                        $"wallWest=>{cellToPrint.GetHasWestWall()}, wallSouth=>{cellToPrint.GetHasSouthWall()}");

                    for (int charRow = 0; charRow < linesByRow; charRow++) 
                    {
                        if (charRow == 0)
                        {
                            if (cellToPrint.GetHasNorthWall())
                            {
                                topRowLine += wallNorthOrSouth;
                            }
                            else
                            {
                                topRowLine += noNorthSouthWallHorizontal;
                            }
                        }
                        else if (charRow == 1) {

                            for (int charColumn = 0; charColumn < charsByColumn - 1; charColumn++) //skip the last char
                            {
                                if (charColumn == 0)
                                {
                                    if (cellToPrint.GetHasWestWall())
                                    {
                                        midRowLine += wallWest;
                                    }
                                    else
                                    {
                                        midRowLine += noWallWest;
                                    }
                                }
                                // skip 1
                                else if (charColumn == 2)
                                {
                                    midRowLine += DirectionMazeToString(direction);
                                }
                                else if (charColumn == 3)
                                {
                                    if (cellToPrint.GetHasEastWall())
                                    {
                                        midRowLine += wallEast;
                                    }
                                    else
                                    {
                                        midRowLine += noWallEast;
                                    }
                                }
                            }

                        }
                        else
                        {
                            if (cellToPrint.GetHasSouthWall())
                            {
                                bottomRowLine += wallNorthOrSouth;
                            }
                            else
                            {
                                bottomRowLine += noNorthSouthWallHorizontal;
                            }
                        }

                    }
                }

                // save row
                lines[row * linesByRow] = topRowLine; //0, 3, 
                lines[row * linesByRow + 1] = midRowLine;//1, 4
                lines[row * linesByRow + 2] = bottomRowLine;//2, 5

                // clear row strings for next row
                topRowLine = "";
                midRowLine = "";
                bottomRowLine = "";

            }// end of row


            // Set a variable to the Documents path.
            string docPath =
              Environment.GetFolderPath(specialFolder);

            // Write the string array to a new file named "WriteLines.txt".
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, $"{fileName}.txt")))
            {
                outputFile.WriteLine($"=== Maze printed at {DateTime.Now} ===\n");

                foreach (string line in lines)
                {
                    outputFile.WriteLine(line);
                }
                
                string blankLine = "-----------------";

                outputFile.WriteLine("\n\n=== DATA CELLS ===\nC: column\nR: row\nD: direction\n" +
                    "N: hasNorthWall\nE: hasEastWall\nW: hasWestWall\nS: hasSouthWall" +
                    "\n" + blankLine +
                    "\n|C|R| D |N|E|W|S|");

                for (int row = 0; row < numberOfRows; row++)
                {
                    for (int col = 0; col < numberOfColumns; col++)
                    {
                        Cell cell = cellMazeMatrix[col, row];
                        outputFile.WriteLine(blankLine);
                        outputFile.WriteLine($"|{cell.GetIdCell().idColumn}|{cell.GetIdCell().idRow}|" +
                            $" {DirectionMazeToString(cell.GetCurrentCellMazeDirection())} |" +
                            $"{BoolToString(cell.GetHasNorthWall())}|" +
                            $"{BoolToString(cell.GetHasEastWall())}|" +
                            $"{BoolToString(cell.GetHasWestWall())}|" +
                            $"{BoolToString(cell.GetHasSouthWall())}|");
                    }
                }

                outputFile.WriteLine(blankLine);

            }

            

            Debug.LogWarning("An ASCII version of the maze have been printed to " + Path.Combine(docPath, $"{fileName}.txt"));
            
            #endif
        }

        private string BoolToString(bool value)
        {
            if (value)
            {
                return "X";
            }
            else
            {
                return " ";
            }
        }

        private string DirectionMazeToString(CellMazeDirection direction)
        {
            if (direction == CellMazeDirection.North)
            {
                return "^";
            }
            else if (direction == CellMazeDirection.South)
            {
                return "v";
            }
            else if (direction == CellMazeDirection.West)
            {
                return "<";
            }
            else if (direction == CellMazeDirection.East)
            {
                return ">";
            }
            return "XXXX";
        }
    }

}

