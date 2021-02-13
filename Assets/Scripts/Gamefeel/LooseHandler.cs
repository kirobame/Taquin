using System;
using UnityEngine;

// Classe lancant un feedback sur le callback de Victoire
public class LooseHandler : MonoBehaviour
{
    [SerializeField] private Animator banner;
    
    void Start() => Manager.SubscribeTo(TaquinEvent.OnLoose, OnLoose);

    void OnLoose() => banner.SetTrigger("Play");
}