using UnityEngine;
using UnityEngine.Events;

public class AnimationEventSender : MonoBehaviour
{
    public UnityEvent<AnimationEvent> onAttack;

    void OnAttack(AnimationEvent info) 
    {
        onAttack.Invoke(info);
    }      
}

