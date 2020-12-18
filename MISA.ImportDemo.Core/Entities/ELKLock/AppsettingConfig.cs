using System;
using System.Collections.Generic;
using System.Text;

namespace MISA.ImportDemo.Core.Entities.ELKLock
{
    public sealed class AppsettingConfig
    {
        public EsginConfig EsginConfig { get; set; }
        public IVANConfig IVANConfig { get; set; }
        public BHXHConfig BHXHConfig { get; set; }
        public MinioConfig MinioConfig { get; set; }
        public ELKConfig ELKConfig { get; set; }
    }
    public sealed class EsginConfig
    {
        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string UserName { get; set; }
        public string Passcode { get; set; }
        public string Password { get; set; }
        public string TaxCode { get; set; }
        public string CertId { get; set; }
    }

    public sealed class IVANConfig
    {
        public string IVANCode { get; set; }
        public string IVANName { get; set; }
        public string IVANPass { get; set; }
        public string RegisterIVAN { get; set; }
        public string BaseUrl { get; set; }
    }

    public sealed class BHXHConfig
    {
        public string SignatureLocationMISA { get; set; }
        public string SignatureLocationUnit { get; set; }
        public string SignatureLocationDeclaration { get; set; }
        public string ObjectSign { get; set; }
    }

    public sealed class MinioConfig
    {
        public string MinIOEndPoint { get; set; }
        public string MinIOAccessKey { get; set; }
        public string MinIOSecretKey { get; set; }
        public string MinIOBucketName { get; set; }
        public string DomainStream { get; set; }
        public string ExpiresTime { get; set; }
    }

    public sealed class ELKConfig
    {
        public string ELKVersion { get; set; }
        public string ELKAppID { get; set; }
        public string ELKSystemID { get; set; }
        public string ELKOn { get; set; }
        public string ELKUrl { get; set; }
    }
}
