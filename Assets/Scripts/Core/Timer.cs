using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private uint limit;
    [SerializeField] private TMP_Text timePassed;
    [SerializeField] private TMP_Text timeLeft;
    [SerializeField] private TMP_Text highscoreText;
    
    private float value; // Le temps qui s'est écoulé depuis le lancement du jeu

    //---SETUP DES CALLBACKS--------------------------------------------------------------------------------------------/
    
    void Awake() => Manager.Open(TaquinEvent.OnLoose);
    
    void Start() => Manager.SubscribeTo(TaquinEvent.OnWin, OnWin);
    void OnDestroy() => Manager.UnsubscribeFrom(TaquinEvent.OnWin, OnWin);
    
    //---CORE-----------------------------------------------------------------------------------------------------------/
    
    void Update()
    {
        if (Manager.state != GameState.Active) return; // Ne metrre à jour le temps que si le jeu n'est pas encore fini
        
        if (value >= limit) // Si le temps a dépassé la limite fixé, appel de l'event de Défaite & Changement correspondant pour l'état de jeu
        {
            Manager.state = GameState.Lost;
            Manager.Call(TaquinEvent.OnLoose);

            return;
        }
        
        // Actualisation du temps & de ses représentations textuelles
        value += Time.deltaTime;
        var round = Mathf.RoundToInt(value);

        timePassed.text = round.Format();
        timeLeft.text = ((int)limit - round).Format();
    }
    
    // Mise à jour du highscore si nécessaire
    void OnWin()
    {
        var round = Mathf.RoundToInt(value);
        var score = PlayerPrefs.GetInt(Manager.ScoreSaveKey);
        
        if (score == -1 || PlayerPrefs.GetInt(Manager.ScoreSaveKey) > round)
        {
            PlayerPrefs.SetInt(Manager.ScoreSaveKey, round);
            highscoreText.gameObject.SetActive(true);
        }
    }
}