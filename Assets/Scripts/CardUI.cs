using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TMP_Text cardText;
    [SerializeField] private TMP_Text posText;
    [SerializeField] private TMP_Text posLabel;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text damageLabel;
    [SerializeField] private TMP_Text categoriesText;
    [SerializeField] private TMP_Text categoriesLabel;
    [SerializeField] private Button actionButton;

    public AudioMixer audioMixer;


    private Card card;
    private System.Action<Card> onCardAction;

    private bool isPurchased = false;
    private Vector2 originalSize;
    private RectTransform rectTransform;

    // Store the original font sizes
    private float originalCardTextFontSize;
    private float originalPosTextFontSize;
    private float originalPosLabelFontSize;
    private float originalCostTextFontSize;
    private float originalDamageTextFontSize;
    private float originalDamageLabelFontSize;
    private float originalCategoriesTextFontSize;
    private float originalCategoriesLabelFontSize;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalSize = rectTransform.sizeDelta; // Save the original size

        // Save the original font sizes of the text components
        originalCardTextFontSize = cardText.fontSize;
        originalPosTextFontSize = posText.fontSize;
        originalPosLabelFontSize = posLabel.fontSize;
        originalCostTextFontSize = costText.fontSize;
        originalDamageTextFontSize = damageText.fontSize;
        originalDamageLabelFontSize = damageLabel.fontSize;
        originalCategoriesTextFontSize = categoriesText.fontSize;
        originalCategoriesLabelFontSize = categoriesLabel.fontSize;
    }

    // Setup the card UI with data
    public void Setup(Card cardData, System.Action<Card> cardAction)
    {
        card = cardData;
        onCardAction = cardAction;

        // Update the UI elements
        cardText.text = card.GetText();
        posText.text = card.GetPartOfSpeech();
        damageText.text = card.GetEgoDamage().ToString();
        if (card.GetCategories() == null || card.GetCategories().Count == 0)
        {
            categoriesText.text = card.IsInsult() ? "Insult" : "Not an Insult";
        }
        else
        {
            categoriesText.text = string.Join("\n", card.GetCategories());
        }
        // Attach the action to the button
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(() => onCardAction?.Invoke(card));

        // Update the opacity based on whether it's purchased
        SetCardOpacity(isPurchased ? 0.5f : 1f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Check if the current scene is "Battle"
        if (SceneManager.GetActiveScene().name == "Battle" || SceneManager.GetActiveScene().name == "ShopScene")
        {
            
            // Increase the size of the card by a factor of 2.5
            rectTransform.sizeDelta = originalSize * 1.5f;

            // Increase the font size of the text components by a factor of 2.5
            cardText.fontSize = originalCardTextFontSize * 1.5f;
            posText.fontSize = originalPosTextFontSize * 1.5f;
            posLabel.fontSize = originalPosLabelFontSize * 1.5f;
            costText.fontSize = originalCostTextFontSize * 1.5f;
            damageText.fontSize = originalDamageTextFontSize * 1.5f;
            damageLabel.fontSize = originalDamageLabelFontSize * 1.5f;
            categoriesText.fontSize = originalCategoriesTextFontSize * 1.5f;
            categoriesLabel.fontSize = originalCategoriesLabelFontSize * 1.5f;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Check if the current scene is "Battle"
        if (SceneManager.GetActiveScene().name == "Battle" || SceneManager.GetActiveScene().name == "ShopScene")
        {
            
            // Reset the size of the card to the original size
            rectTransform.sizeDelta = originalSize;
          
            // Reset the font size of the text components
            cardText.fontSize = originalCardTextFontSize;
            posText.fontSize = originalPosTextFontSize;
            posLabel.fontSize = originalPosLabelFontSize;
            costText.fontSize = originalCostTextFontSize;
            damageText.fontSize = originalDamageTextFontSize;
            damageLabel.fontSize = originalDamageLabelFontSize;
            categoriesText.fontSize = originalCategoriesTextFontSize;
            categoriesLabel.fontSize = originalCategoriesLabelFontSize;
        }
    }

    public Card GetCard()
    {
        return card;
    }

    public bool IsPurchased()
    {
        return isPurchased;
    }

    public void SetPurchased(bool purchased)
    {
        isPurchased = purchased;
        SetCardOpacity(isPurchased ? 0.5f : 1f); // Update opacity based on purchase status
    }

    public void SetCardOpacity(float opacity)
    {
        // Ensure the opacity value is clamped between 0 (fully transparent) and 1 (fully opaque)
        opacity = Mathf.Clamp(opacity, 0f, 1f);

        // Get the button's Image component
        Image buttonImage = actionButton.GetComponent<Image>();

        // Modify the alpha value of the button's Image color
        Color buttonColor = buttonImage.color;
        buttonColor.a = opacity;  // Set the alpha to the desired opacity
        buttonImage.color = buttonColor;
    }

    public Button GetButton()
    {
        return actionButton;
    }

    public TMP_Text GetCostText()
    {
        return costText;
    }
}
