using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DrawLine 
{

    List<Vector2> m_HitList;
    List<Vector2> m_NormalList;
    int m_LineLength = Config.LengthToSampleHit;
    int m_Size;
    Vector3 lastHit;

    int count = 0;
    int timer = Config.TimeToSampleHit;
    LineRenderer lr;

    public int Size
    {
        get 
        {
            return m_Size;
        }
        set
        {
            m_Size = value;
        }
    }

    public List<Vector2> HitList
    {
        get
        {
            return m_HitList;
        }
    }

    public List<Vector2> NormalList
    {
        get
        {
            return m_NormalList;
        }
    }

    public void DrawBegin()
    {
        m_Size = 20;
        count = 0;
        m_HitList.Clear();
        m_NormalList.Clear();
        timer = Config.TimeToSampleHit;
        lastHit = Vector3.zero;
    }

    public void DrawEnd()
    {
        lr.positionCount = 0;
    }

    public void Draw()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.tag == "Ground")
            {
                if (count < m_Size)
                {
                    Vector3 posVec = hit.point;
                    if (lastHit == Vector3.zero)
                    {
                        lastHit = posVec;
                    }

                    if ((lastHit - posVec).magnitude >= Config.LengthToSampleHit)
                    {
                        Vector3 tar = Vector3.ClampMagnitude(posVec - lastHit, Config.LengthToSampleHit) + lastHit;
                        Vector3 normal = Vector3.Cross(posVec - lastHit, Vector3.up).normalized;

                        m_HitList.Add(new Vector2(tar.x, tar.z));
                        m_NormalList.Add(new Vector2(normal.x, normal.z));

                        lastHit = tar;
                        ++count;
                        DisplayDraw();
                    }
                }
            }
        }
    }

    void DisplayDraw()
    {
        int count = m_HitList.Count;
        lr.positionCount = count;
        lr.SetPosition(count-1, m_HitList[count-1]);
    }

    public DrawLine()
    {
        lastHit = Vector3.zero;
        m_HitList = new List<Vector2>();
        m_NormalList = new List<Vector2>();
        GameObject m_Line = (GameObject)Resources.Load("LineRenderer");
        GameObject l = (GameObject)GameObject.Instantiate(m_Line);
        lr = l.GetComponent<LineRenderer>();
        lr.startColor = Color.blue;
        lr.endColor = Color.red;
        lr.startWidth = 10;
        lr.endWidth = 6;
    }

}
