using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecuredSpace.Settings;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using SecuredSpace.UnityExtend.Timer;
using SecuredSpace.UnityExtend;
using SecuredSpace.ClientControl.DBResources;

namespace SecuredSpace.Common.Audio
{
    //[System.Serializable]
    //public class AudioFragment
    //{
    //    public AudioClip audioClip;
    //    private double startTime = -1;
    //    public double StartTime
    //    {
    //        get
    //        {
    //            if (startTime == -1)
    //                return 0;
    //            else
    //                return startTime;
    //        }
    //        set
    //        {
    //            startTime = value;
    //        }
    //    }
    //    [SerializeField]
    //    private double EditorStartTime = -1;
    //    public double GetEditorStartTime => EditorEndTime;
    //    private double endTime = -1;
    //    public double EndTime
    //    {
    //        get
    //        {
    //            if (endTime == -1)
    //                return 0;
    //            else
    //                return endTime;
    //        }
    //        set
    //        {
    //            endTime = value;
    //        }
    //    }
    //    [SerializeField]
    //    private double EditorEndTime = -1;
    //    public double GetEditorEndTime => EditorEndTime;
    //    public bool loop = false;
    //    [Range(0.0f, 1.0f)]
    //    public float volume;
    //    public AnimationCurve fadingCurve = AnimationCurve.Linear(1, 1, 1, 1);

    //    public AudioFragment() { }
    //    public AudioFragment(AudioClipPartData audioClipPartData) {

    //    }
    //}

    //public class IAudio : MonoBehaviour
    //{
    //    private List<AudioSource> AudioSources = new List<AudioSource>();

    //    #region caching

    //    public int countOfCacheAudioSource = 3;
    //    public List<AudioSource> audioSources = new List<AudioSource>();

    //    public void Start()
    //    {
    //        //FillCacheAudioSources();
    //    }

    //    public void FillCacheAudioSources()
    //    {
    //        audioSources = new List<AudioSource>(this.GetComponents<AudioSource>());
    //        if (audioSources.Count < countOfCacheAudioSource)
    //            if (audioSources.Count > 0)
    //                while (audioSources.Count < countOfCacheAudioSource)
    //                    audioSources.Add(audioSources[0].CopyComponent(this.gameObject));
    //            else
    //                while (audioSources.Count < countOfCacheAudioSource)
    //                    audioSources.Add(this.gameObject.AddComponent<AudioSource>());
    //    }

    //    #endregion

    //    #region outControl

    //    public float time { get; set; }
    //    public AudioMixerGroup outputAudioMixerGroup { get; set; }
    //    public bool isPlaying { get; }
    //    public bool isVirtual { get; }
    //    public bool loop { get; set; }
    //    public bool ignoreListenerVolume { get; set; }
    //    public bool playOnAwake { get; set; }
    //    public bool ignoreListenerPause { get; set; }
    //    public AudioVelocityUpdateMode velocityUpdateMode { get; set; }
    //    public float panStereo { get; set; }
    //    public float spatialBlend { get; set; }
    //    public bool spatialize { get; set; }
    //    public bool spatializePostEffects { get; set; }
    //    public float reverbZoneMix { get; set; }
    //    public bool bypassEffects { get; set; }
    //    public bool bypassListenerEffects { get; set; }
    //    public bool bypassReverbZones { get; set; }
    //    public float dopplerLevel { get; set; }
    //    public float spread { get; set; }
    //    public int priority { get; set; }
    //    public bool mute { get; set; }
    //    public float minDistance { get; set; }
    //    public float maxDistance { get; set; }
    //    public AudioRolloffMode rolloffMode { get; set; }
    //    [Obsolete("minVolume is not supported anymore. Use min-, maxDistance and rolloffMode instead.", true)]
    //    public float minVolume { get; set; }
    //    public AudioClip clip { get; set; }
    //    public int timeSamples { get; set; }
    //    [Obsolete("rolloffFactor is not supported anymore. Use min-, maxDistance and rolloffMode instead.", true)]
    //    public float rolloffFactor { get; set; }
    //    public float pitch { get; set; }
    //    public float volume { get; set; }
    //    public float pan { get; set; }
    //    public float panLevel { get; set; }
    //    [Obsolete("maxVolume is not supported anymore. Use min-, maxDistance and rolloffMode instead.", true)]
    //    public float maxVolume { get; set; }

    //    #endregion

    //    public List<AudioFragment> soundBus = new List<AudioFragment>();
    //    [SerializeField]
    //    private int playingFragment = 0;
    //    public int PlayingFragment
    //    {
    //        get
    //        {
    //            return playingFragment;
    //        }
    //        set
    //        {
    //            if (value == soundBus.Count)
    //            {
    //                Stop(true);
    //            }
    //            else
    //            {
    //                playingFragment = value;
    //                GotoFragment(playingFragment);
    //            }
    //        }
    //    }
    //    private UnityTimer fragmentTimer;

        

        

    //    public void NextFragment()
    //    {

    //    }

    //    public void PreviousFragment()
    //    {

    //    }

    //    public void GotoFragment(int number)
    //    {

    //    }

    //    public void AppendFragment(AudioFragment fragment)
    //    {

    //    }

    //    public void Play()
    //    {

    //    }

    //    public void Play(params AudioFragment[] fragments)
    //    {
    //        Stop();
    //        soundBus = fragments.ToList();
    //        PlayingFragment++;
    //    }

    //    public void Pause()
    //    {
    //        fragmentTimer.Pause();
    //        audioSources[PlayingFragment].Pause();
    //    }

    //    public void Resume()
    //    {
    //        fragmentTimer.Pause();
    //        audioSources[PlayingFragment].UnPause();
    //    }

    //    public void Stop(bool fullStop = false)
    //    {
    //        audioSources.ForEach(x =>
    //        {
    //            x.mute = true;
    //            var _testTimer = UnityTimer.Register(
    //            duration: Time.deltaTime,
    //            onComplete: () =>
    //            {
    //                if (fullStop)
    //                    x.Stop();
    //                x.mute = false;
    //            },
    //            onUpdate: new List<System.Action<float>>(){
    //                secondsElapsed =>
    //                {
    //                    //UpdateText.text = string.Format("Timer ran update callback: {0:F2} seconds", secondsElapsed);
    //                }
    //            },
    //            isLooped: false,
    //            useRealTime: false);
    //        });
    //        soundBus.Clear();
    //    }
    //}
}
