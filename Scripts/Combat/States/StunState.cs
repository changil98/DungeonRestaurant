using UnityEngine;

public class StunState : IState
{
    private BaseCombatAI character;

    public StunState(BaseCombatAI character)
    {
        this.character = character;
    }

    public void OnEnter()
    {
        Vector3 effectPos = character.transform.position;
        Vector3 offset = effectPos - character.transform.position;
        offset.y += 1f;
        if(character.team == Team.Ally)
        {
            character.PlayAnimation(AnimationData.Debuff_StunHash, true);
        }
       
        EffectManager.Instance.PlayFollowingParticleEffect(character.stunEffect,character.transform,offset,1f);

        
    }

    public void OnExit()
    {

    }

    public void Update()
    {
        
    }
    public void FixedUpdate()
    {

    }
}