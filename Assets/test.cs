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
    [SerializeField] private float duration;
    
    [Header("UI Elements")] 
    [SerializeField] private TextMeshProUGUI score;

    void Start()
    {
        Dice.OnDiceRolled += OnDiceRolled;
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
    }


}
