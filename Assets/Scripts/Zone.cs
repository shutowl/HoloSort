using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{

    [SerializeField] int stageType = 0;  //0 = gen, 1 = boing, kemomimi = 2
    [SerializeField] int genType = 0; //0 = myth, 1 = promise, 2 = advent
    [SerializeField] bool boing = false;
    [SerializeField] bool kemomimi = false;

    public void SetStage(int stageType)
    {
        this.stageType = stageType;
    }

    public void SetGen(int genType)
    {
        this.genType = genType;
    }

    public void SetBoing(bool boing)
    {
        this.boing = boing;
    }

    public void SetKemomimi(bool kemomimi)
    {
        this.kemomimi = kemomimi;
    }

    public int GetStageType()
    {
        return stageType;
    }

    public int GetGenType()
    {
        return genType;
    }

    public bool GetBoing()
    {
        return boing;
    }

    public bool GetKemomimi()
    {
        return kemomimi;
    }
}
