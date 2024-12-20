using System;
using System.Collections.Generic;
using System.Linq;
using Game.Battle.Character;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class AIPool : MonoBehaviour
    {
        [SerializeField] private AIWarriorPool[] aiWarriorPrefabs;
        private List<AIWarriorPool> _activeWarriors = new List<AIWarriorPool>();
        private int _totalWeight;
        private void Awake()
        {
            ResetPool();
        }

        private void ResetPool()
        {
            Debug.Log("Refreshing AI Pool");
            _activeWarriors = aiWarriorPrefabs.ToList();
        }

        public AIWarrior GetWarriorPrefab()
        {
            // If we have exceeded bounds
            if(_activeWarriors.Count == 0) ResetPool();
            
            int currentWeight = Random.Range(0, _totalWeight);
            int targetWeight = 0;
            int index = 0;
            while (currentWeight > 0)
            {
                targetWeight = aiWarriorPrefabs[index++].SpawnWeight;
                currentWeight -= targetWeight;
            }
            // Remove the weight of the object we landed on
            _totalWeight -= targetWeight; 
            
            return aiWarriorPrefabs[index].Prefab;
        }
        

    }

    [Serializable]
    public struct AIWarriorPool
    {
        [SerializeField, Tooltip("Larger means higher frequency")] private int spawnWeight;
        [SerializeField] private AIWarrior prefab;
        
        public AIWarrior Prefab => prefab;
        public int SpawnWeight => spawnWeight;
        
    }
}