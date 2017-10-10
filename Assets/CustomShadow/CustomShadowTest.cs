using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class CustomShadowTest : MonoBehaviour
{
    [SerializeField] Light _light;
    [SerializeField, Range(0, 5)] float _rejectionDepth = 0.5f;

    [SerializeField, HideInInspector] Shader _shader;

    Material _material;
    CommandBuffer _command;

    void OnDestroy()
    {
        if (_material != null)
        {
            if (Application.isPlaying)
                Destroy(_material);
            else
                DestroyImmediate(_material);
        }
    }

    void OnPreCull()
    {
        // Add the command buffer to the light before camera culling.
        if (_command != null && _light != null)
            _light.AddCommandBuffer(LightEvent.AfterScreenspaceMask, _command);
    }

    void OnPreRender()
    {
        // We can remove the command buffer before starting render with this
        // camera. Note: This is thought to be done in OnPostRender, but for
        // some reasons it crashes if we do it in OnPostRender. So, we do it in
        // OnPreRender instead. Actually I'm not sure why this works though!
        if (_command != null && _light != null)
            _light.RemoveCommandBuffer(LightEvent.AfterScreenspaceMask, _command);
    }

    void Update()
    {
        // Lazy initialization of the material.
        if (_material == null)
        {
            _material = new Material(_shader);
            _material.hideFlags = HideFlags.DontSave;
        }

        // Lazy initialization of the command buffer.
        if (_command == null)
        {
            _command = new CommandBuffer();
            _command.name = "Contact Shadow";
            _command.DrawProcedural(Matrix4x4.identity, _material, 0, MeshTopology.Triangles, 3);
        }

        // We require the camera depth texture.
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;

        // Shader parameters
        var lightDir = (_light != null) ? _light.transform.forward : Vector3.forward;
        _material.SetVector("_LightDirection", transform.InverseTransformDirection(-lightDir));
        _material.SetFloat("_RejectionDepth", _rejectionDepth);
    }
}
