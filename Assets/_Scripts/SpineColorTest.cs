using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class SpineColorTest : MonoBehaviour
{

    public Color testColor;
    public SkeletonAnimation skeletonAnimation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.skeletonAnimation.skeleton.SetColor(this.testColor);
    }
}
