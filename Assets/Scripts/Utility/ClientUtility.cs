using MyLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientUtility
{
    public static MyVector3 ConvertToMyvector3(Vector3 oldVector)
    {
        MyVector3 myVector3 = new MyVector3(oldVector.x, oldVector.y, oldVector.z);
        return myVector3;
    }

}
