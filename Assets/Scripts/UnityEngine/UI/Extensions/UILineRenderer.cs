using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	[AddComponentMenu("UI/Extensions/Primitives/UILineRenderer")]
	public class UILineRenderer : MaskableGraphic
	{
		[SerializeField]
		private Texture m_Texture;

		[SerializeField]
		private Rect m_UVRect = new Rect(0f, 0f, 1f, 1f);

		public float LineThickness = 2f;

		public bool UseMargins;

		public Vector2 Margin;

		public Vector2[] Points;

		public bool relativeSize;

		public override Texture mainTexture => (!(m_Texture == null)) ? m_Texture : Graphic.s_WhiteTexture;

		public Texture texture
		{
			get
			{
				return m_Texture;
			}
			set
			{
				if (!(m_Texture == value))
				{
					m_Texture = value;
					SetVerticesDirty();
					SetMaterialDirty();
				}
			}
		}

		public Rect uvRect
		{
			get
			{
				return m_UVRect;
			}
			set
			{
				if (!(m_UVRect == value))
				{
					m_UVRect = value;
					SetVerticesDirty();
				}
			}
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			if (Points == null || Points.Length < 2)
			{
				Points = new Vector2[2]
				{
					new Vector2(0f, 0f),
					new Vector2(1f, 1f)
				};
			}
			int num = 10;
			float num2 = base.rectTransform.rect.width;
			float num3 = base.rectTransform.rect.height;
			Vector2 pivot = base.rectTransform.pivot;
			float num4 = (0f - pivot.x) * base.rectTransform.rect.width;
			Vector2 pivot2 = base.rectTransform.pivot;
			float num5 = (0f - pivot2.y) * base.rectTransform.rect.height;
			if (!relativeSize)
			{
				num2 = 1f;
				num3 = 1f;
			}
			List<Vector2> list = new List<Vector2>();
			list.Add(Points[0]);
			Vector2 item = Points[0] + (Points[1] - Points[0]).normalized * num;
			list.Add(item);
			for (int i = 1; i < Points.Length - 1; i++)
			{
				list.Add(Points[i]);
			}
			item = Points[Points.Length - 1] - (Points[Points.Length - 1] - Points[Points.Length - 2]).normalized * num;
			list.Add(item);
			list.Add(Points[Points.Length - 1]);
			Vector2[] array = list.ToArray();
			if (UseMargins)
			{
				num2 -= Margin.x;
				num3 -= Margin.y;
				num4 += Margin.x / 2f;
				num5 += Margin.y / 2f;
			}
			vh.Clear();
			Vector2 vector = Vector2.zero;
			Vector2 vector2 = Vector2.zero;
			for (int j = 1; j < array.Length; j++)
			{
				Vector2 vector3 = array[j - 1];
				Vector2 vector4 = array[j];
				vector3 = new Vector2(vector3.x * num2 + num4, vector3.y * num3 + num5);
				vector4 = new Vector2(vector4.x * num2 + num4, vector4.y * num3 + num5);
				float z = Mathf.Atan2(vector4.y - vector3.y, vector4.x - vector3.x) * 180f / (float)Math.PI;
				Vector2 v = vector3 + new Vector2(0f, (0f - LineThickness) / 2f);
				Vector2 v2 = vector3 + new Vector2(0f, LineThickness / 2f);
				Vector2 v3 = vector4 + new Vector2(0f, LineThickness / 2f);
				Vector2 v4 = vector4 + new Vector2(0f, (0f - LineThickness) / 2f);
				v = RotatePointAroundPivot(v, vector3, new Vector3(0f, 0f, z));
				v2 = RotatePointAroundPivot(v2, vector3, new Vector3(0f, 0f, z));
				v3 = RotatePointAroundPivot(v3, vector4, new Vector3(0f, 0f, z));
				v4 = RotatePointAroundPivot(v4, vector4, new Vector3(0f, 0f, z));
				Vector2 zero = Vector2.zero;
				Vector2 vector5 = new Vector2(0f, 1f);
				Vector2 vector6 = new Vector2(0.5f, 0f);
				Vector2 vector7 = new Vector2(0.5f, 1f);
				Vector2 vector8 = new Vector2(1f, 0f);
				Vector2 vector9 = new Vector2(1f, 1f);
				Vector2[] uvs = new Vector2[4]
				{
					vector6,
					vector7,
					vector7,
					vector6
				};
				if (j > 1)
				{
					vh.AddUIVertexQuad(SetVbo(new Vector2[4]
					{
						vector,
						vector2,
						v,
						v2
					}, uvs));
				}
				if (j == 1)
				{
					uvs = new Vector2[4]
					{
						zero,
						vector5,
						vector7,
						vector6
					};
				}
				else if (j == array.Length - 1)
				{
					uvs = new Vector2[4]
					{
						vector6,
						vector7,
						vector9,
						vector8
					};
				}
				vh.AddUIVertexQuad(SetVbo(new Vector2[4]
				{
					v,
					v2,
					v3,
					v4
				}, uvs));
				vector = v3;
				vector2 = v4;
			}
		}

		protected UIVertex[] SetVbo(Vector2[] vertices, Vector2[] uvs)
		{
			UIVertex[] array = new UIVertex[4];
			for (int i = 0; i < vertices.Length; i++)
			{
				UIVertex simpleVert = UIVertex.simpleVert;
				simpleVert.color = color;
				simpleVert.position = vertices[i];
				simpleVert.uv0 = uvs[i];
				array[i] = simpleVert;
			}
			return array;
		}

		public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
		{
			Vector3 point2 = point - pivot;
			point2 = Quaternion.Euler(angles) * point2;
			point = point2 + pivot;
			return point;
		}
	}
}
