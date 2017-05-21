using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Cell
{
    public List<BaseEntity> Members;
    Rect BBox;
    public Cell(float xmin, float ymin, float xmax, float ymax)
    {
        BBox = Rect.MinMaxRect(xmin, ymin, xmax, ymax);
        Members = new List<BaseEntity>();
    }
}

public class CellSpacePartition  
{
    
    List<Cell> m_Cells;
    List<BaseEntity> m_Neighbors;

    float m_SpaceWidth;
    float m_SpaceHeight;
    float m_startX;
    float m_startY;
    float m_SpaceOffsetX;
    float m_SpaceOffsetY;

    int m_NumCellsX;
    int m_NumCellsY;

    float m_CellSizeX;
    float m_CellSizeY;

    int PositionToIndex(Vector2 pos)
    {
        int x = (int)((pos.x - m_SpaceOffsetX - m_startX) / m_CellSizeX);
        int y = (int)((pos.y - m_SpaceOffsetY - m_startY) / m_CellSizeY);
        return(x + y * m_NumCellsX);
    }

    public void Init(float width, float height, int cellsX, int cellsY, float startX, float startY, float offsetX, float offsetY)
    {
        m_SpaceWidth = width;
        m_SpaceHeight = height;
        m_NumCellsX = cellsX;
        m_NumCellsY = cellsY;
        m_startX = startX;
        m_startY = startY;
        m_SpaceOffsetX = offsetX;
        m_SpaceOffsetY = offsetY;

        m_CellSizeX = width / cellsX;
        m_CellSizeY = height / cellsY;

        m_Cells = new List<Cell>();
        m_Neighbors = new List<BaseEntity>();

        float sx = m_startX + m_SpaceOffsetX;
        float sy = m_startY + m_SpaceOffsetY;

        for (int y = 0; y < m_NumCellsY; ++y)
        {
            for (int x = 0; x < m_NumCellsX; ++x)
            {
                Cell c = new Cell(sx, sy, (sx + m_CellSizeX), (sy + m_CellSizeY));
                m_Cells.Add(c);
                sx += m_CellSizeX;
            }
            sy += m_CellSizeY;
        }
    }

    public void AddEntity(BaseEntity ent)
    {
        int i = PositionToIndex(ent.Pos);
        m_Cells[i].Members.Add(ent);
    }

    public void UpdateEntity(BaseEntity ent, Vector2 lastPos)
    {
        int LastIndex = PositionToIndex(lastPos);
        int Index = PositionToIndex(ent.Pos);

        if (Index >= m_Cells.Count || Index < 0)
            Debug.LogError(ent.GetInstanceID() + " has out of range of the cell space");


        if ( LastIndex > m_Cells.Count)
        {
            Debug.LogAssertion("wrong last position: " + lastPos);
            Debug.LogAssertion("wrong last index: " + LastIndex);
        }

        if (LastIndex != Index)
        {
            if (LastIndex>0)
                m_Cells[LastIndex].Members.Remove(ent);
            m_Cells[Index].Members.Add(ent);
        }
    }

    public void CalculateNeighbors(Vector2 TargetPos)
    {
        m_Neighbors.Clear();

        int i = PositionToIndex(TargetPos);
        int x = i % m_NumCellsX;
        int y = i / m_NumCellsX;

        int minX = Mathf.Max(x - 1, 0);
        int minY = Mathf.Max(y - 1, 0);
        int maxX = Mathf.Min(x + 1, m_NumCellsX - 1);
        int maxY = Mathf.Min(y + 1, m_NumCellsY - 1);



        for (int a = minY; a < maxY + 1; ++a)
        {
            for (int b = minX; b < maxX + 1; ++b)
            {
                int j = b + a * m_NumCellsX;
                foreach (BaseEntity ent in m_Cells[j].Members)
                {
                    m_Neighbors.Add(ent);
                }
            }
        }
    }

    public List<BaseEntity> Neighbors()
    {
        return m_Neighbors;
    }

    void EmptyCells()
    {
        foreach (Cell c in m_Cells)
        {
            c.Members.Clear();
        }
    }

    public void RemoveEntity(BaseEntity ent, Vector2 lastPos)
    {
        int LastIndex = PositionToIndex(lastPos);
        m_Cells[LastIndex].Members.Remove(ent);
        /*
        LinkedListNode<BaseEntity> node = m_Cells[LastIndex].Members.Find(ent);
        if (node != null)
        {
            m_Cells[LastIndex].Members.Remove(node);
        }*/
    }
}
