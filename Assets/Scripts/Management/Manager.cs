using System;
using System.Collections.Generic;

public static class Manager
{
    public const string ScoreSaveKey = "sco"; // La clé PlayerPrefs à laquelle la valeur d'highscore est enregistré
    public static GameState state; // L'état actuelle du jeu

    //------------------------------------------------------------------------------------------------------------------/
    
    // Mise en place d'un système évènementielle globale très simple pour découpler le code le plus possible
    // En particulier sur les feedbacks (e.g : Win & LoseHandler
    private static Dictionary<TaquinEvent, EventWrapper> eventRegistry = new Dictionary<TaquinEvent, EventWrapper>();

    public static void ClearEvents() => eventRegistry.Clear();
    public static void Open(TaquinEvent key)
    {
        if (eventRegistry.ContainsKey(key)) return;
        eventRegistry.Add(key, new EventWrapper());
    }
    public static void Call(TaquinEvent key) => eventRegistry[key].Call();

    public static void SubscribeTo(TaquinEvent key, Action method) => eventRegistry[key].Subscribe(method);
    public static void UnsubscribeFrom(TaquinEvent key, Action method) => eventRegistry[key].Unsubscribe(method);
}