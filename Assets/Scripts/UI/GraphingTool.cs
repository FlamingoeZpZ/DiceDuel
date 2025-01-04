using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GraphingTool : MonoBehaviour
    {
        private struct DataContainer
        {
            public TextMeshProUGUI Text;
            public Image Image;
        }
        
        
        [Header("Components")] 
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI xAxis;
        [SerializeField] private TextMeshProUGUI yAxis;
        [SerializeField] private RectTransform scoreParent;
        [SerializeField] private RectTransform barParent;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject barPrefab;
        [SerializeField] private GameObject scorePrefab;

        private readonly List<DataContainer> _bars = new();
        private readonly List<DataContainer> _scores = new();

        private int[] _values;
        private int _minPush;
        private int _numSteps;

        private void OnEnable()
        {
            UpdateGraph();
        }

        public void UpdateGraph()
        {
            //Render the graph.
            float mostRolled = 0;

            //Is there anyway we can merge?
            foreach (var num in _values)
            {
                if (num > mostRolled)
                {
                    mostRolled = num;
                }
            }

            float steps = Mathf.CeilToInt(mostRolled / _numSteps);
            float scale = 1;
            while (steps >= 10)
            {
                steps /= 10;
                scale *= 10;
            }

            
            steps = Mathf.Ceil(steps) * scale;

            float t = steps * _numSteps;
            //This should be using the local min and max
            for (int i = 0; i < _numSteps; i++)
            {
                DataContainer dc = _scores[i];
                dc.Image.color = (i & 1) == 0 ? Color.grey : Color.white;
                dc.Text.text =  (Mathf.CeilToInt(steps * (i + 1))).ToString();
            }
            
            float graphHeight = barParent.rect.height;
            for (int i = 0; i < _bars.Count; ++i)
            {
                _bars[i].Image.rectTransform.sizeDelta = new  Vector2(0 , graphHeight * (_values[i] / t)); //no ctrl over x
            }
        }

        public void CreateGraph(int min, int max, int numSteps = 10)
        {
            _values = new int[max-min+1];
            _minPush = min;
            _numSteps = numSteps;
            

            /*
             * Don't destroy the bars, just create new ones.
             */
            while (_bars.Count < _values.Length)
            {
                GameObject g = Instantiate(barPrefab, barParent);
                _bars.Add(new DataContainer()
                {
                    Image = g.GetComponent<Image>(),
                    Text = g.transform.GetChild(0).GetComponent<TextMeshProUGUI>()
                });
            }
            
            while (_scores.Count < numSteps)
            {
                GameObject g = Instantiate(scorePrefab, scoreParent);
                _scores.Add(new DataContainer()
                {
                    Image = g.transform.GetChild(1).GetComponent<Image>(),
                    Text = g.transform.GetChild(0).GetComponent<TextMeshProUGUI>()
                });
            }


            for (int i = 0; i < _scores.Count; i++)
            {
                _scores[i].Image.transform.parent.gameObject.SetActive(i < numSteps);
            }

            for (int i = 0; i < _bars.Count; ++i)
            {
                if ( i < _values.Length)
                {
                    _bars[i].Image.gameObject.SetActive(true);
                    _bars[i].Text.text = (min+i).ToString();
                }
                else
                {
                    _bars[i].Image.gameObject.SetActive(false);
                }
            }
        }

        public void AddValue(int index)
        {
            _values[index-_minPush]++;
        }


        public void ResetValues()
        {
            for (int i = 0; i < _values.Length; ++i)
            {
                _values[i] = 0;
            }
        }

        public void SetTitle(string abilityName)
        {
            title.text = abilityName;
        }
    }

 

}
