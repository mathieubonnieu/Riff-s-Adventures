using UnityEngine;

public class FloatingTextSpawner : MonoBehaviour
{
    public GameObject floatingTextPrefab;

    public void ShowFloatingText(int damage, bool negative)
    {
        Vector3 spawnPos = transform.position + new Vector3(0f, 1.5f, 0f); // Slightly above player
        GameObject instance = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);
        instance.GetComponent<FloatingText>().SetBold(true);
        if (negative) {
            instance.GetComponent<FloatingText>().SetText("-" + damage.ToString());
            instance.GetComponent<FloatingText>().SetColor(new Color32(227, 20, 20, 255));
        } else {
            instance.GetComponent<FloatingText>().SetText("+" + damage.ToString());
            instance.GetComponent<FloatingText>().SetColor(new Color32(71, 201, 42, 255));
        }
    }
}
