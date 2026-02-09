using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Sean.Combat
{
    public class CombatNotificationUI : MonoBehaviour
    {
        [SerializeField] private GameObject notificationPrefab;
        [SerializeField] private Canvas canvas;
        [SerializeField] private CombatConfigSO config;

        private readonly Queue<GameObject> _pool = new Queue<GameObject>();
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            CombatEvents.OnNotificationRequested += HandleNotification;
        }

        private void OnDisable()
        {
            CombatEvents.OnNotificationRequested -= HandleNotification;
        }

        private void HandleNotification(string text, Vector2 worldPos)
        {
            GameObject obj = GetFromPool();
            obj.SetActive(true);

            TextMeshProUGUI tmp = obj.GetComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.alpha = 1f;

            // Position in screen space
            Vector3 screenPos = _mainCamera.WorldToScreenPoint(worldPos);
            // Add slight random offset so notifications don't stack exactly
            screenPos.x += Random.Range(-30f, 30f);
            screenPos.y += Random.Range(20f, 50f);
            obj.transform.position = screenPos;

            // Set color based on text content
            if (text.Contains("PARRY"))
                tmp.color = new Color(0.3f, 0.5f, 1f); // blue
            else if (text.Contains("DODGE"))
                tmp.color = new Color(0.3f, 1f, 0.3f); // green
            else if (text.Contains("HIT"))
                tmp.color = new Color(1f, 0.3f, 0.3f); // red
            else if (text.Contains("PUNCH"))
                tmp.color = new Color(1f, 0.9f, 0.3f); // yellow
            else
                tmp.color = Color.white;

            StartCoroutine(AnimateNotification(obj, tmp));
        }

        private IEnumerator AnimateNotification(GameObject obj, TextMeshProUGUI tmp)
        {
            float elapsed = 0f;
            float duration = config != null ? config.notificationDuration : 0.8f;
            float floatSpeed = config != null ? config.notificationFloatSpeed : 1.5f;
            Vector3 startPos = obj.transform.position;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // Float upward
                obj.transform.position = startPos + Vector3.up * (floatSpeed * 50f * t);

                // Fade out in the second half
                if (t > 0.5f)
                {
                    tmp.alpha = 1f - ((t - 0.5f) / 0.5f);
                }

                yield return null;
            }

            obj.SetActive(false);
            _pool.Enqueue(obj);
        }

        private GameObject GetFromPool()
        {
            if (_pool.Count > 0)
            {
                return _pool.Dequeue();
            }

            GameObject obj = Instantiate(notificationPrefab, canvas.transform);
            return obj;
        }
    }
}
