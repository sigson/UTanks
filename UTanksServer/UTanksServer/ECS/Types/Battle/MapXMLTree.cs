using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UTanksServer.ECS.Types.Battle.AtomicType;

namespace UTanksServer.ECS.Types.Battle
{
    public static class XMLOperations
    {
		public static WorldPoint XMLPosRotToWorldPoint(Position position, Rotation rotation)
        {
			return new WorldPoint() { Position = new Vector3S() { 
				x = float.Parse(position.X, CultureInfo.InvariantCulture),
				z = float.Parse(position.Y, CultureInfo.InvariantCulture),
				y = float.Parse(position.Z, CultureInfo.InvariantCulture)
			}, Rotation = new Vector3S()
			{
				x = float.Parse(rotation.X, CultureInfo.InvariantCulture),
				z = float.Parse(rotation.Y, CultureInfo.InvariantCulture),
				y = MathEx.RadToDeg(float.Parse(rotation.Z, CultureInfo.InvariantCulture))
			}};
		}

		public static Vector3S XMLPosToVector(Position position)
        {
			return new Vector3S()
			{
				x = float.Parse(position.X, CultureInfo.InvariantCulture),
				z = float.Parse(position.Y, CultureInfo.InvariantCulture),
				y = float.Parse(position.Z, CultureInfo.InvariantCulture)
			};

		}

		public static Vector3S XMLPosToVector(Min position)
		{
			return new Vector3S()
			{
				x = (float)position.X,
				z = (float)position.Y,
				y = (float)position.Z
			};

		}

		public static Vector3S XMLPosToVector(Max position)
		{
			return new Vector3S()
			{
				x = (float)position.X,
				z = (float)position.Y,
				y = (float)position.Z
			};

		}

		public static Vector3S XMLPosToVector(Flagblue position)
		{
			return new Vector3S()
			{
				x = (float)position.X,
				z = (float)position.Y,
				y = (float)position.Z
			};

		}

		public static Vector3S XMLPosToVector(Flagred position)
		{
			return new Vector3S()
			{
				x = (float)position.X,
				z = (float)position.Y,
				y = (float)position.Z
			};

		}
	}

	// using System.Xml.Serialization;
	// XmlSerializer serializer = new XmlSerializer(typeof(Map));
	// using (StringReader reader = new StringReader(xml))
	// {
	//    var test = (Map)serializer.Deserialize(reader);
	// }

	[XmlRoot(ElementName = "skybox")]
	public class Skybox
	{
		[XmlAttribute(AttributeName = "val")]
		public string Val { get; set; }
	}

	[XmlRoot(ElementName = "rotation")]
	public class Rotation
	{
		private string Z1;
		[XmlElement(ElementName = "z")]
		public string Z
		{
			get
			{
				return (Z1 == null ? 0f.ToString() : Z1);
			}
			set
			{
				Z1 = value;
			}
		}
		private string X1;
		[XmlElement(ElementName = "x")]
		public string X
		{
			get
			{
				return (X1 == null ? 0f.ToString() : X1);
			}
			set
			{
				X1 = value;
			}
		}
		private string Y1;
		[XmlElement(ElementName = "y")]
		public string Y
		{
			get
			{
				return (Y1 == null ? 0f.ToString() : Y1);
			}
			set
			{
				Y1 = value;
			}
		}
	}

	[XmlRoot(ElementName = "position")]
	public class Position
	{
		private string X1;
		[XmlElement(ElementName = "x")]
		public string X
		{
			get
			{
				return (X1 == null ? 0f.ToString() : X1);
			}
			set
			{
				X1 = value;
			}
		}
		private string Y1;
		[XmlElement(ElementName = "y")]
		public string Y
		{
			get
			{
				return (Y1 == null ? 0f.ToString() : Y1);
			}
			set
			{
				Y1 = value;
			}
		}
		private string Z1;
		[XmlElement(ElementName = "z")]
		public string Z
		{
			get
			{
				return (Z1 == null ? 0f.ToString() : Z1);
			}
			set
			{
				Z1 = value;
			}
		}
	}

	[XmlRoot(ElementName = "prop")]
	public class Prop
	{

		[XmlElement(ElementName = "rotation")]
		public Rotation Rotation { get; set; }

		[XmlElement(ElementName = "texture-name")]
		public string Texturename { get; set; }

		[XmlElement(ElementName = "position")]
		public Position Position { get; set; }

		[XmlAttribute(AttributeName = "library-name")]
		public string LibraryName { get; set; }

		[XmlAttribute(AttributeName = "group-name")]
		public string GroupName { get; set; }

		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }

		//[XmlText]
		//public string Text { get; set; }
	}

	[XmlRoot(ElementName = "static-geometry")]
	public class Staticgeometry
	{

		[XmlElement(ElementName = "prop")]
		public List<Prop> Prop { get; set; }
	}

	[XmlRoot(ElementName = "collision-plane")]
	public class Collisionplane
	{

		[XmlElement(ElementName = "width")]
		public double Width { get; set; }

		[XmlElement(ElementName = "length")]
		public double Length { get; set; }

		[XmlElement(ElementName = "position")]
		public Position Position { get; set; }

		[XmlElement(ElementName = "rotation")]
		public Rotation Rotation { get; set; }

		[XmlAttribute(AttributeName = "id")]
		public int Id { get; set; }

		//[XmlText]
		//public string Text { get; set; }
	}

	[XmlRoot(ElementName = "v0")]
	public class V0
	{

		[XmlElement(ElementName = "x")]
		public double X { get; set; }

		[XmlElement(ElementName = "y")]
		public double Y { get; set; }

		[XmlElement(ElementName = "z")]
		public double Z { get; set; }
	}

	[XmlRoot(ElementName = "v1")]
	public class V1
	{

		[XmlElement(ElementName = "x")]
		public double X { get; set; }

		[XmlElement(ElementName = "y")]
		public double Y { get; set; }

		[XmlElement(ElementName = "z")]
		public double Z { get; set; }
	}

	[XmlRoot(ElementName = "v2")]
	public class V2
	{

		[XmlElement(ElementName = "x")]
		public double X { get; set; }

		[XmlElement(ElementName = "y")]
		public double Y { get; set; }

		[XmlElement(ElementName = "z")]
		public double Z { get; set; }
	}

	[XmlRoot(ElementName = "collision-triangle")]
	public class Collisiontriangle
	{

		[XmlElement(ElementName = "v0")]
		public V0 V0 { get; set; }

		[XmlElement(ElementName = "v1")]
		public V1 V1 { get; set; }

		[XmlElement(ElementName = "v2")]
		public V2 V2 { get; set; }

		[XmlElement(ElementName = "position")]
		public Position Position { get; set; }

		[XmlElement(ElementName = "rotation")]
		public Rotation Rotation { get; set; }

		[XmlAttribute(AttributeName = "id")]
		public int Id { get; set; }

		//[XmlText]
		//public string Text { get; set; }
	}

	[XmlRoot(ElementName = "size")]
	public class Size
	{

		[XmlElement(ElementName = "x")]
		public double X { get; set; }

		[XmlElement(ElementName = "y")]
		public double Y { get; set; }

		[XmlElement(ElementName = "z")]
		public double Z { get; set; }
	}

	[XmlRoot(ElementName = "collision-box")]
	public class Collisionbox
	{

		[XmlElement(ElementName = "size")]
		public Size Size { get; set; }

		[XmlElement(ElementName = "position")]
		public Position Position { get; set; }

		[XmlElement(ElementName = "rotation")]
		public Rotation Rotation { get; set; }

		[XmlAttribute(AttributeName = "id")]
		public int Id { get; set; }

		//[XmlText]
		//public string Text { get; set; }
	}

	[XmlRoot(ElementName = "collision-geometry")]
	public class Collisiongeometry
	{

		[XmlElement(ElementName = "collision-plane")]
		public List<Collisionplane> Collisionplane { get; set; }

		[XmlElement(ElementName = "collision-triangle")]
		public List<Collisiontriangle> Collisiontriangle { get; set; }

		[XmlElement(ElementName = "collision-box")]
		public List<Collisionbox> Collisionbox { get; set; }
	}

	[XmlRoot(ElementName = "spawn-point")]
	public class Spawnpoint
	{

		[XmlElement(ElementName = "rotation")]
		public Rotation Rotation { get; set; }

		[XmlElement(ElementName = "position")]
		public Position Position { get; set; }

		[XmlAttribute(AttributeName = "type")]
		public string Type { get; set; }

		//[XmlText]
		//public string Text { get; set; }
	}

	[XmlRoot(ElementName = "spawn-points")]
	public class Spawnpoints
	{

		[XmlElement(ElementName = "spawn-point")]
		public List<Spawnpoint> Spawnpoint { get; set; }
	}

	[XmlRoot(ElementName = "min")]
	public class Min
	{

		[XmlElement(ElementName = "x")]
		public double X { get; set; }

		[XmlElement(ElementName = "y")]
		public double Y { get; set; }

		[XmlElement(ElementName = "z")]
		public double Z { get; set; }
	}

	[XmlRoot(ElementName = "max")]
	public class Max
	{

		[XmlElement(ElementName = "x")]
		public double X { get; set; }

		[XmlElement(ElementName = "y")]
		public double Y { get; set; }

		[XmlElement(ElementName = "z")]
		public double Z { get; set; }
	}

	[XmlRoot(ElementName = "bonus-region")]
	public class Bonusregion
	{

		[XmlElement(ElementName = "bonus-type")]
		public List<string> Bonustype { get; set; }

		[XmlElement(ElementName = "game-mode")]
		public List<string> Gamemode { get; set; }

		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }

		[XmlAttribute(AttributeName = "parachute")]
		public bool Parachute { get; set; }

		//[XmlText]
		//public string Text { get; set; }

		[XmlElement(ElementName = "rotation")]
		public Rotation Rotation { get; set; }

		[XmlElement(ElementName = "min")]
		public Min Min { get; set; }

		[XmlElement(ElementName = "max")]
		public Max Max { get; set; }

		[XmlElement(ElementName = "position")]
		public Position Position { get; set; }
	}

	[XmlRoot(ElementName = "bonus-regions")]
	public class Bonusregions
	{

		[XmlElement(ElementName = "bonus-region")]
		public List<Bonusregion> Bonusregion { get; set; }
	}

	[XmlRoot(ElementName = "flag-blue")]
	public class Flagblue
	{

		[XmlElement(ElementName = "x")]
		public double X { get; set; }

		[XmlElement(ElementName = "y")]
		public double Y { get; set; }

		[XmlElement(ElementName = "z")]
		public double Z { get; set; }
	}

	[XmlRoot(ElementName = "flag-red")]
	public class Flagred
	{

		[XmlElement(ElementName = "x")]
		public double X { get; set; }

		[XmlElement(ElementName = "y")]
		public double Y { get; set; }

		[XmlElement(ElementName = "z")]
		public double Z { get; set; }
	}

	[XmlRoot(ElementName = "ctf-flags")]
	public class Ctfflags
	{

		[XmlElement(ElementName = "flag-blue")]
		public Flagblue Flagblue { get; set; }

		[XmlElement(ElementName = "flag-red")]
		public Flagred Flagred { get; set; }
	}

	[XmlRoot(ElementName = "dom-keypoint")]
	public class Domkeypoint
	{

		[XmlElement(ElementName = "position")]
		public Position Position { get; set; }

		[XmlAttribute(AttributeName = "free")]
		public bool Free { get; set; }

		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }

		//[XmlText]
		//public string Text { get; set; }
	}

	[XmlRoot(ElementName = "dom-keypoints")]
	public class Domkeypoints
	{

		[XmlElement(ElementName = "dom-keypoint")]
		public List<Domkeypoint> Domkeypoint { get; set; }
	}

	[XmlRoot(ElementName = "map")]
	public class Map
	{
		[XmlElement(ElementName = "skybox")]
		public Skybox Skybox { get; set; }

		[XmlElement(ElementName = "static-geometry")]
		public Staticgeometry Staticgeometry { get; set; }

		[XmlElement(ElementName = "collision-geometry")]
		public Collisiongeometry Collisiongeometry { get; set; }

		[XmlElement(ElementName = "spawn-points")]
		public Spawnpoints Spawnpoints { get; set; }

		[XmlElement(ElementName = "bonus-regions")]
		public Bonusregions Bonusregions { get; set; }

		[XmlElement(ElementName = "special-geometry")]
		public object Specialgeometry { get; set; }

		[XmlElement(ElementName = "ctf-flags")]
		public Ctfflags Ctfflags { get; set; }

		[XmlElement(ElementName = "dom-keypoints")]
		public Domkeypoints Domkeypoints { get; set; }

		[XmlAttribute(AttributeName = "version")]
		public string Version { get; set; }

		//[XmlText]
		//public string Text { get; set; }
	}


}
