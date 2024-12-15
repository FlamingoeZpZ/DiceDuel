using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UI.Graphing
{
    public abstract class GraphingTool : MonoBehaviour
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

        private List<DataContainer> bars = new();
        private List<DataContainer> scores = new();

        private int[] _values;
        private int _minPush;
        private int _numSteps;
        private void OnEnable()
        {
            
            //When rolling two dice.
            /*
            CreateGraph(2,12);
            
            for (int i = 1; i < 10000; i++)
            {
                AddValue(Random.Range(1, 7) + Random.Range(1, 7));
            }
                 //When rolling 1 d 6, and 1 d 20
            CreateGraph(2,26);
            
            for (int i = 1; i < 10000; i++)
            {
                AddValue(Random.Range(1, 7) + Random.Range(1,21));
            }
            */
            //When rolling 1 d4, 1 d 6, 1 d 8, and 1 d 10, and 1 d 20
            CreateGraph(5,48);
            
            for (int i = 1; i < 100000; i++)
            {
                AddValue(Random.Range(1, 5) + Random.Range(1, 7) + Random.Range(1, 9)+ Random.Range(1, 11) + Random.Range(1,21));
            }
            
            //Render the graph.
            float mostRolled = 0;

            //Is there anyway we can merge?
            for (int i = 0; i < _values.Length; ++i)
            {
                if (_values[i] > mostRolled)
                {
                    mostRolled = _values[i];
                }
            }

            float steps = Mathf.CeilToInt(mostRolled / _numSteps);
            float scale = 1;
            while (steps >= 10)
            {
                steps /= 10;
                scale *= 10;
            }

            Debug.Log(steps);
            
            steps = Mathf.Ceil(steps) * scale;

            float t = steps * _numSteps;
            //This should be using the local min and max
            for (int i = 0; i < _numSteps; i++)
            {
                DataContainer dc = scores[i];
                dc.Image.color = (i & 1) == 0 ? Color.grey : Color.white;
                dc.Text.text =  (Mathf.CeilToInt(steps * (i + 1))).ToString();
            }
            
            float graphHeight = barParent.rect.height;
            for (int i = 0; i < bars.Count; ++i)
            {
                bars[i].Image.rectTransform.sizeDelta = new  Vector2(0 , graphHeight * (_values[i] / t)); //no ctrl over x
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
            while (bars.Count < _values.Length)
            {
                GameObject g = Instantiate(barPrefab, barParent);
                bars.Add(new DataContainer()
                {
                    Image = g.GetComponent<Image>(),
                    Text = g.transform.GetChild(0).GetComponent<TextMeshProUGUI>()
                });
            }
            
            while (scores.Count < numSteps)
            {
                GameObject g = Instantiate(scorePrefab, scoreParent);
                scores.Add(new DataContainer()
                {
                    Image = g.transform.GetChild(1).GetComponent<Image>(),
                    Text = g.transform.GetChild(0).GetComponent<TextMeshProUGUI>()
                });
            }


            for (int i = 0; i < scores.Count; i++)
            {
                scores[i].Image.transform.parent.gameObject.SetActive(i < numSteps);
            }

            for (int i = 0; i < bars.Count; ++i)
            {
                if ( i < _values.Length)
                {
                    bars[i].Image.gameObject.SetActive(true);
                    bars[i].Text.text = (min+i).ToString();
                }
                else
                {
                    bars[i].Image.gameObject.SetActive(false);
                }
            }
            
        }

        public void AddValue(int index)
        {
            Debug.Log($"Adding value of {index-_minPush}");
            _values[index-_minPush]++;
        }

        public void ResetValues()
        {
            for (int i = 0; i < _values.Length; ++i)
            {
                _values[i] = 0;
            }
        }
    }

 

}
