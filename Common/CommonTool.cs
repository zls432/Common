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
        int testValue = 1;  // ����ʹ��ֵΪ 1 �������ֽ�˳��
        byte[] bytes = BitConverter.GetBytes(testValue);

        // �����һ���ֽ�Ϊ 1��˵����С���ֽ���
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
