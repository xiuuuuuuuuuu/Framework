using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManagerTest : MonoBehaviour
{
    public float v;
    public float oldv;
    //测试外部获取音效播放器
    public AudioSource audioSource;
    public MusicMgr musicMgr;
    public void Start()
    {
        PoolManager.Instance.Init();
    }
    void OnGUI()
    {
        #region 背景音乐测试
        /* if (GUILayout.Button("Play Music"))
        {
            MusicMgr.Instance.PlayBackgroundMusic("冬天下雪");
        }
        if (GUILayout.Button("Play Music2"))
        {
            MusicMgr.Instance.PlayBackgroundMusic("鸡叫");
        }
        if (GUILayout.Button("停止音乐"))
        {
            MusicMgr.Instance.StopBackgroundMusic();
        }
        if (GUILayout.Button("暂停音乐"))
        {
            MusicMgr.Instance.PauseBackgroundMusic();
        }
        v = GUILayout.HorizontalSlider(v, 0.0f, 1.0f, GUILayout.Width(200));
        if (oldv != v)
        {
            oldv = v;
            MusicMgr.Instance.SetBackgroundMusicVolume(v);
            MusicMgr.Instance.SetSoundEffectVolume(v);
        }
        #endregion
        #region 音效测试
        if (GUILayout.Button("播放一声吱吱"))
        {
            MusicMgr.Instance.PlaySoundEffect("吱吱");
        }
        if (GUILayout.Button("Play SoundEffect"))
        {
            MusicMgr.Instance.PlaySoundEffect("吱吱", true, false, (source) =>
            {
                audioSource = source;
            });
        }
        if (GUILayout.Button("停止音效"))
        {
            MusicMgr.Instance.StopSoundEffect(audioSource);
            audioSource = null;
        }
        if (GUILayout.Button("暂停所有音效"))
        {
            MusicMgr.Instance.PlayOrPauseAllSoundEffects(false);
        }
        if (GUILayout.Button("播放所有音效"))
        {
            MusicMgr.Instance.PlayOrPauseAllSoundEffects(true);
        } */
        #endregion
        if (GUILayout.Button("初始化音乐管理器"))
        {
            //musicMgr.Init();
            AudioManager.Instance.Init();
        }
        if (GUILayout.Button("播放一次吱吱"))
        {
            //musicMgr.PlaySoundEffect("吱吱");
            AudioManager.Instance.PlaySoundEffect("吱吱");
        }
        if (GUILayout.Button("播放一次冬天下雪"))
        {
            //musicMgr.PlayBackgroundMusic("冬天下雪");
            AudioManager.Instance.PlayBackgroundMusic("冬天下雪");
        }
        if (GUILayout.Button("播放一次鸡叫"))
        {
            //musicMgr.PlayBackgroundMusic("鸡叫");
            AudioManager.Instance.PlayBackgroundMusic("鸡叫");
        }
        if (GUILayout.Button("播放一次升级"))
        {
            //musicMgr.PlaySoundEffect("升级");
            AudioManager.Instance.PlaySoundEffect("升级");
        }
    }
}
