using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] float m_cellSize = 1.0f;
    [SerializeField] Vector3 m_gridOffset = Vector3.zero;

    Dictionary<GridCellKey, GridCellObject> m_blockedCells = null;

    public float cellSize { get { return m_cellSize; } }

    private void Awake()
    {
        m_blockedCells = new Dictionary<GridCellKey, GridCellObject>();
    }

    public GridCellKey GetCellKey(Vector3 position)
    {
        Vector3 intPosition = position - m_gridOffset;

        intPosition /= m_cellSize;
        intPosition.x = FloorPositionVariable(intPosition.x);
        intPosition.y = FloorPositionVariable(intPosition.y);
        intPosition.z = FloorPositionVariable(intPosition.z);


        return new GridCellKey(intPosition);
    }

    float FloorPositionVariable(float posVar)
    {
        if (posVar < 0)
        {
            posVar -= 1;
        }
        return (int)posVar;
    }

    public Vector3 GetCellPosition(GridCellKey gridCellKey)
    {
        Vector3 position = gridCellKey.ToVector3();
        position += (Vector3.one * 0.5f);
        position *= m_cellSize;
        return position + m_gridOffset;
    }

    public Vector3 GetCellPosition(Vector3 worldPosition)
    {
        return GetCellPosition(GetCellKey(worldPosition));
    }

    // Gets a cell position at the bottom of the cell.
    public Vector3 GetCellFloorPosition(GridCellKey gridCellKey)
    {
        return GameManager.instance.gridManager.GetCellPosition(gridCellKey) + Vector3.down * (m_cellSize * 0.5f);
    }

    public bool TryGetCellObject(GridCellKey gridCellKey, out GridCellObject result)
    {
        return m_blockedCells.TryGetValue(gridCellKey, out result);
    }

    public void AddGridCellObject(GridCellKey gridCellKey, GridCellObject toAdd)
    {
        m_blockedCells.Add(gridCellKey, toAdd);
    }

    public void AddGridCellObject(GridCellKey gridCellKey, AntBoid ant)
    {
        GridCellObject toAdd = new GridCellObject(GetCellPosition(gridCellKey), ant);
        m_blockedCells.Add(gridCellKey, toAdd);
    }

    public void AddGridCellObject(Vector3 position, AntBoid ant, out Vector3 gridCellPos)
    {
        GridCellObject toAdd = new GridCellObject(position, ant);
        GridCellKey cellKey = GetCellKey(position);
        AddGridCellObject(cellKey, toAdd);
        gridCellPos = GetCellPosition(cellKey);
    }

    public void RemoveGridCellObject(GridCellKey gridCellKey)
    {
        m_blockedCells.Remove(gridCellKey);
    }

    public class GridCellObject
    {
        Vector3 position;
        AntBoid ant;

        public GridCellObject(Vector3 position, AntBoid ant)
        {
            this.position = position;
            this.ant = ant;
        }
    }

    public struct GridCellKey
    {
        int m_gridX;
        int m_gridY;
        int m_gridZ;

        public int x { get { return m_gridX; } }
        public int y { get { return m_gridY; } }
        public int z { get { return m_gridZ; } }

        public GridCellKey(int x, int y, int z)
        {
            m_gridX = x;
            m_gridY = y;
            m_gridZ = z;
        }

        public GridCellKey(Vector3 cellKey)
        {
            m_gridX = (int)cellKey.x;
            m_gridY = (int)cellKey.y;
            m_gridZ = (int)cellKey.z;
        }

        public Vector3 ToVector3()
        {
            Vector3 result = Vector3.zero;
            result.x = x;
            result.y = y;
            result.z = z;
            return result;
        }
    }

    private void OnDrawGizmos()
    {

    }
}
