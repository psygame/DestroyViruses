﻿using UnityEngine;
using System.Collections;

namespace DestroyViruses
{
    public class AircraftFire : MonoBehaviour
    {
        public float fireSpeed = 100;    // bullets/s
        public Transform fireTransform;

        private float mFireOnceDuration;
        private float mFireOnceBullets;
        private float mFireOnceCD;

        public bool IsFiring { get; private set; }

        private void Awake()
        {
            if (fireTransform == null)
                fireTransform = transform.Find("fireTransform");
            if (fireTransform == null)
                Debug.LogError("fireTransform is null, please take care of this!");
        }

        private void FireOnce()
        {
            for (int i = 0; i < mFireOnceBullets; i++)
            {
                var x = Bullet.BULLET_WIDTH * (i - (mFireOnceBullets - 1) * 0.5f);
                var bullet = Bullet.Create();
                bullet.Reset(UIUtil.GetUIPos(fireTransform), x);
            }
        }

        public void Fire()
        {
            IsFiring = true;
        }

        public void HoldFire()
        {
            IsFiring = false;
        }

        private void Update()
        {
            mFireOnceDuration = Bullet.BULLET_HEIGH / Bullet.BULLET_SPEED;
            mFireOnceBullets = Mathf.CeilToInt(fireSpeed * mFireOnceDuration);
            mFireOnceDuration = mFireOnceBullets / fireSpeed;

            mFireOnceCD = this.UpdateCD(mFireOnceCD);
            if (IsFiring)
            {
                if (mFireOnceCD <= 0)
                {
                    FireOnce();
                    mFireOnceCD = mFireOnceDuration;
                }
            }
        }
    }
}