using System.Collections.Generic;
using System.Xml.Serialization;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo
#pragma warning disable 1591

namespace GameFinder.StoreHandlers.Origin.DTO
{
	[XmlRoot(ElementName="gameVersion")]
	public class GameVersion { 

		[XmlAttribute(AttributeName="version")] 
		public string? Version { get; set; } 
	}

	[XmlRoot(ElementName="featureFlags")]
	public class FeatureFlags { 

		[XmlAttribute(AttributeName="forceTouchupInstallerAfterUpdate")] 
		public int ForceTouchupInstallerAfterUpdate { get; set; } 

		[XmlAttribute(AttributeName="autoUpdateEnabled")] 
		public int AutoUpdateEnabled { get; set; } 

		[XmlAttribute(AttributeName="treatUpdatesAsMandatory")] 
		public int TreatUpdatesAsMandatory { get; set; } 

		[XmlAttribute(AttributeName="useGameVersionFromManifestEnabled")] 
		public int UseGameVersionFromManifestEnabled { get; set; } 

		[XmlAttribute(AttributeName="enableDifferentialUpdate")] 
		public int EnableDifferentialUpdate { get; set; } 

		[XmlAttribute(AttributeName="dynamicContentSupportEnabled")] 
		public int DynamicContentSupportEnabled { get; set; } 
	}

	[XmlRoot(ElementName="requirements")]
	public class Requirements { 

		[XmlAttribute(AttributeName="osMinVersion")] 
		public double OsMinVersion { get; set; } 

		[XmlAttribute(AttributeName="osReqs64Bit")] 
		public string? OsReqs64Bit { get; set; } 
	}

	[XmlRoot(ElementName="minimumRequiredVersionForOnlineUse")]
	public class MinimumRequiredVersionForOnlineUse { 

		[XmlAttribute(AttributeName="version")] 
		public string? Version { get; set; } 
	}

	[XmlRoot(ElementName="buildMetaData")]
	public class BuildMetaData { 

		[XmlElement(ElementName="gameVersion")] 
		public GameVersion? GameVersion { get; set; } 

		[XmlElement(ElementName="featureFlags")] 
		public FeatureFlags? FeatureFlags { get; set; } 

		[XmlElement(ElementName="requirements")] 
		public Requirements? Requirements { get; set; } 

		[XmlElement(ElementName="minimumRequiredVersionForOnlineUse")] 
		public MinimumRequiredVersionForOnlineUse? MinimumRequiredVersionForOnlineUse { get; set; } 
	}

	[XmlRoot(ElementName="contentIDs")]
	public class ContentIDs { 

		[XmlElement(ElementName="contentID")] 
		public List<int>? ContentID { get; set; } 
	}

	[XmlRoot(ElementName="gameTitle")]
	public class GameTitle { 

		[XmlAttribute(AttributeName="locale")] 
		public string? Locale { get; set; } 

		[XmlText] 
		public string? Text { get; set; } 
	}

	[XmlRoot(ElementName="gameTitles")]
	public class GameTitles { 

		[XmlElement(ElementName="gameTitle")] 
		public List<GameTitle>? GameTitle { get; set; } 
	}

	[XmlRoot(ElementName="uninstall")]
	public class Uninstall { 

		[XmlElement(ElementName="path")] 
		public string? Path { get; set; } 
	}

	[XmlRoot(ElementName="name")]
	public class Name { 

		[XmlAttribute(AttributeName="locale")] 
		public string? Locale { get; set; } 

		[XmlText] 
		public string? Text { get; set; } 
	}

	[XmlRoot(ElementName="launcher")]
	public class Launcher { 

		[XmlElement(ElementName="name")] 
		public List<Name>? Name { get; set; } 

		[XmlElement(ElementName="filePath")] 
		public string? FilePath { get; set; } 

		[XmlElement(ElementName="parameters")] 
		public object? Parameters { get; set; } 

		[XmlElement(ElementName="executeElevated")] 
		public int ExecuteElevated { get; set; } 

		[XmlElement(ElementName="requires64BitOS")] 
		public int Requires64BitOS { get; set; } 

		[XmlAttribute(AttributeName="uid")] 
		public string? Uid { get; set; } 

		[XmlText] 
		public string? Text { get; set; } 
	}

	[XmlRoot(ElementName="runtime")]
	public class Runtime { 

		[XmlElement(ElementName="launcher")] 
		public Launcher? Launcher { get; set; } 
	}

	[XmlRoot(ElementName="touchup")]
	public class Touchup { 

		[XmlElement(ElementName="filePath")] 
		public string? FilePath { get; set; } 

		[XmlElement(ElementName="parameters")] 
		public string? Parameters { get; set; } 

		[XmlElement(ElementName="updateParameters")] 
		public string? UpdateParameters { get; set; } 

		[XmlElement(ElementName="repairParameters")] 
		public string? RepairParameters { get; set; } 
	}

	[XmlRoot(ElementName="DiPManifest")]
	public class DiPManifest { 

		[XmlElement(ElementName="buildMetaData")] 
		public BuildMetaData? BuildMetaData { get; set; } 

		[XmlElement(ElementName="contentIDs")] 
		public ContentIDs? ContentIDs { get; set; } 

		[XmlElement(ElementName="gameTitles")] 
		public GameTitles? GameTitles { get; set; } 

		[XmlElement(ElementName="uninstall")] 
		public Uninstall? Uninstall { get; set; } 

		[XmlElement(ElementName="runtime")] 
		public Runtime? Runtime { get; set; } 

		[XmlElement(ElementName="touchup")] 
		public Touchup? Touchup { get; set; } 

		[XmlAttribute(AttributeName="version")] 
		public double Version { get; set; }
	}
}
