﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using UnibusEvent;
using System.IO;

namespace DestroyViruses
{
    public class UpgradeView : ViewBase
    {
        public FadeGroup fadeGroup;
        public LevelPanel levelPanel;
        public WeaponLevelPanel weaponLevelPanel;
        public WeaponPanel weaponPanel;

        protected override void OnOpen()
        {
            base.OnOpen();
            this.BindUntilDisable<EventGameData>(OnEventGameData);
            RefreshUI();
            fadeGroup.FadeIn(() =>
            {
                NavigationView.BlackSetting(true);
            });
        }

        private void RefreshUI()
        {
            levelPanel.SetData();
            weaponLevelPanel.SetData();
            weaponPanel.SetData();
        }

        private void OnEventGameData(EventGameData evt)
        {
            if (!isActiveAndEnabled)
                return;

            if (evt.action == EventGameData.Action.DataChange)
            {
                RefreshUI();
            }
        }
        
        private void OnClickClose()
        {
            NavigationView.BlackSetting(false);
            fadeGroup.FadeOut(Close);
        }
    }
}