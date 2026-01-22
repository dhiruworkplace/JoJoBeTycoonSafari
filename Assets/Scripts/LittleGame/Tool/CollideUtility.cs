using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LittleGame
{

    /// <summary>
    /// 碰撞辅助类
    /// 需要指定检测的tag名称
    /// </summary>
    public class CollideUtility : MonoBehaviour
    {

        public string checkTagName = "";



        private void OnTriggerEnter(Collider other)
        {
            if (checkTagName == "")
            {
                Debug.Log("需要碰撞检测的名称为空" + gameObject);
                return;
            }
            if (other.transform.tag.Equals(checkTagName))
            {
                EventManager.Trigger<GameObject>(ToolEventName.CarCollideEvent, other.gameObject);
                //Debug.Log("撞到了!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                ///删除碰撞到对象身上的碰撞盒,以免多次碰撞
                
                Destroy(other.gameObject.GetComponent<Collider>());
            }
        }

    }
}