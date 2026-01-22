using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UFrame.Common;
using DG.Tweening;

namespace LittleGame
{

    /// <summary>
    /// 过马路小游戏控制类
    /// </summary>
    public class LittleController : MonoBehaviour
    {

        #region property

        /// <summary>
        /// 游戏是否已经开始
        /// </summary>
        private bool isGameRunning;

        /// <summary>
        /// 当前小动物是否被撞了
        /// </summary>
        public bool isAnimalBeHit = false;

        /// <summary>
        /// 小动物被撞了之后存放的队列
        /// </summary>
        private List<GameObject> animalBeHitList;

        /// <summary>
        /// 游戏的开始位置.坐标固定
        /// </summary>
        public Vector3 gameStartPos;

        /// <summary>
        /// 当前关卡的终点.需要计算出
        /// </summary>
        public Vector3 gameEndPos;

        /// <summary>
        /// 检查点
        /// </summary>
        public List<Vector3> checkPos;

        /// <summary>
        /// 用于临时保存移动完成队列的小动物
        /// </summary>
        public List<GameObject> tempMoveGroupList;

        /// <summary>
        /// 当前关卡
        /// </summary>
        public static int CurrentLevel = 1;

        /// <summary>
        /// 当前关卡马路的数量
        /// 这个值需要走表
        /// </summary>
        public int currentLevelRoadNum = 6;

        /// <summary>
        /// 当前关卡笼子的数量
        /// 这个值需要走表
        /// </summary>
        public int currentLevelCageNum = 3;

        /// <summary>
        /// 有笼子的马路下标
        /// </summary>
        private int currentCageIndex = 0;

        /// <summary>
        /// 当前关卡还需要生成的笼子的位置
        /// 参数 马路的ID
        /// </summary>
        public Dictionary<int,Cage> currentCageIndexDic;

        /// <summary>
        /// 当前阶段的起点
        /// </summary>
        private Vector3 currentStartPos;

        /// <summary>
        /// 当前阶段的终点
        /// </summary>
        public Vector3 currentEndPos;

        /// <summary>
        /// 当前到达的检查点位置
        /// </summary>
        private int currentArriveCheckPosInedx;

        /// <summary>
        /// 当前目标检查点的位置
        /// </summary>
        private int currentTargetCheckPosIndex;

        /// <summary>
        /// 计算按下时间计时器
        /// </summary>
        private float timer;

        /// <summary>
        /// 按下之后 多久就算长按了
        /// </summary>
        private float touchStartMoveTime = 0.1f;

        /// <summary>
        /// 添加到移动队列的数量
        /// </summary>
        private int addToMoveGroup;

        /// <summary>
        /// 当前是长按吗
        /// </summary>
        private bool isLongTouch = false;

        /// <summary>
        /// 当前有按钮被按下了吗
        /// </summary>
        private bool isTouch = false;
        /// <summary>
        /// 当前小动物开始移动了吗
        /// </summary>
        private bool isAnimalsMove = false;

        /// <summary>
        /// 当前动物的list
        /// </summary>
        public List<Transform> currentAnimalsList;

        /// <summary>
        /// 记录当前可以使用的小动物数量
        /// </summary>
        public int currentAnimalsCount;

        /// <summary>
        /// 用于标记当前小动物在原始队列中的位置
        /// </summary>
        public static int currentAddToMoveListIndex = -1;

        /// <summary>
        /// 当前关卡所有车子出生点和终点
        /// </summary>
        public Dictionary<int, CarTargetPosition> currentLevelcarPosDic;

        /// <summary>
        /// 记录关卡需要车子字典的下标
        /// </summary>
        private int currentLevelcarPosDicIndex;

        /// <summary>
        /// 当前检查点的下标
        /// </summary>
        private int currentCheckPointIndex;

        /// <summary>
        /// 记录当前已经生成了多少个笼子
        /// </summary>
        private int createCageNum;

        /// <summary>
        /// 准备结束游戏
        /// </summary>
        private bool readyToEndGame;

        /// <summary>
        /// 点击间隔
        /// </summary>
        private float touchInterval = 0.1f;

        /// <summary>
        /// 当前单次点击的操作是否已经完成了
        /// </summary>
        private bool isCurrentTouchMoveDown = true;

        /// <summary>
        /// 当前是否正处于忙的状态下
        /// 如果是的话就无法操作
        /// </summary>
        public bool isBusy = false;

        public static LittleController Instance;


        private GameObject resObj;
        private GameObject carObj;
        private GameObject animalsObj;
        private GameObject roadObj;
        private GameObject ornamentalObj;

        public LittleGameInit littleGameInit;
        LittleGameLogic littleGameLogic;

        #endregion

        #region unity call back

        void Awake()
        {
            Instance = this;
            ///生成当前关卡资源保存点
            resObj = new GameObject("LevelRes");
            carObj = new GameObject("car");
            carObj.transform.SetParent(resObj.transform);
            roadObj = new GameObject("rood");
            roadObj.transform.SetParent(resObj.transform);
            animalsObj = new GameObject("animals");
            animalsObj.transform.SetParent(resObj.transform);
            gameStartPos = GameObject.Find("StartPos").transform.position;
            ///出生点添加偏移量
            animalsObj.transform.position = gameStartPos ;
            //currentEndPos = GameObject.Find("CheckPos").transform.position;

            ornamentalObj = new GameObject("ornamental");
            ornamentalObj.transform.SetParent(resObj.transform);

            tempMoveGroupList = new List<GameObject>();

            ///添加需要辅助类
            littleGameInit = gameObject.AddComponent<LittleGameInit>();
            littleGameLogic = gameObject.AddComponent<LittleGameLogic>();
            ///测试数据生成测试点
            checkPointObj = new GameObject("chechPointObj").transform;
        }

        void Start()
        {
            PageMgr.ShowPage<StartPage>();

            ///注册监听
            EventManager.Add<GameObject>(ToolEventName.CarCollideEvent, OnCarHitAnimals);
            EventManager.Add<int>(ToolEventName.LittleGameStart, OnGameStart);
        }

        void Update()
        {
            if (!isGameRunning)
            {
                return;
            }

            //摄像机移动状态下 不可以操作
            if (CameraController.GetInstance().isCameraMoving)
            {
                return;
            }

            ///现在有的问题  当快速连续点击的时候,会出现小动物没有被赋予正确的终点问题
            ///尝试解决方案:添加新的标志位.当点击之后当前位置的小动物正式开始移动之后,才可以下一次点击

            if(!isBusy)
                ChechTouch();

            //检查是否有车碰到小动物
            //有的话直接从队列中移除
            if (isAnimalBeHit)
            {
                AnimalsBeHit();
            }
        }

        private void LateUpdate()
        {
            if (!isGameRunning)
            {
                return;
            }
            UpdateEndPosition();
        }

        private void OnDestroy()
        {
            EventManager.Remove<GameObject>(ToolEventName.CarCollideEvent, OnCarHitAnimals);
            EventManager.Remove<int>(ToolEventName.LittleGameStart, OnGameStart);
        }

        #endregion

        #region run Time

        /// <summary>
        /// 开始点击的时候
        /// </summary>
        private void OnTouchStart()
        {

        }

        private void OnTouch()
        {

        }

        private void OnTouchEnd()
        {

        }

        /// <summary>
        /// 检查是否有按键按下
        /// </summary>
        private void ChechTouch()
        {
            if (isTouch && !isLongTouch)
            {
                timer += Time.deltaTime;
                if (timer >= touchStartMoveTime && isCurrentTouchMoveDown)
                {
                    //StartCoroutine("StartLongMove");
                    timer = 0;
                }
            }
            if (isTouch)
            {
                timer += Time.deltaTime;
                if (timer >= touchStartMoveTime)
                {
                    timer = 0;
                    isLongTouch = true;
                    StartLongMove();
                }
            }

            //判断当前的点击状态
            if (Input.GetMouseButtonDown(0) && isCurrentTouchMoveDown)
            {
                if (isTouch || isLongTouch)
                    return;
                isAllMoveDone = false;
                isTouch = true;
                //StartCoroutine("StartMove");
                StartMove();
            }

            if (Input.GetMouseButtonUp(0))
            {
                isTouch = false;
                if (isLongTouch)
                {
                    //UpdateAnimalsState();
                    isBusy = true;
                    UpdateLastAnimalsPos();
                    timer = 0;
                    isLongTouch = false;
                }
                //StopCoroutine("StartLongMove");
                timer = 0;
            }

        }

        /// <summary>
        /// 在长按状态结束之后,更新当前小动物的终点位置,如果当前坐标已经超过了当前检查点 那么就直接到下一个检查点
        /// 否则就为当前的检查点位置
        /// </summary>
        private void UpdateAnimalsState()
        {
            var animalsTra = animalsObj.transform;
            isBusy = true;
            for (int i = 0; i < animalsTra.childCount; i++)
            {
                var animPfb = animalsTra.GetChild(i);
                var move = animPfb.GetComponent<MoveUtility>();
                var anim = animPfb.GetComponent<Animals>();
                move.StopMove();
                
                if (animPfb.position.z <= move.endPos.z)
                {
                    Debug.Log("移动到下一个");
                    ///先停止移动
                    anim.isBusy = true;
                    //move.StartMove(littleGameInit.GetAnimalsSpeed(), UpdateLongTouchEndPos(i), MoveCallback, Vector3.forward);
                }
                ///这些都是已经过了当前的终点 无法停止的
                //else
                //{
                //    Debug.Log("移动到当前");
                //    //move.StopMove();
                //    anim.isBusy = true;
                //    move.StartMove(littleGameInit.GetAnimalsSpeed(), UpdateLongTouchEndPos(i), MoveCallback, Vector3.forward);
                //}
            }
        }

        private Vector3 UpdateLongTouchEndPos(int index)
        {
            Vector3 endpos = Vector3.zero;
            if (currentCheckPointIndex == -1)
            {
                return gameStartPos - LittleGameDefine.AnimalWidth * index;
            }
            int temp = currentCheckPointIndex ;
            Debug.Log("当前检查点" + currentCheckPointIndex);
            //temp = Mathf.Clamp(temp, 0, checkPos.Count);
            endpos = checkPos[temp] - LittleGameDefine.AnimalWidth * index;
            return endpos;
        }

        /// <summary>
        /// 更新终点检查
        /// </summary>
        private void UpdateEndPosition()
        {
            //Debug.Log("剩余长度" + tempMoveGroupList.Count);
            ///如果下一个检查点是终点,
            if (readyToEndGame && tempMoveGroupList.Count >= 0)
            {
                
                ///使用直接移除第一位的方式是在小动物连续到达终点的时候,
                ///会导致没有将list中的全部小动物全部移除,先用便利全部移除
                for (int i = 0; i < tempMoveGroupList.Count; i++)
                {
                    GameObject pfb = tempMoveGroupList[0];
                    ///从移动队列中删除到达终点的小动物
                    tempMoveGroupList.RemoveAt(0);
                    ///开始单个结算流程
                    ///
                }

                ///当前所有的小动物已经全部转移完成
                ///开始整局游戏的结算
                //Debug.Log("是否全部移动完成了" + isAllMoveDone);
                if (currentAnimalsList.Count == 0 && tempMoveGroupList.Count == 0 && isAllMoveDone)
                {
                    EventManager.Trigger<float>(UICompentEventName.ScheduleSlider, currentCheckPointIndex);
                    isGameRunning = false;
                    Debug.Log("游戏胜利!!!!!!!!!!!!!!!!!!!!!");
                    GameEndDispose();
                    ///修改 : 当前不会保存数据
                    ///保存当前数量
                    //LittleGameData.LastLevelAnimalsNumber = currentAnimalsCount;
                    EventManager.Trigger(ToolEventName.LittleGameEndSuccess);
                    //CurrentLevel++;
                    Mathf.Clamp(CurrentLevel, CurrentLevel, littleGameInit.GetMaxLevel());
                }
            }
            else
            {
                //检查是否所有的小动物已经全部移动完成了
                //本轮开始移动时,记录当前list的数量 == 临时保存移动到终点的小动物
                //if (currentAnimalsCount == tempMoveGroupList.Count)
                if (currentAnimalsCount == tempMoveGroupList.Count)//&& tempMoveGroupList.Count == (currentAddToMoveListIndex + 1)
                {
                    Debug.Log("移动完成");
                    
                    
                    ///在每次移动完成之后,强制将长按状态取消
                    isLongTouch = false;
                    isTouch = false;
                    currentCheckPointIndex++;
                    //current
                    for (int i = 0; i < tempMoveGroupList.Count; i++)
                    {
                        currentAnimalsList.Add(tempMoveGroupList[i].transform);
                    }
                    ///当前检查点有笼子
                    ///避免在移动最后一个小动物的时候,小动物被销毁了但是还是解救了笼子中的小动物
                    ///需求修改 : 不会增加新的小动物
                    //if (currentCageIndexDic.ContainsKey(currentCheckPointIndex))
                    //{
                    //    if (currentAnimalsCount > 0)
                    //    {
                    //        AddNewAnimals();

                    //    }
                    //}
                    tempMoveGroupList.Clear();
                    currentTurnPos.Clear();
                    currentAddToMoveListIndex = -1;
                    ///在每次移动结束后强制修改下标,保证animalsObj下顺序与currentAnimalsList顺序一致
                    for (int i = 0; i < currentAnimalsList.Count; i++)
                    {
                        animalsObj.transform.SetSiblingIndex(i);
                    }
                    //更新下一次移动的起点和终点
                    ///需要提前检查一下当前是不是到达终点了
                    ///以免在设置移动终点的时候出现越限
                    currentStartPos = currentEndPos;
                    currentEndPos = checkPos[++currentTargetCheckPosIndex];
                    ///如果下一个检查点的位置是终点
                    if (currentEndPos == gameEndPos)
                    {
                        readyToEndGame = true;
                        Debug.Log("下一个是终点");
                    }
                    //更新摄像机位置
                    CameraController.GetInstance().MoveCameraToTarget(currentStartPos);
                    float firstZ = Vector3.Distance(animalsObj.transform.GetChild(0).position, gameStartPos);
                    EventManager.Trigger<float>(UICompentEventName.ScheduleSlider, (firstZ / Vector3.Distance(gameStartPos, gameEndPos)));
                    isBusy = true;
                    int count = 0;
                    ///小动物转向
                    for (int i = 0; i < animalsObj.transform.childCount; i++)
                    {
                        var tween = Utility.TurnBack(animalsObj.transform.GetChild(i), 0);
                        tween.OnComplete(() =>
                        {
                            count++;
                            if (count == animalsObj.transform.childCount - 1)
                                isBusy = false;
                        });
                    }
                }
            }
        }

        private void GameEndFail()
        {
            Debug.Log("游戏结束!!!!!!!!!!!!!!!!!!!!!!!!!!");
            isGameRunning = false;
            GameEndDispose();
            EventManager.Trigger(ToolEventName.LittleGameEndFail);
        }

        bool isFirst = true;

        /// <summary>
        /// 用来记录当前关卡已经死亡的小动物,避免重复删除
        /// </summary>
        private Dictionary<string, Transform> deadAnimalsDic;

        /// <summary>
        /// 小动物被撞了之后的处理
        /// </summary>
        private void AnimalsBeHit()
        {
            ///检查当前被撞的是不是最后一个小动物
            ///检查当前被撞的小动物是否已经被撞过了


            int hitNum = 0;
            //currentAnimalsCount -= animalBeHitList.Count;



            ///处理的时候需注意  有可能会有多个车子撞到同一个小动物,需要判空处理
            ///先临时将被撞到的小动物隐藏起来,等待游戏结束之后统一处理
            ///接下来需要写的  更新当前的移动队列 将被撞到的移除
            for (int i = 0; i < animalBeHitList.Count; i++)
            {
                var animal = animalBeHitList[i].GetComponent<Animals>();
                if (deadAnimalsDic.ContainsKey(animal.name))
                {
                    continue;
                }
                deadAnimalsDic.Add(animal.animalsName, animal.transform);
                hitNum++;
                ///被撞的小动物需要从移动队列中删除
                if (moveList.Contains(animalBeHitList[i].transform))
                {
                    moveList.Remove(animalBeHitList[i].transform);
                }
                animalBeHitList[i].SetActive(false);
            }

            currentAnimalsCount -= hitNum;
            currentAddToMoveListIndex -= hitNum;

            if (currentAnimalsCount <= 0)
            {
                GameEndFail();
            }

            ///更新小动物ID
            UpdateAnimalsID();

            ///重新计算自己的检查点
            for (int i = 0; i < moveList.Count; i++)
            {
                MoveUtility moveUtility = moveList[i].GetComponent<MoveUtility>();
                moveUtility.endPos = CalculatorEndPos(moveList[i].GetComponent<Animals>().id);
            }

            ///清理碰撞list
            animalBeHitList.Clear();
            isAnimalBeHit = false;
        }
        /// <summary>
        /// 判断是否所有的小动物都移动完成了
        /// </summary>
        bool isAllMoveDone = false;
        /// <summary>
        /// 记录真实移动完成的小动物的下标
        /// </summary>
        int realyMoveListIndex;
        /// <summary>
        /// 用来保存正在移动中的小动物队列
        /// </summary>
        public List<Transform> moveList;

        bool needCheckIsHasSpace;

        public Dictionary<Transform, Vector3> currentTurnPos = new Dictionary<Transform, Vector3>();

        /// <summary>
        /// 开始单个点击移动
        /// </summary>
        private void StartMove()
        {
            if (isLongTouch)
            {
                return;
            }

            //Debug.Log("开始单个点击移动");
            //判断如果当前还有需要移动的小动物
            if (currentAnimalsList.Count > 0)
            {
                Animals animals = currentAnimalsList[0].GetComponent<Animals>();
                if (animals.isBusy)
                {
                    Debug.Log("当前小动物正在忙");
                    return;
                }
                animals.isBusy = true;
                isCurrentTouchMoveDown = false;
                //计算当前移动一次移动的终点
                //通知开始移动
                //Debug.Log("判断如果当前还有需要移动的小动物" + currentAddToMoveListIndex);
                currentAddToMoveListIndex++;
                MoveUtility moveUtil = currentAnimalsList[0].GetComponent<MoveUtility>();
                //Debug.Log("当前需要移动的小动物" + currentAnimalsList[0].GetComponent<Animals>().id);
                //var endpos = CalculatorEndPos(currentAddToMoveListIndex);
                animals.isBusy = true;
                var endpos = CalculatorEndPos(animals.id);
                ///增加一步  检查获取的位置是否正确 如果不对则重新获取
                if (endpos.z <= animals.transform.position.z)
                {
                    Debug.Log("坐标获取失败 重新获取");
                    endpos = CalculatorEndPos(animals.id);
                }
                moveUtil.StartMove(littleGameInit.GetAnimalsSpeed(), endpos, MoveCallback, Vector3.forward);
                moveList.Add(currentAnimalsList[0]);
                currentTurnPos.Add(currentAnimalsList[0], endpos);
                //移除第0个
                currentAnimalsList.RemoveAt(0);
                UpdateLastAnimalsPos();
                isCurrentTouchMoveDown = true;
            }
        }

        /// <summary>
        /// 开始自动连续移动
        /// </summary>
        /// <returns></returns>
        //IEnumerator StartLongMove()
        //{
        //    isLongTouch = true;

        //    while (true)
        //    {
        //        Debug.Log("开始连续移动");
        //        //判断如果当前还有需要移动的小动物
        //        if (currentAnimalsList.Count > 0)
        //        {
        //            //计算当前移动一次移动的终点
        //            //通知开始移动
        //            //Debug.Log("判断如果当前还有需要移动的小动物" + currentAddToMoveListIndex);
        //            currentAddToMoveListIndex++;
        //            MoveUtility moveUtil = currentAnimalsList[0].GetComponent<MoveUtility>();
        //            //Debug.Log("当前需要移动的小动物" + currentAnimalsList[0].GetComponent<Animals>().id);
        //            //var endpos = CalculatorEndPos(currentAddToMoveListIndex);
        //            var endpos = CalculatorEndPos(currentAnimalsList[0].GetComponent<Animals>().id);
        //            //增加一步  检查获取的位置是否正确 如果不对则重新获取

        //            moveUtil.StartMove(littleGameInit.GetAnimalsSpeed(), endpos, MoveCallback, Vector3.forward);
        //            moveList.Add(currentAnimalsList[0]);
        //            currentTurnPos.Add(currentAnimalsList[0], endpos);
        //            //移除第0个
        //            currentAnimalsList.RemoveAt(0);
        //            UpdateLastAnimalsPos();
        //        }
        //        yield return new WaitForSeconds(touchStartMoveTime);
        //    }
        //}

        private void StartLongMove()
        {
            //Debug.Log("开始连续移动");
            
            //timer += Time.deltaTime;
            //if (timer >= touchStartMoveTime)
            //{
            //    isLongTouch = true;
            //    timer = 0;
            //    return;
            //}

            //判断如果当前还有需要移动的小动物
            if (currentAnimalsList.Count > 0)
            {
                Animals animals = currentAnimalsList[0].GetComponent<Animals>();
                if (animals.isBusy)
                {
                    Debug.Log("当前小动物正在忙");
                    return;
                }
                animals.isBusy = true;
                //计算当前移动一次移动的终点
                //通知开始移动
                //Debug.Log("判断如果当前还有需要移动的小动物" + currentAddToMoveListIndex);
                currentAddToMoveListIndex++;
                MoveUtility moveUtil = currentAnimalsList[0].GetComponent<MoveUtility>();
                //Debug.Log("当前需要移动的小动物" + currentAnimalsList[0].GetComponent<Animals>().id);
                //var endpos = CalculatorEndPos(currentAddToMoveListIndex);
                var endpos = CalculatorEndPos(currentAnimalsList[0].GetComponent<Animals>().id);
                //增加一步  检查获取的位置是否正确 如果不对则重新获取
                moveUtil.StartMove(littleGameInit.GetAnimalsSpeed(), endpos, MoveCallback, Vector3.forward);
                moveList.Add(currentAnimalsList[0]);
                currentTurnPos.Add(currentAnimalsList[0], endpos);
                //移除第0个
                currentAnimalsList.RemoveAt(0);
            }
        }

        private void MoveCallback(GameObject res)
        {
            ///检查自己的前面有没有被撞的小动物,如果有的话 需要将空位删掉
            ///如果自己与临时list中的最后一位距离 小于= 间距则不管 否则继续移动新的检查点
            MoveUtility tempMoveUtility = res.GetComponent<MoveUtility>();
            Animals animals = res.GetComponent<Animals>();
            //    Debug.Log("不需要补位");
            tempMoveGroupList.Add(res);
            animals.isBusy = false;
            if (currentAddToMoveListIndex == currentAnimalsCount - 1)
            {
                int count = 0;
                //isBusy = true;
                ///全部移动完成之后 转向前方
                for (int i = 0; i < animalsObj.transform.childCount; i++)
                {
                    var tween = Utility.TurnBack(animalsObj.transform.GetChild(i),0);
                    tween.OnComplete(() =>
                    {
                        count++;
                        if (count == animalsObj.transform.childCount -1)
                            isBusy = false;
                    });
                }
                isAllMoveDone = true;
            }
            else
            {
                ///自己转向后方
                //isBusy = true;
                var tweener = Utility.TurnBack(res.transform,180);
                tweener.OnComplete(() =>
                {
                    isBusy = false;
                });
            }
            moveList.Remove(res.transform);
        }

        /// <summary>
        /// 第一个移动之后更新小动物的位置
        /// </summary>
        private void UpdateLastAnimalsPos()
        {
            ///如果当前正在长按状态下,就不需要维护后续小动物的位置了
            //if (isLongTouch)
            //{
            //    //Debug.Log("当前处于长按状态");
            //    return;
            //}
            List<Transform> tempList = new List<Transform>();
            tempList.AddRange(currentAnimalsList);
            ///移动后续的小动物到第一个位置
            int count = 0;
            for (int i = 0; i < tempList.Count; i++)
            {
                MoveUtility move = tempList[i].GetComponent<MoveUtility>();
                Animals animals = tempList[i].GetComponent<Animals>();
                if (animals.isBusy)
                    continue;

                ///如果当前是第一个检查点,那么后续小动物的终点就是起点的位置
                Vector3 pos;
                if (currentCheckPointIndex == 0)
                {
                    pos = gameStartPos - LittleGameDefine.AnimalWidth * i;
                }
                else
                {
                    int index = (currentCheckPointIndex - 1) <= 0 ? 0 : (currentCheckPointIndex - 1);
                    pos = checkPos[index] - LittleGameDefine.AnimalWidth * i;
                    ///如果当前位置有笼子的话 还需要增加一个笼子的宽度
                    ///修改 : 当前不会产生小动物了
                    //if (currentCageIndexDic.ContainsKey(currentCheckPointIndex))
                    //{
                    //    //Debug.Log("当前位置有笼子" +pos);
                    //    pos += new Vector3(0, 0, LittleGameDefine.CageWidth);
                    //    //Debug.Log("当前位置有笼子(修改后位置)" + pos);
                    //}
                }
                //Debug.Log("当前的目标是第" + currentCheckPointIndex);
                if (pos.z <= move.transform.position.z)
                {
                    Debug.Log("坐标获取失败 重新获取");
                    pos = CalculatorEndPos(i);
                }
                count++;
                move.StartMove(littleGameInit.GetAnimalsSpeed(), pos, (res) => 
                {
                    if (count == tempList.Count)
                    {
                        isBusy = false;
                    }
                },Vector3.forward);
            }
        }

        /// <summary>
        /// 添加一个新的小动物到队列的第一位
        /// </summary>
        private void AddNewAnimals()
        {
            Vector3 cagePos = Vector3.zero;

            Transform animalsTra = null;

            ///隐藏笼子的模型
            if (currentCageIndexDic.ContainsKey(currentCheckPointIndex))
            {
                animalsTra = currentCageIndexDic[currentCheckPointIndex].currentFbs.transform.GetChild(0);
                cagePos = currentCageIndexDic[currentCheckPointIndex].currentFbs.transform.position;
                ///笼子飞起来
                Transform cageTrans = currentCageIndexDic[currentCheckPointIndex].currentFbs.transform;
                MoveUtility move = currentCageIndexDic[currentCheckPointIndex].currentFbs.GetComponent<MoveUtility>();
                move.StartMove(littleGameInit.GetCageFlySpeed(), (cageTrans.transform.position + Vector3.up * 20), (res) =>
                {
                    res.gameObject.SetActive(false);
                },Vector3.up);
                //currentCageIndexDic[currentCheckPointIndex].currentFbs.SetActive(false);
                //currentCageIndexDic.Remove(currentCheckPointIndex);
            }

            currentAnimalsCount++;
            Transform pfb = animalsTra;//ResourceData.Instance.GetAnimal().transform;
            pfb.gameObject.SetActive(true);
            pfb.transform.SetParent(animalsObj.transform);
            pfb.transform.SetAsFirstSibling();
            var animal = pfb.gameObject.AddComponent<Animals>();
            animal.animalsName = "animals" + Time.realtimeSinceStartup;
            animal.name = animal.animalsName;
            pfb.gameObject.AddComponent<MoveUtility>();
            List<Transform> newList = new List<Transform>();
            newList.Add(pfb);
            pfb.position = cagePos;
            ///记录之前的小动物位置
            List<Vector3> pos = new List<Vector3>();
            //for (int i = 0; i < animalsObj.transform.childCount; i++)
            //{
            //    if(animalsObj.activeInHierarchy)
            //        pos.Add(animalsObj.transform.GetChild(i).position);
            //}
            for (int i = 0; i < tempMoveGroupList.Count; i++)
            {
                pos.Add(tempMoveGroupList[i].transform.position);
            }
            int index = 0;
            for (int i = 1; i < currentAnimalsCount; i++)
            {
                newList.Add(currentAnimalsList[index]);
                newList[i].position = pos[index];
                index++;
            }
            currentAnimalsList.Clear();
            currentAnimalsList.AddRange(newList);
            //Debug.Log("当前可用的数量是"+currentAnimalsList.Count);
            UpdateAnimalsID();
        }

        /// <summary>
        /// 更新当前列表中的小动物标志位
        /// 用于更新小动物的位置
        /// </summary>
        private void UpdateAnimalsID()
        {
            int count = 0;
            for (int i = 0; i < animalsObj.transform.childCount; i++)
            {
                var obj = animalsObj.transform.GetChild(i);
                if (obj.gameObject.activeInHierarchy)
                {
                    obj.GetComponent<Animals>().id = count;
                    count++;
                }
                else
                {
                    obj.GetComponent<Animals>().id = -1;
                }
            }
        }

        /// <summary>
        /// 计算当前移动终点位置
        /// 当前的终点位置 = 当前的检查点位置 - 每个小动物的宽度 * 当前小动物的位置下标
        /// </summary>
        /// <param name="index">当前移动的小动物下标</param>
        public Vector3 CalculatorEndPos(int index)
        {
            Vector3 pos = Vector3.zero;
            // && index < currentAnimalsCount
            if (index >= 0)
            {
                pos = currentEndPos - LittleGameDefine.AnimalWidth * (index);
            }
            //Debug.Log(index + "本次计算的终点位置:" + pos + "当前小动物队列的长度是:" + currentAnimalsCount);
            return pos;
        }

        #endregion

        #region End Game

        /// <summary>
        /// 游戏结束后的处理
        /// </summary>
        private void GameEndDispose()
        {
            //清理上一关的数据
            littleGameInit.ClearLastLevelRes();
            //清理临时移动队列
            //littleGameInit.ClearList(tempMoveGroupList);
            //清理小动物数据
            ClearAnimal();
            ///停止上一关的车子
            if (currentLevelcarPosDic != null)
                StopLastLevelCar();
            //停止小动物移动
            StopCoroutine("StartMove");

            currentAddToMoveListIndex = -1;

            ///清理上一关的终点位置
            if(resObj.transform.Find("destinationObj"))
                Destroy(resObj.transform.Find("destinationObj").gameObject);

            isAnimalBeHit = false;
            currentTurnPos.Clear();
            deadAnimalsDic.Clear();

            ///保存当前关卡产生数据
            ///记录当前可以使用小动物数量
            ///这个记录的地方不对  应该是只会在胜利的状态下才回记录
            //LittleGameData.LastLevelAnimalsNumber = currentAnimalsCount;

            ///清理当前关卡的小动物资源
            Utility.DestoryAllChild(animalsObj.transform);

            ///清理上一关的道路
            //Utility.DestoryAllChild(resObj.)

            ///删除上一关的所有装饰物
			Utility.DestoryAllChild(ornamentalObj.transform);
            ///执行到这个地方才算清理完成 
            ///这个时候算是完成了下一关准备工作的一般,等待资源准备完成之后才算是全部准备完成

        }
        /// <summary>
        /// 清理游戏过程中产生的临时小动物移动数据
        /// </summary>
        private void ClearAnimal()
        {
            Utility.ClearList(currentAnimalsList);
            Utility.ClearList(tempMoveGroupList);
        }

        #endregion

        #region Init Level Datas

        /// <summary>
        /// 准备下一关需要的信息
        /// </summary>
        /// <param name="level">需要准备的关卡ID,如果为空 则是当前关卡的下一关</param>
        public void InitGameData(int level)
        {
            Debug.Log("初始化游戏");

            
            currentLevelcarPosDic = new Dictionary<int, CarTargetPosition>();
            deadAnimalsDic = new Dictionary<string, Transform>();
            currentLevelcarPosDicIndex = -1;
            checkPos = new List<Vector3>();
            animalBeHitList = new List<GameObject>();
            currentCageIndexDic = new Dictionary<int, Cage>();
            moveList = new List<Transform>();
            currentCageIndex = 0;
            createCageNum = 0;
            gameEndPos = Vector3.zero;
            isTouch = false;
            isLongTouch = false;
            isBusy = false;

            ///从表中读取需要的信息
            ///初始化关卡信息
          
                CurrentLevel = (int)level;
            ///currentLevel++;
            ///初始化小动物数量
            LittleGameData.LastLevelAnimalsNumber = littleGameInit.GetCurrentLevelAnimalsNum(CurrentLevel);

            //初始化小动物
            InitAnimals();
            //初始化笼子数量
            littleGameInit.InitLevelCage();
            //初始化马路
            littleGameInit.InitLevelRoad();
            //初始化车子
            littleGameInit.InitLevelCar();

            //随机笼子产生的位置
            //如果当前小动物的数量超过了25就不刷新了
            ///需求修改 : 不会增加新的小动物
            //if (LittleGameData.LastLevelAnimalsNumber < LittleGameDefine.MaxAnimalsNum)
            //{
            //    //更新当前可以刷新的小动物的数量
            //    currentLevelCageNum = (LittleGameDefine.MaxAnimalsNum - LittleGameData.LastLevelAnimalsNumber) >= littleGameInit.GetCageNum() ? 
            //                            littleGameInit.GetCageNum() : (LittleGameDefine.MaxAnimalsNum - LittleGameData.LastLevelAnimalsNumber);
            //    RandomCage();
            //}

            //初始化场景 模型 贴图 树木栽种位置
            Transform scene = littleGameInit.GetSceneTerrain(CurrentLevel).transform;
            BoxCollider collider;
            if (scene.GetChild(0).gameObject.GetComponent<BoxCollider>() == null)
            {
                collider = scene.GetChild(0).gameObject.AddComponent<BoxCollider>();
            }
            else
            {
                collider = scene.GetChild(0).gameObject.GetComponent<BoxCollider>();
            }
            scene.SetParent(resObj.transform);
            scene.name = "scene";
            scene.position = new Vector3(0, -4.5f, collider.size.z / 2);


            //计算马路的位置
            CalculatorRoadPos();

            //计算检查点的位置
            CalculatorCheckPos();

            //初始化其他数据
            currentStartPos = gameStartPos;
            currentTargetCheckPosIndex = 0;
            currentEndPos = checkPos[currentTargetCheckPosIndex];
            currentArriveCheckPosInedx = 0;
            readyToEndGame = false;
            currentCheckPointIndex = 0;

            //isGameRunning = true;

            ////初始化需要的车子的信息
            //InitAndMoveCarData();

            ///初始化摄像机的位置
            CameraController.GetInstance().SetCameraToDefaultPos();

            Debug.Log("当前是第" + CurrentLevel + "关");

            ///开始让车子移动
            InitAndMoveCarData();

            EventManager.Trigger<int>(UICompentEventName.LevelText, level);
        }

        /// <summary>
        /// 初始化场景信息
        /// </summary>
        private void InitScene()
        {

        }

        /// <summary>
        /// 停止上一关的车子
        /// </summary>
        private void StopLastLevelCar()
        {
            for (int i = 0; i < currentLevelcarPosDic.Count; i++)
            {
                StopCoroutine("StartMoveCar");
            }
        }

        /// <summary>
        /// 开始让车子移动
        /// </summary>
        private void InitAndMoveCarData()
        {
            for (int i = 0; i < currentLevelcarPosDic.Count; i++)
            {
                //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //cube.transform.position = currentLevelcarPosDic[i].startPos;
                StartCoroutine("StartMoveCar", i);
            }
            Invoke("DelayledExecute", 1f);
            
        }

        private void DelayledExecute()
        {
            EventManager.Trigger(ToolEventName.IsInitGameDone);
        }

        IEnumerator StartMoveCar(int index)
        {
            while (true)
            {

                //等待刷新时间移动
                yield return new WaitForSeconds(littleGameInit.GetRandomCarFlushInterval());

                //if (!isGameRunning)
                //    yield break;

                if (!currentLevelcarPosDic.ContainsKey(index))
                {
                    yield break;
                }

                //等待时间到了  开始一个新的车子移动
                GameObject carPfb = littleGameInit.GetCar();
                //添加移动工具类
                MoveUtility moveUtility;
                if (carPfb.GetComponent<MoveUtility>() == null)
                {
                    moveUtility = carPfb.AddComponent<MoveUtility>();
                }
                else
                {
                    moveUtility = carPfb.GetComponent<MoveUtility>();
                }
                if (carPfb.GetComponent<CollideUtility>() == null)
                {
                    //CollideUtility collide = carPfb.AddComponent<CollideUtility>();
                    //collide.checkTagName = "Player";
                }

                ///调整车子的转向
                if (currentLevelcarPosDic[index].dir == -1)
                {
                    carPfb.transform.eulerAngles = new Vector3(0, 90, 0);
                }
                else
                {
                    carPfb.transform.eulerAngles = new Vector3(0, -90, 0);
                }
                //重新初始化开始位置
                carPfb.transform.position = currentLevelcarPosDic[index].startPos;
                carPfb.transform.SetParent(carObj.transform);
                carPfb.gameObject.SetActive(true);
                moveUtility.isEnd = false;
                //开始移动
                moveUtility.StartMove(littleGameInit.GetRandomCarSpeed() * 0.02f, currentLevelcarPosDic[index].endPos, (res) =>
                    {
                        littleGameInit.SaveCar(res);
                    }, carPfb.transform.forward);
            }
        }

        /// <summary>
        /// 临时保存所有的检查点标志
        /// </summary>
        Transform checkPointObj;

        /// <summary>
        /// 计算检查点的位置
        /// </summary>
        private void CalculatorCheckPos()
        {
            if (checkPointObj.childCount != 0)
            {
                Utility.DestoryAllChild(checkPointObj);
            }
            for (int i = 1; i < littleGameInit.currentLevelRood.Count; i++)
            {
                Road road = littleGameInit.currentLevelRood[i].GetComponent<Road>();
                float dis = LittleGameDefine.RoadWidth * (float)(road.needCarNum / 1.5f);
                //if (road.needCarNum == 1)
                //{
                //    dis += LittleGameDefine.RoadWidth;
                //}
                dis += LittleGameDefine.AnimalWidth.z / 2;
                Vector3 pos = road.RoadPos - new Vector3(0, 0, dis);
                ///如果当前位置有笼子的话 还需要减去笼子的宽度
                ///需求修改 : 不会增加新的小动物
                //if (currentCageIndexDic.ContainsKey(i))
                //{
                //    pos = pos - new Vector3(0, 0, LittleGameDefine.CageWidth);
                //}
                checkPos.Add(pos);
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.SetParent(checkPointObj);
                cube.transform.position = pos;
            }
            ///添加终点
            //Debug.Log("终点的位置 : " + gameEndPos);
            checkPos.Add(gameEndPos);
        }

        /// <summary>
        /// 计算马路的位置
        /// </summary>
        private void CalculatorRoadPos()
        {
            //第一条马路的位置在出生点
            Road road01 = littleGameInit.currentLevelRood[0].GetComponent<Road>();
            road01.transform.SetParent(roadObj.transform);
            //初始的位置加一点 加出小动物出生的位置
            int offset = Utility.CheckCurrentRoadIndex(road01) - 1;
            offset = Mathf.Clamp(offset, 1, LittleGameDefine.RoomNumber);
            float zDis = LittleGameDefine.RoadWidth * offset;
            //if (offset == 2)
            //zDis += LittleGameDefine.AnimalWidth.z / 2;
            road01.RoadPos = gameStartPos + new Vector3(0, 0, zDis);
            littleGameInit.currentLevelRood[0].SetActive(true);
            ///计算车子的出生点
            CalculatorCarPos(road01);
            //当前关卡的剩余笼子数量
            int surplusCageNum = currentLevelCageNum;
            int count = 0;
            //计算其他马路的初始点
            for (int i = 1; i <= currentLevelRoadNum; i++)
            {
                count = i - 1;
                ///计算距离
                //上一条马路
                Road lastRoad = littleGameInit.currentLevelRood[i - 1].GetComponent<Road>();
                //上一条马路的碰撞盒
                BoxCollider lastRoadCollider = littleGameInit.currentLevelRood[i - 1].GetComponent<BoxCollider>();

                BoxCollider currentRoadCollider = littleGameInit.currentLevelRood[count].GetComponent<BoxCollider>();

                float currnetNeedSize = currentRoadCollider.size.z;
                var currendRoad = littleGameInit.currentLevelRood[count].GetComponent<Road>();
                if (currendRoad.needCarNum != 1)
                {
                    if (currendRoad.needCarNum != 2)
                        currnetNeedSize *= 0.5f;
                }
                else
                {
                    currnetNeedSize *= 2f;
                }



                ///两条马路之间距离的计算公式
                ///初始位置,+ 上一条马路的宽度 + 小动物的宽度 + 笼子的宽度(笼子的宽度加不加得看当前有没有) + 当前马路宽度的一般
                ///如果之前有笼子的话 下面的马路上还需要增加笼子的宽度
                float distance = lastRoad.RoadPos.z  + currentAnimalsCount * LittleGameDefine.AnimalWidth.z + currnetNeedSize + lastRoadCollider.size.z / GetOffset(lastRoad.needCarNum);// + LittleGameDefine.CageWidth;
                                                                                                                                                        //随机下当前这条马路旁有没有笼子
                                                                                                                                                        //                                                                                                  //RandomHasCage(ref surplusCageNum);

                //Debug.Log("上一条马路的位置:" + lastRoad.RoadPos);

                ///检查之前的位置有没有笼子
                ///需求修改 : 不会增加新的小动物
                //distance += LittleGameDefine.CageWidth * createCageNum;

                ///如果当前的位置有笼子
                ///需求修改 : 不会增加新的小动物
                //if (currentCageIndexDic.ContainsKey(i))
                //{
                //    GameObject cage = littleGameInit.currentLevelCage[currentCageIndex++];
                //    cage.SetActive(true);
                //    cage.transform.position = new Vector3(gameStartPos.x, gameStartPos.y, distance + LittleGameDefine.CageWidth /2);
                //    Transform anim = ResourceData.Instance.GetAnimal().transform;
                //    anim.SetParent(cage.transform,false);
                //    anim.gameObject.SetActive(true);
                //    anim.localPosition = Vector3.zero;
                //    distance += LittleGameDefine.CageWidth;
                //    currentCageIndexDic[i].currentFbs = cage;
                //    createCageNum++;
                //    cage.AddComponent<MoveUtility>();
                //}
                ///为距离增加偏移量
                //distance += (LittleGameDefine.AnimalWidth.z / 2);
                Vector3 pos = new Vector3(gameStartPos.x, gameStartPos.y, distance);
                ///计算马路的位置
                if (i < currentLevelRoadNum)
                {
                    Road road = littleGameInit.currentLevelRood[i].GetComponent<Road>();
                    road.RoadPos = pos;
                    road.transform.SetParent(roadObj.transform);
                    littleGameInit.currentLevelRood[i].SetActive(true);

                    ///计算车子的出生点
                    CalculatorCarPos(road);
                    //计算小摆件的位置
                    if (i < currentLevelRoadNum -1)
                    {
                        CalCulatorOrnamentalPos(pos, (road.transform.position.z - lastRoad.transform.position.z));
                    }
                }
                //计算终点的位置
                else
                {
                    gameEndPos = pos + new Vector3(0,0,1) * lastRoadCollider.size.z;
                    ///显示终点的模型×
                    ///初始化终点的位置
                    GameObject destinationObj = ResourceData.Instance.GetDestination(0);
                    destinationObj.transform.SetParent(resObj.transform);
                    destinationObj.name = "destinationObj";
                    //destinationObj.transform.localScale = new Vector3(10, 1, 10);
                    destinationObj.transform.position = gameEndPos;
                    destinationObj.SetActive(true);
                }
            }
        }

        /// <summary>
        /// 根据不同的马路获得偏移量
        /// </summary>
        /// <returns></returns>
        private float GetOffset(int number)
        {
            float offsetNum = 0f;

            ///根据不同的马路修改不同的偏移量
            switch (number)
            {
                case 1:
                    offsetNum = 1.5f;
                    break;
                case 2:
                    offsetNum = 1.5f;
                    break;
                case 3:
                    offsetNum = 1.5f;
                    break;
                case 4:
                    offsetNum = 1.8f;
                    break;
                case 5:
                    offsetNum = 2f;
                    break;
                case 6:
                    offsetNum = 2;
                    break;
            }
            return offsetNum;
        }

        /// <summary>
        /// 计算装饰物的位置
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="offset"></param>
        private void CalCulatorOrnamentalPos(Vector3 pos,float offset)
        {
            for (int i = 0; i < 2; i++)
            {
                ///随机要不要生成装饰物
                int ran = Random.Range(0, LittleGameDefine.OrnamentalNum);
                GameObject ornamental = null;
                switch (ran)
                {
                    ///不生成装饰物
                    case 0:

                        break;
                    case 1:
                    case 2:
                    case 3:
                        ornamental = littleGameInit.GetOrnamental(ran);
                        break;
                }
                if (ornamental != null)
                {
                    BoxCollider collider;
                    if (ornamental.GetComponent<BoxCollider>() == null)
                    {
                        collider = ornamental.AddComponent<BoxCollider>();
                    }
                    else
                    {
                        collider = ornamental.GetComponent<BoxCollider>();
                    }
                    Vector3 newPos;
                    if (i == 1)
                    {
                        ///在马路左边生成
                        newPos = new Vector3(pos.x - littleGameInit.GetSceneRoadWidth() - collider.size.x / 2, pos.y, pos.z + offset / 2);
                    }
                    else
                    {
                        ///在马路右边生成
                        newPos = new Vector3(pos.x + littleGameInit.GetSceneRoadWidth() + collider.size.x / 2, pos.y, pos.z + offset / 2);
                    }
                    ornamental.transform.SetParent(ornamentalObj.transform);
                    ornamental.transform.position = newPos;
                    ornamental.SetActive(true);
                }
            }
        }

        /// <summary>
        /// 计算车子的出生点
        /// </summary>
        private void CalculatorCarPos(Road road)
        {

            Transform leftOrigin = road.transform.Find("CarStartPos/LeftPos");
            Transform rightOrigin = road.transform.Find("CarStartPos/RightPos");
            for (int j = 0; j < road.needCarNum; j++)
            {
                int dir = Random.Range(0, 10) > 5 ? -1 : 1;
                CarTargetPosition carTarget = new CarTargetPosition();
                Vector3 carStartPos;
                Vector3 carEndPos;
                currentLevelcarPosDicIndex++;
                if (dir == -1)
                {
                    carStartPos = leftOrigin.position + Vector3.forward * j * LittleGameDefine.RoadWidth;
                    carEndPos = rightOrigin.position + Vector3.forward * j * LittleGameDefine.RoadWidth;

                }
                else
                {
                    carStartPos = rightOrigin.position + Vector3.forward * j * LittleGameDefine.RoadWidth;
                    carEndPos = leftOrigin.position + Vector3.forward * j * LittleGameDefine.RoadWidth;
                }
                carTarget.startPos = carStartPos;
                carTarget.endPos = carEndPos;
                carTarget.dir = dir;
                currentLevelcarPosDic.Add(currentLevelcarPosDicIndex, carTarget);
            }
        }

        /// <summary>
        /// 随机生成笼子
        /// </summary>
        /// <param name="num"></param>
        private void RandomCage()
        {
            ///需要考虑的问题 如果前几条马路都没有随机出笼子 那么后面的马路就必须出现笼子了 这个怎么把控呢
            ///这个东西需要好好想想怎么实现 
            ///现在还没有具体的实现方式

            ///根据马路的数量随机3个数字  数字的位置就是笼子的位置  
            for (int i = 0; i < currentLevelCageNum; i++)
            {
                while (true)
                {
                    int num = Utility.GetARandomIntNumber(currentLevelRoadNum);
                    if (!currentCageIndexDic.ContainsKey(num))
                    {
                        currentCageIndexDic.Add(num, new Cage());
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 初始化当前关卡需要小动物信息
        /// </summary>
        private void InitAnimals()
        {
            currentAnimalsList = new List<Transform>();
            if (animalsObj.transform.childCount != 0)
            {
                Utility.DestoryAllChild(animalsObj.transform);
            }
            for (int i = 0; i < LittleGameData.LastLevelAnimalsNumber; i++)
            {
                Transform pfb = ResourceData.Instance.GetAnimal().transform;
                pfb.gameObject.SetActive(true);
                pfb.transform.SetParent(animalsObj.transform);
                var animal = pfb.gameObject.AddComponent<Animals>();
                animal.id = i;
                animal.animalsName = "animals" + Time.realtimeSinceStartup;
                animal.name = animal.animalsName;
                pfb.gameObject.AddComponent<MoveUtility>();
                currentAnimalsList.Add(pfb);
                pfb.localPosition = LittleGameDefine.AnimalWidth * -i;
                if(pfb.GetComponent<BoxCollider>() == null)
                {
                    var collider = pfb.gameObject.AddComponent<BoxCollider>();
                    collider.isTrigger = true;
                }
                if (pfb.GetComponent<Rigidbody>() == null)
                {
                   var rig = pfb.gameObject.AddComponent<Rigidbody>();
                    rig.useGravity = false;
                }
                pfb.tag = "Player";
            }
            currentAnimalsCount = currentAnimalsList.Count;
        }

        #endregion

        #region EventManager CallBack

        /// <summary>
        /// 当车子撞到小动物时回调
        /// </summary>
        /// <param name="res"></param>
        private void OnCarHitAnimals(GameObject res)
        {
            isAnimalBeHit = true;
            res.GetComponent<MoveUtility>().needCheckIsHasSpace = true;
            needCheckIsHasSpace = true;
            animalBeHitList.Add(res);
        }

        /// <summary>
        /// UI界面通知主界面游戏开始
        /// </summary>
        private void OnGameStart(int hardLevel)
        {
            isGameRunning = true;
            InitGameData(hardLevel);
        }

        #endregion
    }
}