using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniDungeon;

public class CellGameObject : MonoBehaviour
{
    [SerializeField] private Vector3 Size;
    [SerializeField] private GameObject northWall;
    [SerializeField] private GameObject southWall;
    [SerializeField] private GameObject eastWall;
    [SerializeField] private GameObject westWall;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyNorthWall()
    {
        Destroy(northWall);
    }
    public void DestroySouthWall()
    {
        Destroy(southWall);
    }
    public void DestroyWestWall()
    {
        Destroy(westWall);
    }
    public void DestroyEastWall() { Destroy(eastWall); }

    public Vector3 GetSize() { return Size; }
}