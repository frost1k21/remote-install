using System;
using System.Collections.Generic;
using System.Xml.Linq;
using RemoteInstall.ConfigFromFile.Models;
using RemoteInstall.EncryptorDecryptor;

namespace RemoteInstall.ConfigFromFile
{
    public class ConfigForRemoteInstall
    {
        public List<InstallSettings> InstallSettingses { get; private set; }
        public List<OneCBases> OneCBaseses { get; private set; }
        public List<SecretCred> SecretCreds { get; private set; }

        private EncryptorDecryptorAes _encryptorDecryptor;

        public ConfigForRemoteInstall(string configFileName)
        {
            if (string.IsNullOrEmpty(configFileName))
                throw new ArgumentNullException("Имя файла с конфигурацией не должено быть пустое ", nameof(configFileName));
            XDocument xdoc = XDocument.Load(configFileName);

            _encryptorDecryptor = new EncryptorDecryptorAes();

            GetIsntallSettings(xdoc);
            GetOneCBasesSettings(xdoc);
            GetCredSettings(xdoc);
        }

        private void GetIsntallSettings(XDocument xdoc)
        {
            InstallSettingses = new List<InstallSettings>();
            foreach (XElement installElement in xdoc.Element("appSettings")
                .Element("installSettings")
                .Elements("value"))
            {
                XAttribute name = installElement.Attribute("productName");
                XAttribute path = installElement.Attribute("productPath");
                XAttribute arguments = installElement.Attribute("arguments");
                InstallSettingses.Add(new InstallSettings
                {
                    ProductName = name.Value,
                    ProductPath = path.Value,
                    Arguments = arguments.Value,
                });
            }
        }

        private void GetOneCBasesSettings(XDocument xdoc)
        {
            OneCBaseses = new List<OneCBases>();
            foreach (XElement installElement in xdoc.Element("appSettings")
                .Element("baseSettings")
                .Elements("base"))
            {
                XAttribute name = installElement.Attribute("nameForUser");
                XAttribute server = installElement.Attribute("serverName");
                XAttribute dbName = installElement.Attribute("dbName");
                OneCBaseses.Add(new OneCBases
                {
                    NameForUser = name.Value,
                    ServerName = server.Value,
                    DbName = dbName.Value
                });
            }
        }

        private void GetCredSettings(XDocument xdoc)
        {
            SecretCreds = new List<SecretCred>();
            foreach (XElement installElement in xdoc.Element("appSettings")
                .Element("creds")
                .Elements("topSecret"))
            {
                XAttribute login = installElement.Attribute("login");
                XAttribute password = installElement.Attribute("password");

                var loginString = Convert.FromBase64String(login.Value);
                var passwordString = Convert.FromBase64String(password.Value);

                SecretCreds.Add(new SecretCred
                {
                    Login = _encryptorDecryptor.DecryptStringFromByteArrayAes(loginString),
                    Password = _encryptorDecryptor.DecryptStringFromByteArrayAes(passwordString)
                });
            }
        }
    }
}
