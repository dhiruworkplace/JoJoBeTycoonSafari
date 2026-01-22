using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowAnimelCamera : MonoBehaviour
{
    public Animation animation;
    public string animationName;
    public bool ShowBool;
    float speed;
    ParticleSystem particleSystem1;
    ParticleSystem particleSystem2;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.LookAt(transform.Find("Plane").position);
        animationName = "idle";
        ShowBool = false;
        speed = Config.globalConfig.getInstace().AnimalShowRotateSpeed;
    }
    private void OnEnable()
    {

    }
    public void SetGameObject( )
    {
        ShowBool = true;
        animation = this.transform.GetComponentInChildren<Animation>();

    }
    // Update is called once per frame
    void Update()
    {
        if (ShowBool == true)
        {
            animation.Play(animationName);
            animation.transform.Rotate(new Vector3(0, speed, 0));

        }
        //particleSystem2.Play();

    }
}
