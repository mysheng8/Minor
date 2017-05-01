using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class yMath 
{

    public static Vector2 PointToLocalSpace(Vector2 BGlobal, Vector2 AHeading, Vector2 ASide, Vector2 AGlobal)
    {
        Vector2 ToALocal = BGlobal - AGlobal;
        return new Vector2(ToALocal.x * AHeading.x + ToALocal.y * AHeading.y, -ToALocal.x * ASide.x - ToALocal.y * ASide.y);
    }

    public static Vector2 PointToWorldSpace(Vector2 Blocal, Vector2 AHeading, Vector2 ASide, Vector2 AGlobal)
    {
        Vector2 ToBGlobal = new Vector2(Blocal.x * AHeading.x - Blocal.y * AHeading.y, Blocal.x * ASide.x - Blocal.y * ASide.y);
        return new Vector2(ToBGlobal.x + AGlobal.x, ToBGlobal.y + AGlobal.y);
    }

    public static Vector2 VectorToWorldSpace(Vector2 localV, Vector2 Heading, Vector2 Side)
    {
        return new Vector2(localV.x * Heading.x - localV.y * Heading.y, localV.x * Side.x - localV.y * Side.y);
    }


    public static bool LineIntersection2D(Vector2 a, Vector2 b, Vector2 c, Vector2 d, out float outDistToIp, out Vector2 outIp)
    {
        outIp = Vector2.zero;
        outDistToIp = -1;
        // 三角形abc 面积的2倍  
        float area_abc = (a.x - c.x) * (b.y - c.y) - (a.y - c.y) * (b.x - c.x);  
  
        // 三角形abd 面积的2倍  
        float area_abd = (a.x - d.x) * (b.y - d.y) - (a.y - d.y) * (b.x - d.x);   
  
        // 面积符号相同则两点在线段同侧,不相交 (对点在线段上的情况,本例当作不相交处理);  
        if ( area_abc*area_abd>=0 ) {  
            return false;  
        }  
  
        // 三角形cda 面积的2倍  
        float area_cda = (c.x - a.x) * (d.y - a.y) - (c.y - a.y) * (d.x - a.x);  
        // 三角形cdb 面积的2倍  
        // 注意: 这里有一个小优化.不需要再用公式计算面积,而是通过已知的三个面积加减得出.  
        float area_cdb = area_cda + area_abc - area_abd ;  
        if (  area_cda * area_cdb >= 0 ) {  
            return false;  
        }  
  
        //计算交点坐标  
        float t = area_cda / ( area_abd- area_abc );  
        float dx= t*(b.x - a.x),  
              dy= t*(b.y - a.y);

        outIp.x = a.x + dx;
        outIp.x = a.y + dy;
        outDistToIp = (outIp - a).magnitude;
        return  true;  
    }
    public static float DistPointToLine2D(Vector2 point, Vector2 lineFrom, Vector2 lineTo)
    {
        float a,b,c;
        a = Vector2.Distance(point, lineFrom);
        if (a < 0.0001)
            return -1.0f;
        b = Vector2.Distance(point, lineTo);
        if (b < 0.0001)
            return -1.0f;
        c = Vector2.Distance(lineFrom, lineTo);
        if (c < 0.0001)
            return -1.0f;
        if (a * a >= b * b + c * c) 
            return -1.0f;
        if (b * b >= a * a + c * c)   
            return -1.0f;
        float l = (a + b + c) / 2;     //周长的一半   
        float s = Mathf.Sqrt(l * (l - a) * (l - b) * (l - c));  //海伦公式求面积   
        return 2 * s / c;  

    }


    public static Vector2 RotationVector2(Vector2 Dir, float angle)
    { 

        return new Vector2(Dir.x*Mathf.Cos(angle)-Dir.y*Mathf.Sin(angle), Dir.x*Mathf.Sin(angle)+Dir.y*Mathf.Cos(angle));
    
    }

    public static int[] getRandoms(int sum, int min, int max)
    {
        int[] arr = new int[sum];
        int j = 0;
        //表示键和值对的集合。
        Hashtable hashtable = new Hashtable();
        System.Random rm = new System.Random();
        while (hashtable.Count < sum)
        {
            //返回一个min到max之间的随机数
            int nValue = rm.Next(min, max);
            // 是否包含特定值
            if (!hashtable.ContainsValue(nValue))
            {
                //把键和值添加到hashtable
                hashtable.Add(nValue, nValue);
                arr[j] = nValue;

                j++;
            }
        }
        return arr;
    }
}
