﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using UnibusEvent;

namespace DestroyViruses
{
    public class MainView : ViewBase
    {
        // drag listener
        public UIEventListener inputListenser;

        // panels
        public LevelPanel levelPanel;
        public RadioObjects[] optionBtnRaido;
        public RadioObjects optionPanelRaido;

        public Button baseOptionBtn;
        public Button aircraftOptionBtn;
        public Button coinOptionBtn;

        // top menu
        public Text coinText;
        public Text energyText;
        public Text diamondText;
        public Button settingBtn;

        // game level
        public DOTweenTrigger gameLevelTweenTrigger;
        public Text previousLevelText;
        public Text currentLevelText;
        public Text nextLevelText;
        public Button previousLevelBtn;
        public Button currentLevelBtn;
        public Button nextLevelBtn;
        public GameObject previousBossTag;
        public GameObject currentBossTag;
        public GameObject nextBossTag;

        // private
        private Vector2 mTotalDrag = Vector2.zero;
        private float mDragBeginThreshold = 100;


        private void Awake()
        {
            InputListenerInit();
            ButtonListenerInit();
            SetMusic();
        }

        private void SetMusic()
        {
            AudioManager.Instance.MusicPlayer.mute = !Option.music;
            AudioManager.Instance.FireMusicPlayer.mute = !Option.sound;
            AudioManager.Instance.SoundPlayer.mute = !Option.sound;
        }


        private void OnEnable()
        {
            this.BindUntilDisable<EventGameData>(OnEventGameData);
        }

        protected override void OnOpen()
        {
            SelectOption(-1);
            mTotalDrag = Vector2.zero;
            UIUtil.aircraftTransform.DOScale(Vector3.one * 1.3f, 0.5f).SetEase(Ease.OutQuad);
            RefreshUI();
            AudioManager.Instance.PlayMusic($"Sounds/BGM{Random.Range(1, 3)}", 1f);
        }

        private void OnDestroy()
        {
            inputListenser.onDrag.RemoveAllListeners();
            inputListenser.onClick.RemoveAllListeners();
        }

        private void InputListenerInit()
        {
            inputListenser.onDrag.AddListener((data) =>
            {
                mTotalDrag += data;
                if (mTotalDrag.magnitude > mDragBeginThreshold)
                {
                    GameManager.ChangeState<BattleState>();
                }
            });

            inputListenser.onClick.AddListener(_ =>
            {
                SelectOption(-1);
            });
        }

        private void ButtonListenerInit()
        {
            baseOptionBtn.OnClick(() => { SelectOption(0); });
            aircraftOptionBtn.OnClick(() => { SelectOption(1); });
            coinOptionBtn.OnClick(() => { SelectOption(2); });
            settingBtn.OnClick(() => { UIManager.Instance.Open<OptionView>(UILayer.Top); });

            previousLevelBtn.OnClick(() =>
            {
                GDM.ins.SelectGameLevel(GDM.ins.gameLevel - 1);
                gameLevelTweenTrigger.DoTrigger();
            });

            currentLevelBtn.OnClick(() =>
            {
                gameLevelTweenTrigger.DoTrigger();
            });

            nextLevelBtn.OnClick(() =>
            {
                GDM.ins.SelectGameLevel(GDM.ins.gameLevel + 1);
                gameLevelTweenTrigger.DoTrigger();
            });
        }

        private void SelectOption(int optionIndex)
        {
            for (int i = 0; i < optionBtnRaido.Length; i++)
            {
                optionBtnRaido[i].Radio(i == optionIndex);
            }
            optionPanelRaido.Radio(optionIndex);
        }

        private void RefreshUI()
        {
            coinText.text = GDM.ins.coin.KMB();
            energyText.text = "100";
            diamondText.text = "0";

            if (GDM.ins.gameLevel - 1 <= 0)
            {
                previousLevelText.text = "-";
                previousBossTag.SetActive(false);
            }
            else
            {
                previousLevelText.text = (GDM.ins.gameLevel - 1).ToString();
                previousBossTag.SetActive(TableGameLevel.Get(GDM.ins.gameLevel - 1).isBoss);
            }

            currentLevelText.text = GDM.ins.gameLevel.ToString();
            currentBossTag.SetActive(TableGameLevel.Get(GDM.ins.gameLevel).isBoss);

            if (TableGameLevel.Get(GDM.ins.gameLevel + 1) == null)
            {
                nextLevelText.text = "-";
                nextLevelText.color = UIUtil.GRAY_COLOR;
                nextBossTag.SetActive(false);
            }
            else
            {
                nextLevelText.text = (GDM.ins.gameLevel + 1).ToString();
                nextBossTag.SetActive(TableGameLevel.Get(GDM.ins.gameLevel + 1).isBoss);
                if (GDM.ins.gameLevel + 1 > GDM.ins.unlockedGameLevel)
                {
                    nextLevelText.color = UIUtil.GRAY_COLOR;
                }
                else
                {
                    nextLevelText.color = currentLevelText.color;
                }
            }

            levelPanel.SetData();
        }

        private void OnEventGameData(EventGameData evt)
        {
            if (evt.action == EventGameData.Action.DataChange)
            {
                RefreshUI();
            }
            else if (evt.action == EventGameData.Action.Error)
            {
                //TODO: TOAST ERROR
                Debug.Log(evt.errorMsg);
            }
        }
    }
}