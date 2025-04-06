using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class MyGameManager : MonoBehaviour
{
    // Singleton
    private static MyGameManager instance;
    public static MyGameManager Instance { get { return instance; } }

    private GameDatas GameDatas = new GameDatas();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Load();
        Application.targetFrameRate = 120;
    }

    public void StartNewGame(GameSlot slot)
    {
        GameDatas.currentGameSlot = slot;

        GameDatas.GameSlots[(int)GameDatas.currentGameSlot].LevelKey = Encrypt("0");
        GameDatas.GameSlots[(int)GameDatas.currentGameSlot].ChapterIndex = 0;

        var translatedChapterTitle = SettingsManager.Instance.IsGameInFrench() ? ConstantManager.ChapterTitle_1_Fr : ConstantManager.ChapterTitle_1_En;
        GameDatas.GameSlots[(int)GameDatas.currentGameSlot].ChapterName = Encrypt(translatedChapterTitle);

        Save();
    }

    public void UpdateLevelKey(string levelkey, int chapterIndex, string chapterName = null)
    {
        // Encrypt
        GameDatas.GameSlots[(int)GameDatas.currentGameSlot].LevelKey = Encrypt(levelkey);

        GameDatas.GameSlots[(int)GameDatas.currentGameSlot].ChapterIndex = (int)chapterIndex;

        if (chapterName != null)
            GameDatas.GameSlots[(int)GameDatas.currentGameSlot].ChapterName = Encrypt(chapterName);

        Save();
    }

    public GameSlotDatas LoadSpecificGameSlot(GameSlot slot)
    {
        if (slot == GameSlot.LOADCURRENTSLOT)
            slot = GameDatas.currentGameSlot;

        if (GameDatas.GameSlots[(int)slot].ChapterIndex != -1)
        {
            if (slot != GameSlot.LOADCURRENTSLOT)
            {
                GameDatas.currentGameSlot = slot;
                Save();
            }

            // Decrypt
            try
            {
                GameDatas.GameSlots[(int)GameDatas.currentGameSlot].LevelKey = Decrypt(GameDatas.GameSlots[(int)GameDatas.currentGameSlot].LevelKey);
                GameDatas.GameSlots[(int)GameDatas.currentGameSlot].ChapterName = Decrypt(GameDatas.GameSlots[(int)GameDatas.currentGameSlot].ChapterName);
            }
            catch (Exception e)
            {
                ExceptionManager.ThrowException(name, nameof(MyGameManager), "Une erreur est survenue lors du décodage des données de partie.");
            }

            return GameDatas.GameSlots[(int)GameDatas.currentGameSlot];
        }
        return null;
    }

    public GameSlotDatas LoadSpecificGameSlot(int index)
    {
        // Decrypt
        try
        {
            GameDatas.GameSlots[index].LevelKey = Decrypt(GameDatas.GameSlots[index].LevelKey);
            GameDatas.GameSlots[index].ChapterName = Decrypt(GameDatas.GameSlots[index].ChapterName);
        }
        catch (Exception e)
        {
            // Files already decrypted
        }

        return GameDatas.GameSlots[index];
    }

    public int GetCurrentGameSlotChapterIndex()
    {
        return GameDatas.GameSlots[(int)GameDatas.currentGameSlot].ChapterIndex;
    }

    public int GetCurrentGameSlotIndex()
    {
        return (int)GameDatas.currentGameSlot;
    }

    public void Save()
    {
        string path = $"{Application.persistentDataPath}/{ConstantManager.GameFileName}";

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        GameDatas.MinimalGameVersionSaveFileRequired = ConstantManager.MinimalGameVersionSaveFileRequired;

        formatter.Serialize(stream, GameDatas);
        stream.Close();
    }

    private void Load()
    {
        string path = $"{Application.persistentDataPath}/{ConstantManager.GameFileName}";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameDatas = formatter.Deserialize(stream) as GameDatas;

            if (!CheckSaveFileMinimumVersionValidity(GameDatas))
            {
                GameDatas = new GameDatas();
                GameDatas.MinimalGameVersionSaveFileRequired = ConstantManager.MinimalGameVersionSaveFileRequired;
            }

            stream.Close();
            Save();
        }
        else
        {
            ExceptionManager.ThrowException(name, nameof(MyGameManager), $"Le fichier de sauvegarde n'existe pas : {path}.");
        }
    }

    private bool CheckSaveFileMinimumVersionValidity(GameDatas? loadedDatas)
        => (loadedDatas == null || loadedDatas.MinimalGameVersionSaveFileRequired == ConstantManager.MinimalGameVersionSaveFileRequired);

    #region Encrypt/Decrypt Stuff

    private protected readonly string key = "FlyingButterGame";

    // This constant is used to determine the keysize of the encryption algorithm in bits.
    // We divide this by 8 within the code below to get the equivalent number of bytes.
    private const int Keysize = 256;

    // This constant determines the number of iterations for the password bytes generation function.
    private const int DerivationIterations = 1000;

    public string Encrypt(string plainText)
    {
        // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
        // so that the same Salt and IV values can be used when decrypting.  
        var saltStringBytes = Generate256BitsOfRandomEntropy();
        var ivStringBytes = Generate256BitsOfRandomEntropy();
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        using (var password = new Rfc2898DeriveBytes(key, saltStringBytes, DerivationIterations))
        {
            var keyBytes = password.GetBytes(Keysize / 8);
            using (var symmetricKey = new RijndaelManaged())
            {
                symmetricKey.BlockSize = 256;
                symmetricKey.Mode = CipherMode.CBC;
                symmetricKey.Padding = PaddingMode.PKCS7;
                using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                            cryptoStream.FlushFinalBlock();
                            // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                            var cipherTextBytes = saltStringBytes;
                            cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                            cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                            memoryStream.Close();
                            cryptoStream.Close();
                            return Convert.ToBase64String(cipherTextBytes);
                        }
                    }
                }
            }
        }
    }

    public string Decrypt(string cipherText)
    {
        // Get the complete stream of bytes that represent:
        // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
        var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
        // Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
        var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
        // Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
        var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
        // Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
        var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

        using (var password = new Rfc2898DeriveBytes(key, saltStringBytes, DerivationIterations))
        {
            var keyBytes = password.GetBytes(Keysize / 8);
            using (var symmetricKey = new RijndaelManaged())
            {
                symmetricKey.BlockSize = 256;
                symmetricKey.Mode = CipherMode.CBC;
                symmetricKey.Padding = PaddingMode.PKCS7;
                using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                {
                    using (var memoryStream = new MemoryStream(cipherTextBytes))
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            var plainTextBytes = new byte[cipherTextBytes.Length];
                            var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                            memoryStream.Close();
                            cryptoStream.Close();
                            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                        }
                    }
                }
            }
        }
    }

    private static byte[] Generate256BitsOfRandomEntropy()
    {
        var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
        using (var rngCsp = new RNGCryptoServiceProvider())
        {
            // Fill the array with cryptographically secure random bytes.
            rngCsp.GetBytes(randomBytes);
        }
        return randomBytes;
    }

    #endregion
}

[Serializable]
public class GameDatas
{
    public GameSlot currentGameSlot = GameSlot.SLOT1;
    public string MinimalGameVersionSaveFileRequired = string.Empty;

    public GameSlotDatas[] GameSlots = new GameSlotDatas[3] { new GameSlotDatas(), new GameSlotDatas(), new GameSlotDatas() };
}

[Serializable]
public class GameSlotDatas
{
    public int ChapterIndex = -1;
    public string ChapterName;

    public string LevelKey;
}

public enum GameSlot
{
    SLOT1 = 0,
    SLOT2 = 1,
    SLOT3 = 2,

    LOADCURRENTSLOT = 3
}
