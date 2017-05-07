using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerRegion 
{
    public virtual bool isTouching(Vector2 EntityPos, float EntityRadius) { return false; }

}

public class TriggerRegion_Circle : TriggerRegion
{
    Vector2 m_Pos;
    float m_Radius;
    public TriggerRegion_Circle(Vector2 pos, float radius)
    {
        m_Pos=pos;
        m_Radius=radius;
    }
    public override bool isTouching(Vector2 pos, float EntityRadius)
    {
        return (m_Pos - pos).sqrMagnitude < (EntityRadius + m_Radius) * (EntityRadius + m_Radius);
    }
}

public class TriggerRegion_Rect : TriggerRegion
{
    Rect m_Rect;
    public TriggerRegion_Rect(float x, float y, float width, float height)
    {
        m_Rect = new Rect(x, y, width, height);
    }
    public override bool isTouching(Vector2 pos, float EntityRadius)
    {
        return yMath.RectHitTest(m_Rect,0, pos, EntityRadius);
    }
}
