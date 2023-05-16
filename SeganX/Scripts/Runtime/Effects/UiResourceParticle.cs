using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SeganX
{
    [AddComponentMenu("UI/SeganX/ResourceParticle", 20), ExecuteAlways]
    public class UiResourceParticle : MaskableGraphic
    {
        private struct Quad
        {
            public float age;
            public float noise;
            public Vector3 start;
            public Vector3 right;
            public Vector3 position;
        }

        [SerializeField] private Sprite sprite = null;
        [SerializeField] private Gradient colorOverLife = new Gradient();
        [SerializeField] private AnimationCurve sizeOverLife = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0) });
        [SerializeField] private AnimationCurve rotateOverLife = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 0) });
        [SerializeField] private AnimationCurve velocityOverLife = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) });
        [SerializeField] private AnimationCurve noiseOverLife = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 0) });
        [SerializeField] private float life = 2;

        private List<Quad> quads = new List<Quad>(64);

        public override Texture mainTexture => sprite ? sprite.texture : base.mainTexture;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            var uv = sprite ? UnityEngine.Sprites.DataUtility.GetOuterUV(sprite) : Vector4.zero;
            var sw = rectTransform.rect.width * 0.5f;
            var sh = rectTransform.rect.height * 0.5f;

            var quad = new UIVertex[4];

            for (int i = 0; i < quads.Count; i++)
            {
                var current = quads[i];
                if (current.age < 0 || current.age > 1) continue;

                var qrotate = rotateOverLife.Evaluate(current.age) * current.noise;
                var qcolor = colorOverLife.Evaluate(current.age) * color;
                var qsize = sizeOverLife.Evaluate(current.age);
                var qw = sw * qsize;
                var qh = sh * qsize;
                var sinr = Mathf.Sin(qrotate);
                var cosr = Mathf.Cos(qrotate);
                var xsin = qw * sinr;
                var xcos = qw * cosr;
                var ysin = qh * sinr;
                var ycos = qh * cosr;

                quad[0] = UIVertex.simpleVert;
                quad[0].color = qcolor;
                quad[0].position = new Vector3(-xcos + ysin, -xsin - ycos, 0) + current.position;
                quad[0].uv0 = new Vector2(uv.x, uv.y);

                quad[1] = UIVertex.simpleVert;
                quad[1].color = qcolor;
                quad[1].position = new Vector3(-xcos - ysin, -xsin + ycos, 0) + current.position;
                quad[1].uv0 = new Vector2(uv.x, uv.w);

                quad[2] = UIVertex.simpleVert;
                quad[2].color = qcolor;
                quad[2].position = new Vector3(xcos - ysin, xsin + ycos, 0) + current.position;
                quad[2].uv0 = new Vector2(uv.z, uv.w);

                quad[3] = UIVertex.simpleVert;
                quad[3].color = qcolor;
                quad[3].position = new Vector3(xcos + ysin, xsin - ycos, 0) + current.position;
                quad[3].uv0 = new Vector2(uv.z, uv.y);

                vh.AddUIVertexQuad(quad);
            }

#if UNITY_EDITOR
            if (Application.isPlaying == false && quads.Count < 1)
            {
                quad[0] = UIVertex.simpleVert;
                quad[0].color = color;
                quad[0].position = new Vector3(-sw, -sh, 0);
                quad[0].uv0 = new Vector2(uv.x, uv.y);

                quad[1] = UIVertex.simpleVert;
                quad[1].color = color;
                quad[1].position = new Vector3(-sw, sh, 0);
                quad[1].uv0 = new Vector2(uv.x, uv.w);

                quad[2] = UIVertex.simpleVert;
                quad[2].color = color;
                quad[2].position = new Vector3(sw, sh, 0);
                quad[2].uv0 = new Vector2(uv.z, uv.w);

                quad[3] = UIVertex.simpleVert;
                quad[3].color = color;
                quad[3].position = new Vector3(sw, -sh, 0);
                quad[3].uv0 = new Vector2(uv.z, uv.y);

                vh.AddUIVertexQuad(quad);
            }
#endif
        }

        private void Update()
        {
            quads.RemoveAll(x => x.age > 1);

            bool isderty = false;
            for (int i = 0; i < quads.Count; i++)
            {
                var quad = quads[i];
                if (quad.age > 1) continue;

                quad.age += Time.unscaledDeltaTime / life;
                var time = velocityOverLife.Evaluate(quad.age);
                quad.position = Vector2.Lerp(quad.start, Vector3.zero, time);
                quad.position += quad.right * quad.noise * noiseOverLife.Evaluate(time) * 100;

                quads[i] = quad;
                isderty = true;
            }

            if (isderty) SetAllDirty();
        }


        public void Add(Vector2 startPoint)
        {
            quads.Add(new Quad()
            {
                age = 0,
                noise = Random.value * 2 - 1,
                start = startPoint,
                right = Vector3.Cross(Vector3.forward, startPoint).normalized,
                position = startPoint
            });
        }
    }
}