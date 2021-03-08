using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using basicClasses.models.String_proc;
using JWT;
using JWT.Serializers;
using JWT.Algorithms;
using System.Security.Cryptography.X509Certificates;
using Jose;
using System.Security.Cryptography;

namespace basicClasses.models.WEB_api
{
    class jwt_gen : ModelBase
    {
        [info("secret private key from .p8 file (to sing)       CngKey.Import(secret, CngKeyBlobFormat.Pkcs8PrivateBlob)")]
        [model("")]
        public static readonly string key = "key";

        [info("ES256 ES384 ES512      HS256 HS384 HS512      PS256 PS384 PS512    RS256 RS384 RS512")]
        [model("")]
        public static readonly string security_algorithm = "security_algorithm";

        [info("list of headers kvp")]
        [model("")]
        public static readonly string headers = "headers";

        [info("list of payload kvp")]
        [model("")]
        public static readonly string payload = "payload";

        [info("")]
        [model("spec_tag")]
        public static readonly string add_bearer_prefix = "add_bearer_prefix";


        public override void Process(opis message)
        {
            opis ms = SpecLocalRunAll();
            var rez = new opis();
            int rezcou = 0;

            string prefix = ms.isHere(add_bearer_prefix) ? "bearer " : "";

            message.body = prefix +  CeateJwtHttpHeader3(ms.V(key), ms[headers], ms[payload], ms.V(security_algorithm), true);
        }


        static string CeateJwtHttpHeader3(string key, opis headers, opis payload, string SecurityAlgorithm = "ES256", bool iat_payload = false)
        {
            string rez = "";

            var now = DateTime.UtcNow;

            var secret = Convert.FromBase64String(key);

         
            var headerz = new Dictionary<string, object>();

            for (int i = 0; i < headers.listCou; i++)
                headerz.Add(headers[i].PartitionName, headers[i].body);

            var paylo = new Dictionary<string, object>();

            for (int i = 0; i < payload.listCou; i++)
                paylo.Add(payload[i].PartitionName, payload[i].body);

            if (iat_payload)
                paylo.Add("iat", Dates.EpochTime(now));

          
            JwsAlgorithm alg = JwsAlgorithm.ES256;

            switch (SecurityAlgorithm)
            {
                case "ES256": alg = JwsAlgorithm.ES256;
                    break;

                case "ES384":
                    alg = JwsAlgorithm.ES384;
                    break;

                case "ES512":
                    alg = JwsAlgorithm.ES512;
                    break;

                case "HS256":
                    alg = JwsAlgorithm.HS256;
                    break;

                case "HS384":
                    alg = JwsAlgorithm.HS384;
                    break;

                case "HS512":
                    alg = JwsAlgorithm.HS512;
                    break;

                case "PS256":
                    alg = JwsAlgorithm.PS256;
                    break;

                case "PS384":
                    alg = JwsAlgorithm.PS384;
                    break;
                case "PS512":
                    alg = JwsAlgorithm.PS512;
                    break;

                case "RS256":
                    alg = JwsAlgorithm.RS256;
                    break;

                case "RS384":
                    alg = JwsAlgorithm.RS384;
                    break;

                case "RS512":
                    alg = JwsAlgorithm.RS512;
                    break;
            }

            try
            {
                CngKey privateKey = CngKey.Import(secret, CngKeyBlobFormat.Pkcs8PrivateBlob);
                rez = Jose.JWT.Encode(paylo, privateKey, alg, headerz);
            }
            catch (Exception e)
            {
                rez = e.Message;
            }


            return rez;
        }

        public static CngKey getPrivateKey(string privateKeyPath)
        {
            var privateKeyLines = System.IO.File.ReadAllLines(privateKeyPath).ToList();
            privateKeyLines.RemoveAt(privateKeyLines.Count - 1);
            privateKeyLines.RemoveAt(0);

            var privateKey = string.Join("", privateKeyLines);
            var secretKeyFile = Convert.FromBase64String(privateKey);
            var secretKey = CngKey.Import(secretKeyFile, CngKeyBlobFormat.Pkcs8PrivateBlob);
            return secretKey;
        }


        static string CeateJwtHttpHeader2(string key, opis headers, opis payload, string SecurityAlgorithm = "ES256", bool iat_payload = false)
        {
            string rez = "";

            var now = DateTime.UtcNow;

            var secret = Convert.FromBase64String(key);

            var secretOrigExampl = Encoding.UTF8.GetBytes(key);

            // Create Security key  using private key above:
            // not that latest version of JWT using Microsoft namespace instead of System

            var headerz = new Dictionary<string, object>();

            for (int i = 0; i < headers.listCou; i++)
                headerz.Add(headers[i].PartitionName, headers[i].body);



            var paylo = new Dictionary<string, object>();

            for (int i = 0; i < payload.listCou; i++)
                paylo.Add(payload[i].PartitionName, payload[i].body);

            if (iat_payload)
                paylo.Add("iat", Dates.EpochTime(now));


            var fac = new JWT.Algorithms.ECDSAAlgorithmFactory();
       //     var alg= fac.Create(new JwtDecoderContext() { Header = new JwtHeader(new EncryptingCredentials() } );
           
            //  JwtAlgorithmName.

            IJwtAlgorithm algorithm =  new HMACSHA256Algorithm(); // symmetric
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            try
            {
                rez = encoder.Encode(headerz, paylo, key);
            }
            catch (Exception e)
            {
                rez = e.Message;
            }
           

            return rez;
        }

        static string CeateJwtHttpHeader(string key, opis headers, opis payload, string SecurityAlgorithm = "ES256", bool iat_payload = false)
        {

            string rez = "";

            var now = DateTime.UtcNow;

            var secret = Convert.FromBase64String(key);

            var secretOrigExampl = Encoding.UTF8.GetBytes(key);

            // Create Security key  using private key above:
            // not that latest version of JWT using Microsoft namespace instead of System
            var securityKey = new SymmetricSecurityKey(secretOrigExampl);

            //  ECDSASHA256
          //  new HMACSHA256Algorithm();

            var SigningCredentials = new SigningCredentials(
      new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
      SecurityAlgorithms.HmacSha256Signature);

            // Also note that securityKey length should be >256b
            // so you have to make sure that your private key has a proper length
            //
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithm );

            //  Finally create a Token
            var header = new JwtHeader();

            for (int i = 0; i < headers.listCou; i++)
                header.Add(headers[i].PartitionName, headers[i].body);

            //Some PayLoad that contain information about the  customer
            // var paylo = new JwtPayload
            //{
            //    { "some ", "hello "},
            //    { "scope", "kkk"},

            //};

            var paylo = new JwtPayload();

            for (int i = 0; i < payload.listCou; i++)
                paylo.Add(payload[i].PartitionName, payload[i].body);

            if (iat_payload)
                paylo.Add("iat", Dates.EpochTime(now));

            //
            var secToken = new JwtSecurityToken(header, paylo);

            try
            {
                // Token to String so you can use it in your client
                var tokenString = new JwtSecurityTokenHandler().WriteToken(secToken);
                rez = tokenString;
            }
            catch (Exception e)
            {
                rez = e.Message;
            }

           

            //Console.WriteLine(tokenString);
            //Console.WriteLine("Consume Token");


            // And finally when  you received token from client
            // you can  either validate it or try to  read
            //var token = handler.ReadJwtToken(tokenString);

            //Console.WriteLine(token.Payload.First().Value);

            return rez;
        }



    }
}
