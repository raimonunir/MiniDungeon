using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MiniDungeon
{

    /*!*****************
     * @brief The cell is the most basic and essential part of the game world. The cell contains 
     * information about its 3D position in the world and what is in that piece of the world 
     * (are there walls, ceiling, floor?, is there a trap?, is there an enemy?, 
     * does it contain a treasure?, is it dark, cold?, etc.). A cell cannot write information 
     * for itself, and a cell can not build by itself, 
     * it is just a piece of a 3D map that contains information that some generator can write/read and some
     * builder can read/build.
     * 
     * NO size
     * YES proportion
     * Cell doesn't have a size (but it has a proportion 1x1x1), 
     * Según la necesidad de detalle que tenga que representar la cell el Generador le otorgará un size
     * mayor o menor si es necesario. Al tener una proporcion 1x1x1, es muy fácil para el Builder
     * construir haciendo Instanciate en una posicion teniendo en cuenta
     * el tamaño real que respresentará cada cell como gameObject 
     * (cell.idColumn*6, cell.idFloor*6, cell.idRow*6)
     * 
     * 
     * a generator can create a "map", a "dungeon" a "maze" where it can be specified the size in 
     * meters of cells. Or a builder can establish the size for its own
     * 
     * Cell is just information, how it is build is upon the builder
     * 
     * The cell is just information, the way it is built depends on the builder and how it interprets 
     * the information.
     * 
     * The Generator must calculate and write any information specific for a cell into the cell
     * 
     * The cell must not calculate in any way information
     * 
     * The generator should be able to specify as much information as possible that the builder will need 
     * ...to build. 
     * The builder should simply read the information and transfer it to Unity GameObjects. 
     * 
     * The generator and the cell are GAME ENGINE AGNOSTIC, the generated information should be able 
     * ...to be interpreted by different builders that build the game even for different engines 
     * ...(Unity, Unreal, Godot, etc.)
     * 
     * @author Raimon Rodríguez Allés
     * @todo improve documentation using Doxygen
     * ********/

    public class Cell
    {
        // 3D world position
        private int idColumn;
        private int idRow;
        private int idFloor;
        // maze info
        private bool inMaze = false;
        private CellMazeDirection currentDirection = CellMazeDirection.North;
        private List<CellMazeDirection> allowedMazeCellDirections = new List<CellMazeDirection>();
        // building
        private bool hasNorthWall = false;
        private bool hasSouthWall = false;
        private bool hasWestWall = false;
        private bool hasEastWall = false;
        private bool hasCelling = false;
        private bool hasFloor = false;

        // constructor
        public Cell(int idColumn, int idRow, int idFloor)
        {
            if(idRow < 0 || idColumn < 0 || idFloor < 0)
            {
                Debug.LogError($"colId, idRow and idFloor must be >=0. colId={idColumn}, idRow={idRow}, idFloor={idFloor}");
            }

            this.idColumn = idColumn;
            this.idRow = idRow;
            this.idFloor = idFloor;

            // inicially all directions are allowed
            foreach (var val in Enum.GetValues(typeof(CellMazeDirection)))
            {
                allowedMazeCellDirections.Add((CellMazeDirection)val);
            }
        }



        public bool GetHasNorthWall() { return hasNorthWall; }
        public bool GetHasSouthWall() { return hasSouthWall; }
        public bool GetHasEastWall() { return hasEastWall; }
        public bool GetHasWestWall() { return hasWestWall; }

        public void SetHasNorthWall(bool hasIt)
        {
            hasNorthWall = hasIt;
        }
        public void SetHasSouthWall(bool hasIt)
        {
            hasSouthWall = hasIt;
        }

        public void SetHasWestWall(bool hasIt)
        {
            hasWestWall = hasIt;
        }

        public void SetHasEastWall(bool hasIt)
        {
            hasEastWall = hasIt;
        }

        public void SetHasAllWalls(bool hasIt)
        {
            hasNorthWall = hasIt; 
            hasSouthWall = hasIt;
            hasEastWall = hasIt;
            hasWestWall = hasIt;
        }

        // override ==
        /*
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

            return (this.idRow == otherCell.idRow) && (this.idColumn == otherCell.idColumn);
        }
        */

        // override
        public override string ToString()
        {
            return ($"cell with idColumn={this.idColumn}, idRow={this.idRow} and idFloor={this.idFloor}");
        }
        public int GetIdColumn()
        {
            return idColumn;
        }

        public int GetIdRow()
        {
            return idRow;
        }

        public int GetIdFloor()
        {
            return idFloor;
        }


        public bool IsInMaze() { 
            return inMaze; 
        }

        public void SetInMaze()
        {
            inMaze = true;
        }

        public CellMazeDirection GetCurrentCellMazeDirection()
        {
            return currentDirection;
        }

        public void SetNotAllowedMazeDirection(CellMazeDirection direction) {
            if (!IsMazeDirectionAllowed(direction))
            {
                Debug.LogError($" direction=>{direction} was already a not allawed direction for the cell=>{this}");
            }
            allowedMazeCellDirections.Remove(direction);
        }

        public bool IsMazeDirectionAllowed(CellMazeDirection direction) { 
            return allowedMazeCellDirections.Contains(direction);
        }

        public List<CellMazeDirection> GetAllowedMazeDirections() { 
            return allowedMazeCellDirections;
        }

        public void SetCurrentMazeDirection(CellMazeDirection direction)
        {
            if (!IsMazeDirectionAllowed(direction))
            {
                Debug.LogError($"Direction NOT ALLOWED direction=>{direction}");
            }
            this.currentDirection = direction;
        }

        public int GetNextRowIdByCurrentMazeDirection() {
            if (currentDirection == CellMazeDirection.North)
            {
                return idRow-1;
            }
            else if (currentDirection == CellMazeDirection.South)
            {
                return idRow+1;
            }
            else
            {
                return idRow;
            }
        }

        public int GetNextIdColByCurrentMazeDirection() {
            if (currentDirection == CellMazeDirection.West)
            {
                return idColumn-1;
            }
            else if (currentDirection == CellMazeDirection.East)
            {
                return idColumn+1;
            }
            else
            {
                return idColumn;
            }
        }
    }

} 