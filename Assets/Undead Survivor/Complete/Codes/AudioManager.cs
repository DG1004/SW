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
        public Slider bgmSlider; // BGM �����̴�
        public Slider sfxSlider; // SFX �����̴�

        public enum Sfx { Dead, Hit, LevelUp = 3, Lose, Melee, Range = 7, Select, Win, Coin }

        void Awake()
        {
            instance = this;
            Init();
        }

        void Init()
        {
            // ����� �÷��̾� �ʱ�ȭ
            GameObject bgmObject = new GameObject("BgmPlayer");
            bgmObject.transform.parent = transform;
            bgmPlayer = bgmObject.AddComponent<AudioSource>();
            bgmPlayer.playOnAwake = false;
            bgmPlayer.loop = true;
            bgmPlayer.volume = bgmVolume;
            bgmPlayer.clip = bgmClip;
            bgmEffect = Camera.main.GetComponent<AudioHighPassFilter>();

            // ȿ���� �÷��̾� �ʱ�ȭ
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

            // �����̴� �ʱ�ȭ
            if (bgmSlider != null)
            {
                bgmSlider.value = bgmVolume; // �����̴� �� ����ȭ
                bgmSlider.onValueChanged.AddListener(SetBgmVolume); // �� ���� �� �޼��� ����
            }
            if (sfxSlider != null)
            {
                sfxSlider.value = sfxVolume; // �����̴� �� ����ȭ
                sfxSlider.onValueChanged.AddListener(SetSfxVolume); // �� ���� �� �޼��� ����
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
            bgmPlayer.volume = bgmVolume; // BGM �÷��̾� ���� ������Ʈ
        }

        public void SetSfxVolume(float volume)
        {
            sfxVolume = volume;
            foreach (var player in sfxPlayers)
            {
                player.volume = sfxVolume; // ��� SFX �÷��̾� ���� ������Ʈ
            }
        }
    }
}
