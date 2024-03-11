using MoonriseGames.CloudsAhoyConnect.Enums;
using MoonriseGames.CloudsAhoyConnect.Extensions;
using MoonriseGames.CloudsAhoyConnect.Functions;
using MoonriseGames.CloudsAhoyConnect.Objects;
using UnityEngine;

[NetworkObject]
public class Projectile : MonoBehaviour
{
    public Vector2 Velocity { get; set; }

    private void Update()
    {
        // The projectile is only translated into a set direction
        transform.Translate(Velocity * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var character = other.gameObject.GetComponentInParent<Character>();
        if (character != null)
            this.Send(character.HitByProjectile);
        this.Send(DestroyAfterCollision);
    }

    [NetworkFunction(Groups.Host, Recipients.All)]
    private void DestroyAfterCollision()
    {
        // Only the host instance should destroy network objects
        // Otherwise, calls might not reach the instance in time before it is destroyed
        Destroy(gameObject);
    }
}
