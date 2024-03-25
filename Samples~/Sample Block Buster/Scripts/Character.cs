using MoonriseGames.Connect.Enums;
using MoonriseGames.Connect.Extensions;
using MoonriseGames.Connect.Functions;
using MoonriseGames.Connect.Objects;
using UnityEngine;
using static MoonriseGames.Connect.Invocation;

namespace Samples.Block_Buster.Scripts
{
    [NetworkObject]
    public class Character : MonoBehaviour
    {
        [SerializeField]
        [Space]
        private float _movementSpeed;

        [SerializeField]
        private float _projectileSpeed;

        [SerializeField]
        private float _attackDelay;

        [SerializeField]
        [Space]
        private Transform _pivot;

        [SerializeField]
        private Transform _attackOrigin;

        [SerializeField]
        [Space]
        private Projectile _projectilePrefab;

        private Rigidbody2D Rigidbody { get; set; }
        private Vector2 MovementDirection { get; set; }
        private float TimeLastShot { get; set; }

        public bool IsControlledLocally { get; set; }

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            UpdateMovement();

            UpdateMovementInput();
            UpdateAimingAndShooting();
        }

        private void UpdateMovementInput()
        {
            // The movement input is only considered if the character is controlled by the local instance
            if (!IsControlledLocally)
                return;

            var inputDirection = Vector2.zero;

            if (Input.GetKey(KeyCode.W))
                inputDirection += Vector2.up;
            if (Input.GetKey(KeyCode.S))
                inputDirection += Vector2.down;
            if (Input.GetKey(KeyCode.A))
                inputDirection += Vector2.left;
            if (Input.GetKey(KeyCode.D))
                inputDirection += Vector2.right;

            if (inputDirection != MovementDirection)
                Call(ChangeMovementDirection, inputDirection, (Vector2)transform.position);
        }

        [NetworkFunction(Groups.All, Recipients.All)]
        public void ChangeMovementDirection(Vector2 direction, Vector2 position)
        {
            // Because this function has authority all, every game instance can call it
            // Here, also the position is send to correct any errors that might have manifested due to physics interactions
            // For a real game it is advisable to deploy a more sophisticated solution to error correction
            transform.position = position;
            MovementDirection = direction;
        }

        private void UpdateAimingAndShooting()
        {
            if (!IsControlledLocally)
                return;

            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var aimDirection = (mousePosition - transform.position).normalized;

            Call(ChangeAimAngle, Vector2.SignedAngle(Vector2.up, aimDirection));

            var canShootProjectile = Time.time - TimeLastShot > _attackDelay;

            if (Input.GetMouseButtonDown(0) && canShootProjectile)
                Call(ValidateShootProjectile, (Vector2)_attackOrigin.position, (Vector2)aimDirection);
        }

        [NetworkFunction(Groups.All, Recipients.All, Transmission.Unreliable)]
        public void ChangeAimAngle(float angle)
        {
            // Because this function has no lasting impact on the game state, it can be called directly by client instances
            _pivot.transform.eulerAngles = Vector3.forward * angle;
        }

        [NetworkFunction(Groups.All, Recipients.Host)]
        public void ValidateShootProjectile(Vector2 origin, Vector2 direction)
        {
            // This function seems nonsensical because all it does it forward the call to another function
            // However, while the validation can be called by all game instances, the execution can only be called by the host
            // To ensure projectiles are registered in the same order, only the host should be able to make such calls
            Call(ExecuteShootProjectile, origin, direction);
        }

        [NetworkFunction(Groups.Host, Recipients.All)]
        public void ExecuteShootProjectile(Vector2 origin, Vector2 direction)
        {
            var projectile = Instantiate(_projectilePrefab, origin, Quaternion.identity);

            // Because projectiles are Network Objects, they have to be registered
            projectile.RegisterInstance();
            projectile.Velocity = direction.normalized * _projectileSpeed;

            TimeLastShot = Time.time;
        }

        private void UpdateMovement()
        {
            // The physics system handles the actual movement and collisions
            // This, however, can also lead to some divergence on the individual game instances
            Rigidbody.velocity = MovementDirection * _movementSpeed;
        }

        [NetworkFunction(Groups.Host, Recipients.All)]
        public void HitByProjectile()
        {
            transform.position = Vector3.zero;
            MovementDirection = Vector2.zero;
        }
    }
}
