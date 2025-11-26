using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MusicMgr : MonoBehaviour
{
    [SerializeField,Tooltip("背景音乐播放器")]
    private AudioSource bgAudioSource;
    //[SerializeField,Tooltip("音效播放器预设体")]
    //private GameObject effectAudioPlayPrefab;
    [SerializeField,Tooltip("音效播放器依附对象根节点")]
    private Transform audioPlayRoot;
    //[SerializeField,Tooltip("音效播放器对象池默认数量")]
    //private int effectAudioDefaultQuantity = 20;
    //音效播放器组件列表
    private List<AudioSource> audioPlayList = new();
    //音效是否暂停
    private bool isEffectPause = false;

    //全局音量大小
    [SerializeField, Range(0, 1)]
    private float globalVolume = 1.0f;
    //背景音乐大小
    [SerializeField, Range(0, 1)]
    private float bgVolumeBase = 0.1f;
    [SerializeField, Range(0, 1)]
    //音效大小
    private float effectVolumeBase = 0.1f;
    public MusicMgr()
    {
        //MonoMgr.Instance.AddFixedUpdateListener(Update);
    }
    public void Init()
    {
        //创建音效播放器依附对象根节点
        if (audioPlayRoot == null)
        {
            GameObject audioPlayRootObj = new GameObject("AudioPlayRoot");
            audioPlayRoot = audioPlayRootObj.transform;
        }
    }
    #region Update遍历回收
    private void Update()
    {
        //如果音效暂停 则直接返回（不回收）
        if (isEffectPause) return;
        //不停的遍历容器 检测有没有音效播放完成 
        //播放完成的音效组件销毁掉（不再销毁，改为放回对象池）
        //为了避免遍历时删除元素导致的问题 采用倒序遍历
        for (int i = audioPlayList.Count - 1; i >= 0; --i)
        {
            if (!audioPlayList[i].isPlaying)
            {
                //GameObject.Destroy(soundEffectList[i]);
                //将音效切片置空 并放回对象池
                audioPlayList[i].clip = null;
                PoolManager.Instance.PushGameObject(audioPlayList[i].gameObject);
                audioPlayList.RemoveAt(i);
            }
        }
    }
    #endregion

    #region 音乐相关
    //播放背景音乐
    public void PlayBackgroundMusic(string musicName)
    {
        //动态创建背景音乐播放组件 且 不随场景切换而销毁
        //保证背景音乐场景切换时依然可以播放
        if (bgAudioSource == null)
        {
            GameObject backgroundMusicPlayer = new GameObject("backgroundMusicPlayer");
            GameObject.DontDestroyOnLoad(backgroundMusicPlayer); // 确保音乐播放器不会在场景切换时被销毁
            bgAudioSource = backgroundMusicPlayer.AddComponent<AudioSource>();
        }
        Debug.Log("Playing music: " + musicName);
        // 实际的播放逻辑可以在这里实现
        AddressableMgr.Instance.LoadAssetAsync<AudioClip>(musicName, (obj) =>
        {
            bgAudioSource.clip = obj.Result;
            bgAudioSource.loop = true; // 设置为循环播放
            bgAudioSource.volume = bgVolumeBase * globalVolume; // 设置音量大小
            bgAudioSource.Play();
        });
    }
    //停止背景音乐
    public void StopBackgroundMusic()
    {
        if (bgAudioSource == null) return;
        bgAudioSource.Stop();

    }
    //暂停背景音乐
    public void PauseBackgroundMusic()
    {
        if (bgAudioSource == null) return;
        bgAudioSource.Pause(); 
    }
    //设置背景音乐大小
    public void SetBackgroundMusicVolume(float volume)
    {
        bgVolumeBase = volume;
        if (bgAudioSource == null) return;
        bgAudioSource.volume = bgVolumeBase * globalVolume;
    }
    #endregion

    #region 音效相关
    /// <summary>
    /// 获取音效播放器
    /// </summary>
    /// <param name="is3D"></param>
    /// <returns></returns>
    private AudioSource GetAudioPlay()
    {
        // 从对象池中获取播放器
        GameObject audioPlay = PoolManager.Instance.GetGameObject("AudioPlay", audioPlayRoot);
        //获取不到就创建一个新的
        if (audioPlay == null)
        {
            audioPlay = new GameObject("AudioPlay");
            audioPlay.AddComponent<AudioSource>();
            audioPlay.transform.parent = audioPlayRoot;
        }
        AudioSource audioSource = audioPlay.GetComponent<AudioSource>();
        return audioSource;
    }


    /// <summary>
    /// 播放指定音效(通过Addressable异步加载)
    /// </summary>
    /// <param name="soundName">音效名</param>
    /// <param name="isLoop">是否循环播放</param>
    /// <param name="isSync">是否同步加载</param>
    /// <param name="callback">加载结束后的回调函数</param>
    public void PlaySoundEffect(string soundName, bool isLoop = false, bool is3D = false, UnityAction<AudioSource> callback = null)
    {
        //异步加载音效资源并播放
        AddressableMgr.Instance.LoadAssetAsync<AudioClip>(soundName, (obj) =>
        {
            AudioSource source = GetAudioPlay();
            source.clip = obj.Result;
            source.loop = isLoop;
            source.volume = effectVolumeBase * globalVolume;
            source.spatialBlend = is3D ? 1f : 0f;
            source.Play();
            //记录播放的音效组件 以便后续管理
            audioPlayList.Add(source);
            //传递给外部使用(循环音效由外部停止播放)
            callback?.Invoke(source);
        });
    }
    // 停止播放指定的音效
    public void StopSoundEffect(AudioSource source)
    {
        if (audioPlayList.Contains(source))
        {
            //停止播放
            source.Stop();
            //从列表移除
            audioPlayList.Remove(source);
            //GameObject.Destroy(source);
            //将音效切片置空 并放回对象池
            source.clip = null;
            PoolManager.Instance.PushGameObject(source.gameObject);
        }
        //TODO ：可以加一个标识，isPool，表示是否放回对象池 不放回则销毁
    }
    
    /// <summary>
    /// 设置所有音效的音量大小
    /// </summary>
    /// <param name="volume">音量大小</param>
    public void SetAllSoundEffectsVolume(float volume)
    {
        effectVolumeBase = volume;
        //遍历所有音效组件 设置音量
        foreach (var source in audioPlayList)
        {
            source.volume = effectVolumeBase * globalVolume;
        }
    }
    /// <summary>
    /// 恢复播放所有音效
    /// </summary>
    public void PlayAllSoundEffects()
    {
        isEffectPause = false;
        //播放所有音效
        foreach (var source in audioPlayList)
        {
            if (!source.isPlaying) source.Play();
        }
    }
    /// <summary>
    /// 暂停所有音效
    /// </summary>
    public void PauseAllSoundEffects()
    {
        isEffectPause = true;
        //暂停所有音效
        foreach (var source in audioPlayList)
        {
            if (source.isPlaying) source.Pause();
        }
    }
    //清空音效相关记录 //!在清空缓存池前调用
    public void ClearSoundEffects()
    {
        //停止所有音效播放 并销毁组件
        foreach (var source in audioPlayList)
        {
            source.Stop();
            //将音效切片置空 并放回对象池
            source.clip = null;
            PoolManager.Instance.PushGameObject(source.gameObject);
        }
        //清空列表
        audioPlayList.Clear();
    }
    #endregion

    #region 全局音量相关
    //设置全局音量大小
    public void SetGlobalVolume(float volume)
    {
        globalVolume = volume;
        //更新背景音乐音量
        if (bgAudioSource != null)
        {
            bgAudioSource.volume = bgVolumeBase * globalVolume;
        }
        //更新所有音效音量
        foreach (var source in audioPlayList)
        {
            source.volume = effectVolumeBase * globalVolume;
        }
    }
    #endregion
}
