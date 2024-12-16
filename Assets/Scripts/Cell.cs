using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniDungeon
{

    public enum Direction { Left, Right, Up, Down };

    public class Cell
    {
        private int idRow;
        private int idCol;
        private bool inMaze = false;
        private Direction currentDirection = Direction.Up;
        private List<Direction> allowedDirections = new List<Direction>();
        /*
        private Cell nextCellLeft;
        private Cell nextCellRight;
        private Cell nextCellUp;
        private Cell nextCellDown;
        */

        // constructor
        public Cell(int rowId, int colId)
        {
            this.idRow = rowId;
            this.idCol = colId;
            this.inMaze = false;

            // inicially all directions are allowed
            foreach (var val in Enum.GetValues(typeof(Direction)))
            {
                allowedDirections.Add((Direction)val);
            }
        }

        // override ==
        public override bool Equals(object obj)
        {
            if (!(obj is Cell))
            {
                return false;
            }

            var otherCell = obj as Cell;

            if (otherCell == null)
            {
                return false;
            }

            return (this.idRow == otherCell.idRow) && (this.idCol == otherCell.idCol);
        }

        // override
        public override string ToString()
        {
            return ($"cell with idRow={this.idRow} and idCol={this.idCol}");
        }

        public int GetIdRow()
        {
            return idRow;
        }

        public int GetIdCol()
        {
            return idCol;
        }

        public bool IsInMaze() { 
            return inMaze; 
        }

        public void SetInMaze()
        {
            inMaze = true;
        }

        public Direction GetCurrentDirection()
        {
            return currentDirection;
        }

        public void SetNotAllowedDirection(Direction direction) {
            if (!isDirectionAllowed(direction))
            {
                Debug.LogError($" direction=>{direction} was already a not allawed direction for the cell=>{this}");
            }
            allowedDirections.Remove(direction);
        }

        public bool isDirectionAllowed(Direction direction) { 
            return allowedDirections.Contains(direction);
        }

        public void SetCurrentDirectionRandomly() {         
            if(allowedDirections.Count == 0)
            {
                Debug.LogError($"There are NOT allowed directions in cell{this}");
            }
            currentDirection = allowedDirections[UnityEngine.Random.Range(0, allowedDirections.Count)]; 
        }

        public int GetNextRowIdByCurrentDirection() {
            if (currentDirection == Direction.Up)
            {
                return idRow--;
            }
            else if (currentDirection == Direction.Down)
            {
                return idRow++;
            }
            else
            {
                return idRow;
            }
        }

        public int GetNextIdColByCurrentDirection() {
            if (currentDirection == Direction.Left)
            {
                return idCol--;
            }
            else if (currentDirection == Direction.Right)
            {
                return idCol++;
            }
            else
            {
                return idCol;
            }
        }

        /*
        public Cell GetNextCellUp()
        {
            return nextCellUp;
        }
        public Cell GetNextCellDown() 
        { 
            return nextCellDown;
        }
        public Cell GetNextCellLeft() { return nextCellLeft; }
        public Cell GetNextCellRight() { return nextCellRight; }

        public void SetNextCellUp(Cell cell)
        {
            if (cell == null)
            {
                Debug.LogError($"Imposible to set nextCellUp in this cell=>{this}, because cell is null");
            }
                nextCellUp = cell;
        }
        public void SetNextCellDown(Cell cell)
        {
            if (cell == null)
            {
                Debug.LogError($"Imposible to set nextCellDown in this cell=>{this}, because cell is null");
            }
            nextCellDown = cell;
        }
        public void SetNextCellLeft(Cell cell)
        {
            if (cell == null)
            {
                Debug.LogError($"Imposible to set nextCellLeft in this cell=>{this}, because cell is null");
            }
            nextCellLeft = cell;   
        }
        public void SetNextCellRight(Cell cell)
        {
            if (cell == null)
            {
                Debug.LogError($"Imposible to set nextCellRight in this cell=>{this}, because cell is null");
            }
            nextCellRight = cell;
        }
        */
    }

} 