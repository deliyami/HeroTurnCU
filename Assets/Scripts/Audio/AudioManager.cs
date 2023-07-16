using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] List<AudioData> sfxList;
    [SerializeField] AudioSource musicPlayer;
    [SerializeField] AudioSource sfxPlayer;
    [SerializeField] float fadeDuration = 0.75f;
    AudioClip currMusic;
    float originalMusicVol;
    Dictionary<AudioId, AudioData> sfxLookup;
    public static AudioManager i { get; private set; }
    private void Awake()
    {
        i = this;
    }
    private void Start()
    {
        originalMusicVol = musicPlayer.volume;

        sfxLookup = sfxList.ToDictionary(x => x.id);
    }
    public void PlaySfx(AudioClip clip, bool pauseMusic = false)
    {
        if (clip == null) return;
        if (pauseMusic)
        {
            musicPlayer.Pause();
            StartCoroutine(UnPauseMusic(clip.length));
        }
        sfxPlayer.PlayOneShot(clip);
    }
    public void PlaySfx(AudioId audioId, bool pauseMusic = false)
    {
        if (!sfxLookup.ContainsKey(audioId)) return;
        var audioData = sfxLookup[audioId];
        PlaySfx(audioData.clip, pauseMusic);
    }
    public void PlayMusic(AudioClip clip, bool loop = true, bool fade = false)
    {
        if (clip == null || clip == currMusic) return;
        currMusic = clip;
        StartCoroutine(PlayMusicAsync(clip, loop, fade));
    }
    public void PlayMusic(AudioId audioId, bool loop = true, bool fade = false)
    {
        if (!sfxLookup.ContainsKey(audioId)) return;
        var audioData = sfxLookup[audioId];
        StartCoroutine(PlayMusicAsync(audioData.clip, loop, fade));
    }
    IEnumerator PlayMusicAsync(AudioClip clip, bool loop, bool fade)
    {
        if (fade)
            yield return musicPlayer.DOFade(0, fadeDuration).WaitForCompletion();

        musicPlayer.clip = clip;
        musicPlayer.loop = loop;
        musicPlayer.Play();

        if (fade)
            yield return musicPlayer.DOFade(originalMusicVol, fadeDuration).WaitForCompletion();
    }
    IEnumerator UnPauseMusic(float delay)
    {
        yield return new WaitForSeconds(delay);

        musicPlayer.volume = 0;
        musicPlayer.UnPause();
        musicPlayer.DOFade(originalMusicVol, delay);
    }
}

public enum AudioId { UISelect, Hit, Faint, ExpGain, ItemObtained, UnitObtained, Intro, Slice, Slice2 }

[System.Serializable]
public class AudioData
{
    public AudioId id;
    public AudioClip clip;
}