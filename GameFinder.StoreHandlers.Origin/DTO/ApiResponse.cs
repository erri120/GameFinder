using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo
#pragma warning disable 1591

namespace GameFinder.StoreHandlers.Origin.DTO
{
    public class Attributes
    {
        [JsonPropertyName("isPurchasable")]
        public bool IsPurchasable { get; set; }
    }

    public class Country
    {
        [JsonPropertyName("attributes")]
        public Attributes? Attributes { get; set; }

        [JsonPropertyName("countryCode")]
        public string? CountryCode { get; set; }
    }

    public class Countries
    {
        [JsonPropertyName("country")]
        public List<Country>? Country { get; set; }
    }

    public class BaseAttributes
    {
        [JsonPropertyName("platform")]
        public string? Platform { get; set; }
    }

    public class CustomAttributes
    {
        [JsonPropertyName("imageServer")]
        public string? ImageServer { get; set; }

        [JsonPropertyName("suppressVaultUpgrade")]
        public bool SuppressVaultUpgrade { get; set; }

        [JsonPropertyName("gameEditionTypeFacetKeyRankDesc")]
        public string? GameEditionTypeFacetKeyRankDesc { get; set; }
    }

    public class LocalizableAttributes
    {
        [JsonPropertyName("longDescription")]
        public string? LongDescription { get; set; }

        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("shortDescription")]
        public string? ShortDescription { get; set; }

        [JsonPropertyName("packArtSmall")]
        public string? PackArtSmall { get; set; }

        [JsonPropertyName("packArtMedium")]
        public string? PackArtMedium { get; set; }

        [JsonPropertyName("packArtLarge")]
        public string? PackArtLarge { get; set; }
    }

    public class PublishingAttributes
    {
        [JsonPropertyName("contentId")]
        public string? ContentId { get; set; }

        [JsonPropertyName("greyMarketControls")]
        public bool GreyMarketControls { get; set; }

        [JsonPropertyName("isDownloadable")]
        public bool IsDownloadable { get; set; }

        [JsonPropertyName("gameDistributionSubType")]
        public string? GameDistributionSubType { get; set; }

        [JsonPropertyName("originDisplayType")]
        public string? OriginDisplayType { get; set; }

        [JsonPropertyName("isPublished")]
        public bool IsPublished { get; set; }
    }

    public class FulfillmentAttributes
    {
        [JsonPropertyName("achievementSetOverride")]
        public string? AchievementSetOverride { get; set; }

        [JsonPropertyName("cloudSaveConfigurationOverride")]
        public string? CloudSaveConfigurationOverride { get; set; }

        [JsonPropertyName("commerceProfile")]
        public string? CommerceProfile { get; set; }

        [JsonPropertyName("downloadPackageType")]
        public string? DownloadPackageType { get; set; }

        [JsonPropertyName("enableDLCuninstall")]
        public bool EnableDLCuninstall { get; set; }

        [JsonPropertyName("executePathOverride")]
        public string? ExecutePathOverride { get; set; }

        [JsonPropertyName("oigClientBehavior")]
        public string? OigClientBehavior { get; set; }

        [JsonPropertyName("installationDirectory")]
        public string? InstallationDirectory { get; set; }

        [JsonPropertyName("installCheckOverride")]
        public string? InstallCheckOverride { get; set; }

        [JsonPropertyName("monitorInstall")]
        public bool MonitorInstall { get; set; }

        [JsonPropertyName("monitorPlay")]
        public bool MonitorPlay { get; set; }

        [JsonPropertyName("multiPlayerId")]
        public string? MultiPlayerId { get; set; }

        [JsonPropertyName("processorArchitecture")]
        public string? ProcessorArchitecture { get; set; }

        [JsonPropertyName("showSubsSaveGameWarning")]
        public bool ShowSubsSaveGameWarning { get; set; }
    }

    public class DownloadURL
    {
        [JsonPropertyName("buildReleaseVersion")]
        public string? BuildReleaseVersion { get; set; }

        [JsonPropertyName("buildMetaData")]
        public string? BuildMetaData { get; set; }

        [JsonPropertyName("effectiveDate")]
        public DateTime EffectiveDate { get; set; }

        [JsonPropertyName("downloadURL")]
        public string? URL { get; set; }

        [JsonPropertyName("downloadURLType")]
        public string? DownloadURLType { get; set; }
    }

    public class DownloadURLs
    {
        [JsonPropertyName("downloadURL")]
        public List<DownloadURL>? DownloadURL { get; set; }
    }

    public class Software
    {
        [JsonPropertyName("softwareId")]
        public string? SoftwareId { get; set; }

        [JsonPropertyName("fulfillmentAttributes")]
        public FulfillmentAttributes? FulfillmentAttributes { get; set; }

        [JsonPropertyName("downloadURLs")]
        public DownloadURLs? DownloadURLs { get; set; }

        [JsonPropertyName("softwarePlatform")]
        public string? SoftwarePlatform { get; set; }
    }

    public class SoftwareList
    {
        [JsonPropertyName("software")]
        public List<Software>? Software { get; set; }
    }

    public class Locale
    {
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public class SoftwareLocales
    {
        [JsonPropertyName("locale")]
        public List<Locale>? Locale { get; set; }
    }

    public class SoftwareControlDate
    {
        [JsonPropertyName("releaseDate")]
        public DateTime ReleaseDate { get; set; }

        [JsonPropertyName("downloadStartDate")]
        public DateTime DownloadStartDate { get; set; }

        [JsonPropertyName("platform")]
        public string? Platform { get; set; }
    }

    public class SoftwareControlDates
    {
        [JsonPropertyName("softwareControlDate")]
        public List<SoftwareControlDate>? SoftwareControlDate { get; set; }
    }

    public class StoreControlDate
    {
        [JsonPropertyName("storeAvailableStartDate")]
        public DateTime StoreAvailableStartDate { get; set; }
    }

    public class StoreControlDates
    {
        [JsonPropertyName("storeControlDate")]
        public List<StoreControlDate>? StoreControlDate { get; set; }
    }

    public class Publishing
    {
        [JsonPropertyName("publishingAttributes")]
        public PublishingAttributes? PublishingAttributes { get; set; }

        [JsonPropertyName("softwareList")]
        public SoftwareList? SoftwareList { get; set; }

        [JsonPropertyName("softwareLocales")]
        public SoftwareLocales? SoftwareLocales { get; set; }

        [JsonPropertyName("softwareControlDates")]
        public SoftwareControlDates? SoftwareControlDates { get; set; }

        [JsonPropertyName("storeControlDates")]
        public StoreControlDates? StoreControlDates { get; set; }
    }

    public class MdmMasterTitle
    {
        [JsonPropertyName("masterTitleId")]
        public int MasterTitleId { get; set; }

        [JsonPropertyName("masterTitle")]
        public string? MasterTitle { get; set; }
    }

    public class MdmFranchise
    {
        [JsonPropertyName("franchiseId")]
        public int FranchiseId { get; set; }

        [JsonPropertyName("franchise")]
        public string? Franchise { get; set; }
    }

    public class MdmHierarchy
    {
        [JsonPropertyName("mdmMasterTitle")]
        public MdmMasterTitle? MdmMasterTitle { get; set; }

        [JsonPropertyName("mdmFranchise")]
        public MdmFranchise? MdmFranchise { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }

    public class MdmHierarchies
    {
        [JsonPropertyName("mdmHierarchy")]
        public List<MdmHierarchy>? MdmHierarchy { get; set; }
    }

    public class EcommerceAttributes
    {
        [JsonPropertyName("cdnAssetRoot")]
        public string? CdnAssetRoot { get; set; }

        [JsonPropertyName("addonsAvailable")]
        public bool AddonsAvailable { get; set; }

        [JsonPropertyName("availableExtraContent")]
        public List<string>? AvailableExtraContent { get; set; }
    }

    public class FirstParty
    {
        [JsonPropertyName("partner")]
        public string? Partner { get; set; }

        [JsonPropertyName("partnerIdType")]
        public string? PartnerIdType { get; set; }

        [JsonPropertyName("partnerId")]
        public string? PartnerId { get; set; }
    }

    public class FirstParties
    {
        [JsonPropertyName("firstParty")]
        public List<FirstParty>? FirstParty { get; set; }
    }

    public class ApiResponse
    {
        [JsonPropertyName("itemId")]
        public string? ItemId { get; set; }

        [JsonPropertyName("storeGroupId")]
        public string? StoreGroupId { get; set; }

        [JsonPropertyName("financeId")]
        public string? FinanceId { get; set; }

        [JsonPropertyName("defaultLocale")]
        public string? DefaultLocale { get; set; }

        [JsonPropertyName("countries")]
        public Countries? Countries { get; set; }

        [JsonPropertyName("baseAttributes")]
        public BaseAttributes? BaseAttributes { get; set; }

        [JsonPropertyName("customAttributes")]
        public CustomAttributes? CustomAttributes { get; set; }

        [JsonPropertyName("localizableAttributes")]
        public LocalizableAttributes? LocalizableAttributes { get; set; }

        [JsonPropertyName("publishing")]
        public Publishing? Publishing { get; set; }

        [JsonPropertyName("mdmHierarchies")]
        public MdmHierarchies? MdmHierarchies { get; set; }

        [JsonPropertyName("ecommerceAttributes")]
        public EcommerceAttributes? EcommerceAttributes { get; set; }

        [JsonPropertyName("updatedDate")]
        public DateTime UpdatedDate { get; set; }

        [JsonPropertyName("firstParties")]
        public FirstParties? FirstParties { get; set; }

        [JsonPropertyName("itemName")]
        public string? ItemName { get; set; }

        [JsonPropertyName("offerType")]
        public string? OfferType { get; set; }

        [JsonPropertyName("offerId")]
        public string? OfferId { get; set; }

        [JsonPropertyName("projectNumber")]
        public string? ProjectNumber { get; set; }
    }
}
