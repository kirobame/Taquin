using TMPro;
using UnityEngine;

// Simple data binding entre un élément de texte d'UI & la valeur d'highscore enregistré dans les PlayerPrefs
public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text textMesh;
    
    void Awake()
    {
        if (!PlayerPrefs.HasKey(Manager.ScoreSaveKey))
        {
            PlayerPrefs.SetInt(Manager.ScoreSaveKey, -1);
            textMesh.text = "Aucun";
        }
        else
        {
            var score = PlayerPrefs.GetInt(Manager.ScoreSaveKey);
            textMesh.text = score == -1 ? "Aucun" : PlayerPrefs.GetInt(Manager.ScoreSaveKey).Format();
        }
    }
}