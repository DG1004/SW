using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Goldmetal.UndeadSurvivor
{
    public class HUD : MonoBehaviour
    {
        public enum InfoType { Coin, Kill, Time, Health , Mana}
        public InfoType type;

        Text myText;
        Slider mySlider;

        void Awake()
        {
            myText = GetComponent<Text>();
            mySlider = GetComponent<Slider>();
        }

        void LateUpdate()
        {
            switch (type) {
                case InfoType.Coin:
                    myText.text = string.Format(":{0:F0}", CoinManager.playerCoins);
                    break;
                case InfoType.Kill:
                    myText.text = string.Format("{0:F0}", GameManager.instance.kill);
                    break;
                case InfoType.Time:
                    float remainTime = GameManager.instance.maxGameTime - GameManager.instance.gameTime;
                    int min = Mathf.FloorToInt(remainTime / 60);
                    int sec = Mathf.FloorToInt(remainTime % 60);
                    myText.text = string.Format("{0:D2}:{1:D2}", min, sec);
                    break;
                case InfoType.Health:
                    float curHealth = GameManager.instance.health;
                    float maxHealth = GameManager.instance.maxHealth;
                    mySlider.value = curHealth / maxHealth;
                    break;
                case InfoType.Mana:
                    float curMana = ManaManager.playerManas;
                    float maxMana = ManaManager.maxManas;
                    mySlider.value = curMana / maxMana;
                    break;
            }
        }
    }
}
