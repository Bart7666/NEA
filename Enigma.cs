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
                _key = value;
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
                            value = value.Replace(" ", "");//Removes all spaces (as they are not valid for Enigma Encryption
                            foreach (char Character in value) //Checks the passed value is purely letters in the english alphabet
                            {
                                if (!((int)Character >= 65 && (int)Character <= 90)) //If it is not a letter set value of RawData to null and stop checking
                                {
                                    _rawData = null;
                                    break;
                                }
                                else //If (this character of ) the key is an english letter
                                {
                                    _rawData = value;
                                }
                            }
                    }
                }
            }
        }

        private int[,,]? _rotors;
        /// <summary>
        /// A 3D array which contains the currently in used rotors in the form of 3 2D arrays with 2 rows and 26 columns
        /// </summary>
        public int[,,]? Rotors
        {
            get
            {
                if (_rotors != null)
                {
                    return _rotors;
                }
                else //Should never occur 
                {
                    return null;
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
        public int[,]? Reflector
        {
            get
            {
                if (_reflector != null)
                {
                    return _reflector;
                }
                else //Should never occur 
                {
                    return null;
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
        public int[]? NotchPositions
        {
            get
            {
                if (_notchPositions != null)
                {
                    return _notchPositions;
                }
                else //Should never occur 
                {
                    return null;
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
            base.ComposeData(InputType);
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
                for (int Loop = 0;  Loop < WorkingRawData.Length; Loop++)
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

        public void Rotate(int RotorToRotate,bool ForwardDirection)
        {
            int[,,] WorkingRotor = new int[3,26,2];
            if (ForwardDirection) // Do a for loop and manually move every element up in both columns.
            {
                Rotors[RotorToRotate,0,0] = 
            }
        }
        public void RotateTo(int RotorToRotate,int SearchItem)
        {

        }
        public void SetInitialRotors()
        {

        }
        public void SetRings()
        {

        }
        public void RototeRotors()
        {

        }
    }
    
}

