using Game.GlobalData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotationCamera : MonoBehaviour
{
    public Transform selectCenter;
    public Vector3 skewingVector = new Vector3(50f,35f,0f);
    public float RotationSpeed = 30f;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    private void OnEnable()
    {
        selectCenter = GlobalDataManager.GetInstance().playerData.playerZoo.BuildShowTransform;
        transform.position = selectCenter.position + skewingVector;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (selectCenter == null)
        {
            selectCenter = GlobalDataManager.GetInstance().playerData.playerZoo.BuildShowTransform;
            transform.position = selectCenter.position + skewingVector;

        }
        transform.RotateAround(selectCenter.position, selectCenter.up, RotationSpeed * Time.deltaTime);
        transform.LookAt(selectCenter.position);


    }
}
