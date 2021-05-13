using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ToolTime
{
    public class BuffController : MonoBehaviour
    {
        // SpeedBuff
        int speedBuffDuration = 10; // Duration of Buff
        float initialSpeed; // Initial speed of player
        float addedSpeed = 2.5f; // Added speed during Buff
        bool speedBuffActive = false;

        // HammerBuff
        int hammer = 10; // Added repair amount
        public int Hammer { get => hammer; }
        int hammerBuffDuration = 10; // Duration of Buff
        bool hammerBuffActive = false;

        // RohrzangeBuff
        float rohrZange = 10f; // Added repair amount
        public float RohrZange { get => rohrZange; }
        int rohrZangeBuffDuration = 10; // Duration of Buff
        bool rohrzangeBuffActive = false;

        // SchraubenZieherBuff
        int schraubenZieher = 10; // Added repair amount
        public int SchraubenZieher { get => schraubenZieher; }
        int schraubenZieherBuffDuration = 10; // Duration of Buff
        bool schraubenZieherBuffActive = false;

        // MaulschluesselBuff
        int maulSchluessel = 10; // Added repair amount
        public int MaulSchluessel { get => maulSchluessel; }
        int maulSchluesselBuffDuration = 10; // Duration of Buff
        bool maulschluesselBuffActive = false;

        // ToolBuff
        int toolBuff = 5; // Added repair amount
        public int ToolBuff { get => toolBuff; }
        int toolBuffDuration = 10; // Duration of Buff
        bool toolBuffActive = false;

        // SkillCheckBuff
        public GameObject skillcheckPrefab;
        int skillcheck = 5; // Horizontal size of triggerbox in "SkillCheck" Prefab
        public int Skillcheck { get => skillcheck; }
        int skillcheckDuration = 10; // Duration of Buff
        bool skillCheckBuffActive = false;

        public PlayerController playerController;

        public Text buffText;

        public GameObject collisionParticle;
        GameObject particle;

        AudioSource source;
        public AudioClip collect;

        private void Awake()
        {
            source = GetComponent<AudioSource>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.CompareTag("SpeedBuff")           ||
                collision.transform.CompareTag("HammerBuff")          ||
                collision.transform.CompareTag("RohrZangeBuff")       ||
                collision.transform.CompareTag("SchraubenZieherBuff") ||
                collision.transform.CompareTag("MaulSchluesselBuff")  ||
                collision.transform.CompareTag("ToolBuff")            ||
                collision.transform.CompareTag("SkillCheckBuff"))
            {
                // Particle
                particle = Instantiate(collisionParticle, transform.position, Quaternion.identity);
                particle.transform.SetParent(gameObject.transform);

                // Sound
                source.clip = collect;
                source.Play();
            }

            if (collision.transform.CompareTag("SpeedBuff"))
            {
                buffText.text = "Speed+";
                StartCoroutine(DeleteText());
                IEnumerator SpeedBuff()
                {
                    speedBuffActive = true;
                    initialSpeed = playerController.speed;
                    playerController.speed += addedSpeed;
                    for (int i = 0; i < speedBuffDuration; i++)
                    {
                        yield return new WaitForSeconds(1);
                    }
                    playerController.speed = initialSpeed;
                    speedBuffActive = false;
                }
                StartCoroutine(SpeedBuff());
                Destroy(collision.transform.gameObject);
            }
            else if (collision.transform.CompareTag("HammerBuff"))
            {
                buffText.text = "Hammer+";
                StartCoroutine(DeleteText());
                IEnumerator HammerBuff()
                {
                    hammerBuffActive = true;
                    hammer += 10;
                    for (int i = 0; i < hammerBuffDuration; i++)
                    {
                        yield return new WaitForSeconds(1);
                    }
                    hammer = 0;
                    hammerBuffActive = false;
                }
                StartCoroutine(HammerBuff());
                Destroy(collision.transform.gameObject);
            }
            else if (collision.transform.CompareTag("RohrZangeBuff"))
            {
                buffText.text = "Rohrzange+";
                StartCoroutine(DeleteText());
                IEnumerator RohrZangeBuff()
                {
                    rohrzangeBuffActive = true;
                    rohrZange += 10;
                    for (int i = 0; i < rohrZangeBuffDuration; i++)
                    {
                        yield return new WaitForSeconds(1);
                    }
                    rohrZange = 0;
                    rohrzangeBuffActive = false;
                }
                StartCoroutine(RohrZangeBuff());
                Destroy(collision.transform.gameObject);
            }
            else if (collision.transform.CompareTag("SchraubenZieherBuff"))
            {
                buffText.text = "Schraubenzieher+";
                StartCoroutine(DeleteText());
                IEnumerator SchraubenZieherBuff()
                {
                    schraubenZieherBuffActive = true;
                    schraubenZieher += 10;
                    for (int i = 0; i < schraubenZieherBuffDuration; i++)
                    {
                        yield return new WaitForSeconds(1);
                    }
                    schraubenZieher = 0;
                    schraubenZieherBuffActive = false;
                }
                StartCoroutine(SchraubenZieherBuff());
                Destroy(collision.transform.gameObject);
            }
            else if (collision.transform.CompareTag("MaulSchluesselBuff"))
            {
                buffText.text = "Maulschlüssel+";
                StartCoroutine(DeleteText());
                IEnumerator MaulSchluesselBuff()
                {
                    maulschluesselBuffActive = true;
                    maulSchluessel += 10;
                    for (int i = 0; i < maulSchluesselBuffDuration; i++)
                    {
                        yield return new WaitForSeconds(1);
                    }
                    maulSchluessel = 0;
                    maulschluesselBuffActive = false;
                }
                StartCoroutine(MaulSchluesselBuff());
                Destroy(collision.transform.gameObject);
            }
            else if (collision.transform.CompareTag("ToolBuff"))
            {
                buffText.text = "All Tools+";
                StartCoroutine(DeleteText());
                IEnumerator ToolBuff()
                {
                    toolBuffActive = true;
                    toolBuff += 5;
                    for (int i = 0; i < toolBuffDuration; i++)
                    {
                        yield return new WaitForSeconds(1);
                    }
                    toolBuff = 0;
                    toolBuffActive = false;
                }
                StartCoroutine(ToolBuff());
                Destroy(collision.transform.gameObject);
            }
            else if (collision.transform.CompareTag("SkillCheckBuff"))
            {
                buffText.text = "Skillcheck+";
                StartCoroutine(DeleteText());
                IEnumerator SkillcheckTrigger()
                {
                    skillCheckBuffActive = true;
                    skillcheck = 10;
                    for (int i = 0; i < skillcheckDuration; i++)
                    {
                        yield return new WaitForSeconds(1);
                    }
                    skillcheck = 5;
                    skillCheckBuffActive = false;
                }
                StartCoroutine(SkillcheckTrigger());
                Destroy(collision.transform.gameObject);
            }
        }

        /// <summary>
        /// Resets the text in "buffText"
        /// </summary>
        /// <returns></returns>
        IEnumerator DeleteText()
        {
            yield return new WaitForSeconds(1);
            buffText.text = "";
        }
    }
}
