﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using Effekseer.Utl;
using System.Threading;
using System.Threading.Tasks;

namespace Effekseer.Data
{
	public class IO
	{
		public static XmlElement SaveObjectToElement(XmlDocument doc, string element_name, object o)
		{
			XmlElement e_o = doc.CreateElement(element_name);

			var properties = o.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

			foreach (var property in properties)
			{
				var io_attribute = property.GetCustomAttributes(typeof(IOAttribute), false).FirstOrDefault() as IOAttribute;
				if (io_attribute != null && !io_attribute.Export) continue;

				var method = typeof(IO).GetMethod("SaveToElement", new Type[] { typeof(XmlDocument), typeof(string), property.PropertyType });
				if (method != null)
				{
					var property_value = property.GetValue(o, null);
					var element = method.Invoke(null, new object[] { doc, property.Name, property_value });

					if (element != null)
					{
						e_o.AppendChild(element as XmlNode);
					}
				}
				else
				{
					if (io_attribute != null && io_attribute.Export)
					{
						var property_value = property.GetValue(o, null);
						var element = SaveObjectToElement(doc, property.Name, property_value);

						if (element.ChildNodes.Count > 0)
						{
							e_o.AppendChild(element as XmlNode);
						}
					}
				}
			}

			return e_o;
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, NodeBase node)
		{
			return SaveObjectToElement(doc, element_name, node);
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, NodeBase.ChildrenCollection children)
		{
			var e = doc.CreateElement(element_name);
			for (int i = 0; i < children.Count; i++)
			{ 
				var e_node = SaveToElement(doc,children[i].GetType().Name,children[i]);
				e.AppendChild(e_node);
			}

			return e;
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.String value)
		{
			var text = value.GetValue().ToString();
			return doc.CreateTextElement(element_name, text);
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.Boolean value)
		{
			var text = value.GetValue().ToString();
			return doc.CreateTextElement(element_name, text);
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.Int value)
		{
			if (value.Value == value.DefaultValue) return null;
			var text = value.GetValue().ToString();
			return doc.CreateTextElement(element_name, text);
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.Float value)
		{
			var text = value.GetValue().ToString();
			return doc.CreateTextElement(element_name, text);
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.IntWithInifinite value)
		{
			var e = doc.CreateElement(element_name);
			var v = SaveToElement(doc, "Value", value.Value);
			var i = SaveToElement(doc, "Infinite", value.Infinite);
			if (v == null && i == null) return null;
			if (v != null) e.AppendChild(v);
			if (i != null) e.AppendChild(i);
			return e;
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.Vector2D value)
		{
			var e = doc.CreateElement(element_name);
			e.AppendChild(SaveToElement(doc, "X", value.X));
			e.AppendChild(SaveToElement(doc, "Y", value.Y));
			return e;
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.Vector3D value)
		{
			var e = doc.CreateElement(element_name);
			e.AppendChild(SaveToElement(doc, "X", value.X));
			e.AppendChild(SaveToElement(doc, "Y", value.Y));
			e.AppendChild(SaveToElement(doc, "Z", value.Z));
			return e;
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.Color value)
		{
			var e = doc.CreateElement(element_name);
			var r = SaveToElement(doc, "R", value.R);
			var g = SaveToElement(doc, "G", value.G);
			var b = SaveToElement(doc, "B", value.B);
			var a = SaveToElement(doc, "A", value.A);

			if (r == null && g == null && b == null && a == null) return null;
			if (r != null) e.AppendChild(r);
			if (g != null) e.AppendChild(g);
			if (b != null) e.AppendChild(b);
			if (a != null) e.AppendChild(a);
			return e;
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.IntWithRandom value)
		{
			var e = doc.CreateElement(element_name);
			e.AppendChild(doc.CreateTextElement("Center", value.Center.ToString()));
			e.AppendChild(doc.CreateTextElement("Max", value.Max.ToString()));
			e.AppendChild(doc.CreateTextElement("Min", value.Min.ToString()));
			e.AppendChild(doc.CreateTextElement("DrawnAs", (int)value.DrawnAs));
			return e;
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.FloatWithRandom value)
		{
			var e = doc.CreateElement(element_name);
			e.AppendChild(doc.CreateTextElement("Center", value.Center.ToString()));
			e.AppendChild(doc.CreateTextElement("Max", value.Max.ToString()));
			e.AppendChild(doc.CreateTextElement("Min", value.Min.ToString()));
			e.AppendChild(doc.CreateTextElement("DrawnAs", (int)value.DrawnAs));
			return e;
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.Vector2DWithRandom value)
		{
			var e = doc.CreateElement(element_name);
			e.AppendChild(SaveToElement(doc, "X", value.X));
			e.AppendChild(SaveToElement(doc, "Y", value.Y));
			e.AppendChild(doc.CreateTextElement("DrawnAs", (int)value.DrawnAs));
			return e;
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.Vector3DWithRandom value)
		{
			var e = doc.CreateElement(element_name);
			e.AppendChild(SaveToElement(doc, "X", value.X));
			e.AppendChild(SaveToElement(doc, "Y", value.Y));
			e.AppendChild(SaveToElement(doc, "Z", value.Z));
			e.AppendChild(doc.CreateTextElement("DrawnAs", (int)value.DrawnAs));
			return e;
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.ColorWithRandom value)
		{
			var e = doc.CreateElement(element_name);
			e.AppendChild(SaveToElement(doc, "R", value.R));
			e.AppendChild(SaveToElement(doc, "G", value.G));
			e.AppendChild(SaveToElement(doc, "B", value.B));
			e.AppendChild(SaveToElement(doc, "A", value.A));
			e.AppendChild(doc.CreateTextElement("DrawnAs", (int)value.DrawnAs));
			e.AppendChild(doc.CreateTextElement("ColorSpace", (int)value.ColorSpace));
			return e;
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.EnumBase value)
		{
			var text = value.GetValueAsInt().ToString();
			return doc.CreateTextElement(element_name, text);
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.Path value)
		{
			var text = value.GetRelativePath();
			return doc.CreateTextElement(element_name, text);
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.FCurveVector2D value)
		{
			var e = doc.CreateElement(element_name);
			var keys = doc.CreateElement("Keys");
			var x = doc.CreateElement("X");
			var y = doc.CreateElement("Y");

			int index = 0;

			Action<Value.FCurve<float>, XmlElement> setValues = (v, xml) =>
				{
					index = 0;
					xml.AppendChild(doc.CreateTextElement("StartType", v.StartType.GetValueAsInt()));
					xml.AppendChild(doc.CreateTextElement("EndType", v.EndType.GetValueAsInt()));
					xml.AppendChild(doc.CreateTextElement("OffsetMax", v.OffsetMax.Value.ToString()));
					xml.AppendChild(doc.CreateTextElement("OffsetMin", v.OffsetMin.Value.ToString()));
					xml.AppendChild(doc.CreateTextElement("Sampling", v.Sampling.Value.ToString()));

					foreach (var k_ in v.Keys)
					{
						var k = doc.CreateElement("Key" + index.ToString());
						k.AppendChild(doc.CreateTextElement("Frame", k_.Frame.ToString()));
						k.AppendChild(doc.CreateTextElement("Value", k_.ValueAsFloat.ToString()));
						k.AppendChild(doc.CreateTextElement("LeftX", k_.LeftX.ToString()));
						k.AppendChild(doc.CreateTextElement("LeftY", k_.LeftY.ToString()));
						k.AppendChild(doc.CreateTextElement("RightX", k_.RightX.ToString()));
						k.AppendChild(doc.CreateTextElement("RightY", k_.RightY.ToString()));

						k.AppendChild(doc.CreateTextElement("InterpolationType", k_.InterpolationType.GetValueAsInt()));

						xml.AppendChild(k);
						index++;
					}
				};

			setValues(value.X, x);
			setValues(value.Y, y);

			keys.AppendChild(x);
			keys.AppendChild(y);
			e.AppendChild(keys);

			return e;
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.FCurveVector3D value)
		{
			var e = doc.CreateElement(element_name);
			var keys = doc.CreateElement("Keys");
			var x = doc.CreateElement("X");
			var y = doc.CreateElement("Y");
			var z = doc.CreateElement("Z");

			int index = 0;

			Action<Value.FCurve<float>, XmlElement> setValues = (v, xml) =>
			{
				index = 0;
				xml.AppendChild(doc.CreateTextElement("StartType", v.StartType.GetValueAsInt()));
				xml.AppendChild(doc.CreateTextElement("EndType", v.EndType.GetValueAsInt()));
				xml.AppendChild(doc.CreateTextElement("OffsetMax", v.OffsetMax.Value.ToString()));
				xml.AppendChild(doc.CreateTextElement("OffsetMin", v.OffsetMin.Value.ToString()));
				xml.AppendChild(doc.CreateTextElement("Sampling", v.Sampling.Value.ToString()));

				foreach (var k_ in v.Keys)
				{
					var k = doc.CreateElement("Key" + index.ToString());
					k.AppendChild(doc.CreateTextElement("Frame", k_.Frame.ToString()));
					k.AppendChild(doc.CreateTextElement("Value", k_.ValueAsFloat.ToString()));
					k.AppendChild(doc.CreateTextElement("LeftX", k_.LeftX.ToString()));
					k.AppendChild(doc.CreateTextElement("LeftY", k_.LeftY.ToString()));
					k.AppendChild(doc.CreateTextElement("RightX", k_.RightX.ToString()));
					k.AppendChild(doc.CreateTextElement("RightY", k_.RightY.ToString()));

					k.AppendChild(doc.CreateTextElement("InterpolationType", k_.InterpolationType.GetValueAsInt()));

					xml.AppendChild(k);
					index++;
				}
			};

			setValues(value.X, x);
			setValues(value.Y, y);
			setValues(value.Z, z);

			keys.AppendChild(x);
			keys.AppendChild(y);
			keys.AppendChild(z);
			e.AppendChild(keys);

			return e;
		}

		public static XmlElement SaveToElement(XmlDocument doc, string element_name, Value.FCurveColorRGBA value)
		{
			var e = doc.CreateElement(element_name);
			var keys = doc.CreateElement("Keys");
			var r = doc.CreateElement("R");
			var g = doc.CreateElement("G");
			var b = doc.CreateElement("B");
			var a = doc.CreateElement("A");

			int index = 0;

			Action<Value.FCurve<byte>, XmlElement> setValues = (v, xml) =>
			{
				index = 0;
				xml.AppendChild(doc.CreateTextElement("StartType", v.StartType.GetValueAsInt()));
				xml.AppendChild(doc.CreateTextElement("EndType", v.EndType.GetValueAsInt()));
				xml.AppendChild(doc.CreateTextElement("OffsetMax", v.OffsetMax.Value.ToString()));
				xml.AppendChild(doc.CreateTextElement("OffsetMin", v.OffsetMin.Value.ToString()));
				xml.AppendChild(doc.CreateTextElement("Sampling", v.Sampling.Value.ToString()));

				foreach (var k_ in v.Keys)
				{
					var k = doc.CreateElement("Key" + index.ToString());
					k.AppendChild(doc.CreateTextElement("Frame", k_.Frame.ToString()));
					k.AppendChild(doc.CreateTextElement("Value", k_.ValueAsFloat.ToString()));
					k.AppendChild(doc.CreateTextElement("LeftX", k_.LeftX.ToString()));
					k.AppendChild(doc.CreateTextElement("LeftY", k_.LeftY.ToString()));
					k.AppendChild(doc.CreateTextElement("RightX", k_.RightX.ToString()));
					k.AppendChild(doc.CreateTextElement("RightY", k_.RightY.ToString()));

					k.AppendChild(doc.CreateTextElement("InterpolationType", k_.InterpolationType.GetValueAsInt()));

					xml.AppendChild(k);
					index++;
				}
			};

			setValues(value.R, r);
			setValues(value.G, g);
			setValues(value.B, b);
			setValues(value.A, a);

			keys.AppendChild(r);
			keys.AppendChild(g);
			keys.AppendChild(b);
			keys.AppendChild(a);			

			e.AppendChild(keys);

			return e;
		}

		public static void LoadObjectFromElement(XmlElement e, ref object o)
		{
			var o_type = o.GetType();

			foreach (var _ch_node in e.ChildNodes)
			{
				var ch_node = _ch_node as XmlElement;
				var local_name = ch_node.LocalName;

				var property = o_type.GetProperty(local_name);
				if (property == null) continue;

				var io_attribute = property.GetCustomAttributes(typeof(IOAttribute), false).FirstOrDefault() as IOAttribute;
				if (io_attribute != null && !io_attribute.Import) continue;

				var method = typeof(IO).GetMethod("LoadFromElement", new Type[] { typeof(XmlElement), property.PropertyType });
				if (method != null)
				{
					var property_value = property.GetValue(o, null);
					method.Invoke(null, new object[] { ch_node, property_value });
				}
				else
				{
					if (io_attribute != null && io_attribute.Import)
					{
						var property_value = property.GetValue(o, null);
						LoadObjectFromElement(ch_node, ref property_value);
					}
				}
			}
		}

		public static void LoadFromElement(XmlElement e, NodeBase node)
		{
			var o = node as object;
			LoadObjectFromElement(e, ref o);
		}

		public static void LoadFromElement(XmlElement e, NodeBase.ChildrenCollection children)
		{
			children.Node.ClearChildren();

			for (var i = 0; i < e.ChildNodes.Count; i++)
			{
				var e_child = e.ChildNodes[i] as XmlElement;
				if (e_child.LocalName != "Node") continue;

				var node = children.Node.AddChild();
				LoadFromElement(e_child, node);
			}
		}

		public static void LoadFromElement(XmlElement e, Value.String value)
		{
			var text = e.GetText();
			value.SetValue(text);
		}

		public static void LoadFromElement(XmlElement e, Value.Boolean value)
		{
			var text = e.GetText();
			var parsed = false;
			if (bool.TryParse(text, out parsed))
			{
				value.SetValue(parsed);
			}
		}

		public static void LoadFromElement(XmlElement e, Value.Int value)
		{
			var text = e.GetText();
			var parsed = 0;
			if (int.TryParse(text, System.Globalization.NumberStyles.Integer, Setting.NFI, out parsed))
			{
				value.SetValue(parsed);
			}
		}

		public static void LoadFromElement(XmlElement e, Value.Float value)
		{
			var text = e.GetText();
			var parsed = 0.0f;
			if (float.TryParse(text, System.Globalization.NumberStyles.Float, Setting.NFI, out parsed))
			{
				value.SetValue(parsed);
			}
		}

		public static void LoadFromElement(XmlElement e, Value.IntWithInifinite value)
		{
			var e_value = e["Value"] as XmlElement;
			var e_infinite = e["Infinite"] as XmlElement;

			if (e_value != null) LoadFromElement(e_value, value.Value);
			if (e_infinite != null) LoadFromElement(e_infinite, value.Infinite);
		}

		public static void LoadFromElement(XmlElement e, Value.Vector2D value)
		{
			var e_x = e["X"] as XmlElement;
			var e_y = e["Y"] as XmlElement;

			if (e_x != null) LoadFromElement(e_x, value.X);
			if (e_y != null) LoadFromElement(e_y, value.Y);
		}

		public static void LoadFromElement(XmlElement e, Value.Vector3D value)
		{
			var e_x = e["X"] as XmlElement;
			var e_y = e["Y"] as XmlElement;
			var e_z = e["Z"] as XmlElement;

			if (e_x != null) LoadFromElement(e_x, value.X);
			if (e_y != null) LoadFromElement(e_y, value.Y);
			if (e_z != null) LoadFromElement(e_z, value.Z);
		}

		public static void LoadFromElement(XmlElement e, Value.Color value)
		{
			var e_r = e["R"] as XmlElement;
			var e_g = e["G"] as XmlElement;
			var e_b = e["B"] as XmlElement;
			var e_a = e["A"] as XmlElement;

			if (e_r != null) LoadFromElement(e_r, value.R);
			if (e_g != null) LoadFromElement(e_g, value.G);
			if (e_b != null) LoadFromElement(e_b, value.B);
			if (e_a != null) LoadFromElement(e_a, value.A);
		}

		public static void LoadFromElement(XmlElement e, Value.IntWithRandom value)
		{
			var e_c = e["Center"];
			var e_max = e["Max"];
			var e_min = e["Min"];
			var e_da = e["DrawnAs"];

			if (e_c != null)
			{
				var center = e_c.GetTextAsInt();
				value.SetCenter(center);
			}

			if (e_max != null)
			{
				var max = e_max.GetTextAsInt();
				value.SetMax(max);
			}

			if (e_min != null)
			{
				var min = e_min.GetTextAsInt();
				value.SetMin(min);
			}

			if (e_da != null)
			{
				value.DrawnAs = (DrawnAs)e_da.GetTextAsInt();
			}
		}

		public static void LoadFromElement(XmlElement e, Value.FloatWithRandom value)
		{
			var e_c = e["Center"];
			var e_max = e["Max"];
			var e_min = e["Min"];
			var e_da = e["DrawnAs"];

			if (e_c != null)
			{
				var center = e_c.GetTextAsFloat();
				value.SetCenter(center);
			}

			if (e_max != null)
			{
				var max = e_max.GetTextAsFloat();
				value.SetMax(max);
			}

			if (e_min != null)
			{
				var min = e_min.GetTextAsFloat();
				value.SetMin(min);
			}

			if (e_da != null)
			{
				value.DrawnAs = (DrawnAs)e_da.GetTextAsInt();
			}
		}

		public static void LoadFromElement(XmlElement e, Value.Vector2DWithRandom value)
		{
			var e_x = e["X"] as XmlElement;
			var e_y = e["Y"] as XmlElement;
			var e_da = e["DrawnAs"];

			if (e_x != null) LoadFromElement(e_x, value.X);
			if (e_y != null) LoadFromElement(e_y, value.Y);

			if (e_da != null)
			{
				value.DrawnAs = (DrawnAs)e_da.GetTextAsInt();
			}
		}

		public static void LoadFromElement(XmlElement e, Value.Vector3DWithRandom value)
		{
			var e_x = e["X"] as XmlElement;
			var e_y = e["Y"] as XmlElement;
			var e_z = e["Z"] as XmlElement;
			var e_da = e["DrawnAs"];

			if (e_x != null) LoadFromElement(e_x, value.X);
			if (e_y != null) LoadFromElement(e_y, value.Y);
			if (e_z != null) LoadFromElement(e_z, value.Z);

			if (e_da != null)
			{
				value.DrawnAs = (DrawnAs)e_da.GetTextAsInt();
			}
		}

		public static void LoadFromElement(XmlElement e, Value.ColorWithRandom value)
		{
			var e_r = e["R"] as XmlElement;
			var e_g = e["G"] as XmlElement;
			var e_b = e["B"] as XmlElement;
			var e_a = e["A"] as XmlElement;
			var e_da = e["DrawnAs"];
			var e_cs = e["ColorSpace"];

			if (e_r != null) LoadFromElement(e_r, value.R);
			if (e_g != null) LoadFromElement(e_g, value.G);
			if (e_b != null) LoadFromElement(e_b, value.B);
			if (e_a != null) LoadFromElement(e_a, value.A);

			if (e_da != null)
			{
				value.DrawnAs = (DrawnAs)e_da.GetTextAsInt();
			}

			if (e_cs != null)
			{
				value.ColorSpace = (ColorSpace)e_cs.GetTextAsInt();
			}
		}

		public static void LoadFromElement(XmlElement e, Value.Path value)
		{
			var text = e.GetText();
			value.SetRelativePath(text);
		}

		public static void LoadFromElement(XmlElement e, Value.EnumBase value)
		{
			var text = e.GetText();
			var parsed = 0;
			if (int.TryParse(text, out parsed))
			{
				value.SetValue(parsed);
			}
		}

		public static void LoadFromElement(XmlElement e, Value.FCurveVector2D value)
		{
			var e_keys = e["Keys"] as XmlElement;
			if (e_keys == null) return;

			var e_x = e_keys["X"] as XmlElement;
			var e_y = e_keys["Y"] as XmlElement;

			Action<Data.Value.FCurve<float>, XmlElement> import = (v_, e_) =>
			{
				foreach (XmlElement r in e_.ChildNodes)
				{
					if (r.Name.StartsWith("Key"))
					{
						var f = r.GetTextAsInt("Frame");
						var v = r.GetTextAsFloat("Value");
						var lx = r.GetTextAsFloat("LeftX");
						var ly = r.GetTextAsFloat("LeftY");
						var rx = r.GetTextAsFloat("RightX");
						var ry = r.GetTextAsFloat("RightY");
						var i = r.GetTextAsInt("InterpolationType");
						var s = r.GetTextAsInt("Sampling");

						var t = new Value.FCurveKey<float>(f, v);
						t.SetLeftDirectly(lx, ly);
						t.SetRightDirectly(rx, ry);
						t.InterpolationType.SetValue(i);

						v_.AddKeyDirectly(t);
					}
					else if (r.Name.StartsWith("StartType"))
					{
						var v = r.GetTextAsInt();
						v_.StartType.SetValue(v);
					}
					else if (r.Name.StartsWith("EndType"))
					{
						var v = r.GetTextAsInt();
						v_.EndType.SetValue(v);
					}
					else if (r.Name.StartsWith("OffsetMax"))
					{
						var v = r.GetTextAsFloat();
						v_.OffsetMax.SetValueDirectly(v);
					}
					else if (r.Name.StartsWith("OffsetMin"))
					{
						var v = r.GetTextAsFloat();
						v_.OffsetMin.SetValueDirectly(v);
					}
					else if (r.Name.StartsWith("Sampling"))
					{
						var v = r.GetTextAsInt();
						v_.Sampling.SetValueDirectly(v);
					}
				}
			};

			import(value.X, e_x);
			import(value.Y, e_y);
		}

		public static void LoadFromElement(XmlElement e, Value.FCurveVector3D value)
		{
			var e_keys = e["Keys"] as XmlElement;
			if (e_keys == null) return;

			var e_x = e_keys["X"] as XmlElement;
			var e_y = e_keys["Y"] as XmlElement;
			var e_z = e_keys["Z"] as XmlElement;

			Action<Data.Value.FCurve<float>, XmlElement> import = (v_, e_) =>
			{
				foreach (XmlElement r in e_.ChildNodes)
				{
					if (r.Name.StartsWith("Key"))
					{
						var f = r.GetTextAsInt("Frame");
						var v = r.GetTextAsFloat("Value");
						var lx = r.GetTextAsFloat("LeftX");
						var ly = r.GetTextAsFloat("LeftY");
						var rx = r.GetTextAsFloat("RightX");
						var ry = r.GetTextAsFloat("RightY");
						var i = r.GetTextAsInt("InterpolationType");

						var t = new Value.FCurveKey<float>(f, v);
						t.SetLeftDirectly(lx, ly);
						t.SetRightDirectly(rx, ry);
						t.InterpolationType.SetValue(i);

						v_.AddKeyDirectly(t);
					}
					else if (r.Name.StartsWith("StartType"))
					{
						var v = r.GetTextAsInt();
						v_.StartType.SetValue(v);
					}
					else if (r.Name.StartsWith("EndType"))
					{
						var v = r.GetTextAsInt();
						v_.EndType.SetValue(v);
					}
					else if (r.Name.StartsWith("OffsetMax"))
					{
						var v = r.GetTextAsFloat();
						v_.OffsetMax.SetValueDirectly(v);
					}
					else if (r.Name.StartsWith("OffsetMin"))
					{
						var v = r.GetTextAsFloat();
						v_.OffsetMin.SetValueDirectly(v);
					}
					else if (r.Name.StartsWith("Sampling"))
					{
						var v = r.GetTextAsInt();
						v_.Sampling.SetValueDirectly(v);
					}
				}
			};

			import(value.X, e_x);
			import(value.Y, e_y);
			import(value.Z, e_z);
		}

		public static void LoadFromElement(XmlElement e, Value.FCurveColorRGBA value)
		{
			Action<Data.Value.FCurve<byte>, XmlElement> import = (v_, e_) =>
				{
					foreach (XmlElement r in e_.ChildNodes)
					{
						if (r.Name.StartsWith("Key"))
						{
							var f = r.GetTextAsInt("Frame");
							var v = r.GetTextAsFloat("Value");
							var lx = r.GetTextAsFloat("LeftX");
							var ly = r.GetTextAsFloat("LeftY");
							var rx = r.GetTextAsFloat("RightX");
							var ry = r.GetTextAsFloat("RightY");
							var i = r.GetTextAsInt("InterpolationType");

							var t = new Value.FCurveKey<byte>(f, (byte)v);
							t.SetLeftDirectly(lx, ly);
							t.SetRightDirectly(rx, ry);
							t.InterpolationType.SetValue(i);

							v_.AddKeyDirectly(t);
						}
						else if (r.Name.StartsWith("StartType"))
						{
							var v = r.GetTextAsInt();
							v_.StartType.SetValue(v);
						}
						else if (r.Name.StartsWith("EndType"))
						{
							var v = r.GetTextAsInt();
							v_.EndType.SetValue(v);
						}
						else if (r.Name.StartsWith("OffsetMax"))
						{
							var v = r.GetTextAsFloat();
							v_.OffsetMax.SetValueDirectly(v);
						}
						else if (r.Name.StartsWith("OffsetMin"))
						{
							var v = r.GetTextAsFloat();
							v_.OffsetMin.SetValueDirectly(v);
						}
						else if (r.Name.StartsWith("Sampling"))
						{
							var v = r.GetTextAsInt();
							v_.Sampling.SetValueDirectly(v);
						}
					}
				};


			var e_keys = e["Keys"] as XmlElement;
			if (e_keys == null) return;

			var e_r = e_keys["R"] as XmlElement;
			var e_g = e_keys["G"] as XmlElement;
			var e_b = e_keys["B"] as XmlElement;
			var e_a = e_keys["A"] as XmlElement;

			import(value.R, e_r);
			import(value.G, e_g);
			import(value.B, e_b);
			import(value.A, e_a);
		}
	}
}
