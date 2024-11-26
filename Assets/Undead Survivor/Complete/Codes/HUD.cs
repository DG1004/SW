using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Goldmetal.UndeadSurvivor
{
    public class HUD : MonoBehaviour
    {
        public enum InfoType { CurCoin, Kill, Time, Health }
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
                case InfoType.CurCoin:
                    myText.text = string.Format(":{0:F0}", GameManager.instance.Coin);
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
            }
        }
    }
}
