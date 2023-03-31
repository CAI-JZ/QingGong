using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBorrow
{
    public bool BorrowPower();
    public void Swing();
    public void Deform();
    public void StopDeform();
    public BorrowableType GetBorrowableType();
}
