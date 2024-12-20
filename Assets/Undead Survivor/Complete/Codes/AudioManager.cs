using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Goldmetal.UndeadSurvivor
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;

        [Header("#BGM")]
        public AudioClip bgmClip;
        public float bgmVolume = 0.1f;
        AudioSource bgmPlayer;
        AudioHighPassFilter bgmEffect;

        [Header("#SFX")]
        public AudioClip[] sfxClips;
        public float sfxVolume = 0.1f;
        public int channels;
        AudioSource[] sfxPlayers;
        int channelIndex;

        [Header("#UI Sliders")]
        public Slider bgmSlider; // BGM 슬라이더
        public Slider sfxSlider; // SFX 슬라이더

        public enum Sfx { Dead, Hit, LevelUp = 3, Lose, Melee, Range = 7, Select, Win, Coin }

        void Awake()
        {
            instance = this;
            Init();
        }

        void Init()
        {
            // 배경음 플레이어 초기화
            GameObject bgmObject = new GameObject("BgmPlayer");
            bgmObject.transform.parent = transform;
            bgmPlayer = bgmObject.AddComponent<AudioSource>();
            bgmPlayer.playOnAwake = false;
            bgmPlayer.loop = true;
            bgmPlayer.volume = bgmVolume;
            bgmPlayer.clip = bgmClip;
            bgmEffect = Camera.main.GetComponent<AudioHighPassFilter>();

            // 효과음 플레이어 초기화
            GameObject sfxObject = new GameObject("SfxPlayer");
            sfxObject.transform.parent = transform;
            sfxPlayers = new AudioSource[channels];

            for (int index = 0; index < sfxPlayers.Length; index++) {
                sfxPlayers[index] = sfxObject.AddComponent<AudioSource>();
                sfxPlayers[index].playOnAwake = false;
                sfxPlayers[index].bypassListenerEffects = true;
                sfxPlayers[index].volume = sfxVolume;

                if (index == sfxPlayers.Length - 1)
                {
                    sfxPlayers[index].volume = sfxVolume / 8;
                }
            }

            // 슬라이더 초기화
            if (bgmSlider != null)
            {
                bgmSlider.value = bgmVolume; // 슬라이더 값 동기화
                bgmSlider.onValueChanged.AddListener(SetBgmVolume); // 값 변경 시 메서드 연결
            }
            if (sfxSlider != null)
            {
                sfxSlider.value = sfxVolume; // 슬라이더 값 동기화
                sfxSlider.onValueChanged.AddListener(SetSfxVolume); // 값 변경 시 메서드 연결
            }
        }

        public void PlayBgm(bool isPlay)
        {
            if (isPlay) {
                bgmPlayer.Play();
            }
            else {
                bgmPlayer.Stop();
            }
        }

        public void EffectBgm(bool isPlay)
        {
            bgmEffect.enabled = isPlay;
        }

        public void PlaySfx(Sfx sfx)
        {
            for (int index = 0; index < sfxPlayers.Length; index++) {
                int loopIndex = (index + channelIndex) % sfxPlayers.Length;

                if (sfxPlayers[loopIndex].isPlaying)
                    continue;

                int ranIndex = 0;
                if (sfx == Sfx.Hit || sfx == Sfx.Melee) {
                    ranIndex = Random.Range(0, 2);
                }

                channelIndex = loopIndex;
                sfxPlayers[loopIndex].clip = sfxClips[(int)sfx + ranIndex];
                sfxPlayers[loopIndex].Play();
                break;
            }
        }

        public void SetBgmVolume(float volume)
        {
            bgmVolume = volume;
            bgmPlayer.volume = bgmVolume; // BGM 플레이어 볼륨 업데이트
        }

        public void SetSfxVolume(float volume)
        {
            sfxVolume = volume;
            foreach (var player in sfxPlayers)
            {
                player.volume = sfxVolume; // 모든 SFX 플레이어 볼륨 업데이트
            }
        }
    }
}
