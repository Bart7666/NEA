using Math.Gmp.Native;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
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
                if (Convert.ToBoolean(AlgorithmConfig[1]) & _key != "Temp") //KeyGeneration
                {
                    _key = "Temp"; //Temporary value before encryption
                }
                else if (value != null)
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
            if (Convert.ToBoolean(AlgorithmConfig[1]) == true) //If generating a random key, set Key value appropiately
            {
                SetKeys(true);
            }
            string WorkingCleanedData = CleanedData; //Saves CleanedData to working variable
            BigInteger LargePrime = BigInteger.Parse((Key.Split(',')[0]));
            BigInteger PublicKey = BigInteger.Parse((Key.Split(',')[1]));
            string[] SplitWorkingCleanedData = WorkingCleanedData.Split(',');
            foreach (string Element in SplitWorkingCleanedData)
            {
                ProcessedData += (ModularExponentation(BigInteger.Parse(Element, NumberStyles.HexNumber), PublicKey, LargePrime)).ToString("X"); //Adds Ciphertext as Hex to Processessed Data
            }

        }
        /// <summary>
        /// Decrypts ciphertext using the RSA algorithm
        /// </summary>
        public override void DecryptData()
        {
            if (Convert.ToBoolean(AlgorithmConfig[1]) == true) //If generating a random key, set Key value appropiately
            {
                SetKeys(false);
            }
            string WorkingCleanedData = CleanedData; //Saves CleanedData to working variable
            BigInteger LargePrime = BigInteger.Parse((Key.Split(',')[0]));
            BigInteger PrivateKey = BigInteger.Parse((Key.Split(',')[1]));
            string[] SplitWorkingCleanedData = WorkingCleanedData.Split(',');
            foreach (string Element in SplitWorkingCleanedData)
            {
                ProcessedData += (ModularExponentation(BigInteger.Parse(Element,NumberStyles.HexNumber), PrivateKey, LargePrime)).ToString("X"); //Adds Plaintext as Hex to Processed Data
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
        /// <summary>
        /// Generates a random BigInteger that is about half the length of the combined key
        /// </summary>
        /// <returns>Random512Prime</returns>
        public BigInteger Generate512Prime()
        {
            string RandomString = "";
            bool PrimeGenerated = false;
            while (!PrimeGenerated) //While a prime hasn't been generated
            {
                RandomString = "";
                for (int i = 0; i < 3; i++) //Create a cryptographicaly random string of integers length 155 (About 2^512 bits)
                {
                    RandomString += Convert.ToString(RandomNumberGenerator.GetInt32(10));
                }
                mpz_t RandomInteger = RandomString; //Converted string to int equivalent
                if (gmp_lib.mpz_probab_prime_p(RandomInteger,24) != 0) //If the randominteger is (almost certaintly) a prime
                {
                    PrimeGenerated = true; //Prime number found
                }
            }
            BigInteger RandomPrime = BigInteger.Parse(RandomString); //Converts genereated prime to a BigInteger
            return RandomPrime; 
        }
        /// <summary>
        /// Gets the lowest common multiple of two BigIntegers, given inputs are 1 less than the two primes for RSA key, returns Carmichaels Totient
        /// </summary>
        /// <param name="Number1"></param>
        /// <param name="Number2"></param>
        /// <returns></returns>
        public BigInteger GetLCM(BigInteger Number1, BigInteger Number2) 
        {
            return ((Number1 * Number2) / GetGCD(Number1, Number2)); //Finds the LCM of the two numbers 
        }
        /// <summary>
        /// Returns the greatest common divisor of two numbers, by calculating it recursively
        /// </summary>
        /// <param name="Number1"></param>
        /// <param name="Number2"></param>
        /// <returns></returns>
        public BigInteger GetGCD(BigInteger Number1, BigInteger Number2)
        {
            if (Number2 == BigInteger.Zero)
            {
                return Number1;
            }
            else
            {
                return GetGCD(Number2,Number1 % Number2);
            }
        }
        /// <summary>
        /// Calculates the modular multiplicative inverse of Small Prime Modulo Lambda LargePrime
        /// </summary>
        /// <param name="SmallPrime"></param>
        /// <param name="LambdaLargePublic"></param>
        /// <returns></returns>
        public BigInteger CreatePrivateKey(BigInteger SmallPrime, BigInteger LambdaLargePublic)
        {
            BigInteger T = 0;
            BigInteger NewT = 1;
            BigInteger TempT;
            BigInteger TempR;
            BigInteger R = LambdaLargePublic;
            BigInteger NewR = SmallPrime;
            BigInteger Quotient;
            
            while (NewR != 0)
            {
                Quotient = (R / NewR); //Floor division by definition as it is an integer result
                TempR = R;
                R = NewR;
                NewR = TempR - Quotient * NewR;
                TempT = T;
                T = NewT;
                NewT = TempT - Quotient * NewT;
            }
            if (T < 0)
            {
                T += LambdaLargePublic;
            }
            return T;//Returns Private Key
        }
        /// <summary>
        /// Generates a set of keys, stored in algorithm config 2-4 in order largepublic, smallpublic, privatekey
        /// </summary>
        public void GenerateKeys()
        {
            bool ValidKey = false;
            BigInteger Prime1;
            BigInteger Prime2;
            BigInteger SmallPublic = 0; //Exponent used for encryption
            BigInteger LargePublic = 0; //Common part of key to use for encryption and decryption
            BigInteger TotientFunction = 0; //Used for calculations
            while (!ValidKey)
            {
                Prime1 = Generate512Prime(); //Creates a prime to use as part of public key
                Prime2 = Generate512Prime(); 
                LargePublic = Prime1 * Prime2; //Finds the Large Number which is the common key
                TotientFunction = GetLCM(Prime1-1, Prime2-1); //Gets the reduced universal totient of LargePublic
                if (TotientFunction % 3 != 0)
                {
                    SmallPublic = 3;
                    ValidKey = true;
                }
                else if (TotientFunction % 7 != 0)
                {
                    SmallPublic = 7;
                    ValidKey = true;
                }
                else if (TotientFunction % 13 != 0)
                {
                    SmallPublic = 13;
                    ValidKey = true;
                }
                else if (TotientFunction % 17 != 0)
                {
                    SmallPublic = 17;
                    ValidKey = true;
                }
                else if (TotientFunction % 23 != 0)
                {
                    SmallPublic = 23;
                    ValidKey = true;
                }
                else if(TotientFunction % (BigInteger.Pow(2, 16) + 1) != 0)
                {
                    SmallPublic = BigInteger.Pow(2,16) + 1;
                    ValidKey = true;
                }
            }
            BigInteger PrivateKey = CreatePrivateKey(SmallPublic, TotientFunction);
            AlgorithmConfig.Add(LargePublic.ToString());
            AlgorithmConfig.Add(SmallPublic.ToString());
            AlgorithmConfig.Add(PrivateKey.ToString());
        }
        /// <summary>
        /// Sets key depending on whether encrypting or decrypt
        /// </summary>
        /// <param name="Encrypt">If encrypting</param>
        public void SetKeys(bool Encrypt) //If Encrypt is true, set public keys, else set large public, privat key
        {
            GenerateKeys();
            if (Encrypt)
            {
                Key = AlgorithmConfig[2] + "," + AlgorithmConfig[3];
            }
            else
            {
                Key = AlgorithmConfig[2] + "," + AlgorithmConfig[4];
            }
        }
    }

}

