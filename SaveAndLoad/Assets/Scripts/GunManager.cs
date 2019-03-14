using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    //最大及最小的x,y轴的旋转角度
    private float maxYRotation = 120;
    private float minYRotation = 0;
    private float maxXRotation = 60;
    private float minXRotation = 0;

    private float shootTime = 1;
    private float shootTimer = 0;

    public GameObject bulletGo;
    public Transform firePosition;
    [Range(1500, 5000)]
    public float force;
    private void Update()
    {
        shootTimer += Time.deltaTime;
        if(shootTimer > shootTime)
        {
            //TODO:可以射击
            if(Input.GetMouseButtonDown(0))
            {
                GameObject bulletCurrent = GameObject.Instantiate(bulletGo, firePosition.position, Quaternion.identity);
                Debug.Log(2);
                bulletCurrent.GetComponent<Rigidbody>().AddForce(transform.forward * force/*, ForceMode.VelocityChange*/);
                shootTimer = 0;
            }
        }
        float xPosPercent = Input.mousePosition.x / Screen.width;
        float yPosPercent = Input.mousePosition.y / Screen.height;

        float xAngle = -Mathf.Clamp(yPosPercent * maxXRotation, minXRotation, maxXRotation) + 15;
        float yAngle = Mathf.Clamp(xPosPercent * maxYRotation, minYRotation, maxYRotation) - 60;

        transform.eulerAngles = new Vector3(xAngle, yAngle, 0);
    }
}
