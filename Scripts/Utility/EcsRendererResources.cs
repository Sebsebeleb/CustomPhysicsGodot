using Godot;

namespace CustomPhysics.Utility;


public partial class EcsRendererResources : Resource
{
    [Export]
    public Texture2D[] sprites;

    public bool TryGetTextureById(int id, out Texture2D tex)
    {
        if (id < 0 || id >= sprites.Length)
        {
            tex = default;
            return false;
        }

        tex = sprites[id];
        return true;
    }
}