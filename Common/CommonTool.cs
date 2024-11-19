using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CommonTool
{

    /// <summary>
    /// Will return value multiplied by height
    /// </summary>
    /// <param name="height"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    public static float GetRatioBaseWidth( float height,float width )
    {
        return height / width;
    }

    /// <summary>
    /// Will return value multiplied by width
    /// </summary>
    /// <param name="height"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    public static float GetRatioBaseHight(float height, float width)
    {
        return width / height;
    }


    public static bool IsLittleEndian()
    {
        int testValue = 1;  // 我们使用值为 1 来测试字节顺序
        byte[] bytes = BitConverter.GetBytes(testValue);

        // 如果第一个字节为 1，说明是小端字节序
        if (bytes[0] == 1)
        {
           return true;
        }
        else
        {
            return false;
        }
    }
}
