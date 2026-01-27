using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ZooGame.GlobalData;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PromptText
{
    public static PromptText Instance()
    {
        if (instance == null)
        {
            instance = new PromptText();
//            instance.Init();
        }
        return instance;
    }

    private static PromptText instance;
    //
    //    private List<string> TipsList = new List<string>();
    //    private float LastTipsTime = 0;
    //    private List<PromptTextItem> TipItems = new List<PromptTextItem>();
    //    private int index = 0;
    //    private Vector2 EndPos = new Vector2(0,250);
    private Color textColor;
    //    private float speed = 0;
    //    private readonly float moveTime = 4;
    //    private readonly float colorTime = 2;
    //    private Dictionary<string,float> intervalTime = new Dictionary<string, float>();
    //    private string lastTextString = "";
    //    private readonly float TipsInterval = 0.7f;
    //
    //    private void Init()
    //    {
    //        Observable.EveryUpdate().Subscribe(Update).AddTo(UIRoot.Instance.promptRoot);
    //        speed = EndPos.y / moveTime;
    //    }
    //
    //    void Update(long aa)
    //    {
    //        if (TipsList.Count > 0 && Time.time - LastTipsTime > (TipsInterval + (intervalTime.ContainsKey(lastTextString) ? intervalTime[lastTextString] : 0)))
    //        {
    //            StartCreateText();
    //        }
    //    }
    //
    //    private void StartCreateText()
    //    {
    //        LastTipsTime = Time.time;
    //        string langeKey = TipsList[0];
    //        TipsList.RemoveAt(0);
    //        lastTextString = langeKey;
    //        if (TipItems.Count < 7)
    //        {
    //            var obj = Resources.Load("UIPrefab/TitleText", typeof(GameObject)) as GameObject;
    //            var go = GameObject.Instantiate<GameObject>(obj, UIRoot.Instance.promptRoot) as GameObject;
    //            go.transform.localScale = Vector3.one;
    //            var rect = go.transform as RectTransform;
    //            Text text = go.GetComponentInChildren<Text>();
    //            textColor = text.color;
    //            text.SetText(langeKey);
    //            rect.anchoredPosition = Vector2.zero;
    //            Tweener tweener = go.transform.DOLocalMove(EndPos, moveTime).SetEase(Ease.OutSine);
    //            AddIntervalTime(text);
    //            text.color = Color.clear;
    //            Observable.NextFrame().Subscribe(a =>
    //            {
    //                text.color = textColor;
    //                Tweener tweenerColor = text.DOFade(0, colorTime).SetEase(Ease.InQuart);
    //                PromptTextItem promptTextItem = new PromptTextItem(rect, tweener, text, tweenerColor);
    //                TipItems.Add(promptTextItem);
    //            });
    //        }
    //        else
    //        {
    //            TipItems[index].tw.Kill(true);
    //            TipItems[index].twColor.Kill(true);
    //            TipItems[index].rectTransform.anchoredPosition = Vector2.zero;
    //            TipItems[index].rectTransform.gameObject.SetActive(true);
    //            TipItems[index].text.SetText(langeKey);
    //            TipItems[index].text.color = textColor;
    //            TipItems[index].tw = TipItems[index].rectTransform.DOLocalMove(EndPos, moveTime).SetEase(Ease.OutSine);
    //            TipItems[index].twColor = TipItems[index].text.DOFade(0, colorTime).SetEase(Ease.InQuart);
    //            index = index >= TipItems.Count - 1 ? 0 : index + 1;
    //            AddIntervalTime(TipItems[index].text);
    //        }
    //    }
    //
    //    private void AddIntervalTime(Text text)
    //    {
    //        float temp = 0;
    //        if (text.preferredHeight > text.fontSize * 2 && !intervalTime.ContainsKey(text.text))
    //        {
    //            Observable.NextFrame().Subscribe(a =>
    //            {
    //                temp = text.GetComponent<RectTransform>().sizeDelta.y / speed - TipsInterval;
    //                if (!intervalTime.ContainsKey(text.text) && temp > 0)
    //                    intervalTime.Add(text.text, temp);
    //            });
    //        }
    //    }
    //
    //
    //    public static  void CreatePromptText(string langeKey)
    //    {
    //        if (string.IsNullOrEmpty(langeKey))
    //            return;
    //        int num = 0;
    //        for (int i = 0; i < Instance().TipsList.Count; i++)
    //        {
    //            if (Instance().TipsList[i].Equals(langeKey))
    //            {
    //                num++;
    //            }
    //            if(num == 2)
    //                return;
    //        }
    //        Instance().TipsList.Add(langeKey);
    //
    //    }
    public static void CreatePromptText(string langeKey)
    {
        Instance().CreateText(langeKey);
    }

    public static void CreatePromptTextPure(string langeKey)
    {
        Instance().CreateTextPure(langeKey);
    }

    private readonly float ColorTime = 0.1f;
//    1 字的透明度从0-255  耗时0.1s
//    2 字的位置从初始位置上移150像素，sineout 耗时1s
//    3 停留在该位置1s
//    4 字的位置再次上移100像素，sinein 耗时0.5s
//    5 字的透明度从255-0  耗时0.1s
    private void CreateText(string langeKey)
    {
        var obj = Resources.Load("uiprefab/TitleText", typeof(GameObject)) as GameObject;
        var go = GameObject.Instantiate<GameObject>(obj, UIRoot.Instance.promptRoot) as GameObject;
        go.transform.localScale = Vector3.one;
        var rect = go.transform as RectTransform;
        Text text = go.GetComponentInChildren<Text>();
        textColor = text.color;
        //text.SetText(langeKey);
        text.text = GlobalDataManager.GetInstance().i18n.GetL10N(langeKey);
        //text.text = "AD_Fail";
        rect.anchoredPosition = Vector2.zero;
        text.color = new Color(textColor.r, textColor.g, textColor.b, 0);
        //1 字的透明度从0-255  耗时0.1s
        text.DOFade(1, ColorTime).SetEase(Ease.Linear).OnComplete(() =>
        {
            //            2 字的位置从初始位置上移150像素，sineout 耗时1s
            rect.DOLocalMove(rect.localPosition + new Vector3(0, 150, 0), 1).SetEase(Ease.OutSine).OnComplete(() =>
                  {
                    //3 停留在该位置1s
                    Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ =>
                      {
                        //    4 字的位置再次上移100像素，sinein 耗时0.5s
                        rect.DOLocalMove(rect.localPosition + new Vector3(0, 100, 0), 0.5f).SetEase(Ease.InSine)
                              .OnComplete(() =>
                              {
                                //    5 字的透明度从255-0  耗时0.1s
                                text.DOFade(0, ColorTime).SetEase(Ease.Linear).OnComplete(() =>
                                  {
                                      GameObject.Destroy(go);
                                  });
                              });
                      });
                  });
        });
    }

    private void CreateTextPure(string langeKey)
    {
        var obj = Resources.Load("uiprefab/TitleText", typeof(GameObject)) as GameObject;
        var go = GameObject.Instantiate<GameObject>(obj, UIRoot.Instance.promptRoot) as GameObject;
        go.transform.localScale = Vector3.one;
        var rect = go.transform as RectTransform;
        Text text = go.GetComponentInChildren<Text>();
        textColor = text.color;
        //text.SetText(langeKey);
        text.text = langeKey;
        //text.text = "AD_Fail";
        rect.anchoredPosition = Vector2.zero;
        text.color = new Color(textColor.r, textColor.g, textColor.b, 0);
        //1 字的透明度从0-255  耗时0.1s
        text.DOFade(1, ColorTime).SetEase(Ease.Linear).OnComplete(() => {
            //            2 字的位置从初始位置上移150像素，sineout 耗时1s
            rect.DOLocalMove(rect.localPosition + new Vector3(0, 150, 0), 1).SetEase(Ease.OutSine).OnComplete(() =>
            {
                //3 停留在该位置1s
                Observable.Timer(TimeSpan.FromSeconds(1)).Subscribe(_ =>
                {
                    //    4 字的位置再次上移100像素，sinein 耗时0.5s
                    rect.DOLocalMove(rect.localPosition + new Vector3(0, 100, 0), 0.5f).SetEase(Ease.InSine)
                          .OnComplete(() => {
                                  //    5 字的透明度从255-0  耗时0.1s
                                  text.DOFade(0, ColorTime).SetEase(Ease.Linear).OnComplete(() => {
                                  GameObject.Destroy(go);
                              });
                          });
                });
            });
        });
    }


}

public class PromptTextItem
{
    public RectTransform rectTransform;
    public Tween tw;
    public Tween twColor;
    public Text text;

    public PromptTextItem(RectTransform obj,Tween tween,Text t,Tween tweencolor)
    {
        rectTransform = obj;
        tw = tween;
        text = t;
        twColor = tweencolor;
    }
}
