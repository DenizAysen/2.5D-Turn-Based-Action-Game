using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleVisuals : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI levelText;
    #endregion

    #region Privates
    private int _currentHealth;
    private int _maxHealth;
    private int _level;

    private Animator _animator;

    #region Consts
    private const string LEVEL_ABB = "Lvl: ";
    private const string IS_ATTACK_PARAM = "IsAttack";
    private const string IS_HIT_PARAM = "IsHit";
    private const string IS_DEAD_PARAM = "IsDead";
    #endregion
    #endregion
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    public void SetStartingValues(int currentHealth, int maxHealth, int level)
    {
        _currentHealth = currentHealth;
        _maxHealth = maxHealth;
        _level = level;
        levelText.text = LEVEL_ABB + level.ToString();
        UpdateHealthBar();
    }
    public void ChangeHealth(int currentHealth)
    {
        _currentHealth = currentHealth;

        if(_currentHealth <= 0)
        {
            PlayDeadAnimation();
            Destroy(gameObject,1f);
        }
        UpdateHealthBar();
    }
    public void UpdateHealthBar()
    {
        healthBar.maxValue = _maxHealth;
        healthBar.value = _currentHealth;
    }
    public void PlayAttackAnimation() => _animator.SetTrigger(IS_ATTACK_PARAM);
    public void PlayHitAnimation() => _animator.SetTrigger(IS_HIT_PARAM);
    public void PlayDeadAnimation() => _animator.SetTrigger(IS_DEAD_PARAM);
}
