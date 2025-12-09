using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA
{
    /// <summary>
    /// Implementation of the Scytale transposition cipher as a class
    /// </summary>
    internal class Scytale : EncryptionAlgorithm
    {

        /// <summary>
        ///Key to use in encryption or decryption by Scytale transposition cipher, Overrides root key definition within EncryptionAlgorithm
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
                    else 
                    {
                        int RowLength = Convert.ToInt32(Key.Split(',')[0]); //Number of rows to use for Scytale
                        int ColumnLength = Convert.ToInt32(Key.Split(',')[1]); //Number of columns to use for Scytale
                        string TestData = RawData.Replace(" ", "");
                        if (RowLength * ColumnLength < TestData.Length)
                        {
                            _key = null;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Override of CleanData for Scytale
        /// </summary>
        /// <param name="InputType"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void CleanData(DataInputType InputType)
        {
            if (InputType == DataInputType.Text)
            {
                string WorkingRawData = RawData; //Saves RawData to working variable
                WorkingRawData = WorkingRawData.Replace(" ", "");
                foreach (char RawDataCharacter in WorkingRawData) //Iterates through every character and appends it to CleanedData
                {
                    if ((int)RawDataCharacter < 256) //If the character is a part of extended ASCII (0-255), add to CleanedData
                    {
                        CleanedData += Convert.ToString(((int)RawDataCharacter), 2).PadLeft(8, '0'); //Converts integer (Extended ASCII) representation into a string containing binary then pads it with zeros untill it is 8 bits long before adding it to cleaned data
                    }
                    else //Else add a question mark
                    {
                        CleanedData += "?";
                    }
                }
            }
            if (InputType == DataInputType.TextFile) { throw new NotImplementedException(); } //not yet implemented
            if (InputType == DataInputType.CSV) { throw new NotImplementedException(); } //not yet implemented
        }
        /// <summary>
        /// Encrypts plaintext using the Scytale transposition Cipher
        /// </summary>
        public override void EncryptData()
        {
            string WorkingCleanedData = CleanedData; //Saves CleanedData to working variable
            int RowIndex = 0; //Used in while loop for constructing Rod
            int ColumnIndex = 0; //Used in while loop for constructing Rod
            int RowLength = Convert.ToInt32(Key.Split(',')[0]); //Number of rows to use for Scytale
            int ColumnLength = Convert.ToInt32(Key.Split(',')[1]); //Number of columns to use for Scytale
            string[,] Rod = new string[RowLength, ColumnLength]; //Array to represent Scytale
            int Index = 0;
            string WorkingCharacter;
            while (Index < WorkingCleanedData.Length)
            {
                WorkingCharacter = WorkingCleanedData.Substring(Index, 8); //Takes 8 characters representing either a character or a comma if it is a CSV file.
                if (WorkingCleanedData[Index] != ',')//Handles CSV files
                {
                    Rod[RowIndex, ColumnIndex] = WorkingCharacter; //Adds current substring to Rod array
                    ColumnIndex++; //Increment Column for next input
                    if (ColumnIndex == ColumnLength) //Move to next Row if at end of current one
                    {
                        ColumnIndex = 0;
                        RowIndex++;
                    }
                    Index += 8;
                }
                else
                {
                    while (WorkingCleanedData[Index] == ',') //Adds commas for CSV files untill a non comma character is reached
                    {
                        Rod[RowIndex, ColumnIndex] = WorkingCharacter; //Adds commma to Rod array
                        ColumnIndex++; //Increment Column for next input
                        if (ColumnIndex == ColumnLength) //Move to next Row if at end of current one
                        {
                            ColumnIndex = 0;
                            RowIndex++;
                        }
                        Index++;
                    }
                }
            }
            for (int OutputColumnIndex = 0; OutputColumnIndex < ColumnLength; OutputColumnIndex++)
            {
                for (int OutputRowIndex = 0; OutputRowIndex < RowLength; OutputRowIndex++)
                {
                    if (Rod[OutputRowIndex, OutputColumnIndex] is not null)
                    {
                        ProcessedData += Rod[OutputRowIndex, OutputColumnIndex];
                    }
                    else
                    {
                        ProcessedData += Convert.ToString((int)('X'), 2).PadLeft(8, '0');
                    }
                }
            }
        }
        /// <summary>
        /// Decrypts ciphertext using the Scytale transposition Cipher
        /// </summary>
        public override void DecryptData()
        {
            string WorkingCleanedData = CleanedData; //Saves CleanedData to working variable
            int RowIndex = 0; //Used in while loop for constructing Rod
            int ColumnIndex = 0; //Used in while loop for constructing Rod
            int RowLength = Convert.ToInt32(Key.Split(',')[0]); //Number of rows to use for Scytale
            int ColumnLength = Convert.ToInt32(Key.Split(',')[1]); //Number of columns to use for Scytale
            string[,] Rod = new string[RowLength, ColumnLength]; //Array to represent Scytale
            int Index = 0;
            string WorkingCharacter;
            while (Index < WorkingCleanedData.Length)
            {
                WorkingCharacter = WorkingCleanedData.Substring(Index, 8); //Takes 8 characters representing either a character or a comma if it is a CSV file.
                if (WorkingCleanedData[Index] != ',')//Handles CSV files
                {
                    Rod[RowIndex, ColumnIndex] = WorkingCharacter; //Adds current substring to Rod array
                    ColumnIndex++; //Increment Column for next input
                    if (ColumnIndex == ColumnLength) //Move to next Row if at end of current one
                    {
                        ColumnIndex = 0;
                        RowIndex++;
                    }
                    Index += 8;
                }
                else
                {
                    while (WorkingCleanedData[Index] == ',') //Adds commas for CSV files untill a non comma character is reached
                    {
                        Rod[RowIndex, ColumnIndex] = WorkingCharacter; //Adds commma to Rod array
                        ColumnIndex++; //Increment Column for next input
                        if (ColumnIndex == ColumnLength) //Move to next Row if at end of current one
                        {
                            ColumnIndex = 0;
                            RowIndex++;
                        }
                        Index++;
                    }
                }
            }
            for (int OutputColumnIndex = 0; OutputColumnIndex < ColumnLength; OutputColumnIndex++)
            {
                for (int OutputRowIndex = 0; OutputRowIndex < RowLength; OutputRowIndex++)
                {
                    if ((Rod[OutputRowIndex, OutputColumnIndex] is not null && Rod[OutputRowIndex, OutputColumnIndex] != "X"))
                    {
                        ProcessedData += Rod[OutputRowIndex, OutputColumnIndex];
                    }
                }
            }
        }
        /// <summary>
        /// Override of ComposeData for Scytale, in order to remove trailing Xs
        /// </summary>
        /// <param name="InputType"></param>
        public override void ComposeData(DataInputType InputType)
        {
            string WorkingProcessedData = ProcessedData; //Saves ProcessedData to working variable
            int Index = 0;
            while (Index < ProcessedData.Length)
            {
                if (WorkingProcessedData[Index] != ',')//Handles CSV files
                {
                    OutputData += Convert.ToString((char)Convert.ToInt32(WorkingProcessedData.Substring(Index, 8), 2)); //First converts ProcessedData into an integer from a string of binary, then converts it back into a string to add to OutputData
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
            OutputData = OutputData.TrimEnd('X');
        }
    }
}


