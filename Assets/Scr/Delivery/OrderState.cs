public enum OrderState
{
    WaitingPickup,              //픽업 대기중
    PickedUp,                   //픽업 완료, 배달 대기
    Completed,                  //배달 완료
    Expired                     //시간 초과
}
