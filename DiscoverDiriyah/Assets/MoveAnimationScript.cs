using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAnimationScript : MonoBehaviour
{
    [SerializeField] Transform targetTransform;
    [SerializeField] float duration;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move()
    {
        LeanTween.moveY(gameObject, targetTransform.position.y, duration);
    }
}
