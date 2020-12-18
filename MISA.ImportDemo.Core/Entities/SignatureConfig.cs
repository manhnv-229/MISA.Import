using System;
using System.Collections.Generic;

namespace MISA.ImportDemo.Core.Entities
{
    public partial class SignatureConfig
    {
        public string EsignAccessToken { get; set; }
        public string EsignRefreshToken { get; set; }
        public ulong? IsEsign { get; set; }
        public ulong? IsUsb { get; set; }
        public ulong? IsOther { get; set; }
        public string EsignCertId { get; set; }
        public string SerialNumber { get; set; }
        public string IssuerName { get; set; }
        public string SubjectName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public Guid OrganizationId { get; set; }
    }
}
