using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Maverick.Models
{
    using System.Linq;

    public partial class GuestProfile
    {
        public System.Collections.Generic.IList<string> PreferenceCodes { get; set; }

        public double? TotalPoints { get; set; }

        public string MemberRatecodes { get; set; }

        public System.Collections.Generic.IList<GuestProfileLoyaltyMemberInfo> ExtraMemberPrograms { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "ExtraAddress")]
        public System.Collections.Generic.IList<GuestProfileAddress> ExtraAddress { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "ExtraPhones")]
        public System.Collections.Generic.IList<GuestProfilePhone> ExtraPhones { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "ExtraEmails")]
        public System.Collections.Generic.IList<GuestProfileEmail> ExtraEmails { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "GuestID")]
        public int? GuestID { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "LoyaltyProgramCode")]
        public string LoyaltyProgramCode { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "LoyaltyMemberID")]
        public string LoyaltyMemberID { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "MemberLevel")]
        public string MemberLevel { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "MemberLevelName")]
        public string MemberLevelName { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "CompanyName")]
        public string CompanyName { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "Saluation")]
        public string Saluation { get; set; }

        /// <summary>
        /// Gets or sets guest Lastname
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "LastName")]
        public string LastName { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "FirstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "LanguageCode")]
        public string LanguageCode { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "Active")]
        public bool? Active { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "ExtGuestID")]
        public string ExtGuestID { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "Birthday")]
        public System.DateTime? Birthday { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "Gender")]
        public string Gender { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "MiddleInitial")]
        public string MiddleInitial { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "SignUpTime")]
        public System.DateTime? SignUpTime { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "Address1")]
        public string Address1 { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "Address2")]
        public string Address2 { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "City")]
        public string City { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "Region")]
        public string Region { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "RegionCode")]
        public string RegionCode { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "Zip")]
        public string Zip { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "Country")]
        public string Country { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "PhoneNumber")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "SourceCode")]
        public string SourceCode { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "SubSourceCode")]
        public string SubSourceCode { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "CcToken")]
        public string CcToken { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "Primary")]
        public bool? Primary { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "UnSubscribe")]
        public bool? UnSubscribe { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "WhetherSignUp")]
        public bool? WhetherSignUp { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "Tags")]
        public System.Collections.Generic.IList<string> Tags { get; set; }

    }

    public partial class GuestProfileLoyaltyMemberInfo
    {
        public string LoyaltyProgramCode { get; set; }
        public string LoyaltyMemberID { get; set; }
    }

    public partial class GuestProfileAddress
    {
        public string UseType { get; set; }
        public string LanguageCode { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string RegionCode { get; set; }
        public string ZipCode { get; set; }
        public string CountryCode { get; set; }
    }

    public partial class GuestProfilePhone
    {
        public string UseType { get; set; }
        public string PhoneType { get; set; }
        public string CountryCode { get; set; }
        public string Body { get; set; }
    }

    public partial class GuestProfileEmail
    {
        public string UseType { get; set; }
        public string EmailAddr { get; set; }
    }

    public partial class GuestPointsInfo
    {
        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "TotalNumberOfPoints")]
        public int? TotalNumberOfPoints { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "TotalAmount")]
        public double? TotalAmount { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "CurrencyCode")]
        public string CurrencyCode { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "ResponseCode")]
        public string ResponseCode { get; set; }

        /// <summary>
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "Message")]
        public string Message { get; set; }

    }


}