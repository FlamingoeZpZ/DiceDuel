using System.Collections.Generic;
using Game.Battle.Character;
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

        private readonly Dictionary<int, GraphingTool> _graphs = new Dictionary<int, GraphingTool>();

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

        private static readonly string[] Names = new string[]
        {
            "x D-4 ",
            "x D-6 ",
            "x D-8 ",
            "x D-10 ",
            "x D-20 ",
        };

        public void RegisterRoll(EDiceType[] dice, int setTotalValue)
        {

            int[] values = new int[5];
            int min = 0;
            int max = 0;
            string graphName = "Rolls: ";

            foreach (EDiceType d in dice)
            {
                values[(int)d]++;
            }

            for (var index = 0; index < values.Length; index++)
            {
                if (values[index] != 0)
                {
                    graphName += values[index] + Names[index];
                    min += values[index];
                    max += values[index] * DiceManager.DiceValues[(EDiceType)index].High;
                }
            }


            int value = min * 1000 + max;
            
            if (!_graphs.TryGetValue(value, out GraphingTool graph))
            {
                graph = Instantiate(graphingToolPrefab, content);
                graph.CreateGraph(min, max);
                graph.SetTitle(graphName);
                _graphs.Add(value, graph);
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
