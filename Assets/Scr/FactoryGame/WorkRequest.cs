using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��û�� Ŭ����
public class WorkRequest
{
    public ProductType productType;
    public int quantity;
    public int reward;

    public WorkRequest(ProductType productType, int quantitiy, int reward)          //������
    {
        this.productType = productType;
        this.quantity = quantitiy;
        this.reward = reward;
    }
}
