using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA
{
    /// <summary>
    /// Implementation of OneTimePad as a class
    /// </summary>
    public class OneTimePad : EncryptionAlgorithm
    {
        /// <summary>
        /// Key to use in encryption or decryption by OneTimePad, Overrides root key definition within EncryptionAlgorithm
        /// </summary>
        public override string Key
        {
            get
            {
                if (_key != null) //Getter method
                {
                    return _key;
                }
                else //Should never occur but to make code more robust
                {
                    return string.Empty;
                }
            }
            set
            {
                if (value != null)
                {
                    value = value.Replace(" ", "");//Removes all spaces, string will not have any newlines as the key field does not accept them.
                    value = value.ToLower(); //Converts passed value to lowercase
                    foreach (char Character in value) //Checks the passed value is purely letters in the english alphabet
                    {
                        if (!((int)Character >= 97 && (int)Character <= 122)) //If it is not a letter set value of key to null and stop checking
                        {
                            _key = null;
                            break;
                        }
                        else //If (this character of ) the key is an english letter
                        {
                            _key = value;
                        }
                    }
                    if (value.Length < RawData.Length) //Key is not long enough to encrypt data
                    {
                        _key = null;
                    }
                }
            }
        }
        /// <summary>
        /// Contents of Input Field after basic data validation, Overides root RawData definition within EncryptionAlgoritmh
        /// </summary>
        public override string RawData
        {
            get
            {
                if (_rawData != null)
                {
                    return _rawData;
                }
                else //Used for ValidateData (if length of RawData after attempting to set is 0)
                {
                    return string.Empty;
                }
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim(); //Removes all whitespace
                    if (value.Length < 536870912) //Checks character limit is correct.
                    {
                        value = value.Replace(" ", "");//Removes all spaces (as they are not valid for Enigma Encryption
                        value = value.Replace("\n", "");//Removes all newlines (as they are not valid for Enigma Encryption
                        value = value.Replace("\r", "");//Removes all newlines (as they are not valid for Enigma Encryption
                        foreach (char Character in value) //Checks the passed value is purely letters in the english alphabet
                        {
                            if (!((int)Character >= 65 && (int)Character <= 90) | !((int)Character >= 97 && (int)Character <= 122)) //If it is not a letter set value of RawData to null and stop checking
                            {
                                _rawData = null;
                                break;
                            }
                            else //If (this character of) the key is an english letter
                            {
                                _rawData = value;
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Encrypts plaintext using a OneTimePad
        /// </summary>
        public override void EncryptData()
        {
            int KeyIndex = 0; //Current index of key to use to encrypt
            char CurrentKeyCharacter; //Character at KeyIndex;
            int CurrentKey; //Conversion of character into an integer
            string WorkingCleanedData = CleanedData; //Saves CleanedData to working variable
            int Index = 0;
            int ASCIICharacter; //Extended ASCII representation of a character
            while (Index < WorkingCleanedData.Length)
            {
                if (KeyIndex >= Key.Length) // If end of key is reached, loop back to start
                {
                    KeyIndex = 0;
                }
                CurrentKeyCharacter = Key[KeyIndex]; //Gets current character of key to use as encryption key
                CurrentKey = (int)CurrentKeyCharacter - 96; //Converts character to general encryption key by making it a 1-26 (a-z)
                int EffectiveLetterKey = CurrentKey % 26; //Finds the actual transformation the key will effect on letters
                int EffectiveNumberKey = CurrentKey % 10; //Finds the actual transformation the key will effect on numbers
                int EffectiveExtendedKey = CurrentKey % 30; //Finds the actual transformation the key will effect on extended characters
                if (WorkingCleanedData[Index] != ',')//Handles CSV files
                {
                    ASCIICharacter = Convert.ToInt32(WorkingCleanedData.Substring(Index, 8), 2); //First converts CleanedData into an integer from a string of binary and saves it to a variable
                    if ((ASCIICharacter >= 65 && ASCIICharacter <= 90) | (ASCIICharacter >= 97 && ASCIICharacter <= 122) | (ASCIICharacter >= 48 && ASCIICharacter <= 57) | (ASCIICharacter >= 192 && ASCIICharacter <= 222) | (ASCIICharacter >= 224 && ASCIICharacter <= 254))
                    {
                        if (ASCIICharacter >= 65 && ASCIICharacter <= 90) //Character is a (standard) capital letter
                        {
                            ASCIICharacter += EffectiveLetterKey;
                            if (ASCIICharacter > 90) //If new character is greater than range of extended ASCII (standard) capital letters
                            {
                                ASCIICharacter -= 26;
                            }
                        }
                        else if (ASCIICharacter >= 97 && ASCIICharacter <= 122) //Character is a (standard) letter
                        {
                            ASCIICharacter += EffectiveLetterKey;
                            if (ASCIICharacter > 122) //If new character is greater than range of extended ASCII (standard) letters
                            {
                                ASCIICharacter -= 26;
                            }
                        }
                        ProcessedData += Convert.ToString(ASCIICharacter, 2).PadLeft(8, '0'); ; //Adds ciphertext to processedData
                    }
                    else // Not an encryptable character
                    {
                        ProcessedData += Convert.ToString(ASCIICharacter, 2).PadLeft(8, '0'); ; //Adds ciphertext to processedData
                    }
                    Index += 8;
                    KeyIndex++;
                }
                else
                {
                    while (WorkingCleanedData[Index] == ',') //Adds commas for CSV files untill a non comma character is reached
                    {
                        ProcessedData += ",";
                        Index++;
                    }
                }
            }
        }
        /// <summary>
        /// Decrypts ciphertext using a OneTimePad
        /// </summary>
        public override void DecryptData()
        {
            int KeyIndex = 0; //Current index of key to use to decrypt
            char CurrentKeyCharacter; //Character at KeyIndex;
            int CurrentKey; //Conversion of character into an integer
            string WorkingCleanedData = CleanedData; //Saves CleanedData to working variable
            int Index = 0;
            int ASCIICharacter; //Extended ASCII representation of a character
            while (Index < WorkingCleanedData.Length)
            {
                if (KeyIndex >= Key.Length) // If end of key is reached, loop back to start
                {
                    KeyIndex = 0;
                }
                CurrentKeyCharacter = (Key[KeyIndex]); //Gets current character of key to use as decryption key
                CurrentKey = (int)CurrentKeyCharacter - 96; //Converts character to general decryption key by making it a 1-26 (a-z)
                int EffectiveLetterKey = CurrentKey % 26; //Finds the actual transformation the key will effect on letters
                int EffectiveNumberKey = CurrentKey % 10; //Finds the actual transformation the key will effect on numbers
                int EffectiveExtendedKey = CurrentKey % 30; //Finds the actual transformation the key will effect on extended characters
                if (WorkingCleanedData[Index] != ',')//Handles CSV files
                {
                    ASCIICharacter = Convert.ToInt32(WorkingCleanedData.Substring(Index, 8), 2); //First converts CleanedData into an integer from a string of binary and saves it to a variable
                    if ((ASCIICharacter >= 65 && ASCIICharacter <= 90) | (ASCIICharacter >= 97 && ASCIICharacter <= 122) | (ASCIICharacter >= 48 && ASCIICharacter <= 57) | (ASCIICharacter >= 192 && ASCIICharacter <= 222) | (ASCIICharacter >= 224 && ASCIICharacter <= 254))
                    {
                        if (ASCIICharacter >= 65 && ASCIICharacter <= 90) //Character is a (standard) capital letter
                        {
                            ASCIICharacter -= EffectiveLetterKey;
                            if (ASCIICharacter < 65) //If new character is greater than range of extended ASCII (standard) capital letters
                            {
                                ASCIICharacter += 26;
                            }
                        }
                        else if (ASCIICharacter >= 97 && ASCIICharacter <= 122) //Character is a (standard) letter
                        {
                            ASCIICharacter -= EffectiveLetterKey;
                            if (ASCIICharacter < 97) //If new character is greater than range of extended ASCII (standard) letters
                            {
                                ASCIICharacter += 26;
                            }
                        }
                        ProcessedData += Convert.ToString(ASCIICharacter, 2).PadLeft(8, '0'); ; //Adds plaintext to processedData
                    }
                    else // Not a decryptable character
                    {
                        ProcessedData += Convert.ToString(ASCIICharacter, 2).PadLeft(8, '0'); ; //Adds plaintext to processedData
                    }
                    Index += 8;
                    KeyIndex++;
                }
                else
                {
                    while (WorkingCleanedData[Index] == ',') //Adds commas for CSV files untill a non comma character is reached
                    {
                        ProcessedData += ",";
                        Index++;

                    }
                }
            }
        }
    }

}
