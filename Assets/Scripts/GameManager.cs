using UnityEngine;
using static Bard;
using static Dictionary;
using static Card;
using static DeckObj;

public class GameManager : MonoBehaviour
{

    public int round_number = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Read card json and add starter cards to (Player)Bards dictionary.
        // Read journal json and add phrases to (Player)Bards journal.
        // Set (Player)Bards starter deck.

        // Read card json and add cards to 3 (Opponent)Bards decks.
        // Read journal json and add phrases to 3 (Opponent)Bards journals.
        

        // Display welcome message.
    }

    void StartMatch() 
    {
        // Run code to start a match against an opponent.
    }

    void DisplayWelcomeModal()
    {
        // Displays the welcome message.
    }

    

    void DisplayGameOverModal()
    {
        // Displays game over message and gives player option to restart.
    }


}
