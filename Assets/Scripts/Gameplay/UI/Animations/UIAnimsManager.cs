using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIAnimsManager : MonoBehaviour
{
    #region Fields

    [Header("References")]
    [Tooltip("Switcher for animation objects.")]
    [SerializeField] private HomoObjectSwitcher _objectSwitcher;

    [Header("Animations")]
    [SerializedDictionary("Animator", "Animation Clip")]
    [SerializeField] private SerializedDictionary<Animator, AnimationClip> _animations;

    private Coroutine[] _cooldownRoutines = new Coroutine[10];

    #endregion


    #region Public Methods

    public void PlayAnimation(int index)
    {
        if (_animations == null || _animations.Count == 0)
        {
            Debug.LogWarning("UIAnimsManager: No animations found");
            return;
        }

        if (index < 0 || index >= _animations.Count)
        {
            Debug.LogWarning($"UIAnimsManager: Index {index} is out of range (0..{_animations.Count - 1}).");
            return;
        }

        var pair = _animations.ElementAt(index);
        Animator animator = pair.Key;
        AnimationClip animation = pair.Value;

        _objectSwitcher.OffAll();
        _objectSwitcher.SetOn(index);

        if (animator != null && animation != null)
        {
            AddToArray(StartCoroutine(AnimCooldownRoutine(animation.length, index)));
            animator.Play(animation.name);
        }
        else
        {
            Debug.LogWarning($"UIAnimsManager: Failed to play animation at index({index}). Animator: {(animator == null ? "null" : "OK")}, Clip: {(animation == null ? "null" : animation.name)}");
        }
    }

    #endregion


    #region Private Methods

    private void AddToArray(Coroutine routine)
    {
        for (int i = 0; i < _cooldownRoutines.Length; i++)
        {
            if (_cooldownRoutines[i] == null)
            {
                _cooldownRoutines[i] = routine;
                break;
            }
        }
    }

    #endregion


    #region Coroutines

    private IEnumerator AnimCooldownRoutine(float animDuration, int arrId)
    {
        yield return new WaitForSeconds(animDuration);
        _objectSwitcher.OffAll();
        _cooldownRoutines[arrId] = null;
    }

    #endregion
}