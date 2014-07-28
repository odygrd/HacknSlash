using UnityEngine;

public class Gold : MonoBehaviour
{
    public int Value { get; set; }

    public Gold()
    {
        Value = Random.Range(50, 200);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerChar.Instance.Gold += Value;
            GameSetting2.SaveCharacterGold(PlayerChar.Instance.Gold); //save setting
            Destroy(gameObject);
        }
    }
}
