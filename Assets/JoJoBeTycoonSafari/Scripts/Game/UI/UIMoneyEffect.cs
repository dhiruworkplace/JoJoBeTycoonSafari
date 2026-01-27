using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using ZooGame;
using System;
using System.Collections;
using System.Collections.Generic;
using UFrame.Common;
using UFrame.MiniGame;
using UnityEngine;
using UnityEngine.UI;

public enum MoneyType
{
    Diamond, //钻石
    Money,   //金币
    Physic,  //体力
}

public class UIMoneyEffect : UIPage
{
    private struct MoneyEffectData
    {
        public RectTransform Trans;
        //public GameObject effect;
        public Image m_Icon;
    }
    private Transform m_MoneyEffectTrans;
    private List<MoneyEffectData> m_MoneyEffectChildList;
    private Animator m_Animator;
    /// <summary>
    /// 需要移动的总数量
    /// </summary>
    private int m_MaxCount;
    /// <summary>
    /// 当前播放的次数
    /// </summary>
    private int m_CurPlayCount;

    private Vector2 m_TargetPosition;
    private MoneyType m_MoneyType;
    private float intervalTime = 0;
    private float moveTime = 0;
    private float money_delay_time = 0;

    private GameObject m_MoneyEffectObj;

    private GameObject m_DiamondEffectObj;

    GameObject physicEffectObj;

    public UIMoneyEffect() : base(UIType.PopUp, UIMode.DoNothing, UICollider.None)
    {
        uiPath = "prefabs/Effect/UIMoneyEffect";
    }

    public override void Awake(GameObject go)
    {
        base.Awake(go);
        this.registerCompents();
    }
    private void registerCompents()
    {
        m_MoneyEffectTrans = this.RegistCompent<Transform>("UIHuoBi");
        if (m_MoneyEffectChildList == null)
            m_MoneyEffectChildList = new List<MoneyEffectData>();

        m_MaxCount = m_MoneyEffectTrans.childCount;
        for (int i = 0; i < m_MaxCount; i++)
        {
            MoneyEffectData data = new MoneyEffectData();
            data.Trans = m_MoneyEffectTrans.GetChild(i).GetComponent<RectTransform>();
            data.m_Icon = m_MoneyEffectTrans.GetChild(i).GetComponent<Image>();
            //data.effect = data.Trans.GetChild(0).gameObject;
            m_MoneyEffectChildList.Add(data);
            // Debug.Log("childName = " + m_MoneyEffectTrans.GetChild(i).name);
        }

        m_Animator = this.RegistCompent<Animator>("UIHuoBi");


        //intervalTime = Excel_Conf.GetConfigFloatValue(Excel_Conf.money_move_interval_time);
        intervalTime = 0.05f;    //金币特效移动间隔时间

        //moveTime = Excel_Conf.GetConfigFloatValue(Excel_Conf.money_move_time);
        moveTime = 0.18f; //金币特效移动时间
        //moveTime = 1f; //金币特效移动时间

        //money_delay_time = Excel_Conf.GetConfigFloatValue(Excel_Conf.money_delay_time);
        money_delay_time = 0.6f;//  金币延迟移动时间


        //m_MoneyEffectObj = ResourceMgr.GetEffect(ObjType.UI_Money_jinbi).gameObject;
        m_MoneyEffectObj = ResourceManager.GetInstance().LoadGameObject(Config.globalConfig.getInstace().ScreenGoldEffect);
        SetParent(m_MoneyEffectObj.transform, this.gameObject.transform, true, true);

        //m_DiamondEffectObj = ResourceMgr.GetEffect(ObjType.UI_Money).gameObject;
        m_DiamondEffectObj = ResourceManager.GetInstance().LoadGameObject(Config.globalConfig.getInstace().ScreenGoldEffect);
        SetParent(m_DiamondEffectObj.transform, this.gameObject.transform, true, true);

        //physicEffectObj = ResourceMgr.GetEffect(ObjType.UI_Money_jinbi).gameObject;
        physicEffectObj = ResourceManager.GetInstance().LoadGameObject(Config.globalConfig.getInstace().ScreenGoldEffect);
        SetParent(physicEffectObj.transform, this.gameObject.transform, true, true);

        m_MoneyEffectObj.SetActive(false);
        m_DiamondEffectObj.SetActive(false);
        physicEffectObj.SetActive(false);

    }
    void SetParent(Transform child, Transform parent, bool resetPosition, bool resetScale)
    {
        if (child != null)
        {
            Vector3 scale = child.localScale;
            Vector3 position = child.localPosition;
            child.parent = parent;
            child.localEulerAngles = Vector3.zero;
            child.localPosition = resetPosition ? Vector3.zero : position;
            child.localScale = resetScale ? Vector3.one : scale;
        }
    }


    public override void Refresh()
    {
        base.Refresh();
        m_MoneyEffectObj.SetActive(false);
        m_DiamondEffectObj.SetActive(false);
    }

    public override void Active()
    {
        base.Active();
        List<object> list = m_data as List<object>;
        m_TargetPosition = (Vector2)list[0];
        m_MoneyType = (MoneyType)list[1];
        Sprite effSprite = null;
        switch (m_MoneyType)
        {
            case MoneyType.Diamond:
                //effSprite = ResourceMgr.LoadSprite("UIAtlas/main/cy_yuanbao");
                effSprite = ResourceManager.LoadSpriteFromPrefab("UIAtlas/main/Gold");
                break;
            case MoneyType.Money:
                //effSprite = ResourceMgr.LoadSprite("UIAtlas/main/cy_jinbi");
                effSprite = ResourceManager.LoadSpriteFromPrefab("UIAtlas/main/Gold");
                break;
            case MoneyType.Physic:
                //effSprite = ResourceMgr.LoadSprite("UIAtlas/main/cy_energy");
                effSprite = ResourceManager.LoadSpriteFromPrefab("UIAtlas/main/Gold");
                break;
        }

        for (int i = 0; i < m_MoneyEffectChildList.Count; i++)
        {
            m_MoneyEffectChildList[i].m_Icon.overrideSprite = effSprite;
        }

        m_MoneyEffectObj.transform.position = m_TargetPosition;
        m_DiamondEffectObj.transform.position = m_TargetPosition;
        physicEffectObj.transform.position = m_TargetPosition;

        m_Animator.enabled = true;
        m_Animator.Play("UIHuoBi");
        TimerMgr.Instance.AddTimer("DelayHide", money_delay_time, () => { StartTweenMoney(); });

    }

    private void StartTweenMoney()
    {
        //Logger.LogWarp.LogError("StartTweenMoney");
        m_Animator.enabled = false;
        //LoadingMgr.Inst.StartCoroutine(PlayMoneyAnimation());
        RunCoroutine.Run(PlayMoneyAnimation());
    }

    private IEnumerator PlayMoneyAnimation()
    {
        //Logger.LogWarp.LogError("PlayMoneyAnimation " + m_MaxCount);
        for (int i = 0; i < m_MaxCount; i++)
        {
            DoTweenMoneyEffect(i);
            yield return new WaitForSeconds(intervalTime);
        }
    }

    Vector3 scale = new Vector3(0.5f, 0.5f, 0.5f);
    TweenerCore<Vector2, Vector2, VectorOptions> tweenPos;
    private void DoTweenMoneyEffect(int index)
    {
        //Logger.LogWarp.LogError("DoTweenMoneyEffect " + index + " " + m_TargetPosition + " " + moveTime);
        MoneyEffectData data = m_MoneyEffectChildList[index];
        data.Trans.localScale = scale;
        //Vector2 p2 = Vector2.zero;
        Vector2 p2 = data.Trans.position;
        //if (tweenPos != null)
        //{
        //    if (tweenPos.IsPlaying())
        //    {
        //        tweenPos.Complete();
        //    }
        //    tweenPos.Kill();

        //}

        tweenPos = DOTween.To(() => p2, x => p2 = x, m_TargetPosition, moveTime);
        tweenPos.SetEase(Ease.InSine);
        //data.effect.SetActive(true);
        //Debug.Log("m_TargetPosition=" + m_TargetPosition);
        tweenPos.OnUpdate(() =>
        {
            //Logger.LogWarp.LogError(data.Trans.name + " " + p2);
            data.Trans.position = p2;
        });
        tweenPos.OnComplete(() =>
        {
            //Logger.LogWarp.LogError("tweenPos.OnComplete " + m_CurPlayCount + " " + m_MaxCount);
            data.Trans.localScale = Vector3.zero;

            m_CurPlayCount++;
            //EventManager.Trigger(EventEnum.MoneyEffectPlayUpdate, m_MoneyType);
            //ShakePhone.ShakeLight();
            if ((m_CurPlayCount + 1) >= m_MaxCount)
            {
                m_CurPlayCount = 0;
                switch (m_MoneyType)
                {
                    case MoneyType.Diamond:
                        m_DiamondEffectObj.SetActive(true);
                        break;
                    case MoneyType.Money:
                        m_MoneyEffectObj.SetActive(true);
                        break;
                    case MoneyType.Physic:
                        physicEffectObj.SetActive(true);
                        break;
                }

                RunCoroutine.Run(PlayCompleteEffect());

            }
        });
    }

    private IEnumerator PlayCompleteEffect()
    {
        yield return new WaitForSeconds(0.54f);
        PageMgr.ClosePage<UIMoneyEffect>();
    }

    public override void Hide()
    {
        base.Hide();
    }
}
