using UnityEngine;

public class RecipeBackButton : MonoBehaviour
{
    public GameObject Recipe;

    public void BackBtn()
    {
        SoundManager.Instance.PlaySound("SFX_UI_Click");
        Recipe.SetActive(false);
    }
}
