using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BoxCharacter : MonoBehaviour
{
    public CharacterBase CB;
    public bool isPlayer;
    private SelectionManager SM;
    private bool isSelected;
    private Image imageBox;

    private void Awake()
    {
        SM = GameObject.FindGameObjectWithTag("Manager").GetComponent<SelectionManager>();
        imageBox = this.gameObject.GetComponent<Image>();
    }

    public void CallClickCommand()
    {

        if (!isSelected)
        {
            if (isPlayer)
            {
                SM.PlayerParty.Add(CB);
            }

            else
            {
                SM.EnemyParty.Add(CB);
            }
            imageBox.color = new Color(1, 1, 1);
            isSelected = true;
        }
        else
        {
            if (isPlayer)
            {
                SM.PlayerParty.Remove(CB);
            }

            else
            {
                SM.EnemyParty.Remove(CB);
            }
            imageBox.color = new Color(0.3f, 0.3f, 0.3f);
            isSelected = false;
        }

        SM.UpdateText();
    }

    public void Init()
    { 
        imageBox.sprite = CB.IconImage;

    }

}
