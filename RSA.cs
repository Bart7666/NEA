using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NEA
{
    //This is the implementation of the RSA algorithm using a class
    internal class RSA : EncryptionAlgorithm
    {
        // <summary>
        /// Key to use in encryption or decryption by the RSA algorithm, Overrides root key definition within EncryptionAlgorithm
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
                if (value != null)
                {
                    int CommaCount = 0; //Limit of one comma to mark seperation of key into rows and columns
                    value = value.Replace(" ", "");//Removes all spaces, string will not have any newlines as the key field does not accept them.
                    foreach (char Character in value) //Checks the passed value is made of purely integers
                    {
                        if (!Char.IsDigit(Character) && CommaCount != 0)
                        {
                            _key = null;
                            break;
                        }
                        else if (Character == ',') //Key split found
                        {
                            _key = value;
                            CommaCount++;
                        }
                        else
                        {
                            _key = value;
                        }
                    }
                    if (CommaCount == 0) //If no key split found
                    {
                        _key = null;
                    }
                }
            }
        }
        /// <summary>
        /// Encrypts plaintext using the RSA algorithm
        /// </summary>
        public override void EncryptData()
        {
            string WorkingCleanedData = CleanedData; //Saves CleanedData to working variable
            BigInteger LargePrime = BigInteger.Parse((Key.Split(',')[0]));
            BigInteger PublicKey = BigInteger.Parse((Key.Split(',')[1]));
            string[] SplitWorkingCleanedData = WorkingCleanedData.Split(',');
            foreach (string Element in SplitWorkingCleanedData)
            {
                BigInteger test = ModularExponentation(BigInteger.Parse(Element, NumberStyles.HexNumber), PublicKey, LargePrime);
                ProcessedData += (ModularExponentation(BigInteger.Parse(Element, NumberStyles.HexNumber), PublicKey, LargePrime)).ToString("X"); //Adds Ciphertext as Hex to Processessed Data
            }

        }
        /// <summary>
        /// Decrypts ciphertext using the RSA algorithm
        /// </summary>
        public override void DecryptData()
        {
            string WorkingCleanedData = CleanedData; //Saves CleanedData to working variable
            BigInteger LargePrime = BigInteger.Parse((Key.Split(',')[0]));
            BigInteger PrivateKey = BigInteger.Parse((Key.Split(',')[1]));
            string[] SplitWorkingCleanedData = WorkingCleanedData.Split(',');
            foreach (string Element in SplitWorkingCleanedData)
            {
                ProcessedData += (ModularExponentation(BigInteger.Parse(Element,NumberStyles.HexNumber), PrivateKey, LargePrime)).ToString("X"); //Adds Ciphertext as Hex to Processed Data
            }
        }


        /// <summary>
        /// Finds the result of an exponentiation of a large number followed by a Modulus
        /// </summary>
        /// <param name="Base"></param>
        /// <param name="Exponent"></param>
        /// <param name="Modulus"></param>
        /// <returns></returns>
        public BigInteger ModularExponentation(BigInteger Base, BigInteger Exponent, BigInteger Modulus)
        {
            return ExponentiationBySquaring(Base,Exponent) % Modulus; //Finds the result of the exponentiation and then applies modulus
        }

        /// <summary>
        /// Performs Exponentation of a large number efficiently
        /// </summary>
        /// <param name="Base"></param>
        /// <param name="Exponent"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public BigInteger ExponentiationBySquaring(BigInteger Base, BigInteger Exponent)
        {
            if (Exponent.Sign == -1)
            {
                return ExponentiationBySquaring(BigInteger.Pow(Base,-1), Exponent * -1);
            }
            else if (Exponent.IsZero)
            {
                return 1;
            }
            else if (Exponent.IsEven)
            {
                return ExponentiationBySquaring(BigInteger.Pow(Base, 2), Exponent / 2);
            }
            else if (!Exponent.IsEven)
            {
                return Base * ExponentiationBySquaring(BigInteger.Pow(Base, 2), (Exponent-1) / 2);
            }
            else
            {
                throw new Exception("Incorrect result of squaring"); //Should never occur by the laws of mathematics
            }
        }
        /// <summary>
        /// Specific implementation of ComposeData for RSA, in order to ouput as Hexadecimaal.
        /// </summary>
        /// <param name="InputType"></param>
        public override void ComposeData(DataInputType InputType)
        {
            string WorkingProcessedData = ProcessedData; //Saves ProcessedData to working variable
            if (WorkingProcessedData.Length % 2 != 0) //If result of encryption is not an even number of Hex digits long
            {
                WorkingProcessedData = WorkingProcessedData.PadLeft(WorkingProcessedData.Length+1,'0'); //Pad to be even
            }
            int Index = 0;
            while (Index < ProcessedData.Length)
            {
                if (WorkingProcessedData[Index] != ',')//Handles CSV files
                {
                    OutputData += WorkingProcessedData.Substring(Index, 2) + " "; //Outputs the result of the encryption or decryption as Hex alongside a space to seperate each character
                    Index += 2;
                }
                else
                {
                    while (WorkingProcessedData[Index] == ',') //Adds commas for CSV files untill a non comma character is reached
                    {
                        OutputData += ",";
                        Index++;
                    }
                }
            }
        }
        /// <summary>
        /// Specific implementation of Clean for RSA, in order to accept either hex or string input.
        /// </summary>
        /// <param name="InputType"></param>
        public override void CleanData(DataInputType InputType)
        {
            if (InputType == DataInputType.Text)
            {
                string WorkingRawData = RawData; //Saves RawData to working variable
                WorkingRawData = WorkingRawData.Replace(" ", ""); //Removes spaces
                WorkingRawData = WorkingRawData.Replace("\n",""); //Removes new lines
                foreach (char RawDataCharacter in WorkingRawData) //Iterates through every character and appends it to CleanedData
                {
                    if (Convert.ToBoolean(AlgorithmConfig[0])) //If Hex is being Input
                    {
                        if (Char.IsAsciiHexDigit(RawDataCharacter)) //If the character is a correct Hex character, else exclude from encryption
                        {
                            CleanedData += RawDataCharacter;
                        }
                    }
                    else //If regular text is being input
                    {
                        if ((int)RawDataCharacter < 256) //If the character is a part of extended ASCII (0-255), add to CleanedData, else exclude from encryption
                        {
                            CleanedData += Convert.ToString(((int)RawDataCharacter), 16); //Converts integer (Extended ASCII) representation of a letter (or anything else) into a hex representation (in ASCII)
                        }
                    }
                    
                }
            }
            if (InputType == DataInputType.TextFile) { throw new NotImplementedException(); } //not yet implemented
            if (InputType == DataInputType.CSV) { throw new NotImplementedException(); } //not yet implemented
        }
    }
}

