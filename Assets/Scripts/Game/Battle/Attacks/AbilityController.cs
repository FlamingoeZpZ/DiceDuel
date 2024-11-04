using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Battle.Interfaces;
using UnityEngine;

public class AbilityController : MonoBehaviour
{
    [SerializeField] private AbilityUI abilityPrefab;
    [SerializeField] private float distance;
    [SerializeField, Range(-6.28f, 6.28f)] private float angle = 1;
    [SerializeField, Range(-6.28f, 6.28f)] private float startAngle = 1;
    
    private readonly List<AbilityUI> _abilities = new ();
    private IAttack _selectedAttack = null;
    private int _numActiveAbilities = 0;
    
    //Get all the attacks the player has, spawn a UI for each and bind it choosing that ability.
    //Make the assumption that these abilities are 
    public void SetAbilities(IAttack[] attacks)
    {
        for (int i = 0; i < attacks.Length; i++)
        {
            IAttack attack = attacks[i];
          
            //Create it if we need to
            if (i == _abilities.Count)
            {
                float ang = i * angle - startAngle;
                Vector3 position = transform.position + new Vector3(Mathf.Cos(ang), Mathf.Sin(ang)) * distance;
                _abilities.Add(Instantiate(abilityPrefab, position, Quaternion.identity, transform));
            }
            AbilityUI ability = _abilities[i];
            ability.gameObject.SetActive(true);
            ability.Bind(this, attack);
        }
        _numActiveAbilities = attacks.Length;
        foreach (AbilityUI ability in _abilities)
        {
            ability.gameObject.SetActive(false);
        }
    }

    public void SetAttack(IAttack attack)
    {
        _selectedAttack = attack;
    }

    public async UniTask<IAttack> SelectAbility()
    {
        _selectedAttack = null;
        for (int i = 0; i < _numActiveAbilities; i++)
        {
            _abilities[i].gameObject.SetActive(true);
        }
        //Wait for something to modify our selected attack.
        await UniTask.WaitUntil(() => _selectedAttack != null);
        for (int i = 0; i < _numActiveAbilities; i++)
        {
            _abilities[i].gameObject.SetActive(false);
        }
        return _selectedAttack;
    }

    private void OnDrawGizmosSelected()
    {
        
        for (int ang = 0; ang < 5; ang += 1)
        {
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(Mathf.Cos(ang * angle - startAngle), Mathf.Sin(ang * angle - startAngle)) * distance);
        }
    }
}
