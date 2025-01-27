using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Bard bard;

    [SerializeField] private Sprite enemyImage;

    private void Start()
    {
        enemyImage = Resources.Load<Sprite>("Sprites/SIB_Bard_2_placeholder");

        InitializeSprite();
    }

    private void InitializeSprite()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        if (enemyImage != null)
        {
            spriteRenderer.sprite = enemyImage;
        }
        else
        {
            // Debug.LogWarning("Enemy image is not assigned. Please assign a sprite in the Inspector.");
        }
    }
}