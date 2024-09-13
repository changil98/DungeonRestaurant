
public class DefaultState : IHungerState
{
    private HungerGauge hungerGauge { get; set; }
    public DefaultState(HungerGauge hungerGauge)
    {
        this.hungerGauge = hungerGauge;
    }

    public void EnterState()
    {
        hungerGauge.StartCoroutine(hungerGauge.Hunger());
    }
    public void UpdateState()
    {
        
    }

    public void ExitState()
    {
        
    }


}
