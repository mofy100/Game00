using UnityEngine;

public class WireCube : MonoBehaviour
{
    [SerializeField] Material material;

    void Start(){
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[]
        {
            // 8 cube corners
            new Vector3(-Block.sizeH / 2, -Block.sizeV / 2, -Block.sizeH / 2),
            new Vector3(Block.sizeH / 2, -Block.sizeV / 2, -Block.sizeH / 2),
            new Vector3(Block.sizeH / 2, -Block.sizeV / 2, Block.sizeH / 2),
            new Vector3(-Block.sizeH / 2, -Block.sizeV / 2, Block.sizeH / 2),
            new Vector3(-Block.sizeH / 2, Block.sizeV / 2, -Block.sizeH / 2),
            new Vector3(Block.sizeH / 2, Block.sizeV / 2, -Block.sizeH / 2),
            new Vector3(Block.sizeH / 2, Block.sizeV / 2, Block.sizeH / 2),
            new Vector3(-Block.sizeH / 2, Block.sizeV / 2, Block.sizeH / 2),
        };

        int[] lines = new int[]
        {
            0,1, 1,2, 2,3, 3,0, // bottom
            4,5, 5,6, 6,7, 7,4, // top
            0,4, 1,5, 2,6, 3,7  // vertical
        };

        mesh.vertices = vertices;
        mesh.SetIndices(lines, MeshTopology.Lines, 0);

        GetComponent<MeshFilter>().mesh = mesh;
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material = material;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;

    }
}