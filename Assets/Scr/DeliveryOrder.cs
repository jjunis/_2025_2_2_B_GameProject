using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


//간단한 배달 주문
[System.Serializable]
public class DeliveryOrder
{
    public int orderld;
    public string restauranName;
    public string customerName;
    public Building restauranBuilding;
    public Building customerBuilding;
    public float orderTime;
    public float timeLimit;
    public float reward;
    public OrderState state;

    public DeliveryOrder(int id, Building restaurant, Building customer, float rewardAmount)
    {
        orderld = id;
        restauranBuilding = restaurant;
        customerBuilding = customer;
        restauranName = restaurant.buildingName;
        customerName = customer.buildingName;
        orderTime = Time.time;
        timeLimit = Random.Range(60f, 120f);            //1~2 분 제한
        reward = rewardAmount;
        state = OrderState.WaitingPickup;
    }

    public float GetRemainingTime()
    {
        return Mathf.Max(0f, timeLimit - (Time.time - orderTime));          //남은 시간 리턴
    }

    public bool IsExpired()
    {
        return GetRemainingTime() <= 0f;
    }
}
