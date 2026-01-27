using UnityEngine;
using System.Collections.Generic;
using ZooGame.GlobalData;
using UFrame.Common;
using UFrame.MessageCenter;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;

namespace ZooGame
{
    public enum FreeItemAdState
    {
        Unstart,
        CoolDown,
        Opening,
        Opened,
        Closing,
        ViewOpened
    }

    public class MiscModel : Singleton<MiscModel>, ISingleton
    {
        private bool inited = false;

        public void Init()
        {
            if (!inited)
            {
                freeItemAdState = FreeItemAdState.Unstart;
                elapseTime = 0;
                inited = true;
            }
        }

        private PlayerData playerData { get { return GlobalDataManager.GetInstance().playerData; } }
        
        public bool IsTutorialComplete() { return !playerData.playerZoo.isGuide; }

        public FreeItemAdState freeItemAdState = FreeItemAdState.Unstart;

        public int freeItemAdCD { get { return Config.globalConfig.getInstace().AdvertFreeItemCD; } }

        public int freeItemAdBeginTime { get { return Config.globalConfig.getInstace().AdvertFreeItemTime; } }

        public float elapseTime = 0;
    }

    public class MiscModule : GameModule
    {
        private bool clocking = false; // 是否在计时中
        private bool detectBalloonClick = false;
        private FreeItemAdConfig freeItemAdConfig;
        private Transform balloonTrans;
        private Animator balloonAnimator;
        private Vector3 balloonBeginPos = Vector3.zero;
        private Vector3 balloonEndPos = Vector3.zero;
        private bool positionBalloon = false;
        private string animStateIdle = "HotAirBalloon_Idle";
        private string animStateCome = "HotAirBalloon_Come";
        private string animStateLeave = "HotAirBalloon_Leave";
        private Coroutine comeCoroutine;
        private Coroutine leaveCoroutine;

        public MiscModule(int orderID) : base(orderID) { }

        public static bool IsPointerOverUI()
        {
#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
        return EventSystem.current != null && Input.touchCount >= 1 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
#else
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
#endif
        }

        public override void Init()
        {
            freeItemAdConfig = Resources.Load<FreeItemAdConfig>("configs/free_item_ad_config");
            miscModel.Init();
            
            if (miscModel.freeItemAdState == FreeItemAdState.Opened ||
                miscModel.freeItemAdState == FreeItemAdState.Opening) // 热气球之前已经出现或者正在出现过程中
                ShowFreeItemAdButton(true);
            else
                clocking = true;

            Logger.LogWarp.Log("-->MiscModule init.");
            //GlobalEventHandler.inst.onApplicationPauseOnResume += OnApplicationPauseOnResume;

        }
        private void OnApplicationPauseOnResume(bool isPause)
        {
            if (!isPause)
            {
                if (PageMgr.allPages != null)
                {
                    UIMainPage mainPage = PageMgr.GetPage<UIMainPage>();
                    if (mainPage != null)
                        mainPage.OpenGuideTaskPanel();
                }
            }
        }
        private MiscModel miscModel { get { return MiscModel.GetInstance(); } }
        private PlayerData playerData { get { return GlobalDataManager.GetInstance().playerData; } }
        private Camera sceneCamera { get { return ZooCamera.GetInstance().cacheCam; } }

        public override void Tick(int deltaTimeMS)
        {
            if (!miscModel.IsTutorialComplete())
                return;

            if (clocking)
            {
                miscModel.elapseTime += Time.deltaTime;
                if (miscModel.freeItemAdState == FreeItemAdState.Unstart)
                {
                    if (miscModel.elapseTime >= miscModel.freeItemAdBeginTime)
                    {
                        miscModel.elapseTime = 0;
                        clocking = false;
                        ShowFreeItemAdButton();
                    }
                }
                else if (miscModel.freeItemAdState == FreeItemAdState.CoolDown)
                {
                    if (miscModel.elapseTime >= miscModel.freeItemAdCD)
                    {
                        miscModel.elapseTime = 0;
                        clocking = false;
                        ShowFreeItemAdButton();
                    }
                }
            }

            if (detectBalloonClick)
                DetectButtonClick();

            //if (positionBalloon)
            //    PositionBalloonToView();
        }

        public override void Release()
        {
            clocking = false;
            detectBalloonClick = false;
            positionBalloon = false;
            if (leaveCoroutine != null)
                GameManager.GetInstance().StopCoroutine(leaveCoroutine);
            if (comeCoroutine != null)
                GameManager.GetInstance().StopCoroutine(comeCoroutine);
            if (balloonTrans != null)
            {
                balloonTrans.DOKill();
                balloonTrans.GetComponent<Collider>().enabled = false;
            }

            //GlobalEventHandler.inst.onApplicationPauseOnResume -= OnApplicationPauseOnResume;

        }

        private IEnumerator FinishBalloonLeave()
        {
            balloonAnimator.Play(animStateLeave, 0, 0);
            while (Mathf.Abs(balloonAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime - 1) > 0.02f)
                yield return null; 

            balloonTrans.gameObject.SetActive(false);
            Object.Destroy(balloonTrans.gameObject);
            balloonTrans = null;
            balloonAnimator = null;

            miscModel.freeItemAdState = FreeItemAdState.CoolDown;
            clocking = true;
        }

        private void OnAdvertTouristPageClose(int detail)
        {
            positionBalloon = false;
            balloonTrans.GetComponent<Collider>().enabled = false;
            miscModel.freeItemAdState = FreeItemAdState.Closing;
            /*balloonBeginPos = sceneCamera.ViewportToWorldPoint(new Vector3(freeItemAdConfig.balloonViewportPos.x, 1.2f, freeItemAdConfig.balloonViewportPos.z));
            balloonTrans.DOMove(balloonBeginPos, freeItemAdConfig.balloonLeaveDuration).OnComplete(() => {
                balloonTrans.gameObject.SetActive(false);
                Object.Destroy(balloonTrans.gameObject);
                balloonTrans = null;

                miscModel.freeItemAdState = FreeItemAdState.CoolDown;
                clocking = true;
            });*/
            leaveCoroutine = GameManager.GetInstance().StartCoroutine(FinishBalloonLeave());
        }

        private void PositionBalloonToView()
        {
            Vector3 pos = sceneCamera.ViewportToWorldPoint(freeItemAdConfig.balloonViewportPos);
            balloonTrans.position = Vector3.Lerp(balloonTrans.position, pos, 0.15f);
        }

        private void DetectButtonClick()
        {
            if (IsPointerOverUI())
                return;

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000f))
                {
                    if (hit.transform == balloonTrans)
                    {
                        miscModel.freeItemAdState = FreeItemAdState.ViewOpened;
                        detectBalloonClick = false;
                        PageMgr.ShowPage<UIAdvertTouristPage>("FreeItemButton");
                        PageMgr.GetPage<UIAdvertTouristPage>().onClose.AddListener(OnAdvertTouristPageClose);
                    }
                }
            }
        }

        private IEnumerator FinishBalloonCome(bool immediate)
        {
            miscModel.freeItemAdState = FreeItemAdState.Opening;

            yield return null;
            balloonTrans.position = balloonEndPos;

            if (immediate)
            {
                detectBalloonClick = true;
                positionBalloon = true;
                balloonAnimator.Play(animStateIdle, 0, 1);
                miscModel.freeItemAdState = FreeItemAdState.Opened;
            }
            else
            {
                balloonTrans.GetComponent<Collider>().enabled = false;
                balloonAnimator.Play(animStateCome, 0, 0);
                while (Mathf.Abs(balloonAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime - 1) > 0.02f)
                    yield return null;

                balloonTrans.GetComponent<Collider>().enabled = true;
                detectBalloonClick = true;
                positionBalloon = true;
                balloonAnimator.Play(animStateIdle, 0, 0);
                miscModel.freeItemAdState = FreeItemAdState.Opened;
            }
        }

        private Vector3 GetBalloonEndPos()
        {
            Vector3 ret = Vector3.zero;
            string[] splitted = Config.globalConfig.getInstace().AdvertFreeItemPos.Split(',');
            if (splitted.Length > 2)
            {
                float.TryParse(splitted[0], out ret.x);
                float.TryParse(splitted[1], out ret.y);
                float.TryParse(splitted[2], out ret.z);
            }
            return ret;
        }

        private void ShowFreeItemAdButton(bool immediate = false)
        {
            GameObject go = GameObject.Instantiate(freeItemAdConfig.balloonTmpl);
            //balloonBeginPos = sceneCamera.ViewportToWorldPoint(new Vector3(freeItemAdConfig.balloonViewportPos.x, 1.2f, freeItemAdConfig.balloonViewportPos.z));
            //balloonEndPos = sceneCamera.ViewportToWorldPoint(freeItemAdConfig.balloonViewportPos);
            balloonEndPos = GetBalloonEndPos();
            balloonTrans = go.transform;
            balloonAnimator = balloonTrans.GetChild(0).GetComponent<Animator>();
            detectBalloonClick = false;

            /*if (immediate)
            {
                go.transform.position = balloonEndPos;
                detectBalloonClick = true;
                positionBalloon = true;
                miscModel.freeItemAdState = FreeItemAdState.Opened;
            }
            else
            {
                go.transform.position = balloonBeginPos;
                go.transform.GetComponent<Collider>().enabled = false;
                miscModel.freeItemAdState = FreeItemAdState.Opening;
                go.transform.DOMove(balloonEndPos, freeItemAdConfig.balloonShowDuration)
                    .SetEase(freeItemAdConfig.balloonShowAnimCurve)
                    .OnComplete(() => {
                        go.transform.GetComponent<Collider>().enabled = true;
                        detectBalloonClick = true;
                        positionBalloon = true;
                        miscModel.freeItemAdState = FreeItemAdState.Opened;
                    });
            }*/
            comeCoroutine = GameManager.GetInstance().StartCoroutine(FinishBalloonCome(immediate));
        }
    }
}
