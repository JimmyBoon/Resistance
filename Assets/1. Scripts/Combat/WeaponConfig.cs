using System;
using RPG.Attributes;
using RPG.Inventories;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RPG.Stats;
using RPG.Core;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : EquipableItem, IModifierProvider
    {
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] Weapon eqippedPrefab = null;
        [SerializeField] float weaponShootRange = 20f;
        [SerializeField] float weaponHitRange = 2f;
        [SerializeField] float weaponShootingDamage = 5f;
        [SerializeField] float weaponHittingDamage = 5f;
        [SerializeField] float percentageBonus = 0f;
        [SerializeField] float weaponNoise = 5f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;
        [SerializeField] bool isHittingWeapon = false;
        [SerializeField] NoiseType noiseType;
        [SerializeField] WeaponClass weaponClass;


        const string weaponName = "Weapon";

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);
            Weapon weapon = null;

            if (eqippedPrefab != null)
            {
                Transform handTransform = GetHandTransform(rightHand, leftHand);

                weapon = Instantiate(eqippedPrefab, handTransform);
                weapon.gameObject.name = weaponName;
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;

            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            return weapon;

        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (oldWeapon == null)
            {
                return;
            }

            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetHandTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            if (isRightHanded) { handTransform = rightHand; }
            else { handTransform = leftHand; }

            return handTransform;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetHandTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(instigator, target, calculatedDamage);

        }

        public WeaponClass GetWeaponClass()
        {
            return weaponClass;
        }


        public float GetWeaponShootRange()
        {
            return weaponShootRange;
        }

        public float GetWeaponHitRange()
        {
            return weaponHitRange;
        }

        public bool GetIsHittingWeapon()
        {
            return isHittingWeapon;
        }

        public float GetWeaponShootingDamage()
        {
            return weaponShootingDamage;
        }

        public float GetWeaponHittingDamage()
        {
            return weaponHittingDamage;
        }

        public float GetPercentageBonus()
        {
            return percentageBonus;
        }

        public float GetWeaponNoise()
        {
            return weaponNoise;
        }

        public NoiseType GetNoiseType()
        {
            return noiseType;
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return weaponShootingDamage;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return percentageBonus;
            }
        }


    }
}


