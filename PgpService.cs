using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PgpCore;

namespace PgpSandbox
{
    public interface ISecretManager
    {
        SecurityKeySource AcquireKeys(string tenant);
    }

    public class SecurityKeySource
    {
        public string PrivateKeySource { get; set; }
        public string PublicKeySource { get; set; }
        public string PassPhrase { get; set; }
    }

    internal class SimpleSecretManager : ISecretManager
    {
        private SecurityKeySource Source { get; }

        public SimpleSecretManager(IOptions<SecurityKeySource> option)
        {
            Source = option.Value;
        }

        public SecurityKeySource AcquireKeys(string tenant)
        {
            return Source;
        }
    }

    internal class PgpService
    {
        private readonly PGP _pgp;
        private readonly ISecretManager _secretManager;
        private ILogger<PgpService> Log { get; }

        public PgpService(ISecretManager secretManager, ILogger<PgpService> log)
        {
            _pgp = new PGP();
            _secretManager = secretManager;
            Log = log;
        }

        public string Decrypt(string tenant, string input, string output)
        {
            var securityKeySource = _secretManager.AcquireKeys(tenant);
            var inputStream = File.Open(input, FileMode.Open, FileAccess.ReadWrite);
            var outStream = new MemoryStream();
            var privateKeyStream = new MemoryStream(Encoding.UTF8.GetBytes(Regex.Unescape(securityKeySource.PrivateKeySource)));
            securityKeySource.PassPhrase = securityKeySource.PassPhrase == "0" ? null : securityKeySource.PassPhrase; // signify it's null/empty in Azure (we can't store null values)
            _pgp.DecryptStream(inputStream, outStream, privateKeyStream, "");
            inputStream.Dispose();
            using (var outputStream = File.Open(output, FileMode.Create, FileAccess.Write))
            outStream.WriteTo(outputStream);
            outStream.Dispose();
            return output;
        }

        public string Encrypt(string tenant, string input, string output)
        {
            var securityKeySource = _secretManager.AcquireKeys(tenant);
            _pgp.EncryptFile(input, output, securityKeySource.PublicKeySource);
            return output;
        }
    }
}
