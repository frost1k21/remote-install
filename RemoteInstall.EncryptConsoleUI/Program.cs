using RemoteInstall.EncryptorDecryptor;
using System;
using System.IO;
using System.Xml.Linq;

namespace RemoteInstall.EncryptConsoleUI
{
    internal class Program
    {
        static void Main()
        {
            Console.WriteLine("Вас приветствует user and password encryptor");
            Console.WriteLine();

            Console.Write(@"Введите имя пользователя (прим. domain\username): ");
            var userName = Console.ReadLine();

            Console.Write("Введите пароль от пользователя: ");
            var password = Console.ReadLine();

            Console.Write("Введите путь к файлу \"settings.xml\"(если оставить пустым, будет искать в текущей папке): ");
            var settingsPath = Console.ReadLine();

            if(string.IsNullOrEmpty(settingsPath))
            {
                settingsPath = Path.Combine(Directory.GetCurrentDirectory(), "settings.xml");
            }

            XDocument xDoc = XDocument.Load(settingsPath);
            var topSecret = xDoc.Element("appSettings").Element("creds").Element("topSecret");
            var userNameFromXml = topSecret.Attribute("login");
            var passwordFromXml = topSecret.Attribute("password");

            var encryptor = new EncryptorDecryptorAes();

            var encryptedUserName = encryptor.EncryptStringToByteArrayAes(userName);
            var encryptedPassword = encryptor.EncryptStringToByteArrayAes(password);

            userNameFromXml.Value = Convert.ToBase64String(encryptedUserName);
            passwordFromXml.Value = Convert.ToBase64String(encryptedPassword);

            xDoc.Save(settingsPath);

            Console.WriteLine("Операция шифрования прошла удачно");

            Console.WriteLine();
            Console.Write("Для выхода нажмите любую кнопку...");
            Console.ReadKey(true);
        }
    }
}
