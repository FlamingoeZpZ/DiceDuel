using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utility;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private float transitionDuration;
    
    private Material _targetMaterial;
    private bool _isLoading;

    public static LoadingManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        gameObject.SetActive(false);

        _targetMaterial = GetComponentInChildren<Image>().material;
        instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public void LoadLevelById(int id)
    {
        if (_isLoading) return;
        gameObject.SetActive(true);
        StartCoroutine(OnTransitionBegin(id));
    }

    private IEnumerator OnTransitionBegin(int id)
    {
        
        _isLoading = true;
        
        float t = 0;
        while (t < transitionDuration)
        {
            t += Time.deltaTime;
            float p = t/ transitionDuration;
            _targetMaterial.SetFloat(StaticUtility.TimeID, p);
            yield return null;
        }

        //Load new, unload old
        AsyncOperation op = SceneManager.LoadSceneAsync(id);

        //Operation cannot be null.
        op!.completed += _ =>
        {
            StartCoroutine(OnTransitionEnd());
        };
        
    }

    private IEnumerator OnTransitionEnd()
    {
        float t = 0;
        //Load level.
        while (t < transitionDuration)
        {
            t += Time.deltaTime;
            float p = t/ transitionDuration;
            _targetMaterial.SetFloat(StaticUtility.TimeID, 1-p); // inverse
            yield return null;
        }

        _isLoading = false;
        gameObject.SetActive(false);

    }




}
