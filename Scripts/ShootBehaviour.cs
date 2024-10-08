using Godot;

namespace CustomPhysics;

public partial class ShootBehaviour : Node2D
{
    [Export] private bool predictAction;
    [Export] private PackedScene bulletPrefab;
    

    public void Shoot()
    {
        var bullet = bulletPrefab.Instantiate();
    }
}