namespace NEA
{
    /// <summary>
    /// Implementation of CeasarCipher as a class.
    /// </summary>
    public class CaesarCipher : EncryptionAlgorithm
    {
        /// <summary>
        ///Key to use in encryption or decryption by Caesar Cipher, Overrides root key definition within EncryptionAlgorithm
        /// </summary>
        public override string Key
        {
            get
            {
                if (_key != null)
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
                if(value != null)
                {
                    value = value.Replace(" ", "");//Removes all spaces, string will not have any newlines as the key field does not accept them.
                    foreach(char Character in value) //Checks the passed value is made of purely integers
                    {
                        if (Char.IsDigit(Character)) { }
                        else
                        {
                            break;
                        }
                        _key = value;
                    }
                    
                }
                
            }
        }
        /// <summary>
        /// Encrypts plaintext using the Caesar Cipher
        /// </summary>
        public override void EncryptData()
        {
            int EffectiveKey = Int32.Parse(Key) % 26; //Finds the actual transformation the key will effect
            string WorkingCleanedData = CleanedData; //Saves CleanedData to working variable
            int Index = 0;
            int ASCIICharacter; //Extended ASCII representation of a character
            while (Index < WorkingCleanedData.Length)
            {
                if (WorkingCleanedData[Index] != ',')//Handles CSV files
                {
                    ASCIICharacter = Convert.ToInt32(WorkingCleanedData.Substring(Index, 8), 2); //First converts CleanedData into an integer from a string of binary and saves it to a variable
                    if ((ASCIICharacter >=  65 && ASCIICharacter <= 90) | (ASCIICharacter >= 97 && ASCIICharacter <= 122) | (ASCIICharacter >= 48 && ASCIICharacter <= 57) | (ASCIICharacter >= 192 && ASCIICharacter <= 222) | (ASCIICharacter >= 224 && ASCIICharacter <= 254))
                    {
                        if (ASCIICharacter >= 65 && ASCIICharacter <= 90) //Character is a (standard) capital letter
                        {
                            ASCIICharacter += EffectiveKey;
                            if (ASCIICharacter > 90) //If new character is greater than range of extended ASCII (standard) capital letters
                            {
                                ASCIICharacter -= 26;
                            }
                        }
                        else if (ASCIICharacter >= 97 && ASCIICharacter <= 122) //Character is a (standard) letter
                        {
                            ASCIICharacter += EffectiveKey;
                            if (ASCIICharacter > 122) //If new character is greater than range of extended ASCII (standard) letters
                            {
                                ASCIICharacter -= 26;
                            }
                        }
                        else if (ASCIICharacter >= 48 && ASCIICharacter <= 57) //Character is a number
                        {
                            ASCIICharacter += EffectiveKey;
                            if (ASCIICharacter > 57) //If new character is greater than range of extended ASCII mumbers
                            {
                                ASCIICharacter -= 10;
                            }
                        }
                        else if (ASCIICharacter >= 192 && ASCIICharacter <= 222) //Character is a (extended) capital letter
                        {
                            ASCIICharacter += EffectiveKey;
                            if (ASCIICharacter > 222) //If new character is greater than range of extended ASCII (extended) letters
                            {
                                ASCIICharacter -= 30;
                            }
                        }
                        else if (ASCIICharacter >= 224 && ASCIICharacter <= 254) //Character is a (extended) capital letter
                        {
                            ASCIICharacter += EffectiveKey;
                            if (ASCIICharacter > 254) //If new character is greater than range of extended ASCII (extended) letters
                            {
                                ASCIICharacter -= 30;
                            }
                        }
                        ProcessedData += Convert.ToString(ASCIICharacter,2).PadLeft(8, '0'); ; //Adds ciphertext to processedData
                    }
                    else
                    {
                        ProcessedData += Convert.ToString(ASCIICharacter,2).PadLeft(8, '0'); ; //Adds ciphertext to processedData
                    }
                    Index += 8;
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
        /// Decrypts ciphertext using the Caesar Cipher
        /// </summary>
        public override void DecryptData()
        {
            int EffectiveKey = Int32.Parse(Key) % 26; //Finds the actual transformation the key will effect
            string WorkingCleanedData = CleanedData; //Saves CleanedData to working variable
            int Index = 0;
            int ASCIICharacter; //Extended ASCII representation of a character
            while (Index < WorkingCleanedData.Length)
            {
                if (WorkingCleanedData[Index] != ',')//Handles CSV files
                {
                    ASCIICharacter = Convert.ToInt32(WorkingCleanedData.Substring(Index, 8), 2); //First converts CleanedData into an integer from a string of binary and saves it to a variable
                    if ((ASCIICharacter >= 65 && ASCIICharacter <= 90) | (ASCIICharacter >= 97 && ASCIICharacter <= 122) | (ASCIICharacter >= 48 && ASCIICharacter <= 57) | (ASCIICharacter >= 192 && ASCIICharacter <= 222) | (ASCIICharacter >= 224 && ASCIICharacter <= 254))
                    {
                        if (ASCIICharacter >= 65 && ASCIICharacter <= 90) //Character is a (standard) capital letter
                        {
                            ASCIICharacter += EffectiveKey;
                            if (ASCIICharacter > 90) //If new character is greater than range of extended ASCII (standard) capital letters
                            {
                                ASCIICharacter -= 26;
                            }
                        }
                        else if (ASCIICharacter >= 97 && ASCIICharacter <= 122) //Character is a (standard) letter
                        {
                            ASCIICharacter += EffectiveKey;
                            if (ASCIICharacter > 122) //If new character is greater than range of extended ASCII (standard) letters
                            {
                                ASCIICharacter -= 26;
                            }
                        }
                        else if (ASCIICharacter >= 48 && ASCIICharacter <= 57) //Character is a number
                        {
                            ASCIICharacter += EffectiveKey;
                            if (ASCIICharacter > 57) //If new character is greater than range of extended ASCII mumbers
                            {
                                ASCIICharacter -= 10;
                            }
                        }
                        else if (ASCIICharacter >= 192 && ASCIICharacter <= 222) //Character is a (extended) capital letter
                        {
                            ASCIICharacter += EffectiveKey;
                            if (ASCIICharacter > 222) //If new character is greater than range of extended ASCII (extended) letters
                            {
                                ASCIICharacter -= 30;
                            }
                        }
                        else if (ASCIICharacter >= 224 && ASCIICharacter <= 254) //Character is a (extended) capital letter
                        {
                            ASCIICharacter += EffectiveKey;
                            if (ASCIICharacter > 254) //If new character is greater than range of extended ASCII (extended) letters
                            {
                                ASCIICharacter -= 30;
                            }
                        }
                        ProcessedData += Convert.ToString(ASCIICharacter); //Adds plaintext to processedData
                    }
                    else
                    {
                        ProcessedData += Convert.ToString(ASCIICharacter); //Adds plaintext to processedData
                    }
                    Index += 8;
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

