namespace SponsorMyAthlete.Infrastructure.Config;

public class AbrOptions
{
    public const string SectionName = "Abr";

    /// <summary>ABR web services GUID — register free at https://abr.business.gov.au/Tools/WebServices</summary>
    public string Guid { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://abr.business.gov.au/abrxmlsearch/AbrXmlSearch.asmx";
}
