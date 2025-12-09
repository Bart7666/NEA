using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA
{
    /// <summary>
    /// Implementation of Enigma as a class
    /// </summary>
    internal class Enigma : EncryptionAlgorithm
    {
        /// <summary>
        /// Key to use in encryption or decryption by Enigma, Overrides root Key definition within EncryptionAlgorithm
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
                    value = value.ToUpper();
                    value = value.Trim(); //Removes all whitespace
                    if (value.Length <= 3) //Checks character limit is correct.
                    {
                            foreach (char Character in value) //Checks the passed value is purely letters in the english alphabet
                            {
                                if (!((int)Character >= 65 && (int)Character <= 90)) //If it is not a letter set value of RawData to null and stop checking
                                {
                                    _key = null;
                                    break;
                                }
                                else //If (this character of) the key is an english letter save value of key
                                {
                                    _key = value;
                                }
                            }
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
                    value = value.ToUpper();
                    value = value.Trim(); //Removes all whitespace
                    if (value.Length < 536870912) //Checks character limit is correct.
                    {
                            value = value.Replace(" ", "");//Removes all spaces (as they are not valid for Enigma Encryption
                            foreach (char Character in value) //Checks the passed value is purely letters in the english alphabet
                            {
                                if (!((int)Character >= 65 && (int)Character <= 90)) //If it is not a letter set value of RawData to null and stop checking
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

        private List<int[,]>? _rotors;
        /// <summary>
        /// A 3D array which contains the currently in used rotors in the form of 3 2D arrays with 2 rows and 26 columns
        /// </summary>
        public List<int[,]> Rotors
        {
            get
            {
                if (_rotors != null)
                {
                    return _rotors;
                }
                else
                {
                    _rotors = new List<int[,]> ();
                    return _rotors;
                }
            }
            set
            {
                if(value != null)
                {
                    _rotors = value;
                }
            }
        }

        private int[,]? _reflector;
        /// <summary>
        /// Contains the currently in use reflector which will be represented as a 2d array with 2 rows and 26 columns
        /// </summary>
        public int[,] Reflector
        {
            get
            {
                if (_reflector != null)
                {
                    return _reflector;
                }
                else //Should never occur but to make code more robust
                {
                    _reflector = new int[26, 2];
                    return _reflector;
                }
            }
            set
            {
                if(value != null)
                {
                    _reflector = value;
                }
            }
        }

        private int[]? _notchPositions;
        /// <summary>
        /// Contains the Notch position for the currently in use rotor as an index for the position at which a step will occur
        /// </summary>
        public int[] NotchPositions
        {
            get
            {
                if (_notchPositions != null)
                {
                    return _notchPositions;
                }//Should never occur but to make code more robust
                else
                {
                    _notchPositions = new int[3];
                    return _notchPositions;
                }
            }
            set
            {
                if (value != null)
                {
                    _notchPositions = value;
                }
                
            }
        }


        /// <summary>
        /// Cleans the Rawdata into binary and sets to uppercase to be used in encryption or decryption within Enigma
        /// </summary>
        /// <param name="InputType"></param>
        public override void CleanData(DataInputType InputType) //DataInputType is the data
        {
            if (InputType == DataInputType.Text)
            {
                string WorkingRawData = RawData; //Saves RawData to working variable
                WorkingRawData = WorkingRawData.ToUpper(); //Converts RawData value to uppercase (As Enigma works in one case and so I will use UpperCase)
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
        /// Override of ComposeData for Enigma, Runs exact same algorithm except calling ReintroduceSpacing() at the end
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
            ReintroduceSpacing(InputType); 

        }
        /// <summary>
        /// Reintroduces spaces and new lines into OutputData after encryption using Enigma
        /// </summary>
        /// <param name="InputType"></param>
        public void ReintroduceSpacing(DataInputType InputType)
        {
            if (InputType == DataInputType.Text)
            {
                string WorkingRawData = RawData; //Saves RawData to working variable
                WorkingRawData = WorkingRawData.ToUpper(); //Converts RawData value to uppercase (As Enigma works in one case and so I will use UpperCase)
                for (int Loop = 0;  Loop < WorkingRawData.Length; Loop++) //Loops through each character in the data input
                {
                    if (((int)WorkingRawData[Loop] == 32)) //If Character at current index is a space add space at that index in output
                    {
                        OutputData = OutputData.Substring(0, Loop) + " " + OutputData.Substring(Loop,OutputData.Length-Loop); 
                    }
                    else if (((int)WorkingRawData[Loop] == 10))
                    {
                        OutputData = OutputData.Substring(0, Loop) + "\n" + OutputData.Substring(Loop, OutputData.Length - Loop);
                    }
                }
            }
            if (InputType == DataInputType.TextFile) { throw new NotImplementedException(); } //not yet implemented
            if (InputType == DataInputType.CSV) { throw new NotImplementedException(); } //not yet implemented
        }
        /// <summary>
        /// Rotates a given Rotor either forwards or backwards one
        /// </summary>
        /// <param name="RotorToRotate"></param>
        /// <param name="ForwardDirection"></param>
        public void Rotate(int RotorToRotate,bool ForwardDirection)
        {
            if (ForwardDirection) 
            {
                int[] TopValues = { Rotors[RotorToRotate][ 0, 0], Rotors[RotorToRotate][ 0, 1] }; //Stores the values at the zero index
                for (int Loop = 0; Loop < 25; Loop++) //"Moves" every value in the rotor up one
                {
                    Rotors[RotorToRotate][ Loop, 0] = Rotors[RotorToRotate][ Loop+1, 0];
                    Rotors[RotorToRotate][ Loop, 1] = Rotors[RotorToRotate][ Loop+1, 1];
                }
                Rotors[RotorToRotate][ 25, 0] = TopValues[0]; //First element loops around to be last
                Rotors[RotorToRotate][ 25, 1] = TopValues[1]; //First element loops around to be last
            }
            else
            {
                int[] BottomValues = { Rotors[RotorToRotate][ 25, 0], Rotors[RotorToRotate][ 25, 1] }; //Stores the values at the end index
                for (int Loop = 25; Loop > 0; Loop--) //"Moves" every value in the rotor down one 
                {
                    Rotors[RotorToRotate][ Loop, 0] = Rotors[RotorToRotate][ Loop - 1, 0];
                    Rotors[RotorToRotate][ Loop, 1] = Rotors[RotorToRotate][ Loop - 1, 1];
                }
                Rotors[RotorToRotate][ 0, 0] = BottomValues[0]; //Last element loops around to be first
                Rotors[RotorToRotate][0, 1] = BottomValues[1]; //Last element loops around to be first
            }
        }
        /// <summary>
        /// Rotates the rotor untill the value at index 0 of the left column of the selected rotor is the SearchItem
        /// </summary>
        /// <param name="RotorToRotate"></param>
        /// <param name="SearchItem"></param>
        public void RotateTo(int RotorToRotate,int SearchItem)
        {
            while (Rotors[RotorToRotate][0, 0] != SearchItem)
            {
                Rotate(RotorToRotate, true);
            }
        }
        /// <summary>
        /// Sets the initial position of the rotors depending on the key input
        /// </summary>
        public void SetInitialRotors()
        {
            RotateTo(0, (int)Key[0]-65);
            RotateTo(1, (int)Key[1]-65);
            RotateTo(2, (int)Key[2]-65);
        }
        /// <summary>
        /// First sets Rotors, Reflectors and notch positions, then Adjusts the notch position based on offset user sets
        /// </summary>
        public void SetRings()
        {
            //Below is all the hisotrical rotors and reflectors, where the first value in each sub array is the alphabet indexed 0-25 and the second is the rotor wiring for that letter
            int[,] I = new int[,] { {0,4},{1,10},{2,12},{3,5},{4,11},{5,6},{6,3},{7,16},{8,21},{9,25},{10,13},{11,19},{12,14},{13,22},{14,24},{15,7},{16,23},{17,20},{18,18},{19,15},{20,0},{21,8},{22,1},{23,17},{24,2},{25,9} }; //Historical rotor "I"
            int[,] II = new int[,] { {0,0},{1,9},{2,3},{3,10},{4,18},{5,8},{6,17},{7,20},{8,23},{9,1},{10,11},{11,7},{12,22},{13,19},{14,12},{15,2},{16,16},{17,6},{18,25},{19,13},{20,15},{21,24},{22,5},{23,21},{24,14},{25,4} }; //Historical rotor "II"
            int[,] III = new int[,] { {0,1},{1,3},{2,5},{3,7},{4,9},{5,11},{6,2},{7,15},{8,17},{9,19},{10,23},{11,21},{12,25},{13,13},{14,24},{15,4},{16,8},{17,22},{18,6},{19,0},{20,10},{21,12},{22,20},{23,18},{24,16},{25,14} }; //Historical rotor "III"
            int[,] IV = new int[,] { {0,4},{1,18},{2,14},{3,21},{4,15},{5,25},{6,9},{7,0},{8,24},{9,16},{10,20},{11,8},{12,17},{13,7},{14,23},{15,11},{16,13},{17,5},{18,19},{19,6},{20,10},{21,3},{22,2},{23,12},{24,22},{25,1} };  //Historical rotor "IV"
            int[,] V = new int[,] { {0,21},{1,25},{2,1},{3,17},{4,6},{5,8},{6,19},{7,24},{8,20},{9,15},{10,18},{11,3},{12,13},{13,7},{14,11},{15,23},{16,0},{17,22},{18,12},{19,9},{20,16},{21,14},{22,5},{23,4},{24,2},{25,10} }; //Historical rotor "V"
            int[,] B = new int[,] { {0,24},{1,17},{2,20},{3,7},{4,16},{5,18},{6,11},{7,3},{8,15},{9,23},{10,13},{11,6},{12,14},{13,10},{14,12},{15,8},{16,4},{17,1},{18,5},{19,25},{20,2},{21,22},{22,21},{23,9},{24,0},{25,19} }; //Historical reflector "B"
            int[,] C = new int[,] { {0,5},{1,21},{2,15},{3,9},{4,8},{5,0},{6,14},{7,24},{8,4},{9,3},{10,17},{11,25},{12,23},{13,22},{14,6},{15,2},{16,19},{17,10},{18,20},{19,16},{20,18},{21,1},{22,13},{23,12},{24,7},{25,11} }; //Historical reflector "C"

            for (int RotorSlot = 0;  RotorSlot < 3; RotorSlot++) // Iterate RotorSlot through each rotor
            {
                if (AlgorithmConfig[RotorSlot] == "I") //Adds Rotor and sets NotchPosition
                {
                    Rotors.Add((int[,])I.Clone());
                    NotchPositions[RotorSlot] = 16; 
                }
                else if (AlgorithmConfig[RotorSlot] == "II") //Adds Rotor and sets NotchPosition
                {
                    Rotors.Add((int[,])II.Clone());
                    NotchPositions[RotorSlot] = 4;
                }
                else if (AlgorithmConfig[RotorSlot] == "III") //Adds Rotor and sets NotchPosition
                {
                    Rotors.Add((int[,])III.Clone());
                    NotchPositions[RotorSlot] = 21;
                }
                else if (AlgorithmConfig[RotorSlot] == "IV") //Adds Rotor and sets NotchPosition
                {
                    Rotors.Add((int[,])IV.Clone());
                    NotchPositions[RotorSlot] = 9;
                }
                else if (AlgorithmConfig[RotorSlot] == "V") //Adds Rotor and sets NotchPosition
                {
                    Rotors.Add((int[,])V.Clone());
                    NotchPositions[RotorSlot] = 25;
                }
            }
            if (AlgorithmConfig[6] == "B") // If selected reflector (in config) is Reflector B, Copy B into Reflector
            {
                Reflector = B;
            }
            else if (AlgorithmConfig[6] == "C") // If selected reflector (in config) is Reflector C, Copy C into Reflector
            {
                Reflector = C;
            }
            //The folllowing 3 loops offset each 
            for (int Rotor1Offset = 0;  Rotor1Offset < Int32.Parse(AlgorithmConfig[3]);  Rotor1Offset++)
            {
                Rotate(0, false);
                NotchPositions[0] = (NotchPositions[0] - 1+26) % 26;
            }
            for (int Rotor2Offset = 0; Rotor2Offset < Int32.Parse(AlgorithmConfig[4]); Rotor2Offset++)
            {
                Rotate(1, false);
                NotchPositions[1] = (NotchPositions[1] - 1+26) % 26;
            }
            for (int Rotor3Offset = 0; Rotor3Offset < Int32.Parse(AlgorithmConfig[5]); Rotor3Offset++)
            {
                Rotate(2, false);
                NotchPositions[2] = (NotchPositions[2] - 1 + 26) % 26;
            }
        }
        /// <summary>
        /// Contains the actual encrypting process which turns Engima from a relatively simple substitution cipher to a polyalphabetic substition cipher, by cause the wheels to rotate depending on the notch positions
        /// </summary>
        public void RotateRotors()
        {
            if (Rotors[1][0, 0] == NotchPositions[1] & Rotors[2][0, 0] == NotchPositions[2]) //If 2nd and 3rd Rotor are at notch position, rotate all three rotors
            {
                Rotate(0, true);
                Rotate(1, true);
                Rotate(2, true);
            }
            else if (Rotors[1][0, 0] == NotchPositions[1]) //If only the 2nd Rotor is in notch position, rotate all three rotors, this is known as the double step
            {
                Rotate(0, true);
                Rotate(1, true);
                Rotate(2, true);
            }
            else if (Rotors[2][0, 0] == NotchPositions[2]) //If only the 3rd Rotor is in notch position, rotate the 2nd and 3rd rotor
            {
                Rotate(1, true);
                Rotate(2, true);
            }
            else //If no rotors are in notch position, rotate the 3rd rotor
            {
                Rotate(2, true);
            }
        }
        /// <summary>
        /// Searches the right column of a given rotor for a given item
        /// </summary>
        /// <param name="RotorToSearch"></param>
        /// <param name="SearchItem"></param>
        /// <returns> int SearchIndex </returns>
        public int SearchRotor(int RotorToSearch,int SearchItem)
        {
            for (int SearchIndex = 0; SearchIndex < Rotors[RotorToSearch].GetLength(0); SearchIndex++)
            {
                if (Rotors[RotorToSearch][SearchIndex, 1] == SearchItem)
                {
                    return SearchIndex;
                }
            }
            throw new ArgumentOutOfRangeException(); //Should never occur
        }
        /// <summary>
        /// Encrypts Data via the Enigma machine, then saves result into ProcessedData
        /// </summary>
        public override void EncryptData()
        {
            SetRings();
            SetInitialRotors();
            int ASCIICharacter; // ASCII representation of a character
            string WorkingCleanedData = CleanedData; //Saves CleanedData to working variable
            int Signal;
            int Index = 0;
            while (Index < WorkingCleanedData.Length)
            {
                if (WorkingCleanedData[Index] != ',')//Handles CSV files
                {
                    RotateRotors();
                    ASCIICharacter = Convert.ToInt32(WorkingCleanedData.Substring(Index, 8), 2); //First converts CleanedData into an integer from a string of binary and saves it to a variable
                    ASCIICharacter -= 65;
                    Signal = Rotors[2][ASCIICharacter, 1]; //Encryption through third rotor
                    Signal = Rotors[1][Signal, 1]; //Encryption through second rotor
                    Signal = Rotors[0][Signal, 1]; //Encryption through first rotor
                    Signal = Reflector[Signal, 1]; //Encryption through the reflector
                    Signal = SearchRotor(0, Signal); //Encryption through first rotor
                    Signal = SearchRotor(1, Signal); //Encryption through second rotor
                    Signal = SearchRotor(2, Signal); //Encryption through third rotor
                    Signal += 65; //Convert Signal back to ASCII representation of a capital letter
                    ProcessedData += Convert.ToString(Signal, 2).PadLeft(8,'0'); //Adds encrypted character in binary ASCII to ProcessedData
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
        /// Decrypts Data via the Enigma machine, then saves result into ProcessedData
        /// </summary>
        public override void DecryptData()
        {
            SetRings();
            SetInitialRotors();
            int ASCIICharacter; // ASCII representation of a character
            string WorkingCleanedData = CleanedData; //Saves CleanedData to working variable
            int Signal;
            int Index = 0;
            while (Index < WorkingCleanedData.Length)
            {
                if (WorkingCleanedData[Index] != ',')//Handles CSV files
                {
                    RotateRotors();
                    ASCIICharacter = Convert.ToInt32(WorkingCleanedData.Substring(Index, 8), 2); //First converts CleanedData into an integer from a string of binary and saves it to a variable
                    ASCIICharacter -= 65;
                    Signal = Rotors[2][ASCIICharacter, 1]; //Decryption through third rotor
                    Signal = Rotors[1][Signal, 1]; //Decryption through second rotor
                    Signal = Rotors[0][Signal, 1]; //Decryption through first rotor
                    Signal = Reflector[Signal, 1]; //Decryption through the reflector
                    Signal = SearchRotor(0, Signal); //Decryption through first rotor
                    Signal = SearchRotor(1, Signal); //Decryption through second rotor
                    Signal = SearchRotor(2, Signal); //Decryption through third rotor
                    Signal += 65; //Convert Signal back to ASCII representation of a capital letter
                    ProcessedData += Convert.ToString(Signal, 2).PadLeft(8, '0'); //Adds decrypted character in binary ASCII to ProcessedData
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

