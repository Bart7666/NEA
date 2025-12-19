using System.Diagnostics.Metrics;
using System.Windows;
using Microsoft.VisualBasic;

namespace NEA
{
    /// <summary>
    /// The root class for all implemented encryption algorithms, it is abstract.
    /// </summary>
    public abstract class EncryptionAlgorithm
    {
        protected string? _rawData;
        /// <summary>
        /// Contents of Input Field, Virtual as to allow override as neccesary in specific algorithms
        /// </summary>
        public virtual string RawData 
        {
            get
            {
                if (_rawData != null)
                {
                    return _rawData;
                }
                else //Used for ValidateData (if length of RawData after attempting to set is 0
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
                        _rawData = value;
                    }
                }
            }
        }

        protected string? _cleanedData;
        /// <summary>
        /// Cleaned data in binary ready to encrypt or decrypt, Virtual as to allow override as neccesary in specific algorithms 
        /// </summary>
        public virtual string CleanedData 
        {
            get
            {
                if (_cleanedData != null)
                {
                    return _cleanedData;
                }
                else //Should never occur but to make code more robust
                {
                    return string.Empty;
                }
            }
            set //Does not need validation as it is handled with the CleanData method
            {
                _cleanedData = value;
            }
        }

        protected string? _processedData;
        /// <summary>
        ///Ciphertext or Plaintext in binary, Virtual as to allow override as neccesary in specific algorithms
        /// </summary>
        public virtual string ProcessedData 
        {
            get
            {
                if (_processedData != null)
                {
                    return _processedData;
                }
                else //Should never occur but to make code more robust
                {
                    return string.Empty;
                }
            }
            set //Does not need validation as all user input validation has already been done and this is purely internal
            {
                _processedData = value;
            }
        }

        protected string? _outputData;
        /// <summary>
        ///Ciphertext or Plaintext in binary, Virtual as to allow override as neccesary in specific algorithms
        /// </summary>
        public virtual string OutputData
        {
            get
            {
                if (_outputData != null)
                {
                    return _outputData;
                }
                else //Should never occur but to make code more robust
                {
                    return string.Empty;
                }
            }
            set //Does not need validation as all user input validation has already been done and this is purely internal
            {
                _outputData = value;
            }
        }

        protected string? _key;
        /// <summary>
        ///Key to use in encryption or decryption, Virtual as to allow override as neccesary in specific algorithms
        /// </summary>
        public virtual string Key
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
            set //Validation not included as this is the key for the abstract class and each algorithm neeeds a different validation
            {
                _key = value;
            }
        }

        protected List<string>? _algorithmConfig;
        /// <summary>
        /// List that will contain all the algoithm config options (if relevant) for each algorithm, it is virtual and contains no validation as each algorithm will have seperate validatio
        /// </summary>
        public virtual List<string> AlgorithmConfig
        {
            get
            {
                if (_algorithmConfig != null)
                {
                    return _algorithmConfig;
                }
                else //Should never occur but to make code more robust
                {
                    return new List<string> { string.Empty };
                }
            }
            set //Validation will be changed depending on algorithm so none implemented here
            {
                _algorithmConfig = value;
            }
        }
        /// <summary>
        /// Cleans the Rawdata into binary to be used in encryption or decryption, Virtual as to allow override as neccesary in specific algorithms
        /// </summary>
        /// <param name="InputType"></param>
        public virtual void CleanData(DataInputType InputType) //DataInputType is the data
        {
            if (InputType != DataInputType.CSV)
            {
                string WorkingRawData = RawData; //Saves RawData to working variable
                foreach (char RawDataCharacter in WorkingRawData) //Iterates through every character and appends it to CleanedData
                {
                    if((int)RawDataCharacter < 256) //If the character is a part of extended ASCII (0-255), add to CleanedData
                    {
                        CleanedData += Convert.ToString(((int)RawDataCharacter), 2).PadLeft(8,'0'); //Converts integer (Extended ASCII) representation into a string containing binary then pads it with zeros untill it is 8 bits long before adding it to cleaned data
                    }
                    else //Else add a question mark
                    {
                        CleanedData += "00111111";
                    }
                }
            }
            if (InputType == DataInputType.CSV) { throw new NotImplementedException(); } //not yet implemented
        }
        /// <summary>
        /// Abstract method for EncryptionAlgorithm Class (to guarantee uniformity within specific implementations
        /// </summary>
        public abstract void EncryptData();
        /// <summary>
        /// Abstract method for EncryptionAlgorithm Class (to guarantee uniformity within specific implementations
        /// </summary>
        public abstract void DecryptData();
        /// <summary>
        /// Converts result of Encryption / Decryption (stored in ProcessedData) into human readable data (stored in OutputData)
        /// </summary>
        /// <param name="InputType"></param>
        /// <param name="FilePath"></param>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void ComposeData(DataInputType InputType, string FilePath)
        {
            string WorkingProcessedData = ProcessedData; //Saves ProcessedData to working variable
            int Index = 0;
            while (Index < ProcessedData.Length)
            {
                if (WorkingProcessedData[Index] != ',')//Handles CSV files
                {
                    OutputData+=Convert.ToString((char)Convert.ToInt32(WorkingProcessedData.Substring(Index, 8),2)); //First converts ProcessedData into an integer from a string of binary, then converts it back into a string to add to OutputData
                    Index += 8;
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
            if (InputType == DataInputType.TextFile & FilePath != "") //Saves OutputData (and Key + MetaData into a file at the same location as input file
            {
                string FileContents = ""; //Data to be saved to file
                string MetaData = Interaction.InputBox("Enter Notes", "MetaData", ""); //Opens input box to add metadata to file
                if(Convert.ToBoolean(AlgorithmConfig[AlgorithmConfig.Count - 1])) //If key is being saved add its length to FileContents
                {
                    FileContents += (Convert.ToString(Key.Length)).PadLeft(2,'0');
                }
                else //Else add 0 for keylength
                {
                    FileContents += "00";
                }
                FileContents += (Convert.ToString(MetaData.Length)).PadLeft(6,'0'); //AddMetaData input to FileContents
                if (Convert.ToBoolean(AlgorithmConfig[AlgorithmConfig.Count - 1])) //If key is being saved add it to FileContents
                {
                    FileContents += Key;
                }
                FileContents += MetaData; //Add metadata to FileContents
                FileContents += OutputData; //Add payload to FileContents
                FilePath = FilePath.Replace(".txt", "_Ciphered.txt");//Adjust file to create a new version to not overwrite old version
                System.IO.File.WriteAllText(FilePath,FileContents); //Create file at same location with _Ciphered appended to file name which contains FileContent
                MessageBox.Show("Ouput saved to file at file input location");
            }
            if (InputType == DataInputType.CSV) { throw new NotImplementedException(); } //not yet implemented
        }
        /// <summary>
        /// Attempts to set value of RawData, Key, and ConfigSettings then returns the validity of the inputdata
        /// </summary>
        /// <returns></returns>
        public virtual ValidationResult SetAndValidateData(string RawDataInput, string KeyInput, List<string> ConfigSettings)
        {
            if(RawData == string.Empty)
            {
                RawData = RawDataInput;
            }
            AlgorithmConfig = ConfigSettings;
            if (Key == string.Empty)
            {
                Key = KeyInput;
            }
            if (RawData == string.Empty & Key == string.Empty & AlgorithmConfig.Count == 0) //Ciphertext / Plaintext invalid and Key invalid and Config settings invalid for encryption / decryption
            {
                return ValidationResult.KeyAndDataAndConfigInvalid;
            }
            else if (RawData == string.Empty & Key == string.Empty) //Ciphertext / Plaintext invalid and Key invalid for encryption / decryption
            {
                return ValidationResult.KeyAndDataInvalid;
            }
            else if (RawData == string.Empty & AlgorithmConfig.Count == 0) //Ciphertext / Plaintext invalid and ConfigSettings invalid for encryption / decryption
            {
                return ValidationResult.DataAndConfigInvalid;
            }
            else if (Key == string.Empty & AlgorithmConfig.Count == 0) //Key invalid and ConfigSettings invalid for encryption / decryption
            {
                return ValidationResult.KeyAndConfigInvalid;
            }
            else if (Key == string.Empty) //Key invalid for encryption / decryption
            {
                return ValidationResult.KeyInvalid;
            }
            else if (AlgorithmConfig.Count == 0) //ConfigSettings invalid for encryption / decryption
            {
                return ValidationResult.ConfigInvalid;
            }
            else if (RawData == string.Empty) //Ciphertext / Plaintext invalid for encryption / decryption
            {
                return ValidationResult.DataInvalid;
            }
            else //All user input Valid and encryption can take place
            {
                return ValidationResult.Valid;
            }
        }
    }
    
}

