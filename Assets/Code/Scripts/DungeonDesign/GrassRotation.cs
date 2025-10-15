using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    void Start()
    {
        // Tourne l'objet autour de l'axe Y d'un angle aléatoire entre 0 et 360 degrés
        float randomY = Random.Range(0f, 360f);
        transform.Rotate(0f, randomY, 0f);
    }
}