using System.Collections.Generic;
using Game.Battle.ScriptableObjects;
using UI;
using UnityEngine;

namespace Managers
{
    public class GraphManager : MonoBehaviour
    {
        [SerializeField] private GraphingTool graphingToolPrefab;
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject root;

        public static GraphManager Instance { get; private set; }

        private readonly Dictionary<AbilityBaseStats, GraphingTool> _graphs = new Dictionary<AbilityBaseStats, GraphingTool>();

        private Vector2 _prefabSizeDelta;

        public void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            _prefabSizeDelta = ((RectTransform)(graphingToolPrefab.transform)).sizeDelta;
        }


        public void RegisterRoll(AbilityBaseStats ability, int setTotalValue)
        {
            if (!_graphs.TryGetValue(ability, out GraphingTool graph))
            {
                graph = Instantiate(graphingToolPrefab, content);
                //graph.CreateGraph(ability.MinRollValue + ability.BaseValue, ability.MaxRollValue + ability.BaseValue);
                graph.SetTitle(ability.name);
                _graphs.Add(ability, graph);
                content.sizeDelta = new Vector2( content.sizeDelta.x, _prefabSizeDelta.y * _graphs.Count);
            }
            graph.AddValue(setTotalValue);
        }

        public void Hide()
        {
            root.SetActive(false);
        }

        public void Show()
        {
            root.SetActive(true);

            foreach (GraphingTool graph in _graphs.Values)
            {
                graph.UpdateGraph();
            }
        }

    }
}
