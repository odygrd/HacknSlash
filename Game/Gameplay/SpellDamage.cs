using UnityEngine;

public class SpellDamage : MonoBehaviour
{
    public int spellPower {get; set;}
    // Use this for initialization
    void Start()
    {
        Destroy(gameObject, 3);
    }

    void FixedUpdate()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * 10);
    }

    void OnCollisionEnter(Collision collision)
    {
        foreach (var contact in collision.contacts)
        {
            var Hitted_Collider = contact.otherCollider.gameObject;

            if (Hitted_Collider.CompareTag("Enemy")) 
            {
                if (gameObject.name.Contains("Fireball"))
                {
                    Instantiate(Resources.Load(GameSetting2.EFFECTS_PATH + "Explosion"), transform.position, transform.rotation);
                }
          
                var mobScript = Hitted_Collider.transform.parent.GetComponent<Mob>();
                if (mobScript != null)
                {
                    var damage = (int)(spellPower * (Random.Range(10, 12)));
                    mobScript.MobDamageReceived(damage);
                    //Create combat text
                    var PointTransform = Instantiate(Resources.Load(GameSetting2.EFFECTS_PATH + "PointEffect"), Hitted_Collider.transform.position, transform.rotation) as GameObject;
                    if (PointTransform != null) PointTransform.gameObject.GetComponent<CombatText>().effectName = damage + " Damage";
                }

                Destroy(gameObject);
            }
        }
    }
}
