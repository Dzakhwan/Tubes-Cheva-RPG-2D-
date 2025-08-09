using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class ExpManager : MonoBehaviour
{
    public int level;
    public int currentExp;
    public int expToLevelUp = 10;
    public Slider expSlider;
    public TMP_Text levelText;
    public void Start()
    {
        UpdateUI();
    }
    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Return))
        // {
        //     GainExp(2);
        // }
    }
    private void OnEnable()
    {
        EnemyHealth.OnMonsterDefeated += GainExp;
    }

    private void OnDisable()
    {
        EnemyHealth.OnMonsterDefeated -= GainExp;
    }

    public void GainExp(int amount)
    {
        currentExp += amount;
        if (currentExp >= expToLevelUp)
        {
            LevelUp();

        }
        UpdateUI();
    }
    private void LevelUp()
    {
        level++;
        currentExp -= expToLevelUp;
        expToLevelUp *= 2;
    }
    public void UpdateUI()
    {
        expSlider.maxValue = expToLevelUp;
        expSlider.value = currentExp;
        levelText.text = "Level " + level;
    }
}
