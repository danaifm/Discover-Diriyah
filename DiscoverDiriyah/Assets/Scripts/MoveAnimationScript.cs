using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAnimationScript : MonoBehaviour
{
    [SerializeField] Transform targetTransformUp, targetTransformDown;
    [SerializeField] float duration;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveUp()
    {
        LeanTween.moveY(gameObject, targetTransformUp.position.y, duration);
    }

    public void MoveDown()
    {
        LeanTween.moveY(gameObject, targetTransformDown.position.y, duration);
    }
}
