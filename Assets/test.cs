using System.Collections;
using Game.Battle.Attacks;
using Managers;
using TMPro;
using UnityEngine;

public class GameHud : MonoBehaviour
{
    private int _val;

    [Header("Prefabs")] 
    [SerializeField] private TextMeshProUGUI diceScorePopup;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private AnimationCurve bubbleTextScale;
    [SerializeField] private float duration;
    [SerializeField] private float bubbleDuration = 0.4f;
    
    [Header("UI Elements")] 
    [SerializeField] private TextMeshProUGUI score;

    private float textSize;

    private readonly Quaternion _cachedRotation = Quaternion.LookRotation(Vector3.down);
    
    void Start()
    {
        Dice.OnDiceRolled += OnDiceRolled;
        textSize = score.fontSize;
    }

    private void OnDestroy()
    {
        Dice.OnDiceRolled -= OnDiceRolled;
    }

    private void OnDiceRolled(EDiceType type, Dice dice)
    {
        StartCoroutine(CreateAndHandlePopup(dice.CurrentValue, Camera.main!.WorldToScreenPoint(dice.transform.position)));
    }

    private IEnumerator CreateAndHandlePopup(int value, Vector3 location)
    {
        TextMeshProUGUI popup = Instantiate(diceScorePopup, location, Quaternion.identity, transform);
        popup.text = value.ToString();

        yield return BubbleText(popup, popup.fontSize);
        
        float life = 0;
        Vector3 target = score.rectTransform.rect.center + (Vector2)score.transform.position;
        Transform tr = popup.transform; // yes this does optimize
        while (life < duration)
        {
            life += Time.deltaTime;
            tr.position = Vector3.Slerp(location, target, curve.Evaluate(life / duration));
            yield return null;
        }
        _val += value;
        score.text = _val.ToString();
        Destroy(popup.gameObject);

        target.y -= score.rectTransform.rect.center.y;
        Vector3 loc = Camera.main.ScreenToWorldPoint(target);
        EffectManager.instance.PlaySparks(loc, _cachedRotation, Color.white);
        
        yield return BubbleText(score, textSize);
    }

    private IEnumerator BubbleText(TextMeshProUGUI target, float originalScale)
    {
        float life = 0;

        while (life < bubbleDuration)
        {
            life += Time.deltaTime;
            target.fontSize = originalScale * bubbleTextScale.Evaluate(life / bubbleDuration);
            yield return null;
        }
        target.fontSize = originalScale;
    }
}
