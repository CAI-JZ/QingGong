using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBorrow
{
    public void RechargeQi();
    public BorrowableType GetBorrowableType();
}
