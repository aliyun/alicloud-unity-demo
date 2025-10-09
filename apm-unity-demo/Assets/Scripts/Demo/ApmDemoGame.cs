using UnityEngine;

namespace AlicloudApmDemo
{
    public class ApmDemoGame
    {
        private const float CubeBaseHeight = 0.6f;
        private const float GroundOffsetY = -0.6f;
        private GameObject _cube;
        private GameObject _ground;
        private Rect _uiBlockRect; // screen-space rect to ignore touches (UI area)
        private float _cubeSpeed = 3f;
        private float _cubeRotSpeed = 60f;

        public void EnsureScene()
        {
            if (_cube != null)
                return;

            // Cube with unlit material for consistent mobile rendering (avoid CreatePrimitive to not require BoxCollider on iOS)
            _cube = new GameObject("DemoCube");
            var cubeMf = _cube.AddComponent<MeshFilter>();
            var cubeMr = _cube.AddComponent<MeshRenderer>();
            cubeMf.sharedMesh = BuildCubeMesh(1f, 1f, 1f);
            cubeMr.sharedMaterial = CreateColorMaterial(new Color(0.2f, 0.6f, 1f, 1f));
            _cube.transform.position = new Vector3(0f, CubeBaseHeight, 0f);

            // Simple ground using generated quad mesh and unlit material (no MeshCollider to avoid platform issues)
            _ground = new GameObject("DemoGround");
            var mf = _ground.AddComponent<MeshFilter>();
            var mr = _ground.AddComponent<MeshRenderer>();
            mf.sharedMesh = BuildQuadMesh(10f, 10f);
            mr.sharedMaterial = CreateColorMaterial(new Color(0.92f, 0.92f, 0.92f, 1f));
            _ground.transform.position = new Vector3(0, GroundOffsetY, 0);

            var light = new GameObject("DemoLight");
            var l = light.AddComponent<Light>();
            l.type = LightType.Directional;
            l.intensity = 1.0f;
            light.transform.rotation = Quaternion.Euler(55, -35, 0);
            if (Camera.main == null)
            {
                var camObj = new GameObject("DemoCamera");
                var cam = camObj.AddComponent<Camera>();
                cam.transform.position = new Vector3(0, 5, -8);
                cam.transform.LookAt(_cube.transform);
                cam.clearFlags = CameraClearFlags.Skybox;
            }
        }

        public void Tick()
        {
            if (_cube == null)
                return;
            // Keyboard/Controller axes (Editor/Standalone)
            var h = Input.GetAxisRaw("Horizontal");
            var v = Input.GetAxisRaw("Vertical");
            var dir = new Vector3(h, 0, v);

            // Touch drag (Mobile)
            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    var t = Input.GetTouch(i);
                    // Ignore touches over UI panel
                    if (
                        _uiBlockRect.width > 0
                        && _uiBlockRect.height > 0
                        && _uiBlockRect.Contains(t.position)
                    )
                        continue;
                    if (t.phase == TouchPhase.Moved)
                    {
                        var d = t.deltaPosition; // pixels since last frame
                        // Convert to world movement on XZ plane
                        float dpi = Screen.dpi > 0 ? Screen.dpi : 160f;
                        float scale = 1f / Mathf.Max(10f, dpi * 0.8f); // smaller factor on high DPI
                        dir += new Vector3(d.x, 0, d.y) * (scale * 50f); // tuned factor
                    }
                }
            }

            _cube.transform.Translate(dir * (Time.deltaTime * _cubeSpeed), Space.World);
            var pos = _cube.transform.position;
            pos.y = CubeBaseHeight;
            _cube.transform.position = pos;
            _cube.transform.Rotate(0f, _cubeRotSpeed * Time.deltaTime, 0f, Space.World);
        }

        public void SetUiBlockRect(Rect screenRect)
        {
            _uiBlockRect = screenRect;
        }

        private static Mesh BuildQuadMesh(float width, float height)
        {
            var hw = width * 0.5f;
            var hh = height * 0.5f;
            var mesh = new Mesh();
            mesh.vertices = new[]
            {
                new Vector3(-hw, 0, -hh),
                new Vector3(hw, 0, -hh),
                new Vector3(hw, 0, hh),
                new Vector3(-hw, 0, hh),
            };
            mesh.uv = new[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),
            };
            mesh.triangles = new[] { 0, 2, 1, 0, 3, 2 };
            mesh.RecalculateNormals();
            return mesh;
        }

        private static Mesh BuildCubeMesh(float w, float h, float d)
        {
            float x = w * 0.5f,
                y = h * 0.5f,
                z = d * 0.5f;
            var v = new[]
            {
                // Front
                new Vector3(-x, -y, z),
                new Vector3(x, -y, z),
                new Vector3(x, y, z),
                new Vector3(-x, y, z),
                // Back
                new Vector3(x, -y, -z),
                new Vector3(-x, -y, -z),
                new Vector3(-x, y, -z),
                new Vector3(x, y, -z),
                // Left
                new Vector3(-x, -y, -z),
                new Vector3(-x, -y, z),
                new Vector3(-x, y, z),
                new Vector3(-x, y, -z),
                // Right
                new Vector3(x, -y, z),
                new Vector3(x, -y, -z),
                new Vector3(x, y, -z),
                new Vector3(x, y, z),
                // Top
                new Vector3(-x, y, z),
                new Vector3(x, y, z),
                new Vector3(x, y, -z),
                new Vector3(-x, y, -z),
                // Bottom
                new Vector3(-x, -y, -z),
                new Vector3(x, -y, -z),
                new Vector3(x, -y, z),
                new Vector3(-x, -y, z),
            };
            var uv = new[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),
            };
            var t = new int[]
            {
                0,
                2,
                1,
                0,
                3,
                2, // Front
                4,
                6,
                5,
                4,
                7,
                6, // Back
                8,
                10,
                9,
                8,
                11,
                10, // Left
                12,
                14,
                13,
                12,
                15,
                14, // Right
                16,
                18,
                17,
                16,
                19,
                18, // Top
                20,
                22,
                21,
                20,
                23,
                22, // Bottom
            };
            var m = new Mesh();
            m.vertices = v;
            m.uv = uv;
            m.triangles = t;
            m.RecalculateNormals();
            return m;
        }

        private static Material CreateColorMaterial(Color color)
        {
            Shader shader = Shader.Find("Unlit/Color");
            if (shader == null)
            {
                shader = Shader.Find("Sprites/Default");
            }
            if (shader == null)
            {
                shader = Shader.Find("Legacy Shaders/Diffuse");
            }
            var mat = new Material(shader);
            if (mat.HasProperty("_Color"))
                mat.color = color;
            return mat;
        }
    }
}
