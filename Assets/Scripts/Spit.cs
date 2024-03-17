using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace LlamAcademy
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class Spit : MonoBehaviour
    {
        [SerializeField]
        private float AutoDestroyTime = 1f;

        [SerializeField] private float Force = 100;
        
        private WaitForSeconds Wait;
        private Rigidbody Rigidbody;

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine(DelayDisable());
        }

        private IEnumerator DelayDisable()
        {
            if (Wait == null)
            {
                Wait = new WaitForSeconds(AutoDestroyTime);
            }

            yield return null;
            
            Rigidbody.AddForce(transform.forward * Force);
            
            yield return Wait;
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            Rigidbody.angularVelocity = Vector3.zero;
            Rigidbody.velocity = Vector3.zero;
        }

        private void OnTriggerEnter(Collider other)
        {
            // you'd want to apply damage or something here as well.
            gameObject.SetActive(false);
        }
    }
}
