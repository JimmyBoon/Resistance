using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using RPG.Attributes;
using UnityEngine;
using RPG.Stats;
using GameDevTV.Utils;
using System;
using RPG.Control;
using RPG.Inventories;
using UnityEngine.AI;

namespace RPG.Combat
{
    /// <summary>
    /// Conducts attacking and arming of weapons
    /// </summary>

    public class Fighter : MonoBehaviour, IAction, ISaveable, INoise
    {
        [SerializeField] float timeBetweenAttacks = 1f;

        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;
        [SerializeField] string defaultWeaponName = "Unarmed";
        float attackSpeed = 5.6f;

        float outputNoise;

        float timeSinceLastAttack = Mathf.Infinity;

        LazyValue<Weapon> currentWeapon;
        WeaponConfig currentWeaponConfig;

        Health target;
        Mover mover;
        Animator animator;
        AISenses aISenses;

        Equipment equipment;

        private void Awake()
        {
            mover = GetComponent<Mover>();
            animator = GetComponent<Animator>();
            aISenses = GetComponent<AISenses>();
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);

            equipment = GetComponent<Equipment>();
            if (equipment)
            {
                equipment.equipmentUpdated += UpdateWeapon;
            }

        }

        private Weapon SetupDefaultWeapon()
        {

            return AttachWeapon(defaultWeapon);
        }

        private void Start()
        {
            currentWeapon.ForceInit();
        }



        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (target && target.IsAlive())
            {
                //bool isInRange = Vector3.Distance(transform.position, target.transform.position) < currentWeaponConfig.GetWeaponRange();

                if (aISenses != null && !aISenses.InSight(target.gameObject))
                {
                    mover.MoveTo(aISenses.GetLastSeenLocation(), attackSpeed);
                }

                if (!IsInRange(target.transform))
                {
                    mover.MoveTo(target.transform.position, attackSpeed);
                }

                else
                {
                    mover.Cancel();
                    AttackBehaviour();
                }
            }
        }

        private void AttackBehaviour()
        {
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                TriggerAttack();
            }
        }

        private void TriggerAttack()
        {
            transform.LookAt(target.transform);

            animator.ResetTrigger("stopAttack");

            //this will trigger the Shoot() event
            if(currentWeaponConfig.HasProjectile() && !IsInHittingRange(target.transform))
            {
                animator.SetTrigger("attackShoot");
            }
            else if(currentWeaponConfig.GetIsHittingWeapon() && IsInHittingRange(target.transform))
            {
                animator.SetTrigger("attack");
            }
            else if (!currentWeaponConfig.GetIsHittingWeapon())
            {
                animator.SetTrigger("attackShoot");
            }
            
            timeSinceLastAttack = 0;
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (!mover.CanMoveTo(combatTarget.transform.position) && !IsInRange(combatTarget.transform)) { return false; }
            if (combatTarget == null) { return false; }
            Health targetToTest = combatTarget.GetComponent<Health>();

            return targetToTest != null && targetToTest.IsAlive();

        }

        public void Attack(GameObject combatTarget, float attackSpeed)
        {
            this.attackSpeed = attackSpeed;
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            target = null;
            animator.ResetTrigger("attack");
            animator.ResetTrigger("attackShoot");
            animator.SetTrigger("stopAttack");
            //todo investigate if adding mover.cancel() here makes an improvement.
        }

        // public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        // {
        //     if (stat == Stat.Damage)
        //     {
        //         yield return currentWeaponConfig.GetWeaponDamage();
        //     }
        // }

        // public IEnumerable<float> GetPercentageModifiers(Stat stat)
        // {
        //     if (stat == Stat.Damage)
        //     {
        //         yield return currentWeaponConfig.GetPercentageBonus();
        //     }
        // }

        private float CalculatedDamage(float actionDamage)
        {
            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            return damage + actionDamage;
        }

        void Hit() //*** Animation Event ****
        {
            StartCoroutine(Noise(currentWeaponConfig.GetWeaponNoise()));
            if (!target) { return; }

            if (currentWeapon.value != null)
            {
                currentWeapon.value.OnHit();
            }

            if (IsInRange(target.transform))
            {
                target.TakeDamage(gameObject, CalculatedDamage(currentWeaponConfig.GetWeaponHittingDamage()));
            }

        }

        void Shoot() //*** Animation Event ****
        {
            if (!target) { return; }

            if (currentWeaponConfig.HasProjectile() && IsInRange(target.transform))
            {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, CalculatedDamage(currentWeaponConfig.GetWeaponShootingDamage()));
                StartCoroutine(Noise(currentWeaponConfig.GetWeaponNoise()));
            }
        }

        private bool IsInRange(Transform targetTransform)
        {
            if(currentWeaponConfig.HasProjectile())
            {
                return Vector3.Distance(transform.position, targetTransform.position) < currentWeaponConfig.GetWeaponShootRange();
            }
            else
            {
                return Vector3.Distance(transform.position, targetTransform.position) < currentWeaponConfig.GetWeaponHitRange();
            }
            
        }

        private bool IsInHittingRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) < currentWeaponConfig.GetWeaponHitRange();
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        private void UpdateWeapon()
        {
            WeaponConfig weaponInSlot = (WeaponConfig)equipment.GetItemInSlot(EquipLocation.Weapon);
            if (weaponInSlot)
            {
                EquipWeapon(weaponInSlot);
            }
            else
            {
                EquipWeapon(defaultWeapon);
            }

        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            return weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            WeaponConfig weapon = UnityEngine.Resources.Load<WeaponConfig>((string)state);
            EquipWeapon(weapon);
        }

        private IEnumerator Noise(float noise)
        {
            outputNoise = noise;
            yield return new WaitForSeconds(0f);
            outputNoise = 0f;
        }


        public NoiseInstance GetNoiseInstance()
        {
            NoiseInstance noise = new NoiseInstance();
            noise.noiseAmount = outputNoise;
            noise.noiseLocation = transform.position;
            noise.noiseSource = this.gameObject;
            noise.noiseType = currentWeaponConfig.GetNoiseType();
            return noise;
        }


        public Health GetTarget()
        {
            return target;
        }

        public WeaponConfig GetWeaponConfig()
        {
            return currentWeaponConfig;
        }


    }
}