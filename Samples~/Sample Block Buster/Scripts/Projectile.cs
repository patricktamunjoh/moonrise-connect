using MoonriseGames.Connect.Enums;
using MoonriseGames.Connect.Functions;
using MoonriseGames.Connect.Objects;
using UnityEngine;
using static MoonriseGames.Connect.Invocation;

namespace Samples.Block_Buster.Scripts
{
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
                Call(character.HitByProjectile);
            Call(DestroyAfterCollision);
        }

        [NetworkFunction(Groups.Host, Recipients.All)]
        private void DestroyAfterCollision()
        {
            // Only the host instance should destroy network objects
            // Otherwise, calls might not reach the instance in time before it is destroyed
            Destroy(gameObject);
        }
    }
}
