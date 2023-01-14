using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStay
{
    //返回一个float值，用于当做气力值的持续消耗的系数，系数与摩擦力相关
    public float Stay();
}
