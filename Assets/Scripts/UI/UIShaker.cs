using System;
using UnityEngine;

namespace UI
{
    public class UIShaker : MonoBehaviour
    {
        [SerializeField] private float sizeMult = 1;
        [SerializeField] private float speed;
        [SerializeField] private float maxAngle;
        
        private Vector3 originalScale;
        private void OnEnable()
        {
            originalScale = transform.localScale;
            transform.localScale = originalScale * sizeMult;
        }

        private void OnDisable()
        {
            transform.localScale = originalScale;
            transform.rotation = Quaternion.identity;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            transform.Rotate(Vector3.forward, Time.deltaTime * speed);
            if(transform.eulerAngles.z > maxAngle && transform.eulerAngles.z < 180) speed = -Mathf.Abs(speed);
            else if(transform.eulerAngles.z < 360-maxAngle && transform.eulerAngles.z > 180) speed = Mathf.Abs(speed);
        }
    }
}
