using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBorrow
{
    public void RechargeQi(Vector3 velocity);
    public BorrowableType GetBorrowableType();
}
