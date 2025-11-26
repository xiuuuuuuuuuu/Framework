using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WindowBase : WindowBehaviour
{
    private List<Button> mAllButtonList = new();//所有Button列表
    private List<Toggle> mAllToggleList = new();//所有Toggle列表
    private List<InputField> mAllInputList = new();//所有输入框列表

    private CanvasGroup mUIMask;
    protected Transform mUIContent;

    // 初始化基础组件
    private void InitBaseComponent()
    {
        mUIMask = transform.Find("UIMask").GetComponent<CanvasGroup>();
        mUIContent = transform.Find("UIContent").transform;
    }

    #region 生命周期
    public override void OnAwake()
    {
        base.OnAwake();
        InitBaseComponent();
    }
    public override void OnShow()
    {
        base.OnShow();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
    }
    public override void OnHide()
    {
        base.OnHide();
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        RemoveAllButtonListener();
        RemoveAllToggleListener();
        RemoveAllInputFieldListener();
        mAllButtonList.Clear();
        mAllToggleList.Clear();
        mAllInputList.Clear();
    }
    #endregion

    public override void SetVisible(bool isVisible)
    {
        gameObject.SetActive(isVisible);//临时代码
        Visible = isVisible;
    }

    #region UI事件注册与移除
    public void AddButtonClickListener(Button btn,UnityAction action)
    {
        if (btn != null)
        {
            if (!mAllButtonList.Contains(btn))
            {
                mAllButtonList.Add(btn);
            }
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(action);
        }
    }

    public void AddToggleClickListener(Toggle toggle,UnityAction<bool, Toggle> action)
    {
        if (toggle != null)
        {
            if (!mAllToggleList.Contains(toggle))
            {
                mAllToggleList.Add(toggle);
            }
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener((isOn) => { action?.Invoke(isOn, toggle); });
        }
    }
    
    public void AddInputFieldListener(InputField input,UnityAction<string> onChangeAction,UnityAction<string> endAction)
    {
        if (input != null)
        {
            if (!mAllInputList.Contains(input))
            {
                mAllInputList.Add(input);
            }
            input.onValueChanged.RemoveAllListeners();
            input.onEndEdit.RemoveAllListeners();
            input.onValueChanged.AddListener(onChangeAction);
            input.onEndEdit.AddListener(endAction);
        }
    }

    public void RemoveAllButtonListener()
    {
        foreach (var btn in mAllButtonList)
        {
            btn.onClick.RemoveAllListeners();
        }
    }
    public void RemoveAllToggleListener()
    {
        foreach (var toggle in mAllToggleList)
        {
            toggle.onValueChanged.RemoveAllListeners();
        }
    }
    public void RemoveAllInputFieldListener()
    {
        foreach (var input in mAllInputList)
        {
            input.onValueChanged.RemoveAllListeners();
            input.onEndEdit.RemoveAllListeners();
        }
    }
    #endregion

    public void SetMaskVisible(bool isVisible)
    {
        if (!UISetting.Instance.SINGMASK_SYSTEM) return;
        mUIMask.alpha = isVisible ? 1 : 0;
    }
}
